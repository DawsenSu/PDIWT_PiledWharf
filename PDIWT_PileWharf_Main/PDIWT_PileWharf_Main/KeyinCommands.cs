using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Encryption;
using PDIWT_PiledWharf_Core;
namespace PDIWT_PiledWharf_Main
{
    public class KeyinCommands
    {
        #region Settings Key-ins
        public static void Settings(String unparsed)
        {
            var _verifyStatus = EncryptionEntrance.Verify();
            if (_verifyStatus == ActivationStatus.Success)
                PDIWT_PiledWharf_Core.SettingsWindow.ShowWindow(Program.Addin);
        }

        #endregion

        public static void PileFrameDivider(string unparsed)
        {
            var _verifyStatus = EncryptionEntrance.Verify();
            if (_verifyStatus == ActivationStatus.Success)
                PileFrameDividerWindow.ShowWindow(Program.Addin);
        }

        public static void Input_ImportFromFile(string unparsed)
        {
            var _verifyStatus = EncryptionEntrance.Verify();
            if (_verifyStatus == ActivationStatus.Success)
                PDIWT_PiledWharf_Core.Input_ImportFromFileWindow.ShowWindow(Program.Addin);
        }
        public static void Input_PilePlacement(string unparsed)
        {
            //var _pilePlacementTool = new PDIWT_PiledWharf_Core.Model.Tools.PilePlacementTool(Program.Addin);
            //_pilePlacementTool.InstallNewInstance();
        }
        public static void Input_AttachBCECInstance(string unparsed)
        {
            var _verifyStatus = EncryptionEntrance.Verify();
            if (_verifyStatus == ActivationStatus.Success)
            {
                var _BCECAttachTool = new PDIWT_PiledWharf_Core.Model.Tools.AttachBCInstanceTool(Program.Addin);
                _BCECAttachTool.InstallNewInstance();
            }

        }
        public static void Process_CalculateBearingCapacity(string unparsed)
        {
            //Bentley.MstnPlatformNET.MessageCenter.Instance.StatusMessage = "Input Process CalcuateBearingCapacity";
            var _verifyStatus = EncryptionEntrance.Verify();
            if (_verifyStatus == ActivationStatus.Success)
                PDIWT_PiledWharf_Core.BearingCapacityWindow.ShowWindow(Program.Addin);
        }

        public static void Process_CalculatePileLengthBasedOnBearingCapacity(string unparsed)
        {
            var _verifyStatus = EncryptionEntrance.Verify();
            if (_verifyStatus == ActivationStatus.Success)
                PileLengthCalculatorWindow.ShowWindow(Program.Addin);
        }

        public static void Process_CalculateCurrentForce(string unparsed)
        {
            var _verifyStatus = EncryptionEntrance.Verify();
            if (_verifyStatus == ActivationStatus.Success)
                CurrentForceWindow.ShowWindow(Program.Addin);
        }

        public static void Process_CalculateWaveForce(string unparsed)
        {
            var _verifyStatus = EncryptionEntrance.Verify();
            if (_verifyStatus == ActivationStatus.Success)
                PDIWT_PiledWharf_Core.WaveForceWindow.ShowWindow(Program.Addin);
            //Bentley.MstnPlatformNET.MessageCenter.Instance.StatusMessage = "Input Process CalcuateWaveForce";
        }
        public static void Process_DetectCollision(string unparsed)
        {
            var _verifyStatus = EncryptionEntrance.Verify();
            if (_verifyStatus == ActivationStatus.Success)
                //Bentley.MstnPlatformNET.MessageCenter.Instance.StatusMessage = "Input Process Detectcollision";
                PileCollisionDetectionWindow.ShowWindow(Program.Addin);
        }

        public static void Output_CalculationNote(string unparsed)
        {
            var _verifyStatus = EncryptionEntrance.Verify();
            if (_verifyStatus == ActivationStatus.Success)
                Bentley.MstnPlatformNET.MessageCenter.Instance.StatusMessage = "Input Output CalculateNote";

        }

        public static void Output_PilePositionDrawing(string unparsed)
        {
            var _verifyStatus = EncryptionEntrance.Verify();
            if (_verifyStatus == ActivationStatus.Success)
                Bentley.MstnPlatformNET.MessageCenter.Instance.StatusMessage = "Input Output PilePositionDrawing";
        }
    }
}
