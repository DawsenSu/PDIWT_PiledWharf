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

BD::StatusInt PDIWT_PiledWharf_Core_Cpp::ECFrameWorkWraper::ImportECSchemaInActiveDgn(System::String^ ecSchemaFullName)
{
	pin_ptr<const WCHAR> _ecSchemaFullName = PtrToStringChars(ecSchemaFullName);
	return (BD::StatusInt)PDIWTECFramework::ImportECSChemaInActiveDgn(WString(_ecSchemaFullName));
}

BD::StatusInt PDIWT_PiledWharf_Core_Cpp::EntityCreation::CreatePile(String^ pileName, PileTypeManaged pileType, double pileWidth, double pileInsideDiameter, double pileConcreteCoreLength, Bentley::GeometryNET::DPoint3d topPoint, Bentley::GeometryNET::DPoint3d bottomPoint)
{
	DPoint3d _topPointUnmanaged = DPoint3d::From(topPoint.X, topPoint.Y, topPoint.Z);
	DPoint3d _bottomPointUnmanaged = DPoint3d::From(bottomPoint.X, bottomPoint.Y, bottomPoint.Z);

	pin_ptr<const WChar> _pileNameStr = PtrToStringChars(pileName);
	//bmap<PileType, WString> _bPileTypeMap;
	//for each (auto _item in pileMap)
	//{
	//	pin_ptr<const WChar> _pileTypeString = PtrToStringChars(_item.Value);
	//	_bPileTypeMap.Insert(static_cast<PileType>(_item.Key), _pileTypeString);
	//}

	PileEntityCreation _pile(_pileNameStr,static_cast<PileType>(pileType),pileWidth, pileInsideDiameter, pileConcreteCoreLength, _topPointUnmanaged, _bottomPointUnmanaged);
	BentleyStatus _status = _pile.CreatPile();
	return (BD::StatusInt)_status;
}

PDIWT_PiledWharf_Core_Cpp::TransientSegmentElement::TransientSegmentElement()
{
	_trans = NULL;
}

void PDIWT_PiledWharf_Core_Cpp::TransientSegmentElement::Show(System::IntPtr% ElementRefptr)
{
	ElementRefP _ptr = static_cast<ElementRefP>(ElementRefptr.ToPointer());
	_trans = mdlTransient_addElemDescr(NULL, MSElementDescr::Allocate(_ptr), 0, 0x00ff, DRAW_MODE_Normal, true, false, 1);
}

void PDIWT_PiledWharf_Core_Cpp::TransientSegmentElement::Free()
{
	if (_trans != NULL)
	{
		pin_ptr<TransDescrP> _p = &_trans;
		mdlTransient_free(_p, true);
	}
}