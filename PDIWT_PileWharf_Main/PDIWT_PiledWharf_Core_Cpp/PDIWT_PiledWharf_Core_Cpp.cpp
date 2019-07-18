#include "stdafx.h"

#include "PDIWT_PiledWharf_Core_Cpp.h"

#pragma unmanaged
// Write setting into active dgnmodel
StatusInt PDIWT_PiledWharf_Core_Cpp::PDIWTECFramework::WriteSettingsOnActiveModel(WString ecschemaFullName, WString ecClassName, bmap<WString, WString> propList)
{
	DgnECManagerR _dgnECManager = DgnECManager::GetManager();

	try
	{
		// parse the full schema name
		WString _pdiwtSchemaName;
		unsigned int _versionMajor, _versionMinor;
		if (ECOBJECTS_STATUS_Success != ECSchema::ParseSchemaFullName(_pdiwtSchemaName, _versionMajor, _versionMinor, ecschemaFullName))
		{
			mdlOutput_error(WPrintfString(L"Can't parse %s", ecschemaFullName));
			return ERROR;
		}

		// Get Schema paths defined by variable at Organization Level
		bvector<WString> _ecSchemaXMLFilePaths;
		if (GetOrganizationECSchemaFile(_pdiwtSchemaName, WString(L"PDIWT_ORGANIZATION_ECSCHEMAPATH"), &_ecSchemaXMLFilePaths) != SUCCESS)
		{
			mdlOutput_error(WPrintfString(L"Can't find %s in related directories.", ecschemaFullName));
			return ERROR;
		}

		// If dgnfile doesn't contain the designated schema, Import it.
		SchemaInfo _pdiwtSchemaInfo(SchemaKey(_pdiwtSchemaName.GetWCharCP(), _versionMajor, _versionMinor), *ISessionMgr::GetActiveDgnFile());
		if (!_dgnECManager.IsSchemaContainedWithinFile(_pdiwtSchemaInfo, SCHEMAMATCHTYPE_Identical))
		{
			ECSchemaPtr _pdiwtSchemaImportPtr = nullptr;
			for each (auto _filepath in _ecSchemaXMLFilePaths)
			{
				if (_dgnECManager.ReadSchemaFromXmlFile(_pdiwtSchemaImportPtr, WString(_filepath + ecschemaFullName).GetWCharCP(), ISessionMgr::GetActiveDgnFile())
					== SchemaReadStatus::SCHEMA_READ_STATUS_Success)
					break;
				mdlOutput_error(L"Can't Read Schema");
				return ERROR;
			}
			if (_dgnECManager.ImportSchema(*_pdiwtSchemaImportPtr, *ISessionMgr::GetActiveDgnFile()) != SchemaImportStatus::SCHEMAIMPORT_Success)
			{
				mdlOutput_error(L"Can't Import Schema");
				return ERROR;
			}
		}

		// Locate SchemaPtr in Dgnfile
		ECSchemaPtr _pdiwtSchemaPtr = _dgnECManager.LocateSchemaInDgnFile(_pdiwtSchemaInfo, SCHEMAMATCHTYPE_Identical);
		if (_pdiwtSchemaPtr == nullptr)
		{
			mdlOutput_error(WPrintfString(L"Can't locate schema %s in dgnfile", _pdiwtSchemaInfo.GetSchemaName()));
			return ERROR;
		}

		// Try to find designated ECInstance on model, if not add it.
		ECClassP _pdwitECClassP = _pdiwtSchemaPtr->GetClassP(ecClassName.GetWCharCP());
		if (_pdwitECClassP == NULL)
		{
			mdlOutput_error(WPrintfString(L"Can't get ECClass %s", ecClassName));
			return ERROR;
		}
		FindInstancesScopePtr _pFindInstancesScope = FindInstancesScope::CreateScope(*ACTIVEMODEL, FindInstancesScopeOption(DgnECHostType::Model));
		ECQueryPtr _pECQuery = ECQuery::CreateQuery(_pdiwtSchemaName.GetWCharCP(), ecClassName.GetWCharCP());
		DgnECInstanceIterable _ecInterable = _dgnECManager.FindInstances(*_pFindInstancesScope, *_pECQuery);

		if (!_ecInterable.empty())
		{
			for (auto _ecInstance : _ecInterable)
				_ecInstance->Delete();
		}

		DgnECInstancePtr _pdiwtInstance = nullptr;
		DgnECInstanceEnablerP _pdiwtInstanceEnabler = _dgnECManager.ObtainInstanceEnabler(*_pdwitECClassP, *ISessionMgr::GetActiveDgnFile());
		StandaloneECInstanceR _pdiwtWIPInstance = _pdiwtInstanceEnabler->GetSharedWipInstance();
		SetPropValueList(_pdiwtWIPInstance, propList);
		_pdiwtInstanceEnabler->CreateInstanceOnModel(&_pdiwtInstance, _pdiwtWIPInstance, *ISessionMgr::GetActiveDgnModelP());

	}
	catch (const std::exception& ex)
	{
		mdlOutput_error(WString(ex.what()).GetWCharCP());
	}

	return SUCCESS;
}

// Get the ECSChema file at organization level.
StatusInt PDIWT_PiledWharf_Core_Cpp::PDIWTECFramework::GetOrganizationECSchemaFile(WString ecSchemaFullName, WString definedVariable, bvector<WString>* ecSchemaFilePaths)
{
	if (!ConfigurationManager::IsVariableDefined(definedVariable.GetWCharCP()))
		return ERROR;

	WString _folderPaths;
	if (ConfigurationManager::GetVariable(_folderPaths, definedVariable.GetWCharCP(), ConfigurationVariableLevel::Organization) != SUCCESS ||
		WString::IsNullOrEmpty(_folderPaths.GetWCharCP()))
		return ERROR;

	BeStringUtilities::Split(_folderPaths.GetWCharCP(), L";", *ecSchemaFilePaths);

	return SUCCESS;
}

void PDIWT_PiledWharf_Core_Cpp::PDIWTECFramework::SetPropValueList(IECInstanceR ecInstance, bmap<WString, WString> propList)
{
	for (auto _propvalue : propList)
	{
		WString _propTypeName = ecInstance.GetClass().GetPropertyP(_propvalue.first.GetWCharCP())->GetTypeName();
		_propTypeName.ToLower();
		if (_propTypeName.Equals(L"string"))
		{
			ecInstance.SetValue(_propvalue.first.GetWCharCP(), ECValue(_propvalue.second.GetWCharCP()));
			continue;
		}
		if (_propTypeName.Equals(L"int"))
		{
			ecInstance.SetValue(_propvalue.first.GetWCharCP(), ECValue(BeStringUtilities::Wtoi(_propvalue.second.GetWCharCP())));
			continue;
		}
		if (_propTypeName.Equals(L"boolean"))
		{
			WString _propvaluesecond = _propvalue.second;
			bool _propvaluebooleanvalue = true;
			_propvaluesecond.ToLower();
			if (_propvaluesecond.Equals(L"true") || _propvaluesecond.Equals(L"0"))
				_propvaluebooleanvalue = true;
			else
				_propvaluebooleanvalue = false;
			ecInstance.SetValue(_propvalue.first.GetWCharCP(), ECValue(_propvaluebooleanvalue));
			continue;
		}
		if (_propTypeName.Equals(L"double"))
		{
			ecInstance.SetValue(_propvalue.first.GetWCharCP(), ECValue(BeStringUtilities::Wtof(_propvalue.second.GetWCharCP())));
			continue;
		}
		if (_propTypeName.Equals(L"datetime"))
		{
			Bentley::DateTime _dateTime;
			Bentley::DateTime::FromString(_dateTime, _propvalue.second.GetWCharCP());
			ecInstance.SetValue(_propvalue.first.GetWCharCP(), ECValue(_dateTime));
			continue;
		}
		mdlOutput_error(WPrintfString(L"Unknown Type %s, Can't set property", _propTypeName));
	}
}

#pragma managed


PDIWT_PiledWharf_Core_Cpp::SettingsWriteStatus PDIWT_PiledWharf_Core_Cpp::ECFrameWorkWraper::WriteSettingsOnActiveModel(String^ ecSchemaFullName, String^ ecClassName, Dictionary<String^, String^>^ propList)
{
	pin_ptr<const WCHAR> _ecSchemaFullName = PtrToStringChars(ecSchemaFullName);
	pin_ptr<const WCHAR> _ecClassName = PtrToStringChars(ecClassName);
	bmap<WString, WString> _ecPropValueList;
	for each (auto _propvalue in propList)
	{
		pin_ptr<const WCHAR> _prop = PtrToStringChars(_propvalue.Key);
		pin_ptr<const WCHAR> _value = PtrToStringChars(_propvalue.Value);
		_ecPropValueList.Insert(_prop, _value);
	}
	if (PDIWTECFramework::WriteSettingsOnActiveModel(WString(_ecSchemaFullName), WString(_ecClassName), _ecPropValueList) == ERROR)
		SettingsWriteStatus::ERROR;
	return SettingsWriteStatus::SUCCESS;
}

