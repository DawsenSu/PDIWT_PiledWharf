/*--------------------------------------------------------------------------------------+
|
|     $Source: MstnExamples/Elements/ElementsExample/ElementsExampleEditTool.cpp $
|
|  $Copyright: (c) 2015 Bentley Systems, Incorporated. All rights reserved. $
|
+--------------------------------------------------------------------------------------*/
#include "ElementsExample.h"

USING_NAMESPACE_BENTLEY_DGNPLATFORM;
USING_NAMESPACE_BENTLEY_MSTNPLATFORM;
USING_NAMESPACE_BENTLEY_MSTNPLATFORM_ELEMENT;

enum ElementsExampleEditOptions
    {
    ElementsExampleEdit_DisplayDefault,
    ElementsExampleEdit_DisplayActive,
    ElementsExampleEdit_DisplayCustom,
    ElementsExampleEdit_Geometry,
    ElementsExampleEdit_FillNone,
    ElementsExampleEdit_FillSolid,
    ElementsExampleEdit_FillPattern,
    ElementsExampleEdit_FillGradient
    };

/*=================================================================================**//**
* Helper class to edit display/geometric/fill properties of different elements.
* @bsiclass                                                               Bentley Systems
+===============+===============+===============+===============+===============+======*/
class ElementsEditHelper
{

private:

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
static bool EditShapeElement (EditElementHandleR eeh);

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
static bool EditMultilineElement (EditElementHandleR eeh);

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
static bool EditElementDisplay (EditElementHandleR eeh, ElementsExampleEditOptions editOption, WStringR errorMessage);

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
static bool EditElementGeometry (EditElementHandleR eeh, WStringR errorMessage);

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
static bool RemoveElementFill (EditElementHandleR eeh);

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
static bool AddElementSolidFill (EditElementHandleR eeh);

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
static bool AddElementPatternFill (EditElementHandleR eeh);

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
static bool AddElementGradientFill (EditElementHandleR eeh);

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
static bool EditElementFill (EditElementHandleR eeh, ElementsExampleEditOptions editOption, WStringR errorMessage);

public:

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
static bool EditElement (EditElementHandleR eeh, ElementsExampleEditOptions editOption, WStringR errorMessage);

};

/*---------------------------------------------------------------------------------**//**
* Edit Shape element.
* Get the linestring as a bvector of DPoint3d
* Reverse the points using std::reverse
* Create another CurveVector with these new points and set the CurveVector for the element.
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
bool ElementsEditHelper::EditShapeElement (EditElementHandleR eeh)
    {
    CurveVectorPtr pathCurve = ICurvePathQuery::ElementToCurveVector (eeh);

    if (pathCurve.IsValid ())
        {
        ICurvePrimitivePtr& pathMember = pathCurve->front ();

        if(ICurvePrimitive::CURVE_PRIMITIVE_TYPE_LineString == pathCurve->HasSingleCurvePrimitive ())
            {
            bvector <DPoint3d> points = *pathMember->GetLineStringCP ();

            std::reverse(points.begin(), points.end());

            ICurvePrimitivePtr curvePrim = ICurvePrimitive::CreateLineString(points);

            CurveVectorPtr newPathCurve = CurveVector::Create(pathCurve->GetBoundaryType ());

            newPathCurve->push_back(curvePrim);

            Handler&  elmHandler = eeh.GetHandler ();
            ICurvePathEdit* curveObj = dynamic_cast <ICurvePathEdit*> (&elmHandler);

            if(SUCCESS == curveObj->SetCurveVector(eeh, *newPathCurve))
                return true;
            else
                mdlDialog_openInfoBox(L"Set Curve Vector failed.");
            }
        else
            mdlDialog_openInfoBox(L"Not a shape.");
        }
    else
        mdlDialog_openInfoBox(L"Path Curve Invalid.");

    return false;
    }

/*---------------------------------------------------------------------------------**//**
* Edit Multiline element.
* Dynamic cast element to MultilineHandler
* Get points
* Reverse the points and replace points in the element.
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
bool ElementsEditHelper::EditMultilineElement (EditElementHandleR eeh)
    {
    Handler&  elmHandler = eeh.GetHandler ();

    MultilineHandler* mLineObj = dynamic_cast <MultilineHandler*> (&elmHandler);
    if(NULL != mLineObj)
        {
        bvector<DPoint3d> points;

        for(UInt32 i=0; i<mLineObj->GetPointCount(eeh); i++)
            {
            MultilinePointPtr mPoint = mLineObj->GetPoint(eeh, i);
            points.push_back(mPoint->GetPoint());
            }

        size_t j = points.size()-1;
        for(UInt32 i=0; i<mLineObj->GetPointCount(eeh); i++)
            {
            if(SUCCESS != mLineObj->ReplacePoint(eeh, points[j], i, MlineModifyPoint::None))
                return false;
            j--;
            }
        }

    return true;
    }

/*---------------------------------------------------------------------------------**//**
* Edit element basic properties
* This example only covers Color, Weight and LineStyle.
* This example uses ElementPropertiesSetter for setting these properties.
* ElementPropertiesSetter is an implementation of IEditProperties for setting elements basic properties.
* Depending on "editOption" either applies active settings (that will actually change properties other than stated above)
* or changes the above stated properties to either default value of 0 or a const value of 3.
* See ElementPropertiesGetter, PropertyContext and IEditProperties for more detail.
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
bool ElementsEditHelper::EditElementDisplay (EditElementHandleR eeh, ElementsExampleEditOptions editOption, WStringR errorMessage)
    {

    bool propsChanged = false;

    if(ElementsExampleEdit_DisplayActive == editOption)
        {
        ElementPropertyUtils::ApplyActiveSettings (eeh);
        propsChanged = true;
        }
    else
        {
        UInt32 newProp = (ElementsExampleEdit_DisplayDefault == editOption)? 0 : 3;

        ElementPropertiesSetterPtr remapper = ElementPropertiesSetter::Create ();
        remapper->SetColor (newProp);
        remapper->SetWeight (newProp);
        remapper->SetLinestyle ((Int32)newProp, NULL);
        propsChanged = remapper->Apply (eeh);
        }

    if(!propsChanged)
        errorMessage = WString(L"Element Properties were not changed.");

    return propsChanged;
    }

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
bool ElementsEditHelper::EditElementGeometry (EditElementHandleR eeh, WStringR errorMessage)
    {
    if(&ShapeHandler::GetInstance() == &eeh.GetHandler())
        return EditShapeElement(eeh);
    else if(&MultilineHandler::GetInstance() == &eeh.GetHandler())
        return EditMultilineElement(eeh);

    //We reached here because the element is not covered by this example.
    errorMessage = WString(L"Element is not supported by this Edit Example. This Edit Example only supports Shape and Multiline elements.");

    return false;
    }

/*---------------------------------------------------------------------------------**//**
* Remove element fill.
* Removes Solid or Gradient fill.
* If none exists, it removes Pattern fill.
* See IAreaFillPropertiesEdit in MicroStation Documentation for more information.
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
bool ElementsEditHelper::RemoveElementFill (EditElementHandleR eeh)
    {
    IAreaFillPropertiesEdit * fillEditP = dynamic_cast<IAreaFillPropertiesEdit *>(&eeh.GetHandler());

    if(NULL == fillEditP)
        return false;

    //Remove Solid or Gradient fill. If that does not exist remove pattern at index 0.
    bool removeFill = false;
    removeFill = fillEditP->RemoveAreaFill (eeh);

    if(!removeFill)
        removeFill = fillEditP->RemovePattern (eeh, 0); //Note that The Multiline element is the only element type that currently supports having more than one pattern. 

    return removeFill;
    }

/*---------------------------------------------------------------------------------**//**
* Add element solid fill
* This example is using a constant color index for demonstration.
* See IAreaFillPropertiesEdit in MicroStation Documentation for more information.
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
bool ElementsEditHelper::AddElementSolidFill (EditElementHandleR eeh)
    {
    IAreaFillPropertiesEdit * fillEditP = dynamic_cast<IAreaFillPropertiesEdit *>(&eeh.GetHandler());

    if(NULL == fillEditP)
        return false;

    UInt32 fillcolor = 3;
    bool alwaysfilled = false;

    return fillEditP->AddSolidFill (eeh, &fillcolor, &alwaysfilled);
    }

/*---------------------------------------------------------------------------------**//**
* Add element pattern.
* Sets only few properties of PatternParams
* See IAreaFillPropertiesEdit and PatternParams in MicroStation Documentation for more information.
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
bool ElementsEditHelper::AddElementPatternFill (EditElementHandleR eeh)
    {
    IAreaFillPropertiesEdit * fillEditP = dynamic_cast<IAreaFillPropertiesEdit *>(&eeh.GetHandler());

    if(NULL == fillEditP)
        return false;

    PatternParamsPtr params = PatternParams::Create ();
    params->SetPrimarySpacing(5);
    params->SetPrimaryAngle(PI/4);
    params->SetSecondarySpacing(5);
    params->SetSecondaryAngle(PI/3);
    params->SetTolerance(0);

    return fillEditP->AddPattern(eeh, *params, NULL);
    }

/*---------------------------------------------------------------------------------**//**
* Add element gradient fill.
* Sets 3 different colors at different fractions in Spherical mode.
* See IAreaFillPropertiesEdit and GradientSymb in MicroStation Documentation for more information.
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
bool ElementsEditHelper::AddElementGradientFill (EditElementHandleR eeh)
    {
    IAreaFillPropertiesEdit * fillEditP = dynamic_cast<IAreaFillPropertiesEdit *>(&eeh.GetHandler());

    if(NULL == fillEditP)
        return false;

    const int EXAMPLE_NUMKEYS = 3;

    GradientSymbPtr gradient = GradientSymb::Create ();

    RgbColorDef colors[EXAMPLE_NUMKEYS];
    double pvalues[EXAMPLE_NUMKEYS];

    colors[0].red = 255;  colors[0].green = 0;  colors[0].blue = 0;
    colors[1].red = 0;  colors[1].green = 255;   colors[1].blue = 0;
    colors[2].red = 0; colors[2].green = 0; colors[2].blue = 255;
    pvalues[0] = 0.0000;
    pvalues[1] = 0.5000;
    pvalues[2] = 1.0000;
    
    gradient->SetMode (GradientMode::Spherical);
    gradient->SetFlags (0);
    gradient->SetAngle (0);
    gradient->SetTint (0);
    gradient->SetShift (0);
    gradient->SetKeys (EXAMPLE_NUMKEYS, colors, pvalues);

    return fillEditP->AddGradientFill (eeh, *gradient);
    }

/*---------------------------------------------------------------------------------**//**
* Based on "editOption" calls appropriate filling method.
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
bool ElementsEditHelper::EditElementFill (EditElementHandleR eeh, ElementsExampleEditOptions editOption, WStringR errorMessage)
    {
    bool fillResult = false;
    switch(editOption)
        {
        case ElementsExampleEdit_FillNone:
            fillResult = RemoveElementFill(eeh);
            break;
        case ElementsExampleEdit_FillSolid:
            fillResult = AddElementSolidFill(eeh);
            break;
        case ElementsExampleEdit_FillPattern:
            fillResult = AddElementPatternFill(eeh);
            break;
        case ElementsExampleEdit_FillGradient:
            fillResult = AddElementGradientFill(eeh);
            break;
        }

    if(!fillResult)
        errorMessage = L"Cannot change Fill Properties. Possible Reasons\n - The element does not suppot fill.\n - The element currently does not have fill.";

    return fillResult;
    }

/*---------------------------------------------------------------------------------**//**
* This same method is called from ElementsExampleEditTool and this method will route the
* element to proper edit method.
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
bool ElementsEditHelper::EditElement (EditElementHandleR eeh, ElementsExampleEditOptions editOption, WStringR errorMessage)
    {
    if(ElementsExampleEdit_DisplayDefault == editOption || ElementsExampleEdit_DisplayActive == editOption || ElementsExampleEdit_DisplayCustom == editOption)
        return EditElementDisplay(eeh, editOption, errorMessage);
    else if(ElementsExampleEdit_Geometry == editOption)
        return EditElementGeometry(eeh, errorMessage);
    else
        return EditElementFill(eeh, editOption, errorMessage);
    }

/*=================================================================================**//**
* Tool class to edit elements.
* 1. Key-in one of the following
*   ELEMENTSEXAMPLE ELEMENT EDIT DISPLAYDEFAULT
*   ELEMENTSEXAMPLE ELEMENT EDIT DISPLAYACTIVE
*   ELEMENTSEXAMPLE ELEMENT EDIT DISPLAYCUSTOM
*   ELEMENTSEXAMPLE ELEMENT EDIT GEOMETRY
*   ELEMENTSEXAMPLE ELEMENT EDIT FILLNONE
*   ELEMENTSEXAMPLE ELEMENT EDIT FILLSOLID
*   ELEMENTSEXAMPLE ELEMENT EDIT FILLPATTERN
*   ELEMENTSEXAMPLE ELEMENT EDIT FILLGRADIENT
* 2. Select element and proper edit operation will be performed on the element.
* @bsiclass                                                               Bentley Systems
+===============+===============+===============+===============+===============+======*/
struct ElementsExampleEditTool : DgnElementSetTool
{
private:

int m_toolId;
ElementsExampleEditOptions m_editOption;

ElementsExampleEditTool (int toolId, ElementsExampleEditOptions editOption) : DgnElementSetTool (toolId), m_toolId(toolId)
    {
    m_editOption = editOption;
    }

void _OnPostInstall() override;
bool _OnDataButton(DgnButtonEventCR ev) override;
void _OnRestartTool() override;

public:

int _OnElementModify(EditElementHandleR element) override;
static void InstallNewInstance (int toolId, ElementsExampleEditOptions editOption);

};

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
/*--------------+---------------+---------------+---------------+---------------+------*/
void ElementsExampleEditTool::_OnPostInstall()
    {
    _SetLocateCursor (true);
    }

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
/*--------------+---------------+---------------+---------------+---------------+------*/
bool ElementsExampleEditTool::_OnDataButton(DgnButtonEventCR ev)
    {

    //Locate element.
    HitPathCP hitPath = _DoLocate(ev, true, Bentley::DgnPlatform::ComponentMode::Innermost);

    //If an element is located, edit it.
    if(NULL != hitPath)
        {
        ElementRefP elemRef = hitPath->GetHeadElem();
        EditElementHandle elem(elemRef);

        //If edit is successful, add the modified element to model otherwise issue an error message.
        WString errorMessage = L"";
        if(ElementsEditHelper::EditElement (elem, m_editOption, errorMessage))
            elem.ReplaceInModel(elemRef);
        else
            mdlDialog_openInfoBox (errorMessage.c_str());
        }

    return true;
    }

/*---------------------------------------------------------------------------------**//**
* Restart tool.
* @bsimethod                                                              Bentley Systems
/*--------------+---------------+---------------+---------------+---------------+------*/
void ElementsExampleEditTool::_OnRestartTool()
    {
    InstallNewInstance(m_toolId, m_editOption);
    }

/*---------------------------------------------------------------------------------**//**
* All modification is done in _OnDataButton so return Error.
* @bsimethod                                                              Bentley Systems
/*--------------+---------------+---------------+---------------+---------------+------*/
int ElementsExampleEditTool::_OnElementModify(EditElementHandleR element)
    {
    return ERROR;
    }

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
/*--------------+---------------+---------------+---------------+---------------+------*/
void ElementsExampleEditTool::InstallNewInstance (int toolId, ElementsExampleEditOptions editOption)
    {
    ElementsExampleEditTool* exampleTool = new ElementsExampleEditTool(toolId, editOption);
    exampleTool->InstallTool();
    }

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
Public void elementsExampleEditElementDisplayDefault (WCharCP unparsed)
    {
    ElementsExampleEditTool::InstallNewInstance (CMDNAME_ElementsExampleEditElementDisplay, ElementsExampleEdit_DisplayDefault);
    }

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
Public void elementsExampleEditElementDisplayActive (WCharCP unparsed)
    {
    ElementsExampleEditTool::InstallNewInstance (CMDNAME_ElementsExampleEditElementDisplay, ElementsExampleEdit_DisplayActive);
    }

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
Public void elementsExampleEditElementDisplayCustom (WCharCP unparsed)
    {
    ElementsExampleEditTool::InstallNewInstance (CMDNAME_ElementsExampleEditElementDisplay, ElementsExampleEdit_DisplayCustom);
    }

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
Public void elementsExampleEditElementGeometry (WCharCP unparsed)
    {
    ElementsExampleEditTool::InstallNewInstance (CMDNAME_ElementsExampleEditElementGeometry, ElementsExampleEdit_Geometry);
    }

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
Public void elementsExampleEditElementFillNone (WCharCP unparsed)
    {
    ElementsExampleEditTool::InstallNewInstance (CMDNAME_ElementsExampleEditElementFill, ElementsExampleEdit_FillNone);
    }

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
Public void elementsExampleEditElementFillSolid (WCharCP unparsed)
    {
    ElementsExampleEditTool::InstallNewInstance (CMDNAME_ElementsExampleEditElementFill, ElementsExampleEdit_FillSolid);
    }

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
Public void elementsExampleEditElementFillPattern (WCharCP unparsed)
    {
    ElementsExampleEditTool::InstallNewInstance (CMDNAME_ElementsExampleEditElementFill, ElementsExampleEdit_FillPattern);
    }

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
Public void elementsExampleEditElementFillGradient (WCharCP unparsed)
    {
    ElementsExampleEditTool::InstallNewInstance (CMDNAME_ElementsExampleEditElementFill, ElementsExampleEdit_FillGradient);
    }

