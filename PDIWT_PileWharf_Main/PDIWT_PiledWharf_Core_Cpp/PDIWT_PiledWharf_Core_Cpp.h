#pragma once

#include "stdafx.h"
#include <cstdlib>
#include <cmath>


using namespace System;
using namespace System::Collections::Generic;

namespace PDIWT_PiledWharf_Core_Cpp {
	public class PDIWTECFramework
	{
	public:
		// Write designated ecclass value onto active model
		// @param[in]	ecschemaFullName	Full name of schema that you want to import
		// @param[in]	ecClassName			Name of class that you want to import
		// @param[in]	propList			List of pairs of property name and its value
		//	@Returns	true, if writing it successfully.
		static StatusInt WriteSettingsOnActiveModel(WString ecschemaFullName, WString ecClassName, bmap<WString, WString> propList);

		// Get Organization Level ECSchemaFile from defined environment variables
		// @param[in]	ecSchemaFullName	the full schema name, like(xxx.01.00.schema.xml)
		// @param[in]	definedVariable		variable name defines the folder path of stored schema XML file
		// @param[out]	ecSchemaFilePaths	collections of paths of schema xml file
		// @Return	true, if gets the file paths
		static StatusInt GetOrganizationECSchemaFile(WString ecSchemaFullName, WString definedVariable, bvector<WString>* ecSchemaFilePaths);
	private:
		// Set Property values to given ecInstance
		// @param[in]	ecInstance	The instance which will hold the given property values
		// @param[in]	propList	Properties and values list
		static void SetPropValueList(IECInstanceR ecInstance, bmap<WString, WString> propList);
	};

	public enum struct SettingsWriteStatus { SUCCESS, ERROR };

	public ref class ECFrameWorkWraper
	{
	public:
		static SettingsWriteStatus WriteSettingsOnActiveModel(String^ ecSchemaFullName, String^ ecClassName, Dictionary<String^, String^>^ propList);
	};
}
