/*--------------------------------------------------------------------------------------+
|
|     $Source: MstnExamples/Elements/ElementsExample/ElementsExampleQueryTool.cpp $
|
|  $Copyright: (c) 2015 Bentley Systems, Incorporated. All rights reserved. $
|
+--------------------------------------------------------------------------------------*/
#include "ElementsExample.h"

USING_NAMESPACE_BENTLEY_DGNPLATFORM;
USING_NAMESPACE_BENTLEY_MSTNPLATFORM;
USING_NAMESPACE_BENTLEY_MSTNPLATFORM_ELEMENT;

enum ElementsExampleQueryOptions
    {
    ElementsExampleQuery_Display,
    ElementsExampleQuery_Geometry,
    ElementsExampleQuery_Fill
    };

/*=================================================================================**//**
* Helper class to query different elements for different properties.
* @bsiclass                                                               Bentley Systems
+===============+===============+===============+===============+===============+======*/
class ElementsQueryHelper
{

private:

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
static WString QueryShapeElement (ElementHandleCR eh, WCharCP type);

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
static WString QueryMultilineElement (ElementHandleCR eh, WCharCP type);

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
static WString QueryElementDisplay (ElementHandleCR eh);

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
static WString QueryElementGeometry (ElementHandleCR eh);

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
static WString QueryElementFill (ElementHandleCR eh);

public:

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
static WString QueryElement (ElementHandleCR eh, ElementsExampleQueryOptions queryOption);

};

/*---------------------------------------------------------------------------------**//**
* Query shape element geometric properties like corner points.
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
WString ElementsQueryHelper::QueryShapeElement (ElementHandleCR eh, WCharCP type)
    {
    WString messageInfo = L"";
    WString format1 = WString(type) + L"\nPoints Count: %ld\nLength/Perimeter: %lf\n";
    WString format2 = L"Point[%ld] : (%lf, %lf, %lf)\n";

    CurveVectorPtr pathCurve = ICurvePathQuery::ElementToCurveVector (eh);

    if (pathCurve.IsValid ())
        {
        ICurvePrimitivePtr& pathMember = pathCurve->front ();

        //Make sure the contained Curve Primitive is CURVE_PRIMITIVE_TYPE_LineString.
        if(ICurvePrimitive::CURVE_PRIMITIVE_TYPE_LineString == pathCurve->HasSingleCurvePrimitive ())
            {
            //Get linestring as a bvector of DPoint3d.
            bvector<DPoint3d> points = *pathMember->GetLineStringCP();

            double length;
            pathMember->Length(length);
            WString::Sprintf(messageInfo, format1.c_str(), points.size(), length);

            for(size_t i=0; i<points.size(); i++)
                {
                WString pointStr;
                WString::Sprintf(pointStr, format2.c_str(), i, points[i].x, points[i].y, points[i].z);

                messageInfo.append(pointStr);
                }
            }
        }

    return messageInfo;
    }

/*---------------------------------------------------------------------------------**//**
* Query Multiline element geometric properties like joining points.
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
WString ElementsQueryHelper::QueryMultilineElement (ElementHandleCR eh, WCharCP type)
    {
    WString messageInfo = L"";
    WString format1 = WString(type) + L"\nPoints Count: %ld\n";
    WString format2 = L"Point[%ld] : (%lf, %lf, %lf)\n";

    Handler&  elmHandler = eh.GetHandler ();

    //Dynamic cast element to MultilineHandler and query points.
    MultilineHandler* mLineObj = dynamic_cast <MultilineHandler*> (&elmHandler);
    if(NULL != mLineObj)
        {
        WString::Sprintf(messageInfo, format1.c_str(), mLineObj->GetPointCount(eh));

        for(UInt32 i=0; i<mLineObj->GetPointCount(eh); i++)
            {
            MultilinePointPtr mPoint = mLineObj->GetPoint(eh, i);
            DPoint3d point = mPoint->GetPoint();

            WString pointStr;
            WString::Sprintf(pointStr, format2.c_str(), i, point.x, point.y, point.z);

            messageInfo.append(pointStr);
            }
        }

    return messageInfo;
    }

/*---------------------------------------------------------------------------------**//**
* Query element basic properties like color, weight and linestyle.
* This example is using ElementPropertiesGetter for demonstration.
* ElementPropertiesGetter is an implementation of IQueryProperties.
* See ElementPropertiesGetter, PropertyContext and IQueryProperties for more detail.
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
WString ElementsQueryHelper::QueryElementDisplay (ElementHandleCR eh)
    {
    WString format = L"Element\'s Basic Properties\nColor    : %ld\nWeight   : %ld\nLineStyle: %ld\n";

    ElementPropertiesGetterPtr propGetter = ElementPropertiesGetter::Create (eh);

    WString messageInfo = L"";
    if(propGetter.IsValid())
        {
        UInt32 color     = propGetter->GetColor ();
        UInt32 weight    = propGetter->GetWeight ();
        UInt32 linestyle = propGetter->GetLineStyle (NULL);

        WString::Sprintf(messageInfo, format.c_str(), color, weight, linestyle);
        }
    else
        messageInfo = L"Cannot retrieve element basic Properties.";

    return messageInfo;
    }

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
WString ElementsQueryHelper::QueryElementGeometry (ElementHandleCR eh)
    {
    Handler& h = eh.GetHandler();
    WString type;
    h.GetTypeName(type, 256);

    //Check for correct element handler and call corresponding query method.
    if(&ShapeHandler::GetInstance() == &eh.GetHandler())
        return QueryShapeElement(eh, type.c_str());
    else if(&MultilineHandler::GetInstance() == &eh.GetHandler())
        return QueryMultilineElement(eh, type.c_str());

    //We reached here because the element is not covered by this example.
    WString errorMessage = WString(L"Element is not supported by this Query Example. This Query Example only supports Shape and Multiline elements.");

    return errorMessage;
    }

/*---------------------------------------------------------------------------------**//**
* Query element fill properties.
* Checks element Area type i.e. Solid or Hole.
* Queries element fill
*   Solid Fill.
*   Gradient Fill.
*   Pattern Fill.
* See IAreaFillPropertiesQuery for more information.
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
WString ElementsQueryHelper::QueryElementFill (ElementHandleCR eh)
    {
    WString messageInfo = L"";

    IAreaFillPropertiesQuery * fillQueryP = dynamic_cast<IAreaFillPropertiesQuery *>(&eh.GetHandler());

    if(NULL == fillQueryP)
        {
        messageInfo = L"Cannot retrieve Fill Properties. Possible Reason: The element does not suppot fill.";
        return messageInfo;
        }

    WString format = L"Element\'s Fill Properties\nArea Type (Solid/Hole) : %ls\n";

    bool isHole;
    if(fillQueryP->GetAreaType (eh, &isHole))
        WString::Sprintf(messageInfo, format.c_str(), (isHole) ? L"Hole" : L"Solid");

    WString fillStr = L"";

    UInt32 fillColor;
    bool alwaysFilled;
    GradientSymbPtr gradient;
    PatternParamsPtr params;

    if(fillQueryP->GetSolidFill(eh, &fillColor, &alwaysFilled))
        {
        WString format2 = L"Element has Solid Fill.\nFill Color : %ld.\nAlways Filled: %ld\n";
        WString::Sprintf(fillStr, format2.c_str(), fillColor, alwaysFilled);

        messageInfo.append(fillStr);
        }
    else if(fillQueryP->GetGradientFill (eh, gradient))
        {
        UInt16 flags = gradient->GetFlags();
        double angle = gradient->GetAngle();
        double tint  = gradient->GetTint();
        double shift = gradient->GetShift();

        WString format2 = L"Element has Gradient Fill.\nFlags : %ld.\nAngle: %lf\nTint : %lf\nShift: %lf\n";
        WString::Sprintf(fillStr, format2.c_str(), flags, angle, tint, shift);

        messageInfo.append(fillStr);

        for (int i = 0; i < gradient->GetNKeys (); i++)
            {
            RgbColorDef color;
            double pvalue;
            gradient->GetKey (color, pvalue, i);

            WString fillStr2 = L"";
            WString format3 = L"Gradient Info.\nFraction: %lf.\nColor (RGB): (%ld, %ld, %ld)\n";
            WString::Sprintf(fillStr2, format3.c_str(), pvalue, color.red, color.green, color.blue);

            messageInfo.append(fillStr2);
            }
        }
    else if(fillQueryP->GetPattern (eh, params, NULL, NULL, 0))
        {
        WString format2 = L"Element has Pattern Fill.\nPrimary Angle: %lf.\nPrimary Spacing: %lf\nSecondary Angle: %lf\nSecondary Spacing: %lf\nColor: %ld\nWeight: %ld\nStyle: %ld\n";
        WString::Sprintf(fillStr, format2.c_str(), params->GetPrimaryAngle(), params->GetPrimarySpacing(), params->GetSecondaryAngle(), params->GetSecondarySpacing(), params->GetColor(), params->GetWeight(), params->GetStyle());

        messageInfo.append(fillStr);
        }

    return messageInfo;
    }

/*---------------------------------------------------------------------------------**//**
* This same method is called from ElementsQueryTool and this method will route the
* element to proper query method.
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
WString ElementsQueryHelper::QueryElement (ElementHandleCR eh, ElementsExampleQueryOptions queryOption)
    {

    if(ElementsExampleQuery_Display == queryOption)
        return QueryElementDisplay(eh);
    else if(ElementsExampleQuery_Geometry== queryOption)
        return QueryElementGeometry(eh);
    else
        return QueryElementFill (eh);
    }

/*=================================================================================**//**
* Tool class for querying elements.
* 1. Key-in of the following
*       ELEMENTSEXAMPLE ELEMENT QUERY DISPLAY
*       ELEMENTSEXAMPLE ELEMENT QUERY GEOMETRY
*       ELEMENTSEXAMPLE ELEMENT QUERY FILL
* 2. Select element and proper query operation will be performed on the element.
* Note that this example just displays the information in an info box.
* @bsiclass                                                               Bentley Systems
+===============+===============+===============+===============+===============+======*/
struct ElementsExampleQueryTool : DgnElementSetTool
{
private:

int m_toolId;
ElementsExampleQueryOptions m_queryOption;

ElementsExampleQueryTool (int toolId, ElementsExampleQueryOptions queryOption) : DgnElementSetTool (toolId), m_toolId(toolId)
    {
    m_queryOption = queryOption;
    }

void _OnPostInstall() override;
bool _OnDataButton(DgnButtonEventCR ev) override;
void _OnRestartTool() override;

public:

int _OnElementModify(EditElementHandleR element) override;
static void InstallNewInstance (int toolId, ElementsExampleQueryOptions queryOption);

};

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
/*--------------+---------------+---------------+---------------+---------------+------*/
void ElementsExampleQueryTool::_OnPostInstall()
    {
    _SetLocateCursor (true);
    }

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
/*--------------+---------------+---------------+---------------+---------------+------*/
bool ElementsExampleQueryTool::_OnDataButton(DgnButtonEventCR ev)
    {

    HitPathCP hitPath = _DoLocate(ev, true, Bentley::DgnPlatform::ComponentMode::Innermost);

    //If an element is located, query it.
    if(NULL != hitPath)
        {
        ElementRefP elemRef = hitPath->GetHeadElem();
        ElementHandle elem(elemRef);

        WString messageInfo = ElementsQueryHelper::QueryElement(elem, m_queryOption);
        mdlDialog_openInfoBox (messageInfo.c_str());
        }

    return true;
    }

/*---------------------------------------------------------------------------------**//**
* Restart tool.
* @bsimethod                                                              Bentley Systems
/*--------------+---------------+---------------+---------------+---------------+------*/
void ElementsExampleQueryTool::_OnRestartTool()
    {
    InstallNewInstance(m_toolId, m_queryOption);
    }

/*---------------------------------------------------------------------------------**//**
* No modification is required so return Error.
* @bsimethod                                                              Bentley Systems
/*--------------+---------------+---------------+---------------+---------------+------*/
int ElementsExampleQueryTool::_OnElementModify(EditElementHandleR element)
    {
    return ERROR;
    }

/*---------------------------------------------------------------------------------**//**
* Install a new instance of ElementsExampleQueryTool.
* @bsimethod                                                              Bentley Systems
/*--------------+---------------+---------------+---------------+---------------+------*/
void ElementsExampleQueryTool::InstallNewInstance (int toolId, ElementsExampleQueryOptions queryOption)
    {
    ElementsExampleQueryTool* exampleTool = new ElementsExampleQueryTool(toolId, queryOption);
    exampleTool->InstallTool();
    }

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
/*--------------+---------------+---------------+---------------+---------------+------*/
Public void elementsExampleQueryElementDisplay (WCharCP  unparsed)
    {
    ElementsExampleQueryTool::InstallNewInstance (CMDNAME_ElementsExampleQueryElementDisplay, ElementsExampleQuery_Display);
    }

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
/*--------------+---------------+---------------+---------------+---------------+------*/
Public void elementsExampleQueryElementGeometry (WCharCP  unparsed)
    {
    ElementsExampleQueryTool::InstallNewInstance (CMDNAME_ElementsExampleQueryElementGeometry, ElementsExampleQuery_Geometry);
    }

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
/*--------------+---------------+---------------+---------------+---------------+------*/
Public void elementsExampleQueryElementFill (WCharCP  unparsed)
    {
    ElementsExampleQueryTool::InstallNewInstance (CMDNAME_ElementsExampleQueryElementFill, ElementsExampleQuery_Fill);
    }

