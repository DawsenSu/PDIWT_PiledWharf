#include <Mstn\MdlApi\MdlApi.h>
#include <DgnPlatform\DgnPlatformApi.h>
#include <DgnView\DgnElementSetTool.h>

#include "ElementCreator.h"

USING_NAMESPACE_BENTLEY_DGNPLATFORM
USING_NAMESPACE_BENTLEY_MSTNPLATFORM
USING_NAMESPACE_BENTLEY_MSTNPLATFORM_ELEMENT

void ElementCreator::CreateSphereToolImp(double posX, double posY, double posZ, double dRadius)
{
	DPoint3d dp3dCenter;
	dp3dCenter.x = posX; dp3dCenter.y = posY; dp3dCenter.z = posZ;
	DgnSphereDetail dsd(dp3dCenter, dRadius);
	ISolidPrimitivePtr pSolid = ISolidPrimitive::CreateDgnSphere(dsd);
	EditElementHandle eeh;
	DraftingElementSchema::ToElement(eeh, *pSolid, nullptr, *ACTIVEMODEL);
	eeh.AddToModel();
}
