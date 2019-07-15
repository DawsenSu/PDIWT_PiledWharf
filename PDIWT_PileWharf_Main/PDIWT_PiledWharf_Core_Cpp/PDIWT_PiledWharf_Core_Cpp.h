#pragma once

#include "stdafx.h"
#include <cstdlib>
#include <cmath>


using namespace System;
using namespace System::Collections::Generic;

namespace PDIWT_PiledWharf_Core_Cpp {
	public class PDIWTECFramework
	{
		// TODO: Add your methods for this class here.
	public:
		static StatusInt WriteSettingsOnActiveModel(bmap<WString, double> propList);
	private:
		static StatusInt ImportSChemaXMLFileOnActiveModel(WString xmlFilePath);
		static StatusInt GetOrganizationECSchemaFile(BeFileNameR ecSchemaFile, WString ecSchemaFullName);
	};

	public ref class ECFrameWorkWraper
	{
	public:
		static Bentley::DgnPlatformNET::StatusInt WriteSettingsOnActiveModel(Dictionary<String^, double>^ propList);
	};
}
