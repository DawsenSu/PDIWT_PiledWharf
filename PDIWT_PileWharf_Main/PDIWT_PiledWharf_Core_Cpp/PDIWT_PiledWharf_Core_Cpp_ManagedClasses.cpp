#include "stdafx.h"
#include "PDIWT_PiledWharf_Core_Cpp.h"
#include "PDIWT_PiledWharf_Core_Cpp_ManagedClasses.h"

PDIWT_PiledWharf_Core_Cpp::SettingsWriteStatus PDIWT_PiledWharf_Core_Cpp::ECFrameWorkWraper::WriteSettingsOnActiveModel(
	System::String^ ecSchemaFullName,
	System::String^ ecClassName,
	System::Collections::Generic::Dictionary<System::String^, System::String^>^ propList)
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

void PDIWT_PiledWharf_Core_Cpp::EntityCreation::CreatePile(PileTypeManaged pileType, double pileWidth, double pileInsideDiameter, double pileConcreteCoreLength, Bentley::GeometryNET::DPoint3d topPoint, Bentley::GeometryNET::DPoint3d bottomPoint)
{
	DPoint3d _topPointUnmanaged = DPoint3d::From(topPoint.X, topPoint.Y, topPoint.Z);
	DPoint3d _bottomPointUnmanaged = DPoint3d::From(bottomPoint.X, bottomPoint.Y, bottomPoint.Z);

	PileEntityCreation _pile(static_cast<PileType>(pileType), pileWidth, pileInsideDiameter, pileConcreteCoreLength, _topPointUnmanaged, _bottomPointUnmanaged);
	_pile.CreatPile();
}