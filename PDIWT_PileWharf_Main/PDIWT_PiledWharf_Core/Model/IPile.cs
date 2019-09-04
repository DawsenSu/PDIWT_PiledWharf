using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BG = Bentley.GeometryNET;

namespace PDIWT_PiledWharf_Core.Model
{
    public interface IPile
    {
        BG.DRay3d GetPileRay3D();
        void DrawInActiveModel();
    }
}
