#include "stdafx.h"

#include "PDIWT_PiledWharf_Core_Cpp.h"

#pragma unmanaged

//! ****************************************************
//!		ECFramework Part 
//! ****************************************************
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

//! ****************************************************
//!		Pile Creation Part
//! ****************************************************
//PDIWT_PiledWharf_Core_Cpp::PileEntityCreation::PileEntityCreation()
//{
//	_pileType = PileType::SqaurePile;
//	_pileWidth = 0;
//	_pileInsideDiameter = 0;
//	_pileConcreteCoreLength = 0;
//	_topPoint = DPoint3d::FromZero();
//	_bottomPoint = DPoint3d::FromZero();
//	InitSQLiteDb();
//}

PDIWT_PiledWharf_Core_Cpp::PileEntityCreation::~PileEntityCreation()
{
	sqlite3_close_v2(_db);
}

BentleyStatus PDIWT_PiledWharf_Core_Cpp::PileEntityCreation::CreatPile()
{
	ISolidKernelEntityPtr _solid;
	switch (_pileType)
	{
	case PileType::SqaurePile:
		if (SUCCESS != CreateSquarePile(_solid))
		{
			mdlOutput_error(L"Fail to Create square Pile!");
			return ERROR;
		}
		break;
	case PileType::TubePile:
		if (SUCCESS != CreateTubePile(_solid))
		{
			mdlOutput_error(L"Fail to Create tube Pile!");
			return ERROR;
		}
		break;
	case PileType::PHCTubePile:
		if (SUCCESS != CreatePHCTubePile(_solid))
		{
			mdlOutput_error(L"Fail to Create PHC Tube Pile!");
			return ERROR;
		}
		break;
	case PileType::SteelTubePile:
		if (SUCCESS != CreateSteelTubePile(_solid))
		{
			mdlOutput_error(L"Fail to Create Steel Tube Pile!");
			return ERROR;
		}
		break;
	default:
		break;
	}
	if (_solid != nullptr)
	{
		EditElementHandle _eeh;
		if (SUCCESS != CreateWarperCellElement(_eeh, _solid))
		{
			mdlOutput_error(L"Fail to Attach IFC Information!");
			return ERROR;
		}

		if (SUCCESS != _eeh.AddToModel())
		{
			mdlOutput_error(L"Fail to add to element");
			return ERROR;
		}

		if (SUCCESS != BuildECInstanceOnElement(_eeh))
		{
			mdlOutput_error(L"Fail to attach information to element");
			return ERROR;
		}
	}
	return SUCCESS;
}



BentleyStatus PDIWT_PiledWharf_Core_Cpp::PileEntityCreation::CreateSquarePile(ISolidKernelEntityPtr & out)
{
	//! There still problem for this
	DVec3d _vectorX = DVec3d::FromCrossProduct(DVec3d::UnitY(), DVec3d::FromStartEnd(_bottomPoint, _topPoint));
	DVec3d _vectorY = DVec3d::FromCrossProduct(DVec3d::UnitX(), DVec3d::FromStartEnd(_bottomPoint, _topPoint));
	double _nouse;
	if (!_vectorX.TryNormalize(_vectorX, _nouse) || !_vectorY.TryNormalize(_vectorY, _nouse))
		return ERROR;
	//mdlOutput_message(WPrintfString(L"(%f, %f, %f) -> [%f], original length: [%f]", _vectorY.x, _vectorY.y, _vectorY.z, _vectorY.Magnitude(), _nouse));
	DgnBoxDetail _dgnBoxDetail = DgnBoxDetail::InitFromCenters(_bottomPoint, _topPoint, _vectorX, _vectorY, _pileWidth, _pileWidth, _pileWidth, _pileWidth, true);
	ISolidPrimitivePtr _dgnBox = ISolidPrimitive::CreateDgnBox(_dgnBoxDetail);

	return SolidUtil::Create::BodyFromSolidPrimitive(out, *_dgnBox, *ACTIVEMODEL);
}

BentleyStatus PDIWT_PiledWharf_Core_Cpp::PileEntityCreation::CreateTubePile(ISolidKernelEntityPtr & out)
{
	DgnConeDetail _dgnConeDetail(_bottomPoint, _topPoint, _pileWidth / 2, _pileWidth / 2, true);
	ISolidPrimitivePtr _dgnCone = ISolidPrimitive::CreateDgnCone(_dgnConeDetail);
	return SolidUtil::Create::BodyFromSolidPrimitive(out, *_dgnCone, *ACTIVEMODEL);
}

BentleyStatus PDIWT_PiledWharf_Core_Cpp::PileEntityCreation::CreatePHCTubePile(ISolidKernelEntityPtr & out)
{
	DgnConeDetail _dgnConeDetailOuter(_bottomPoint, _topPoint, _pileWidth / 2, _pileWidth / 2, true);
	ISolidPrimitivePtr _dgnConeOuter = ISolidPrimitive::CreateDgnCone(_dgnConeDetailOuter);

	DgnConeDetail _dgnConeDetailInner(_bottomPoint, _topPoint, _pileInsideDiameter / 2, _pileInsideDiameter / 2, true);
	ISolidPrimitivePtr _dgnConeInner = ISolidPrimitive::CreateDgnCone(_dgnConeDetailInner);
	if (_pileInsideDiameter >= _pileWidth)
	{
		mdlOutput_error(L"Fail to create inner cone entities! Inner diameter is greater than outer diameter!");
		return ERROR;
	}
	ISolidKernelEntityPtr _dgnConeInnerEntity;
	if (SolidUtil::Create::BodyFromSolidPrimitive(out, *_dgnConeOuter, *ACTIVEMODEL) != SUCCESS
		|| SolidUtil::Create::BodyFromSolidPrimitive(_dgnConeInnerEntity, *_dgnConeInner, *ACTIVEMODEL) != SUCCESS)
		mdlOutput_error(L"Fail to create cone entities!");

	return SolidUtil::Modify::BooleanSubtract(out, &_dgnConeInnerEntity, 1);
}

BentleyStatus PDIWT_PiledWharf_Core_Cpp::PileEntityCreation::CreateSteelTubePile(ISolidKernelEntityPtr & out)
{
	DgnConeDetail _dgnConeDetailOuter(_bottomPoint, _topPoint, _pileWidth / 2, _pileWidth / 2, true);
	ISolidPrimitivePtr _dgnConeOuter = ISolidPrimitive::CreateDgnCone(_dgnConeDetailOuter);

	DSegment3d _segment = DSegment3d::From(_topPoint, _bottomPoint);

	if (_pileConcreteCoreLength < 0)
	{
		mdlOutput_error(L"Pile Concrete Core is less than 0!");
		return ERROR;
	}
	else if (_pileConcreteCoreLength == 0)
		return CreatePHCTubePile(out);
	else
	{
		if (abs(_segment.Length() - _pileConcreteCoreLength) < 1e-3)
			return CreateTubePile(out);
		else
		{
			double _length = _segment.Length();
			double _factor = abs(_segment.Length() - _pileConcreteCoreLength) / _segment.Length();
			DPoint3d _concreteCoreTopPoint = _segment.FractionToPoint(_factor);
			DgnConeDetail _dgnConeDetailInner(_concreteCoreTopPoint, _topPoint, _pileInsideDiameter / 2, _pileInsideDiameter / 2, true);
			ISolidPrimitivePtr _dgnConeInner = ISolidPrimitive::CreateDgnCone(_dgnConeDetailInner);

			ISolidKernelEntityPtr _dgnConeInnerEntity;
			if (SolidUtil::Create::BodyFromSolidPrimitive(out, *_dgnConeOuter, *ACTIVEMODEL) != SUCCESS
				|| SolidUtil::Create::BodyFromSolidPrimitive(_dgnConeInnerEntity, *_dgnConeInner, *ACTIVEMODEL) != SUCCESS)
				mdlOutput_error(L"Fail to create cone entities!");

			return SolidUtil::Modify::BooleanSubtract(out, &_dgnConeInnerEntity, 1);
		}
	}
	

}

BentleyStatus PDIWT_PiledWharf_Core_Cpp::PileEntityCreation::CreateWarperCellElement(EditElementHandleR out, ISolidKernelEntityPtr in)
{
	// Create the Cell Element which acts like a warper for whole pile entity.
	NormalCellHeaderHandler::CreateCellElement(out, L"Pile", _topPoint, RotMatrix::FromIdentity(), true, *ACTIVEMODEL);

	// Convert three dimensional entity to elementhandle
	EditElementHandle _entityeeh;
	if (SUCCESS != SolidUtil::Convert::BodyToElement(_entityeeh, *in, nullptr, *ACTIVEMODEL))
	{
		mdlOutput_error(L"Fail to convert body to element!");
		return ERROR;
	}
	// Create Line which represents the axis line of pile
	ICurvePrimitivePtr _linePtr = ICurvePrimitive::CreateLine(DSegment3d::From(_topPoint, _bottomPoint));
	EditElementHandle _lineeeh;
	if (SUCCESS != DraftingElementSchema::ToElement(_lineeeh, *_linePtr, nullptr, true, *ACTIVEMODEL))
	{
		mdlOutput_error(L"Fail to pile axis element!");
		return ERROR;
	}
	ElementPropertiesSetterPtr _linePropSetter = ElementPropertiesSetter::Create();
	_linePropSetter->SetElementClass(DgnElementClass::Construction);
	_linePropSetter->SetTransparency(1.0); //_line is invisible.
	_linePropSetter->Apply(_lineeeh);


	if (NormalCellHeaderHandler::AddChildElement(out, _entityeeh) != SUCCESS
		|| NormalCellHeaderHandler::AddChildElement(out, _lineeeh) != SUCCESS
		|| NormalCellHeaderHandler::AddChildComplete(out))
	{
		mdlOutput_error(L"Fail to add child element to cell!");
		return ERROR;
	}
	return SUCCESS;
}

BentleyStatus PDIWT_PiledWharf_Core_Cpp::PileEntityCreation::BuildECInstanceOnElement(EditElementHandleR inout)
{
	// Import IfcPort ECSchema into dgnModel and attach the IfcPile onto the entity.
	WString _ifcPortSchemaFullName(L"IfcPort.01.00.ecschema.xml");
	bvector<WString> _ifcPortSchemaPath;
	PDIWTECFramework::GetOrganizationECSchemaFile(_ifcPortSchemaFullName, WString(L"PDIWT_ORGANIZATION_ECSCHEMAPATH"), &_ifcPortSchemaPath);

	DgnECManagerR _dgnECManager = DgnECManager::GetManager();

	//ParseFull Schema Name
	WString _ifcPortSchemaName;
	uint32_t _ifcMajorVersion, _ifcMinorVersion;
	ECSchema::ParseSchemaFullName(_ifcPortSchemaName, _ifcMajorVersion, _ifcMinorVersion, _ifcPortSchemaFullName);
	SchemaInfo _ifcPortSchemaInfo(SchemaKey(_ifcPortSchemaName.GetWCharCP(), _ifcMajorVersion, _ifcMinorVersion), *ISessionMgr::GetActiveDgnFile());

	ECSchemaPtr _ifcPort;
	if (!_dgnECManager.IsSchemaContainedWithinFile(_ifcPortSchemaInfo, SCHEMAMATCHTYPE_Identical))
	{
		if (SchemaReadStatus::SCHEMA_READ_STATUS_Success != _dgnECManager.ReadSchemaFromXmlFile(_ifcPort, WString(_ifcPortSchemaPath.at(0) + _ifcPortSchemaFullName).GetWCharCP(), ISessionMgr::GetActiveDgnFile()))
		{
			mdlOutput_error(L"Can not load ifcPort Schema");
			return ERROR;
		}
		if (SchemaImportStatus::SCHEMAIMPORT_Success != _dgnECManager.ImportSchema(*_ifcPort, *ISessionMgr::GetActiveDgnFile()))
		{
			mdlOutput_error(L"Can not import ifcPort Schema");
			return ERROR;
		}
	}

	_ifcPort = _dgnECManager.LocateSchemaInDgnFile(_ifcPortSchemaInfo, SchemaMatchType::SCHEMAMATCHTYPE_Identical);


	DgnElementECInstancePtr _ifcPileElementInstancePtr = nullptr;
	//ECClassP _ifcPileClassP = _ifcPort->GetClassP(L"IfcPile");
	//DgnECInstanceEnablerP _ifcPileInstanceEnablerP = _dgnECManager.ObtainInstanceEnabler(*_ifcPileClassP, *ISessionMgr::GetActiveDgnFile());
	DgnECInstanceEnablerP _ifcPileInstanceEnablerP = _dgnECManager.ObtainInstanceEnablerByName(_ifcPortSchemaName.GetWCharCP(), L"IfcPile", *ISessionMgr::GetActiveDgnFile());
	StandaloneECInstanceR _ifcPileWIPECInstance = _ifcPileInstanceEnablerP->GetSharedWipInstance();

	//Build PropList and Set Values
	bmap<WString, WString> _proplist;
	double _uorpermm = ACTIVEMODEL->GetModelInfoCP()->GetUorPerMeter() / 1000;
	

	_proplist.Insert(L"Length", WPrintfString(L"%f",DSegment3d::From(_bottomPoint, _topPoint).Length()/_uorpermm));
	if (_pileType == PileType::PHCTubePile || _pileType == PileType::SteelTubePile || _pileType == PileType::TubePile)
		_proplist.Insert(L"OutsideDiameter", WPrintfString(L"%f", _pileWidth / _uorpermm));
	if (_pileType == PileType::PHCTubePile || _pileType == PileType::SteelTubePile)
	{
		_proplist.Insert(L"WallThickness", WPrintfString(L"%f", (_pileWidth - _pileInsideDiameter) / _uorpermm));
		_proplist.Insert(L"InnerDiameter", WPrintfString(L"%f", _pileInsideDiameter / _uorpermm));
	}
	if (_pileType == PileType::SqaurePile)
	{
		_proplist.Insert(L"CrossSectionLength", WPrintfString(L"%f", _pileWidth / _uorpermm));
		_proplist.Insert(L"CrossSectionWidth", WPrintfString(L"%f", _pileWidth / _uorpermm));
	}
	_proplist.Insert(L"TopElevation", WPrintfString(L"%f", _topPoint.z / _uorpermm));

	DVec3d _pileVec3d = DVec3d::FromStartEnd(_topPoint, _bottomPoint);
	double _theta = _pileVec3d.AngleTo(DVec3d::From(0, 0, -1));
	if (_theta == 0)
		_proplist.Insert(L"Skewness", WPrintfString(L"%0.2f", 0));
	else
		_proplist.Insert(L"Skewness", WPrintfString(L"%0.2f", 1/tan(_theta)));
	_proplist.Insert(L"PlanRotationAngle", WPrintfString(L"%f¡ã", _pileVec3d.AngleXY()));
	_proplist.Insert(L"BottomElevation", WPrintfString(L"%f", _bottomPoint.z / _uorpermm));

	double _pileVolume;
	MSElementDescrP _elementDescr = inout.GetElementDescrCP()->h.firstElem;
	while (_elementDescr != nullptr)
	{
		if (SUCCESS == mdlMeasure_volumeProperties(&_pileVolume, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, _elementDescr, 1e-4))
		{
			_proplist.Insert(L"Volume", WPrintfString(L"%f", _pileVolume / pow(ACTIVEMODEL->GetModelInfoCP()->GetUorPerMeter(),3)));
			break;
		}
		_elementDescr = _elementDescr->h.next;
	}
	
	_proplist.Insert(L"Code", GetCodeString(L"IfcPile"));
	//WString _pileTypeStr;
	//switch (_pileType)
	//{
	//case PDIWT_PiledWharf_Core_Cpp::SqaurePile:
	//	_pileTypeStr = L"Square Pile";
	//	break;
	//case PDIWT_PiledWharf_Core_Cpp::TubePile:
	//	_pileTypeStr = L"Round Pile";
	//	break;
	//case PDIWT_PiledWharf_Core_Cpp::PHCTubePile:
	//	_pileTypeStr = L"PHC Tube Pile";
	//	break;
	//case PDIWT_PiledWharf_Core_Cpp::SteelTubePile:
	//	_pileTypeStr = L"Steel Tube Pile";
	//	break;
	//default:
	//	_pileTypeStr = L"Unknown";
	//	break;
	//}
	_proplist.Insert(L"Type", _pileTypeMap[_pileType]);

	PDIWTECFramework::SetPropValueList(_ifcPileWIPECInstance, _proplist);
	DgnECInstanceStatus _status = _ifcPileInstanceEnablerP->CreateInstanceOnElement(&_ifcPileElementInstancePtr, _ifcPileWIPECInstance, inout);
	if (DgnECInstanceStatus::DGNECINSTANCESTATUS_Success != _status)
	{
		mdlOutput_error(L"Can not Create IfcPile Instance on pile element");
		return ERROR;
	}

	return SUCCESS;
}


BentleyStatus PDIWT_PiledWharf_Core_Cpp::PileEntityCreation::InitSQLiteDb()
{
	// Fetch data from SQLite database
	// 1. Get Variable Path
	WString _sqliteDatabase;
	if (SUCCESS != ConfigurationManager::GetVariable(_sqliteDatabase, L"PDIWT_ORGANIZATION_DATABASEPATH", ConfigurationVariableLevel::Organization))
	{
		mdlOutput_error(L"Can't get environmental predefined variables PDIWT_ORGANIZATION_DATABASEPATH!");
		return ERROR;
	}
	// 2.Connection
	Utf8String _sqliteFileName(_sqliteDatabase);
	_sqliteFileName.append("BIMClassificationAndCode.db");
	int _rc = sqlite3_open_v2(_sqliteFileName.c_str(), &_db,SQLITE_OPEN_READONLY,NULL);
	if (_rc)
	{
		mdlOutput_error(L"Can not open Db");
		return ERROR;
	}
	
	return SUCCESS;
}

WString PDIWT_PiledWharf_Core_Cpp::PileEntityCreation::GetCodeString(WString codeName)
{
	sqlite3_stmt *_pstmt;

	WString _level3Code, _level2Code, _level1Code, _previousLevelCodeName;
	//Obtain Level3 Code
	WPrintfString _sqlstatement(L"SELECT Code, PreviousLevelCodeName FROM IfcPortComponentAndEquipment_Level3 WHERE CodeName = '%s'", codeName);
	Utf8String _sqlstatementUtf8;
	//Utf8PrintfString  _sqlstatement("SELECT Code, PreviousLevelCodeName FROM IfcPortComponentAndEquipment_Level3 WHERE CodeName = '%s'", codeName);
	BeStringUtilities::WCharToUtf8(_sqlstatementUtf8, _sqlstatement);
	sqlite3_prepare_v2(_db, _sqlstatementUtf8.c_str(), (int)_sqlstatementUtf8.length(), &_pstmt, NULL);	
	while (sqlite3_step(_pstmt) == SQLITE_ROW)
	{
		BeStringUtilities::Utf8ToWChar(_level3Code, (Utf8CP)sqlite3_column_text(_pstmt,0));
		BeStringUtilities::Utf8ToWChar(_previousLevelCodeName, (Utf8CP)sqlite3_column_text(_pstmt, 1));
	}

	//Obtain Level2 Code 
	_sqlstatement = WPrintfString(L"SELECT Code, PreviousLevelCodeName FROM IfcPortComponentAndEquipment_Level2 WHERE CodeName = '%s'", _previousLevelCodeName);
	BeStringUtilities::WCharToUtf8(_sqlstatementUtf8, _sqlstatement);
	sqlite3_prepare_v2(_db, _sqlstatementUtf8.c_str(), (int)_sqlstatementUtf8.length(), &_pstmt, NULL);
	while (sqlite3_step(_pstmt) == SQLITE_ROW)
	{
		BeStringUtilities::Utf8ToWChar(_level2Code, (Utf8CP)sqlite3_column_text(_pstmt, 0));
		BeStringUtilities::Utf8ToWChar(_previousLevelCodeName, (Utf8CP)sqlite3_column_text(_pstmt, 1));
	}

	//Obtain Level1 Code
	_sqlstatement = WPrintfString(L"SELECT Code, PreviousLevelCodeName FROM IfcPortComponentAndEquipment_Level1 WHERE CodeName = '%s'", _previousLevelCodeName);
	BeStringUtilities::WCharToUtf8(_sqlstatementUtf8, _sqlstatement);
	sqlite3_prepare_v2(_db, _sqlstatementUtf8.c_str(), (int)_sqlstatementUtf8.length(), &_pstmt, NULL);
	while (sqlite3_step(_pstmt) == SQLITE_ROW)
	{
		BeStringUtilities::Utf8ToWChar(_level1Code, (Utf8CP)sqlite3_column_text(_pstmt, 0));
		BeStringUtilities::Utf8ToWChar(_previousLevelCodeName, (Utf8CP)sqlite3_column_text(_pstmt, 1));
	}

	sqlite3_finalize(_pstmt);

	WString _delimiter(L"/");

	return _level1Code + _delimiter + _level2Code + _delimiter + _level3Code;
}





