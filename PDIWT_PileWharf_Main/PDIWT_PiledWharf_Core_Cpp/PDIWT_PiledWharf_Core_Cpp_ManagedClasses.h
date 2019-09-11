#pragma once
using namespace System;
using namespace System::Collections::Generic;

namespace BD = Bentley::DgnPlatformNET;
namespace BG = Bentley::GeometryNET;

namespace PDIWT_PiledWharf_Core_Cpp {

	// #region Managed Code
	public enum struct SettingsWriteStatus { SUCCESS, ERROR };

	public enum struct PileTypeManaged
	{
		[ComponentModel::DescriptionAttribute("SquarePile")]
		SqaurePile = 0,
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
		static BD::StatusInt ImportECSchemaInActiveDgn(System::String^ ecSchemaFullName);
	};

	public ref class EntityCreation
	{
	public:
		static BD::StatusInt CreatePile(String^ pileName, PileTypeManaged pileType, double pileWidth, double pileInsideDiameter, double pileConcreteCoreLength, Bentley::GeometryNET::DPoint3d topPoint, Bentley::GeometryNET::DPoint3d bottomPoint);
	};

	public ref class TransientSegmentElement
	{
	public:
		TransientSegmentElement();
		void Show(System::IntPtr% ElementRefptr);
		void Free();
	private:
		//BentleyStatus CreateTransientElement(EditElementHandleR out);
		//BG::DSegment3d _internalSeg;
		TransDescrP _trans;
	};
}