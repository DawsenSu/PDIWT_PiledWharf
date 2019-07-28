/*-------------------------------------------------------------+
|   HelloWorld.cpp                                                |
+-------------------------------------------------------------*/
#include <Mstn\MdlApi\MdlApi.h>
#include <DgnPlatform\DgnPlatformApi.h>
#include <DgnView\DgnElementSetTool.h>

#include "MDL_TechTestCmd.h"
#include "ElementCreator.h"
#include "ClickSelectTool.h"
//#include "MDL_TechTest.h"

//#include "..\MixedBridge\MixedBridge4Cpp.h"
//#include "..\NativeDLL\NativeDllTest.h"

//int TestA(int a, int b);

extern "C" __declspec(dllexport)
void ShowMsgHelloWorld()
{
	mdlDialog_dmsgsPrint(L"Hello World Test");
	//MixedBridge4Cpp::UIEventRegister::MsgBoxDNTest(12);
	//DLLPureTest::NativeDllTest::Test();
}


/*-------------------------------------------------------------+
|   HelloWorld.cpp                                                |
+-------------------------------------------------------------*/


USING_NAMESPACE_BENTLEY_DGNPLATFORM
USING_NAMESPACE_BENTLEY_MSTNPLATFORM
USING_NAMESPACE_BENTLEY_MSTNPLATFORM_ELEMENT

double g_1mu;

struct PlaceBsSurfaceTool :DgnPrimitiveTool
{
	PlaceBsSurfaceTool(int toolId, int promptId) :
		DgnPrimitiveTool(toolId, promptId)
	{}

	bool CreateBsSurface(EditElementHandleR eeh, DPoint3dCP pBasePt)
	{
		MSBsplineSurface bsSurface;
		MSBsplineCurve   bsCurves[4];
		DPoint3d         center[4];
		RotMatrix        rMatrix[4];
		double           radius = g_1mu / 2;

		center[0] = center[1] = center[2] = center[3] = *pBasePt;
		center[0].x += radius;
		center[1].x += g_1mu;     center[1].y += radius;
		center[2].x += radius;    center[2].y += g_1mu;
		center[3].y += radius;

		DVec3d xVec = DVec3d::From(1, 0, 0), negativeXVec = DVec3d::From(-1, 0, 0);
		DVec3d yVec = DVec3d::From(0, 1, 0), negativeYVec = DVec3d::From(0, -1, 0);
		DVec3d zVec = DVec3d::From(0, 0, 1);
		rMatrix[0].InitFrom2Vectors(xVec, zVec);  //Front View
		rMatrix[1].InitFrom2Vectors(yVec, zVec);  //Right View
		rMatrix[2].InitFrom2Vectors(negativeXVec, zVec);  //Back View
		rMatrix[3].InitFrom2Vectors(negativeYVec, zVec);  //Left View

		for (int i = 0; i < 4; i++)
		{
			bsCurves[i].InitEllipticArc(center[i], radius, radius, 0, PI, &rMatrix[i]);
		}
		if (SUCCESS != mdlBspline_coonsPatch(&bsSurface, bsCurves))
		{
			for (int i = 0; i < 4; i++)
				mdlBspline_freeCurve(&bsCurves[i]);
			return false;
		}
		DraftingElementSchema::ToElement(eeh, bsSurface, nullptr, *ACTIVEMODEL);
		mdlBspline_freeSurface(&bsSurface);
		for (int i = 0; i < 4; i++)
			mdlBspline_freeCurve(&bsCurves[i]);
		return true;
	}

	virtual void _OnPostInstall() override
	{
		_BeginDynamics();
		AccuSnap::GetInstance().EnableSnap(true);
		__super::_OnPostInstall();
	}

	virtual void _OnRestartTool() override
	{
		PlaceBsSurfaceTool *pTool = new PlaceBsSurfaceTool(GetToolId(), GetToolPrompt());
		pTool->InstallTool();
	}

	virtual void _OnDynamicFrame(DgnButtonEventCR ev) override
	{
		EditElementHandle eeh;
		if (!CreateBsSurface(eeh, ev.GetPoint()))
			return;

		RedrawElems redrawElems;
		redrawElems.SetDynamicsViews(IViewManager::GetActiveViewSet(), ev.GetViewport());
		redrawElems.SetDrawMode(DRAW_MODE_TempDraw);
		redrawElems.SetDrawPurpose(DrawPurpose::Dynamics);
		redrawElems.DoRedraw(eeh);
	}

	virtual bool _OnDataButton(DgnButtonEventCR ev) override
	{
		EditElementHandle  eeh;
		if (CreateBsSurface(eeh, ev.GetPoint()))
			eeh.AddToModel();
		_OnReinitialize();
		return true;
	}

	virtual bool _OnResetButton(DgnButtonEventCR ev) override
	{
		
		_EndDynamics();
		_ExitTool();
		return true;
	}
};

void createALine(WCharCP unParsed)
{
	DPoint3d basePt = DPoint3d::FromZero();
	EditElementHandle eeh;
	DSegment3d seg;
	seg.Init(basePt, DPoint3d::From(basePt.x + g_1mu * 2, basePt.y + g_1mu));
	ICurvePrimitivePtr pCurve = ICurvePrimitive::CreateLine(seg);
	DraftingElementSchema::ToElement(eeh, *pCurve, nullptr, ACTIVEMODEL->Is3d(), *ACTIVEMODEL);
	eeh.AddToModel();
}

void createAComplexShape(WCharCP unParsed)
{
	DPoint3d basePt = DPoint3d::FromZero();;
	basePt.x += g_1mu * 1.5;
	basePt.y -= g_1mu * 0.3;
	basePt.z -= g_1mu * 0.6;

	EditElementHandle eeh;
	DPoint3d         pts[3];

	pts[0] = pts[1] = pts[2] = basePt;
	pts[1].x += g_1mu * 0.3;    pts[1].y += g_1mu * 0.7;
	pts[0].x += g_1mu;        pts[0].y += g_1mu;
	DEllipse3d arcPts = DEllipse3d::FromPointsOnArc(pts[2], pts[1], pts[0]);
	CurveVectorPtr pCurveVec = CurveVector::Create(CurveVector::BOUNDARY_TYPE_Outer);
	pCurveVec->Add(ICurvePrimitive::CreateArc(arcPts));

	pts[1].x = pts[0].x;    pts[1].y = pts[2].y;
	pCurveVec->Add(ICurvePrimitive::CreateLineString(pts, 3));
	DraftingElementSchema::ToElement(eeh, *pCurveVec, nullptr, ACTIVEMODEL->Is3d(), *ACTIVEMODEL);
	eeh.AddToModel();
}

void createAProjectedSolid(WCharCP unParsed)
{
	DPoint3d basePt = DPoint3d::FromZero();;
	basePt.x += g_1mu * 2.2;
	basePt.y -= g_1mu * 1.7;
	basePt.z -= g_1mu * 0.6;
	DPoint3d  pts[6];
	pts[0] = basePt;
	pts[1].x = pts[0].x;               pts[1].y = pts[0].y - g_1mu / 2;   pts[1].z = pts[0].z;
	pts[2].x = pts[1].x + g_1mu / 2;   pts[2].y = pts[1].y;               pts[2].z = pts[0].z;
	pts[3].x = pts[2].x;               pts[3].y = pts[2].y - g_1mu / 2;   pts[3].z = pts[0].z;
	pts[4].x = pts[3].x + g_1mu / 2;   pts[4].y = pts[3].y;               pts[4].z = pts[0].z;
	pts[5].x = pts[4].x;               pts[5].y = pts[0].y;               pts[5].z = pts[0].z;

	CurveVectorPtr pCurveVec = CurveVector::CreateLinear(pts, 6, CurveVector::BOUNDARY_TYPE_Outer);
	DVec3d extrusionVec = DVec3d::From(0, 0, g_1mu);
	DgnExtrusionDetail  data(pCurveVec, extrusionVec, true);
	ISolidPrimitivePtr pSolid = ISolidPrimitive::CreateDgnExtrusion(data);
	EditElementHandle eeh;
	DraftingElementSchema::ToElement(eeh, *pSolid, nullptr, *ACTIVEMODEL);
	eeh.AddToModel();
}


void createABsplineSurface(WCharCP unparsed)
{
	PlaceBsSurfaceTool *pTool = new PlaceBsSurfaceTool(0, 0);
	pTool->InstallTool();
}

void CreateQueryTool(WCharCP unparsed)
{
	ClickSelectTool::InstallNewInstance(
		1, 
		ElementsQueryOptions::ElementsExampleQuery_Display);
}

void CreateSphereTool(WCharCP unparsed)
{
	double radius = 0.2 * g_1mu;
	ElementCreator::CreateSphereToolImp(0,0,0, radius);
}

MdlCommandNumber cmdNums[] =
{
	{ (CmdHandler)createALine,		        CMD_HELLOWORLD_CREATE_LINE },
	{ (CmdHandler)createAComplexShape,	    CMD_HELLOWORLD_CREATE_COMPLEXSHAPE },
	{ (CmdHandler)createAProjectedSolid,	CMD_HELLOWORLD_CREATE_PROJECTEDSOLID },
	{ (CmdHandler)createABsplineSurface,    CMD_HELLOWORLD_CREATE_BSPLINESURFACE },
	{ (CmdHandler)CreateSphereTool,    CMD_HELLOWORLD_CREATE_SPHERESOLID },
	{ (CmdHandler)CreateQueryTool,    CMD_HELLOWORLD_QUERY },
	0
};


extern "C" DLLEXPORT void MdlMain(int argc, WCharCP argv[])
{
	ModelInfoCP pInfo = ACTIVEMODEL->GetModelInfoCP();
	g_1mu = pInfo->GetUorPerStorage();

	//DPoint3d basePt = DPoint3d::FromZero();
	//createALine(basePt);
	//basePt.x += g_1mu * 1.7;     basePt.y -= g_1mu * 0.3;    basePt.z -= g_1mu * 0.6;
	//createAComplexShape(basePt);
	//basePt.x += g_1mu * 1.5;     basePt.y -= g_1mu * 0.3;    basePt.z -= g_1mu * 0.6;
	//createAProjectedSolid(basePt);
	//basePt.x += g_1mu * 2.2;     basePt.y -= g_1mu * 1.7;    basePt.z -= g_1mu * 0.6;
	//createABsplineSurface(basePt);

	RscFileHandle rscFileH;
	mdlResource_openFile(&rscFileH, NULL, RSC_READONLY);
	mdlParse_loadCommandTable(NULL);

	mdlSystem_registerCommandNumbers(cmdNums);
}

extern "C" __declspec(dllexport) void MdlMain2(int argc, WCharCP argv[])
{
	ShowMsgHelloWorld();
}