using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;

namespace PDIWT_PiledWharf_Core.Common
{
    public interface IPDIWTDialogService<TInfo>
    {
        PDIWTDialogResults ShowDialog(TInfo info);
    }

    public enum PDIWTDialogResults
    {
        None = 1,
        Ok,
        Cancel
    }
}
