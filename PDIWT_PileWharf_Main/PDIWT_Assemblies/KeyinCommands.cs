using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDIWT_Assemblies_Core;
using PDIWT_Assemblies_Core.Model.Tools;
using Encryption;

namespace PDIWT_Assemblies
{
    public class KeyinCommands
    {
        public static void CurrentVectors(string unparsed)
        {
            var _verifyStatus = EncryptionEntrance.Verify();
            if ( _verifyStatus == ActivationStatus.Success)
                CurrentVectorsWindow.ShowWindow(Program.Addin);
        }

        public static void Stair(string unparsed)
        {
            var _verifyStatus = EncryptionEntrance.Verify();
            if (_verifyStatus == ActivationStatus.Success)
            {
                StairCreationTool _tool = new StairCreationTool(Program.Addin);
                _tool.InstallNewInstance();
            }
        }

    }
}
