
#include "TechTest.h"
#include "ElementCreator.h"


int TechTest::mst_iClickCounter = 0;
DRay3d TechTest::mst_ray;

bool TechTest::TestIntersectionRayAndMesh(ElementHandle elem)
{
	ModelInfoCP pInfo = ACTIVEMODEL->GetModelInfoCP();
	auto g1mu = pInfo->GetUorPerStorage();
	try {
		//Code for testing the function caculating intersection points between two elements.
		++mst_iClickCounter;
		if (mst_iClickCounter == 1)
		{
			CurveVectorPtr pCurve = ICurvePathQuery::ElementToCurveVector(elem);
			if (false == pCurve.IsValid())
			{
				throw L"pCurve is not valid.";
			}
			auto primCurve = pCurve->GetCyclic(0);
			if (false == primCurve.IsValid())
			{
				throw L"pCurve is not valid.";
			}
			ICurvePrimitive::CurvePrimitiveType  prmCvType = primCurve->GetCurvePrimitiveType();
			if (ICurvePrimitive::CurvePrimitiveType::CURVE_PRIMITIVE_TYPE_Line != prmCvType)
			{
				throw L"Curve is not line type.";
			}

			DSegment3d segment;
			if (false == primCurve->TryGetLine(segment))
			{
				throw L"Cannot get the Line by primeCurve.";
			}

			mst_ray.InitFrom(segment);
		}
		if (mst_iClickCounter == 2)
		{
			mst_iClickCounter = 0;
			//Get the PolyfaceVisitor
			auto pH = &(elem.GetHandler());
			auto query = dynamic_cast<IMeshQuery*>(pH);

			if (nullptr == query)
			{
				throw L"query is null.";
			}
			PolyfaceHeaderPtr piPoly;
			if (SUCCESS != query->GetMeshData(elem, piPoly))
			{
				throw L"mesh is not a polyface mesh.";
			}
			auto visitor = PolyfaceVisitor::Attach(*piPoly, true);

			DPoint3d dp3dIntersect;
			double dFraction = 0;
			bool rtl = visitor->AdvanceToFacetBySearchRay(mst_ray, 0.5, dp3dIntersect, dFraction);
			if (rtl == false)
				throw L"not have intersection point.";

			dp3dIntersect.x = dp3dIntersect.x;// / g1mu;
			dp3dIntersect.y = dp3dIntersect.y;// / g1mu;
			dp3dIntersect.z = dp3dIntersect.z;// / g1mu;

			//Verify and study the mean of dFraction.
			//dFraction means that scale between lenth from origin point to intersection and line lenth.

			//DPoint3d dpOrigin;
			//DPoint3d dpTarget;
			//m_ray.EvaluateEndPoints(dpOrigin, dpTarget);
			//auto lenLine = dpTarget.Distance(dpOrigin);
			//auto len = dp3dIntersect.Distance(m_ray.origin);
			//double percent = len / lenLine;
			//lenLine = percent;

			ElementCreator::CreateSphereToolImp(dp3dIntersect.x, dp3dIntersect.y, dp3dIntersect.z, 0.2 * g1mu);
		}
	}
	catch (WCharCP strErr)
	{
		mdlDialog_openInfoBox(strErr);
		mst_iClickCounter = 0;
		return false;
	}

	return true;
}
