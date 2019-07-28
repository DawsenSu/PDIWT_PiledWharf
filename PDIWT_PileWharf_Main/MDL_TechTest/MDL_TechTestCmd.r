#include <Mstn\MdlApi\rscdefs.r.h>
#include <Mstn\MdlApi\cmdclass.r.h>

/*----------------------------------------------------------------------+
|   Local Defines												
+----------------------------------------------------------------------*/
enum CmdTableIds
{
   CT_NONE = 0,
   CT_MAIN,
   CT_SUB,
   CT_CREATE
};
/*----------------------------------------------------------------------+
|   HelloWorld commands											
+----------------------------------------------------------------------*/
CommandTable   CT_MAIN =
{ 
    { 1,  CT_SUB,	PLACEMENT,	  REQ,		"HELLOWORLD" },
};
CommandTable	   CT_SUB =
{
    { 1,  CT_CREATE,    INHERIT,          NONE, 	"CREATE" },
	{ 2,  CT_NONE,    INHERIT,          NONE, 	"QUERY" },
};
CommandTable	   CT_CREATE =
{
    { 1,  CT_NONE, 	INHERIT, 	  DEF, 	        "Line" },
    { 2,  CT_NONE, 	INHERIT, 	  NONE,        "ComplexShape" },
    { 3,  CT_NONE, 	INHERIT, 	  NONE,        "ProjectedSolid" },
    { 4,  CT_NONE, 	INHERIT, 	  NONE,        "BsplineSurface"},
	{ 5,  CT_NONE, 	INHERIT, 	  NONE,        "SphereSolid"},
};