using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;

using BM = Bentley.MstnPlatformNET;
using BD = Bentley.DgnPlatformNET;
using BCOM = Bentley.Interop.MicroStationDGN;


namespace PDIWT_PiledWharf_Main
{
    [BM.AddIn(MdlTaskID = "PDIWT_PiledWharf_Main")]
    internal sealed class Program : BM.AddIn
    {
        public static Program Addin = null;

        private Program(IntPtr mdlDesc) : base(mdlDesc)
        {
            Addin = this;
        }

        protected override int Run(string[] commandLine)
        {
            ReloadEvent += PDIWT_PiledWharf_Main_ReloadEvent;
            UnloadedEvent += PDIWT_PiledWharf_Main_UnloadedEvent;
            return 0;
        }

        private void PDIWT_PiledWharf_Main_ReloadEvent(BM.AddIn sender, ReloadEventArgs eventArgs)
        {

        }

        private void PDIWT_PiledWharf_Main_UnloadedEvent(BM.AddIn sender, UnloadedEventArgs eventArgs)
        {

        }

        public static BD.DgnFile GetActiveDgnFile() => BM.Session.Instance.GetActiveDgnFile();
        public static BD.DgnModelRef GetActiveDgnModelRef() => BM.Session.Instance.GetActiveDgnModelRef();
        public static BD.DgnModel GetActiveDgnModel() => BM.Session.Instance.GetActiveDgnModel();

        public static BCOM.Application COM_App
        {
            get
            {
                return BM.InteropServices.Utilities.ComApp;
            }
        }

    }
}
