#pragma once
using namespace System;
using namespace System::Collections::Generic;

namespace PDIWT_PiledWharf_Core_Cpp {

	// #region Managed Code
	public enum struct SettingsWriteStatus { SUCCESS, ERROR };

	public enum struct PileTypeManaged
	{
		SqaurePile = 1,
		TubePile,
		PHCTubePile,
		SteelTubePile
	};

	public ref class ECFrameWorkWraper
	{
	public:
		static SettingsWriteStatus WriteSettingsOnActiveModel(System::String^ ecSchemaFullName, System::String^ ecClassName, System::Collections::Generic::Dictionary<System::String^, System::String^>^ propList);
	};

	public ref class EntityCreation
	{
	public:
		static void CreatePie(PileTypeManaged pileType, double pileWidth, double pileInsideDiameter, double pileConcreteCoreLength, Bentley::GeometryNET::DPoint3d topPoint, Bentley::GeometryNET::DPoint3d bottomPoint);
	};
}