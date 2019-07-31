using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PDIWT_PiledWharf_Main
{
    public class KeyinCommands
    {
        #region Settings Key-ins
        public static void Settings(String unparsed)
        {
            PDIWT_PiledWharf_Core.SettingsWindow.ShowWindow(Program.Addin);
        }
        #endregion

        public static void Input_ImportFromFile(string unparsed)
        {
            PDIWT_PiledWharf_Core.Input_ImportFromFileWindow.ShowWindow(Program.Addin);
            //Bentley.MstnPlatformNET.MessageCenter.Instance.StatusMessage = "Input Import FromFile";
        }
        public static void Input_PilePlacement(string unparsed)
        {
            Bentley.MstnPlatformNET.MessageCenter.Instance.StatusMessage = "Input Import PliePlacement";
        }
        

        public static void Process_CalculateBearingCapacity(string unparsed)
        {
            Bentley.MstnPlatformNET.MessageCenter.Instance.StatusMessage = "Input Process CalcuateBearingCapacity";

        }
        public static void Process_CalculateCurrentForce(string unparsed)
        {
            PDIWT_PiledWharf_Core.CurrentForceWindow.ShowWindow(Program.Addin);
        }

        public static void Process_CalculateWaveForce(string unparsed)
        {

            Bentley.MstnPlatformNET.MessageCenter.Instance.StatusMessage = "Input Process CalcuateWaveForce";
        }
        public static void Process_DetectCollision(string unparsed)
        {
            Bentley.MstnPlatformNET.MessageCenter.Instance.StatusMessage = "Input Process Detectcollision";

        }

        public static void Output_CalculationNote(string unparsed)
        {
            Bentley.MstnPlatformNET.MessageCenter.Instance.StatusMessage = "Input Output CalculateNote";

        }

        public static void Output_PilePositionDrawing(string unparsed)
        {
            Bentley.MstnPlatformNET.MessageCenter.Instance.StatusMessage = "Input Output PilePositionDrawing";

        }
    }
}
