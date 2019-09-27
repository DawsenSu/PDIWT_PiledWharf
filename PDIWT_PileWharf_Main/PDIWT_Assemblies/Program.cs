using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using BM = Bentley.MstnPlatformNET;
using BD = Bentley.DgnPlatformNET;
using BCOM = Bentley.Interop.MicroStationDGN;

namespace PDIWT_Assemblies
{
    [BM.AddIn(MdlTaskID ="PDIWT_Assemblies")]
    internal sealed class Program : BM.AddIn
    {
        public static Program Addin = null;

        private Program(IntPtr mdlDesc) : base(mdlDesc)
        {
            Addin = this;

            string _language = "en-US";
            string _languageVariable = "PDIWT_LANGUAGE";
            if (BD.ConfigurationManager.IsVariableDefined(_languageVariable))
            {
                _language = BD.ConfigurationManager.GetVariable(_languageVariable, BD.ConfigurationVariableLevel.Organization);
            }
            List<string> _languagelist = new List<string> { "en-US", "zh-CN" };
            if (!_languagelist.Contains(_language))
            {
                BM.MessageCenter.Instance.ShowMessage(BM.MessageType.Warning, $"Variable {_languageVariable} in organization level doesn't define correctly", "change to default en-US", BM.MessageAlert.Balloon);
                _language = "en-US";
            }
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(_language);
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
