/*--------------------------------------------------------------------------------------+
|
|     $Source: MstnExamples/Elements/ElementsExample/ElementsExample.h $
|
|  $Copyright: (c) 2015 Bentley Systems, Incorporated. All rights reserved. $
|
+--------------------------------------------------------------------------------------*/
#pragma once

#include <Mstn/MdlApi/MdlApi.h>
#include <DgnView/AccuDraw.h>
#include <DgnView/DgnElementSetTool.h>
#include <DgnPlatform/ITextEdit.h>
#include <DgnPlatform/TextHandlers.h>
#include <Mstn/isessionmgr.h>

#include <DgnPlatform\DgnPlatformAPI.h>
#include <Mstn/ElementPropertyUtils.h>

#include "ElementsExampleids.h"
#include "ElementsExamplecmd.h"

USING_NAMESPACE_BENTLEY_DGNPLATFORM;
USING_NAMESPACE_BENTLEY_MSTNPLATFORM;
USING_NAMESPACE_BENTLEY_MSTNPLATFORM_ELEMENT;

//Create

void elementsExampleCreateMlineNone (WCharCP);
void elementsExampleCreateMlineActive (WCharCP);
void elementsExampleCreateMlineCustom (WCharCP);

void elementsExampleCreateShapeNone (WCharCP);
void elementsExampleCreateShapeActive (WCharCP);
void elementsExampleCreateShapeCustom (WCharCP);

//Copy

void elementsExampleCopy (WCharCP);

//Edit

void elementsExampleEditElementDisplayDefault (WCharCP);
void elementsExampleEditElementDisplayActive (WCharCP);
void elementsExampleEditElementDisplayCustom (WCharCP);
void elementsExampleEditElementGeometry (WCharCP);
void elementsExampleEditElementFillNone (WCharCP);
void elementsExampleEditElementFillSolid (WCharCP);
void elementsExampleEditElementFillPattern (WCharCP);
void elementsExampleEditElementFillGradient (WCharCP);

//Query

void elementsExampleQueryElementDisplay (WCharCP);
void elementsExampleQueryElementGeometry (WCharCP);
void elementsExampleQueryElementFill (WCharCP);

