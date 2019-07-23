/*--------------------------------------------------------------------------------------+
|
|     $Source: MstnExamples/Elements/ElementsExample/ElementsExampleCreateTool.cpp $
|
|  $Copyright: (c) 2015 Bentley Systems, Incorporated. All rights reserved. $
|
+--------------------------------------------------------------------------------------*/
#include "ElementsExample.h"

USING_NAMESPACE_BENTLEY_DGNPLATFORM;
USING_NAMESPACE_BENTLEY_MSTNPLATFORM;
USING_NAMESPACE_BENTLEY_MSTNPLATFORM_ELEMENT;

/*=================================================================================**//**
* Helper class to create different elements.
* See MicroStation API documentation for a detailed description of each element Create method.
* They are mostly in the form XYZHandler::CreateXYZElement() for some element XYZ.
* @bsiclass                                                               Bentley Systems
+===============+===============+===============+===============+===============+======*/
class ElementsCreateHelper
{

public:

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
static bool CreateShapeElement (EditElementHandleR eeh, bvector<DPoint3d> points, bool applyActiveSettings);

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
static bool CreateMultilineElement (EditElementHandleR eeh, bvector<DPoint3d> points, bool applyActiveSettings);

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
static void SetElementProperties (EditElementHandleR eeh, WCharCP unparsed);

};

/*---------------------------------------------------------------------------------**//**
* Create shape element with supplied corner points optionally applying active settings.
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
bool ElementsCreateHelper::CreateShapeElement (EditElementHandleR eeh, bvector<DPoint3d> points, bool applyActiveSettings)
    {
    DgnModelP model = ISessionMgr::GetActiveDgnModelP ();

    if (SUCCESS != ShapeHandler::CreateShapeElement (eeh, NULL, points.data(), points.size(), model->Is3d (), *model))
        return false;

    if(applyActiveSettings)
        ElementPropertyUtils::ApplyActiveSettings (eeh);

    return true;
    }

/*---------------------------------------------------------------------------------**//**
* Create Multiline element with supplied points optionally applying active settings.
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
bool ElementsCreateHelper::CreateMultilineElement (EditElementHandleR eeh, bvector<DPoint3d> points, bool applyActiveSettings)
    {
    DgnModelP model = ISessionMgr::GetActiveDgnModelP ();

    double styleScale = 1.0;
    DVec3d normal = DVec3d::From(0, 0, 1);
    MultilineStylePtr activeStyle = MultilineStyle::GetSettings (*(ISessionMgr::GetActiveDgnFile ()));

    if (SUCCESS != MultilineHandler::CreateMultilineElement (eeh, NULL, *activeStyle, styleScale, normal, points.data(), (int)points.size(), model->Is3d (), *model))
        return false;

    if(applyActiveSettings)
        ElementPropertyUtils::ApplyActiveSettings (eeh);

    return true;
    }

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
void ElementsCreateHelper::SetElementProperties (EditElementHandleR eeh, WCharCP unparsed)
    {
    int intValue = -1;
    swscanf(unparsed, L"%ld", &intValue);

    if(-1 < intValue)
        {
        UInt32 color  = (UInt32) (intValue % 256);
        UInt32 weight = (UInt32) (intValue % 32);
        Int32  style  = (Int32) (intValue % 8);
        ElementPropertiesSetterPtr remapper = ElementPropertiesSetter::Create ();
        remapper->SetColor (color);
        remapper->SetWeight (weight);
        remapper->SetLinestyle (style, NULL);
        remapper->Apply (eeh);
        }
    }

enum ElementPropsOptions
    {
    ElementPropsOptions_None,
    ElementPropsOptions_Custom,
    ElementPropsOptions_Active
    };

/*=================================================================================**//**
* Tool class to create a shape element.
* 1. Key-in the following
*       ELEMENTSEXAMPLE SHAPE CREATE NONE
*       ELEMENTSEXAMPLE SHAPE CREATE ACTIVE
*       ELEMENTSEXAMPLE SHAPE CREATE CUSTOM
* 2. Enter two data points i.e. Top-left and Right-bottom opposite corners.
* A shape element is created with 1) no properties 2) or active properties 3) or with some constant properties.
* @bsiclass                                                               Bentley Systems
+===============+===============+===============+===============+===============+======*/
struct ElementsExampleCreateShapeTool : DgnPrimitiveTool
{
protected:

ElementPropsOptions m_elemProps;
WString             m_propertyString;

bvector<DPoint3d>   m_points;

ElementsExampleCreateShapeTool (int toolName, int toolPrompt, ElementPropsOptions elemProps) : DgnPrimitiveTool (toolName, toolPrompt), m_elemProps(elemProps) {}

virtual void _OnPostInstall () override;
virtual void _OnRestartTool () override {InstallNewInstance (GetToolId (), GetToolPrompt (), m_elemProps, m_propertyString.c_str());}
virtual bool _OnDataButton (DgnButtonEventCR ev) override;
virtual bool _OnResetButton (DgnButtonEventCR ev) override {_OnRestartTool (); return true;}
virtual void _OnDynamicFrame (DgnButtonEventCR ev) override;

bool CreateShapeElement (EditElementHandleR eeh, bvector<DPoint3d> const& points);
void SetupAndPromptForNextAction ();
void SetPropertyString(WCharCP unparsed);
public:

static void InstallNewInstance (int toolId, int toolPrompt, ElementPropsOptions elemProps, WCharCP unparsed);

};

/*---------------------------------------------------------------------------------**//**
* Calculate shape corner points from first point and opposite corner point
* and call helper method to create the shape.
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
bool ElementsExampleCreateShapeTool::CreateShapeElement (EditElementHandleR eeh, bvector<DPoint3d> const& points)
    {
    if (2 != points.size ())
        return false;

    bvector<DPoint3d> somePoints;
    somePoints.push_back(points.at(0));
    somePoints.push_back(points.at(1));
    somePoints.push_back(points.at(1));
    somePoints.push_back(points.at(1));
    somePoints.push_back(points.at(0));

    somePoints[1].y = somePoints[0].y;
    somePoints[3].x = somePoints[0].x;

    bool applyActiveSettings = (ElementPropsOptions_Active == m_elemProps) ? true : false;

    if (!ElementsCreateHelper::CreateShapeElement (eeh, somePoints, applyActiveSettings))
        return false;

    if(ElementPropsOptions_Custom == m_elemProps)
        ElementsCreateHelper::SetElementProperties (eeh, m_propertyString.c_str());

    return true;
    }

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
void ElementsExampleCreateShapeTool::SetupAndPromptForNextAction ()
    {
    UInt32      msgId;
    msgId = PROMPT_FirstPoint;

    if(0 != m_points.size())
        msgId = PROMPT_OppositeCorner;

    mdlOutput_rscPrintf (MSG_PROMPT, NULL, STRINGLISTID_Prompts, msgId);
    }

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
void ElementsExampleCreateShapeTool::_OnDynamicFrame (DgnButtonEventCR ev)
    {
    bvector<DPoint3d>   tmpPts = m_points;
    EditElementHandle   eeh;

    tmpPts.push_back (*ev.GetPoint ()); // Use current button location as end point.

    if (!CreateShapeElement (eeh, tmpPts))
        return;

    RedrawElems redrawElems;

    redrawElems.SetDynamicsViews (IViewManager::GetActiveViewSet (), ev.GetViewport ()); // Display in all views, draws to cursor view first...
    redrawElems.SetDrawMode (DRAW_MODE_TempDraw);
    redrawElems.SetDrawPurpose (DrawPurpose::Dynamics);

    redrawElems.DoRedraw (eeh);
    }

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
bool ElementsExampleCreateShapeTool::_OnDataButton (DgnButtonEventCR ev)
    {
    if (0 == m_points.size ())
        _BeginDynamics (); // Start dynamics on first point. Enables AccuDraw and triggers _OnDynamicFrame being called.

    m_points.push_back (*ev.GetPoint ()); // Save current data point location.
    SetupAndPromptForNextAction ();

    if (m_points.size () < 2)
        return false;

    EditElementHandle   eeh;

    if (CreateShapeElement (eeh, m_points))
        eeh.AddToModel (); // Add new shape element to active model.

    m_points.clear ();

    return _CheckSingleShot ();
    }

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
void ElementsExampleCreateShapeTool::_OnPostInstall ()
    {
    AccuSnap::GetInstance ().EnableSnap (true); // Enable snapping for create tools.

    __super::_OnPostInstall ();
    }

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
void ElementsExampleCreateShapeTool::SetPropertyString(WCharCP unparsed)
    {
    m_propertyString = unparsed;
    }

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
void ElementsExampleCreateShapeTool::InstallNewInstance (int toolId, int toolPrompt, ElementPropsOptions elemProps, WCharCP unparsed)
    {
    ElementsExampleCreateShapeTool* tool = new ElementsExampleCreateShapeTool (toolId, toolPrompt, elemProps);

    if(ElementPropsOptions_Custom == elemProps)
        tool->SetPropertyString(unparsed);

    tool->InstallTool ();
    }

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
Public void elementsExampleCreateShapeNone (WCharCP unparsed)
    {
    ElementsExampleCreateShapeTool::InstallNewInstance (CMDNAME_ElementsExampleCreateShapeNone, PROMPT_FirstPoint, ElementPropsOptions_None, unparsed);
    }

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
Public void elementsExampleCreateShapeActive (WCharCP unparsed)
    {
    ElementsExampleCreateShapeTool::InstallNewInstance (CMDNAME_ElementsExampleCreateShapeActive, PROMPT_FirstPoint, ElementPropsOptions_Active, unparsed);
    }

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
Public void elementsExampleCreateShapeCustom (WCharCP unparsed)
    {
    ElementsExampleCreateShapeTool::InstallNewInstance (CMDNAME_ElementsExampleCreateShapeCustom, PROMPT_FirstPoint, ElementPropsOptions_Custom, unparsed);
    }

/*=================================================================================**//**
* Tool class to create a multiline element.
* 1. Key-in the following
*       ELEMENTSEXAMPLE MULTILINE CREATE NONE
*       ELEMENTSEXAMPLE MULTILINE CREATE ACTIVE
*       ELEMENTSEXAMPLE MULTILINE CREATE CUSTOM
* 2. Enter 2 or more data points
* A multiline element is created with 1) no properties 2) or active properties 3) or with some constant properties.
* @bsiclass                                                               Bentley Systems
+===============+===============+===============+===============+===============+======*/
struct ElementsExampleCreateMlineTool : DgnPrimitiveTool
{
protected:

ElementPropsOptions m_elemProps;
WString             m_propertyString;

bvector<DPoint3d>   m_points;

ElementsExampleCreateMlineTool (int toolName, int toolPrompt, ElementPropsOptions elemProps) : DgnPrimitiveTool (toolName, toolPrompt), m_elemProps(elemProps) {}

virtual void _OnPostInstall () override;
virtual void _OnRestartTool () override {InstallNewInstance (GetToolId (), GetToolPrompt (), m_elemProps, m_propertyString.c_str());}
virtual bool _OnDataButton (DgnButtonEventCR ev) override;
virtual bool _OnResetButton (DgnButtonEventCR ev) override;
virtual void _OnDynamicFrame (DgnButtonEventCR ev) override;

bool CreateMlineElement (EditElementHandleR eeh, bvector<DPoint3d> const& points);
void SetupAndPromptForNextAction ();
void SetPropertyString(WCharCP unparsed);

public:

static void InstallNewInstance (int toolId, int toolPrompt, ElementPropsOptions elemProps, WCharCP unparsed);

};

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
bool ElementsExampleCreateMlineTool::CreateMlineElement (EditElementHandleR eeh, bvector<DPoint3d> const& points)
    {
    if (points.size () < 2)
        return false;

    bool applyActiveSettings = (ElementPropsOptions_Active == m_elemProps) ? true : false;

    if (!ElementsCreateHelper::CreateMultilineElement (eeh, points, applyActiveSettings))
        return false;

    if(ElementPropsOptions_Custom == m_elemProps)
        ElementsCreateHelper::SetElementProperties (eeh, m_propertyString.c_str());

    return true;
    }

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
void ElementsExampleCreateMlineTool::SetupAndPromptForNextAction ()
    {
    UInt32      msgId;
    msgId = PROMPT_FirstPoint;

    if(1 == m_points.size())
        msgId = PROMPT_NextPoint;
    else
        msgId = PROMPT_NextPointOrReset;

    mdlOutput_rscPrintf (MSG_PROMPT, NULL, STRINGLISTID_Prompts, msgId);
    }

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
void ElementsExampleCreateMlineTool::_OnDynamicFrame (DgnButtonEventCR ev)
    {
    bvector<DPoint3d>   tmpPts = m_points;
    EditElementHandle   eeh;

    tmpPts.push_back (*ev.GetPoint ()); // Use current button location as end point.

    if (!CreateMlineElement (eeh, tmpPts))
        return;

    RedrawElems redrawElems;

    redrawElems.SetDynamicsViews (IViewManager::GetActiveViewSet (), ev.GetViewport ()); // Display in all views, draws to cursor view first...
    redrawElems.SetDrawMode (DRAW_MODE_TempDraw);
    redrawElems.SetDrawPurpose (DrawPurpose::Dynamics);

    redrawElems.DoRedraw (eeh);
    }

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
bool ElementsExampleCreateMlineTool::_OnDataButton (DgnButtonEventCR ev)
    {
    m_points.push_back (*ev.GetPoint ());

    if (1 == m_points.size ())
        {
        _BeginDynamics (); // Start dynamics on first point.
        }

    SetupAndPromptForNextAction ();

    return false;
    }

/*---------------------------------------------------------------------------------**//**
* Element is actually created on Reset.
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
bool ElementsExampleCreateMlineTool::_OnResetButton (DgnButtonEventCR ev)
    {
    EditElementHandle   eeh;

    if (CreateMlineElement (eeh, m_points))
        eeh.AddToModel ();

    _OnRestartTool ();

    return true;
    }

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
void ElementsExampleCreateMlineTool::_OnPostInstall ()
    {
    AccuSnap::GetInstance ().EnableSnap (true); // Enable snapping for create tools.

    __super::_OnPostInstall ();
    }

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
void ElementsExampleCreateMlineTool::SetPropertyString(WCharCP unparsed)
    {
    m_propertyString = unparsed;
    }

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
void ElementsExampleCreateMlineTool::InstallNewInstance (int toolId, int toolPrompt, ElementPropsOptions elemProps, WCharCP unparsed)
    {
    ElementsExampleCreateMlineTool* tool = new ElementsExampleCreateMlineTool (toolId, toolPrompt, elemProps);

    if(ElementPropsOptions_Custom == elemProps)
        tool->SetPropertyString(unparsed);

    tool->InstallTool ();
    }

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
Public void elementsExampleCreateMlineNone (WCharCP unparsed)
    {
    ElementsExampleCreateMlineTool::InstallNewInstance (CMDNAME_ElementsExampleCreateMlineNone, PROMPT_FirstPoint, ElementPropsOptions_None, unparsed);
    }

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
Public void elementsExampleCreateMlineActive (WCharCP unparsed)
    {
    ElementsExampleCreateMlineTool::InstallNewInstance (CMDNAME_ElementsExampleCreateMlineActive, PROMPT_FirstPoint, ElementPropsOptions_Active, unparsed);
    }

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
Public void elementsExampleCreateMlineCustom (WCharCP unparsed)
    {
    ElementsExampleCreateMlineTool::InstallNewInstance (CMDNAME_ElementsExampleCreateMlineCustom, PROMPT_FirstPoint, ElementPropsOptions_Custom, unparsed);
    }

