#pragma once

#define winNT	1
#include <Mstn\MstnDefs.h>
#include <Mstn\MdlApi\MdlApi.h>
#include <Mstn\MstnPlatformAPI.h>
#include <DgnPlatform\DgnPlatformApi.h>
#include <Mstn\PSolid\mssolid.h>
#include <Mstn\PSolid\mssolid.fdf>
#include <PSolid/PSolidCoreAPI.h>

#include <DgnView/DgnElementSetTool.h>
#include <DgnView/DgnTool.h>
#include <DgnView/AccuDraw.h>
#include <DgnView/AccuSnap.h>

#include <vcclr.h>
#include <msclr/marshal.h>
#include <msclr/marshal_cppstd.h>
#include <vcruntime_string.h>

#include <DawsenSu/sqlite3.h>

USING_NAMESPACE_BENTLEY
USING_NAMESPACE_BENTLEY_DGNPLATFORM
USING_NAMESPACE_BENTLEY_MSTNPLATFORM
USING_NAMESPACE_BENTLEY_MSTNPLATFORM_ELEMENT
USING_NAMESPACE_BENTLEY_ECOBJECT
