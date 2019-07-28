#pragma once

#include <DgnView/DgnElementSetTool.h>
#include <Mstn/ElementPropertyUtils.h>
#include "ElementsQueryHelper.h"

USING_NAMESPACE_BENTLEY_DGNPLATFORM;
USING_NAMESPACE_BENTLEY_MSTNPLATFORM;
USING_NAMESPACE_BENTLEY_MSTNPLATFORM_ELEMENT;


class ClickSelectTool: DgnElementSetTool
{
public:
	static void InstallNewInstance(int toolId, ElementsQueryOptions queryOption);
	ClickSelectTool(int toolId, ElementsQueryOptions queryOption);
	virtual ~ClickSelectTool();

	int _OnElementModify(EditElementHandleR element) override;

private:
	int m_toolId;
	ElementsQueryOptions m_queryOption;

	void TestQueryElement(DgnButtonEventCR ev);
	void _OnPostInstall() override;
	bool _OnDataButton(DgnButtonEventCR ev) override;
	void _OnRestartTool() override;

};

