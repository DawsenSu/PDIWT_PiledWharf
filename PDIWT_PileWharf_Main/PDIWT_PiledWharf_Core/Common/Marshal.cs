using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PDIWT_PiledWharf_Core.Common
{
    public static class Marshal
    {
        [DllImport("ustation.dll", CharSet = CharSet.Auto,CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr mdlDialog_completionBarOpen(string messageText);

        [DllImport("ustation.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public extern static void mdlDialog_completionBarUpdate(IntPtr dialog, string messageText, int percentComplete);
        [DllImport("ustation.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public extern static void mdlDialog_completionBarClose(IntPtr dialog);
    }
}
