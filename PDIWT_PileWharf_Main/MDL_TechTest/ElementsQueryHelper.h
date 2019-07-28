#pragma once

#include <Mstn/MdlApi/MdlApi.h>
//#include <DgnView/AccuDraw.h>
//#include <DgnView/DgnElementSetTool.h>
//#include <Mstn/ElementPropertyUtils.h>


enum ElementsQueryOptions
{
	ElementsExampleQuery_Display,
	ElementsExampleQuery_Geometry,
	ElementsExampleQuery_Fill
};

class ElementsQueryHelper
{
private:

	/*---------------------------------------------------------------------------------**//**
	* @bsimethod                                                              Bentley Systems
	+---------------+---------------+---------------+---------------+---------------+------*/
	static WString QueryShapeElement(ElementHandleCR eh, WCharCP type);

	/*---------------------------------------------------------------------------------**//**
	* @bsimethod                                                              Bentley Systems
	+---------------+---------------+---------------+---------------+---------------+------*/
	static WString QueryMultilineElement(ElementHandleCR eh, WCharCP type);

	/*---------------------------------------------------------------------------------**//**
	* @bsimethod                                                              Bentley Systems
	+---------------+---------------+---------------+---------------+---------------+------*/
	static WString QueryElementDisplay(ElementHandleCR eh);

	/*---------------------------------------------------------------------------------**//**
	* @bsimethod                                                              Bentley Systems
	+---------------+---------------+---------------+---------------+---------------+------*/
	static WString QueryElementGeometry(ElementHandleCR eh);

	/*---------------------------------------------------------------------------------**//**
	* @bsimethod                                                              Bentley Systems
	+---------------+---------------+---------------+---------------+---------------+------*/
	static WString QueryElementFill(ElementHandleCR eh);

public:

	/*---------------------------------------------------------------------------------**//**
	* @bsimethod                                                              Bentley Systems
	+---------------+---------------+---------------+---------------+---------------+------*/
	static WString QueryElement(ElementHandleCR eh, ElementsQueryOptions queryOption);

};

