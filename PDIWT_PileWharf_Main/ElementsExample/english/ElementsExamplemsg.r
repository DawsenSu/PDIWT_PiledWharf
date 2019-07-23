/*--------------------------------------------------------------------------------------+
|
|     $Source: MstnExamples/Elements/ElementsExample/english/ElementsExamplemsg.r $
|
|  $Copyright: (c) 2015 Bentley Systems, Incorporated. All rights reserved. $
|
+--------------------------------------------------------------------------------------*/
#include <Mstn\MdlApi\rscdefs.r.h>
#include "ElementsExampleids.h"

/*----------------------------------------------------------------------+
|    Command Names                                                      |
+----------------------------------------------------------------------*/
MessageList STRINGLISTID_Commands =
{
    {
    { CMDNAME_ElementsExampleCreateMlineNone,              "Create Mline With No Properties" },
    { CMDNAME_ElementsExampleCreateMlineActive,            "Create Mline With Active Properties" },
    { CMDNAME_ElementsExampleCreateMlineCustom,            "Create Mline With Custom Properties" },
    { CMDNAME_ElementsExampleCreateShapeNone,              "Create Shape With No Properties" },
    { CMDNAME_ElementsExampleCreateShapeActive,            "Create Shape With Active Properties" },
    { CMDNAME_ElementsExampleCreateShapeCustom,            "Create Shape With Custom Properties" },
    { CMDNAME_ElementsExampleCopy,                         "Copy Element" },
    { CMDNAME_ElementsExampleQueryElementDisplay,          "Query Element Display Properties" },
    { CMDNAME_ElementsExampleQueryElementGeometry,         "Query Element Geometric Properties" },
    { CMDNAME_ElementsExampleQueryElementFill,             "Query Element Fill Properties" },
    { CMDNAME_ElementsExampleEditElementDisplay,           "Edit Element Display Properties" },
    { CMDNAME_ElementsExampleEditElementGeometry,          "Edit Element Geometric Properties" },
    { CMDNAME_ElementsExampleEditElementFill,              "Edit Element Fill Properties" },
    }
};

MessageList STRINGLISTID_Prompts =
{
    {
    { PROMPT_FirstPoint,            "Enter first point" },
    { PROMPT_NextPoint,             "Enter next point" },
    { PROMPT_OppositeCorner,        "Enter opposite corner" },
    { PROMPT_NextPointOrReset,      "Enter next point or reset to complete" },
    { PROMPT_SelectElement,         "Select element to copy" },
    { PROMPT_CopyPoint,             "Enter data point to copy the selected element" },
    }
};



