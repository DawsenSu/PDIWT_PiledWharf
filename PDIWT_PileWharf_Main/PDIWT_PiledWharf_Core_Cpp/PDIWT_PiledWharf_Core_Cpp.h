#pragma once

#include "stdafx.h"
#include <cstdlib>
#include <cmath>

namespace BDNative = Bentley::DgnPlatform;
namespace BECN = Bentley::ECN;

namespace PDIWT_PiledWharf_Core_Cpp {

	// @brief ECFramework Helper 
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

		// Set Property values to given ecInstance
		// @param[in]	ecInstance	The instance which will hold the given property values
		// @param[in]	propList	Properties and values list
		static void SetPropValueList(IECInstanceR ecInstance, bmap<WString, WString> propList);
	};

	public enum PileType
	{
		SqaurePile = 1,
		TubePile,
		PHCTubePile,
		SteelTubePile
	};
	// @brief Pile Creation class
	public class PileEntityCreation
	{
	public:
		//PileEntityCreation();
		PileEntityCreation(PileType pileType, bmap<PileType,WString> pileTypeMap, double pileWidth, double pileInsideDiameter, double pileConcreteCoreLength, DPoint3d topPoint, DPoint3d bottomPoint)
			:_pileType(pileType), _pileTypeMap(pileTypeMap), _pileWidth(pileWidth), _pileInsideDiameter(pileInsideDiameter), _pileConcreteCoreLength(pileConcreteCoreLength), _topPoint(topPoint), _bottomPoint(bottomPoint)
		{
			InitSQLiteDb();
			//double _uorpermm = ACTIVEMODEL->GetModelInfoCP()->GetUorPerMeter() / 1000;
			//_pileWidth *= _uorpermm;
			//_pileInsideDiameter *= _uorpermm;
			//_pileConcreteCoreLength *= _uorpermm;
		};
		~PileEntityCreation();
		BentleyStatus CreatPile();
	private:
		BentleyStatus CreateSquarePile(ISolidKernelEntityPtr& out);
		BentleyStatus CreateTubePile(ISolidKernelEntityPtr& out);
		BentleyStatus CreatePHCTubePile(ISolidKernelEntityPtr& out);
		BentleyStatus CreateSteelTubePile(ISolidKernelEntityPtr& out);
		BentleyStatus CreateWarperCellElement(EditElementHandleR out, ISolidKernelEntityPtr in);
		BentleyStatus BuildECInstanceOnElement(EditElementHandleR inout);


		BentleyStatus InitSQLiteDb();
		WString GetCodeString(WString codeName);
	private:
		// Engineering data
		PileType _pileType;
		double _pileWidth;
		double _pileInsideDiameter;
		double _pileConcreteCoreLength;
		DPoint3d _topPoint, _bottomPoint;
		bmap<PileType, WString> _pileTypeMap;
		sqlite3 *_db;
	};
}
