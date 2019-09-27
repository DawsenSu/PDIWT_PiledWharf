using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDIWT_Assemblies_Core;
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


    }
}
