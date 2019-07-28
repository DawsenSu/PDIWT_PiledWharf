#pragma once

#include <DgnView/DgnElementSetTool.h>
#include <Mstn/ElementPropertyUtils.h>
#include "ElementsQueryHelper.h"

USING_NAMESPACE_BENTLEY_DGNPLATFORM;
USING_NAMESPACE_BENTLEY_MSTNPLATFORM;
USING_NAMESPACE_BENTLEY_MSTNPLATFORM_ELEMENT;


class TechTest
{
public:
	static bool TestIntersectionRayAndMesh(ElementHandle elem);

private:
	static int mst_iClickCounter;
	//ElementHandle m_eh1;
	//ElementHandle m_eh2;
	static DRay3d mst_ray;
};

