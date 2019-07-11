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
