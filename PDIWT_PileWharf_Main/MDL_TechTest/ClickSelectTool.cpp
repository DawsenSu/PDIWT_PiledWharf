
#include "ElementCreator.h"
#include "TechTest.h"
#include "ClickSelectTool.h"

ClickSelectTool::ClickSelectTool(int toolId, ElementsQueryOptions queryOption)
{
}

ClickSelectTool::~ClickSelectTool()
{
}

int ClickSelectTool::_OnElementModify(EditElementHandleR element)
{
	return ERROR;
}


void ClickSelectTool::InstallNewInstance(int toolId, ElementsQueryOptions queryOption)
{
	ClickSelectTool* clickTool = new ClickSelectTool(toolId, queryOption);
	clickTool->InstallTool();
}

void ClickSelectTool::_OnPostInstall()
{
	_SetLocateCursor(true);
}


void ClickSelectTool::TestQueryElement(DgnButtonEventCR ev)
{
	HitPathCP hitPath = _DoLocate(ev, true, Bentley::DgnPlatform::ComponentMode::Innermost);
	ElementHandle elem;
	//If an element is located, query it.
	if (NULL == hitPath)
	{
		mdlDialog_openInfoBox(L"hitPath is Null.");
		return;
	}

	ElementRefP elemRef = hitPath->GetHeadElem();
	elem = ElementHandle(elemRef);

	WString messageInfo = ElementsQueryHelper::QueryElement(elem, m_queryOption);
	mdlDialog_openInfoBox(messageInfo.c_str());
}

int m_iClickCounter = 0;
//ElementHandle m_eh1;
//ElementHandle m_eh2;
DRay3d m_ray;

bool ClickSelectTool::_OnDataButton(DgnButtonEventCR ev)
{
	HitPathCP hitPath = _DoLocate(ev, true, Bentley::DgnPlatform::ComponentMode::Innermost);
	if (NULL == hitPath)
	{
		mdlDialog_openInfoBox(L"hitPath is Null.");
		return false;
	}
	ElementRefP elemRef = hitPath->GetHeadElem();
	ElementHandle elem = ElementHandle(elemRef);

	return TechTest::TestIntersectionRayAndMesh(elem);
}

void ClickSelectTool::_OnRestartTool()
{
	InstallNewInstance(m_toolId, m_queryOption);
}