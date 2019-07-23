/*--------------------------------------------------------------------------------------+
|
|     $Source: MstnExamples/Elements/ElementsExample/ElementsExample.cpp $
|
|  $Copyright: (c) 2015 Bentley Systems, Incorporated. All rights reserved. $
|
+--------------------------------------------------------------------------------------*/
#include "ElementsExample.h"

/*---------------------------------------------------------------------------------**//**
* Map key-in to function
* Commands
+---------------+---------------+---------------+---------------+---------------+------*/
static MdlCommandNumber s_commandNumbers[] =
    {
    {elementsExampleCreateShapeNone            , CMD_ELEMENTSEXAMPLE_SHAPE_CREATE_NONE},
    {elementsExampleCreateShapeActive          , CMD_ELEMENTSEXAMPLE_SHAPE_CREATE_ACTIVE},
    {elementsExampleCreateShapeCustom          , CMD_ELEMENTSEXAMPLE_SHAPE_CREATE_CUSTOM},
    {elementsExampleCreateMlineNone            , CMD_ELEMENTSEXAMPLE_MULTILINE_CREATE_NONE},
    {elementsExampleCreateMlineActive          , CMD_ELEMENTSEXAMPLE_MULTILINE_CREATE_ACTIVE},
    {elementsExampleCreateMlineCustom          , CMD_ELEMENTSEXAMPLE_MULTILINE_CREATE_CUSTOM},
    {elementsExampleCopy                       , CMD_ELEMENTSEXAMPLE_ELEMENT_COPY},
    {elementsExampleEditElementDisplayDefault  , CMD_ELEMENTSEXAMPLE_ELEMENT_EDIT_DISPLAYDEFAULT},
    {elementsExampleEditElementDisplayActive   , CMD_ELEMENTSEXAMPLE_ELEMENT_EDIT_DISPLAYACTIVE},
    {elementsExampleEditElementDisplayCustom   , CMD_ELEMENTSEXAMPLE_ELEMENT_EDIT_DISPLAYCUSTOM},
    {elementsExampleEditElementGeometry        , CMD_ELEMENTSEXAMPLE_ELEMENT_EDIT_GEOMETRY},
    {elementsExampleEditElementFillNone        , CMD_ELEMENTSEXAMPLE_ELEMENT_EDIT_FILLNONE},
    {elementsExampleEditElementFillSolid       , CMD_ELEMENTSEXAMPLE_ELEMENT_EDIT_FILLSOLID},
    {elementsExampleEditElementFillPattern     , CMD_ELEMENTSEXAMPLE_ELEMENT_EDIT_FILLPATTERN},
    {elementsExampleEditElementFillGradient    , CMD_ELEMENTSEXAMPLE_ELEMENT_EDIT_FILLGRADIENT},
    {elementsExampleQueryElementDisplay        , CMD_ELEMENTSEXAMPLE_ELEMENT_QUERY_DISPLAY},
    {elementsExampleQueryElementGeometry       , CMD_ELEMENTSEXAMPLE_ELEMENT_QUERY_GEOMETRY},
    {elementsExampleQueryElementFill           , CMD_ELEMENTSEXAMPLE_ELEMENT_QUERY_FILL},
    // end of list
    0
    };

/*---------------------------------------------------------------------------------**//**
* MdlMain
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
extern "C" void MdlMain (int argc, WCharCP argv[])
    {
    RscFileHandle rfHandle;

    mdlResource_openFile (&rfHandle, NULL, RSC_READONLY);
    mdlState_registerStringIds (STRINGLISTID_Commands, STRINGLISTID_Prompts);
    mdlSystem_registerCommandNumbers (s_commandNumbers);
    mdlParse_loadCommandTable (NULL);
    }
