#pragma once
using namespace System;
using namespace System::Collections::Generic;

namespace BD = Bentley::DgnPlatformNET;

namespace PDIWT_PiledWharf_Core_Cpp {

	// #region Managed Code
	public enum struct SettingsWriteStatus { SUCCESS, ERROR };

	public enum struct PileTypeManaged
	{
		[ComponentModel::DescriptionAttribute("SquarePile")]
		SqaurePile = 1,
		[ComponentModel::DescriptionAttribute("TubePile")]
		TubePile,
		[ComponentModel::DescriptionAttribute("PHCTubePile")]
		PHCTubePile,
		[ComponentModel::DescriptionAttribute("SteelTubePile")]
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
		static BD::StatusInt CreatePile(PileTypeManaged pileType, Dictionary<PileTypeManaged, String^>^ pileMap, double pileWidth, double pileInsideDiameter, double pileConcreteCoreLength, Bentley::GeometryNET::DPoint3d topPoint, Bentley::GeometryNET::DPoint3d bottomPoint);
	};
}