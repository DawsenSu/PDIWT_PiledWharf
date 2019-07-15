#include "stdafx.h"

#include "PDIWT_PiledWharf_Core_Cpp.h"

#pragma unmanaged
// Write setting into active dgnmodel
StatusInt PDIWT_PiledWharf_Core_Cpp::PDIWTECFramework::WriteSettingsOnActiveModel(bmap<WString, double> propList)
{
	DgnECManagerR _dgnECManager = DgnECManager::GetManager();

	WString _ecSchemaFullName = WString(L"PDIWT.01.00.ecschema.xml");

	try
	{
		// parse the full schema name
		WString _pdiwtSchemaName;
		unsigned int _versionMajor, _versionMinor;
		if (ECOBJECTS_STATUS_Success != ECSchema::ParseSchemaFullName(_pdiwtSchemaName, _versionMajor, _versionMinor, _ecSchemaFullName))
			mdlOutput_error(WPrintfString(L"Can't parse %s", _ecSchemaFullName));

		BeFileName _pdiwtECSchemaName;
		if (GetOrganizationECSchemaFile(_pdiwtECSchemaName, _ecSchemaFullName) != SUCCESS)
			mdlOutput_error(WPrintfString(L"Can't find %s in related directories.", _ecSchemaFullName));
		// If dgnfile doesn't contain the designated schema, Import it.


	}
	catch (const std::exception& ex)
	{
		mdlOutput_error(WString(ex.what()).GetWCharCP());
	}
	WString _className = WString(L"PiledWharfSetting");

	return SUCCESS;
}

// Import xmlECSchema Into ActiveModel
StatusInt PDIWT_PiledWharf_Core_Cpp::PDIWTECFramework::ImportSChemaXMLFileOnActiveModel(WString xmlFilePath)
{
	ECSchemaPtr _ecschemaptr = nullptr;
	DgnECManagerR _dgnECManager = DgnECManager::GetManager();
	if (_dgnECManager.ReadSchemaFromXmlFile(_ecschemaptr, xmlFilePath.GetWCharCP(), ISessionMgr::GetActiveDgnFile()) != SchemaReadStatus::SCHEMA_READ_STATUS_Success)
		return ERROR;
	if (_dgnECManager.ImportSchema(*_ecschemaptr, *ISessionMgr::GetActiveDgnFile()) != SchemaImportStatus::SCHEMAIMPORT_Success)
		return ERROR;
	return SUCCESS;
}

// Get the ECSChema file at organization level.
StatusInt PDIWT_PiledWharf_Core_Cpp::PDIWTECFramework::GetOrganizationECSchemaFile(BeFileNameR ecSchemaFile, WString ecSchemaFullName)
{
	if (!ConfigurationManager::IsVariableDefined(L"PDIWT_ORGANIZATION_ECSCHEMAPATH"))
		return ERROR;

	WString _folderPaths;
	if (ConfigurationManager::GetVariable(_folderPaths, L"PDIWT_ORGANIZATION_ECSCHEMAPATH", ConfigurationVariableLevel::Organization) != SUCCESS ||
		WString::IsNullOrEmpty(_folderPaths.GetWCharCP()))
		return ERROR;

	//if (mdlFile_find(ecSchemaFile, ecSchemaFullName.GetWCharCP(), L"$(PDIWT_ORGANIZATION_ECSCHEMAPATH)", NULL, FINDFILEOPTION_CurrentDirectoryLast) != SUCCESS ||
	//	ecSchemaFile == nullptr)
	//	return Bentley::DgnPlatformNET::StatusInt::Success;
	bvector<WString> _bvectorFolderPaths;
	BeStringUtilities::Split(_folderPaths.GetWCharCP(), L";", _bvectorFolderPaths);

	for (auto _folderName : _bvectorFolderPaths)
	{
		BeFile _potentialFile;
		WString _ecschemaFullName = _folderName + ecSchemaFullName;
		if (_potentialFile.Open(_ecschemaFullName.GetWCharCP(),BeFileAccess::Read,BeFileSharing::None) == BeFileStatus::Success)
		{
			ecSchemaFile = (BeFileName)_ecschemaFullName.GetWCharCP();
			return SUCCESS;
		}
	}
	return ERROR;
}

#pragma managed


Bentley::DgnPlatformNET::StatusInt PDIWT_PiledWharf_Core_Cpp::ECFrameWorkWraper::WriteSettingsOnActiveModel(Dictionary<String^, double>^ propList)
{
	PDIWTECFramework::WriteSettingsOnActiveModel(bmap<WString,double>());
	return Bentley::DgnPlatformNET::StatusInt::Success;
}

//Bentley::DgnPlatformNET::StatusInt PDIWT_PiledWharf_Core_Cpp::ECFrameWorkWraper::ImportSChemaXMLFileOnActiveModel(String ^xmlFilePath)
//{
//	// TODO: insert return statement here
//	pin_ptr<const WChar> _xmlfilepath = PtrToStringChars(xmlFilePath);
//	ECSchemaPtr _ecschemaptr = nullptr;
//	DgnECManagerR _dgnECManager = DgnECManager::GetManager();
//	if (_dgnECManager.ReadSchemaFromXmlFile(_ecschemaptr, _xmlfilepath, ISessionMgr::GetActiveDgnFile()) != SchemaReadStatus::SCHEMA_READ_STATUS_Success)
//		return Bentley::DgnPlatformNET::StatusInt::Error;
//	if (_dgnECManager.ImportSchema(*_ecschemaptr, *ISessionMgr::GetActiveDgnFile()) != SchemaImportStatus::SCHEMAIMPORT_Success)
//		return Bentley::DgnPlatformNET::StatusInt::Error;
//	return Bentley::DgnPlatformNET::StatusInt::Success;
//}
//
//Bentley::DgnPlatformNET::StatusInt PDIWT_PiledWharf_Core_Cpp::ECFrameWorkWraper::GetOrganizationECSchemaFile(BeFileNameP ecSchemaFile, WString ecSchemaFullName)
//{
//	if (!ConfigurationManager::IsVariableDefined(L"PDIWT_ORGANIZATION_ECSCHEMAPATH"))
//		return Bentley::DgnPlatformNET::StatusInt::Error;
//
//	WString _folderPaths;
//	if (ConfigurationManager::GetVariable(_folderPaths, L"PDIWT_ORGANIZATION_ECSCHEMAPATH", ConfigurationVariableLevel::Organization) != SUCCESS || 
//		WString::IsNullOrEmpty(_folderPaths.GetWCharCP()))
//		return Bentley::DgnPlatformNET::StatusInt::Error;
//	
//	//if (mdlFile_find(ecSchemaFile, ecSchemaFullName.GetWCharCP(), L"$(PDIWT_ORGANIZATION_ECSCHEMAPATH)", NULL, FINDFILEOPTION_CurrentDirectoryLast) != SUCCESS ||
//	//	ecSchemaFile == nullptr)
//	//	return Bentley::DgnPlatformNET::StatusInt::Success;
//	bvector<WString> _bvectorFolderPaths;
//	BeStringUtilities::Split(_folderPaths.GetWCharCP(), L";", _bvectorFolderPaths);
//	for (auto _folderName : _bvectorFolderPaths)
//	{
//		WString _ecschemafullName = _folderName + ecSchemaFullName;
//		if (mdlFile_find(ecSchemaFile, _ecschemafullName.GetWCharCP(), NULL, NULL, FINDFILEOPTION_CurrentDirectoryLast) != SUCCESS
//			|| ecSchemaFile == nullptr)
//			return Bentley::DgnPlatformNET::StatusInt::Success;
//	}
//	return Bentley::DgnPlatformNET::StatusInt::Error;
//}

