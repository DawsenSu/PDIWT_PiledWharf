

#include <Mstn/MdlApi/MdlApi.h>
#include <DgnView/AccuDraw.h>
#include <DgnView/DgnElementSetTool.h>
#include <DgnPlatform/ITextEdit.h>
#include <DgnPlatform/TextHandlers.h>
#include <Mstn/isessionmgr.h>

#include <DgnPlatform\DgnPlatformAPI.h>
#include <Mstn/ElementPropertyUtils.h>

#include "ElementsQueryHelper.h"


USING_NAMESPACE_BENTLEY_DGNPLATFORM;
USING_NAMESPACE_BENTLEY_MSTNPLATFORM;
USING_NAMESPACE_BENTLEY_MSTNPLATFORM_ELEMENT;


WString ElementsQueryHelper::QueryShapeElement(ElementHandleCR eh, WCharCP type)
{
	WString messageInfo = L"";
	WString format1 = WString(type) + L"\nPoints Count: %ld\nLength/Perimeter: %lf\n";
	WString format2 = L"Point[%ld] : (%lf, %lf, %lf)\n";
	
	CurveVectorPtr pathCurve = ICurvePathQuery::ElementToCurveVector(eh);

	if (pathCurve.IsValid())
	{
		ICurvePrimitivePtr& pathMember = pathCurve->front();

		//Make sure the contained Curve Primitive is CURVE_PRIMITIVE_TYPE_LineString.
		if (ICurvePrimitive::CURVE_PRIMITIVE_TYPE_LineString == pathCurve->HasSingleCurvePrimitive())
		{
			//Get linestring as a bvector of DPoint3d.
			bvector<DPoint3d> points = *pathMember->GetLineStringCP();

			double length;
			pathMember->Length(length);
			WString::Sprintf(messageInfo, format1.c_str(), points.size(), length);

			for (size_t i = 0; i < points.size(); i++)
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
WString ElementsQueryHelper::QueryMultilineElement(ElementHandleCR eh, WCharCP type)
{
	WString messageInfo = L"";
	WString format1 = WString(type) + L"\nPoints Count: %ld\n";
	WString format2 = L"Point[%ld] : (%lf, %lf, %lf)\n";

	Handler&  elmHandler = eh.GetHandler();

	//Dynamic cast element to MultilineHandler and query points.
	MultilineHandler* mLineObj = dynamic_cast <MultilineHandler*> (&elmHandler);
	if (NULL != mLineObj)
	{
		WString::Sprintf(messageInfo, format1.c_str(), mLineObj->GetPointCount(eh));

		for (UInt32 i = 0; i < mLineObj->GetPointCount(eh); i++)
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
WString ElementsQueryHelper::QueryElementDisplay(ElementHandleCR eh)
{
	WString format = L"Element\'s Basic Properties\n \
					Color    : %ld\n \
					Weight   : %ld\n \
					LineStyle: %ld\n \
					Ele Id is : %ld\n";// \
					//Ele Dec is : %s\n";

	ElementPropertiesGetterPtr propGetter = ElementPropertiesGetter::Create(eh);
	
	WString messageInfo = L"";
	if (propGetter.IsValid())
	{
		UInt32 color = propGetter->GetColor();
		UInt32 weight = propGetter->GetWeight();
		UInt32 linestyle = propGetter->GetLineStyle(NULL);
		ElementId eleId = eh.GetElementId();
		WString::Sprintf(messageInfo, format.c_str(), color, weight, linestyle, eleId);
	}
	else
		messageInfo = L"Cannot retrieve element basic Properties.";

	return messageInfo;
}

/*---------------------------------------------------------------------------------**//**
* @bsimethod                                                              Bentley Systems
+---------------+---------------+---------------+---------------+---------------+------*/
WString ElementsQueryHelper::QueryElementGeometry(ElementHandleCR eh)
{
	Handler& h = eh.GetHandler();
	WString type;
	h.GetTypeName(type, 256);

	//Check for correct element handler and call corresponding query method.
	if (&ShapeHandler::GetInstance() == &eh.GetHandler())
		return QueryShapeElement(eh, type.c_str());
	else if (&MultilineHandler::GetInstance() == &eh.GetHandler())
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
WString ElementsQueryHelper::QueryElementFill(ElementHandleCR eh)
{
	WString messageInfo = L"";

	IAreaFillPropertiesQuery * fillQueryP = dynamic_cast<IAreaFillPropertiesQuery *>(&eh.GetHandler());

	if (NULL == fillQueryP)
	{
		messageInfo = L"Cannot retrieve Fill Properties. Possible Reason: The element does not suppot fill.";
		return messageInfo;
	}

	WString format = L"Element\'s Fill Properties\nArea Type (Solid/Hole) : %ls\n";

	bool isHole;
	if (fillQueryP->GetAreaType(eh, &isHole))
		WString::Sprintf(messageInfo, format.c_str(), (isHole) ? L"Hole" : L"Solid");

	WString fillStr = L"";

	UInt32 fillColor;
	bool alwaysFilled;
	GradientSymbPtr gradient;
	PatternParamsPtr params;

	if (fillQueryP->GetSolidFill(eh, &fillColor, &alwaysFilled))
	{
		WString format2 = L"Element has Solid Fill.\nFill Color : %ld.\nAlways Filled: %ld\n";
		WString::Sprintf(fillStr, format2.c_str(), fillColor, alwaysFilled);

		messageInfo.append(fillStr);
	}
	else if (fillQueryP->GetGradientFill(eh, gradient))
	{
		UInt16 flags = gradient->GetFlags();
		double angle = gradient->GetAngle();
		double tint = gradient->GetTint();
		double shift = gradient->GetShift();

		WString format2 = L"Element has Gradient Fill.\nFlags : %ld.\nAngle: %lf\nTint : %lf\nShift: %lf\n";
		WString::Sprintf(fillStr, format2.c_str(), flags, angle, tint, shift);

		messageInfo.append(fillStr);

		for (int i = 0; i < gradient->GetNKeys(); i++)
		{
			RgbColorDef color;
			double pvalue;
			gradient->GetKey(color, pvalue, i);

			WString fillStr2 = L"";
			WString format3 = L"Gradient Info.\nFraction: %lf.\nColor (RGB): (%ld, %ld, %ld)\n";
			WString::Sprintf(fillStr2, format3.c_str(), pvalue, color.red, color.green, color.blue);

			messageInfo.append(fillStr2);
		}
	}
	else if (fillQueryP->GetPattern(eh, params, NULL, NULL, 0))
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
WString ElementsQueryHelper::QueryElement(ElementHandleCR eh, ElementsQueryOptions queryOption)
{

	if (ElementsExampleQuery_Display == queryOption)
		return QueryElementDisplay(eh);
	else if (ElementsExampleQuery_Geometry == queryOption)
		return QueryElementGeometry(eh);
	else
		return QueryElementFill(eh);
}

int testSumEle(int a, int b)
{
	return a + b;
}
