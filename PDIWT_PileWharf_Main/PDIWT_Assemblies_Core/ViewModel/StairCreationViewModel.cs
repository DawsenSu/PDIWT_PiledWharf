using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

using BD = Bentley.DgnPlatformNET;
using BM = Bentley.MstnPlatformNET;
using BDEC = Bentley.DgnPlatformNET.DgnEC;
using BES = Bentley.ECObjects.Schema;
using BEI = Bentley.ECObjects.Instance;
using BDE = Bentley.DgnPlatformNET.Elements;
using BMW = Bentley.MstnPlatformNET.WPF;
using BG = Bentley.GeometryNET;

namespace PDIWT_Assemblies_Core.ViewModel
{
    public  class StairCreationViewModel : ViewModelBase
    {
        private RelayCommand _test;

        /// <summary>
        /// Gets the Test.
        /// </summary>
        public RelayCommand Test
        {
            get
            {
                return _test
                    ?? (_test = new RelayCommand(ExecuteTest));
            }
        }

        private void ExecuteTest()
        {
            _mc.ShowInfoMessage("Test", "", BM.MessageAlert.None);
        }

        readonly BM.MessageCenter _mc = BM.MessageCenter.Instance;

    }
}
