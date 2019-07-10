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
            //MessageBox.Show("Test!", "This is a functional test");
            //PDIWT_PiledWharf_Core.MainWindow.ShowWindow(Program.Addin);
            PDIWT_PiledWharf_Core_Cpp.TestClass test = new PDIWT_PiledWharf_Core_Cpp.TestClass();
            test.OutputMessage("Test");
        }
        #endregion
    }
}
