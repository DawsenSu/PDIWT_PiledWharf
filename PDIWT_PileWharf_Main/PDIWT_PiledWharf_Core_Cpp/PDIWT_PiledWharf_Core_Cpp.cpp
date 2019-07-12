#include "stdafx.h"

#include "PDIWT_PiledWharf_Core_Cpp.h"

PDIWT_PiledWharf_Core_Cpp::TestClass::TestClass()
{
	//throw gcnew System::NotImplementedException();
}

void PDIWT_PiledWharf_Core_Cpp::TestClass::OutputMessage(String ^message)
{
	pin_ptr<const WChar> msg = PtrToStringChars(message);
	mdlOutput_prompt(msg);
}

Bentley::DgnPlatformNET::StatusInt PDIWT_PiledWharf_Core_Cpp::ECFrameWorkWraper::ImportSChemaXMLFileOnActiveModel(String ^xmlFilePath)
{
	// TODO: insert return statement here
	pin_ptr<const WChar> _xmlfilepath = PtrToStringChars(xmlFilePath);
	ECSchemaPtr _ecschemaptr = nullptr;
	DgnECManagerR _dgnECManager = DgnECManager::GetManager();
	if (_dgnECManager.ReadSchemaFromXmlFile(_ecschemaptr, _xmlfilepath, ISessionMgr::GetActiveDgnFile()) != SchemaReadStatus::SCHEMA_READ_STATUS_Success)
		return Bentley::DgnPlatformNET::StatusInt::Error;
	if (_dgnECManager.ImportSchema(*_ecschemaptr, *ISessionMgr::GetActiveDgnFile()) != SchemaImportStatus::SCHEMAIMPORT_Success)
		return Bentley::DgnPlatformNET::StatusInt::Error;
	return Bentley::DgnPlatformNET::StatusInt::Success;
}
