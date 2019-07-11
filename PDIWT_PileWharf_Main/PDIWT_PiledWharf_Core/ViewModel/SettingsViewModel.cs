using System;
using System.Collections;
using System.Windows;
using System.IO;
using System.Collections.Generic;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using PDIWT_PiledWharf_Core.Model;

using BM = Bentley.MstnPlatformNET;
using BD = Bentley.DgnPlatformNET;
using BDEC = Bentley.DgnPlatformNET.DgnEC;
using BDECS = Bentley.ECObjects.Schema;
using System.Linq;
using Bentley.ECN;

namespace PDIWT_PiledWharf_Core.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class SettingsViewModel : ViewModelBase
    { 
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public SettingsViewModel()
        {

        }


        private double _concreteModulus = 3.2e7;
        /// <summary>
        /// Concrete's Modulus
        /// </summary>
        public double ConcreteModulus
        {
            get { return _concreteModulus; }
            set { Set(ref _concreteModulus, value); }
        }

        private double _concretePoisson = 0.2;
        /// <summary>
        /// Concrete's Poisson Ration
        /// </summary>
        public double ConcretePoisson
        {
            get { return _concretePoisson; }
            set { Set(ref _concretePoisson, value); }
        }


        private double _concreteDensity = 25;
        /// <summary>
        /// Concrete's Density
        /// </summary>
        public double ConcreteDensity
        {
            get { return _concreteDensity; }
            set { Set(ref _concreteDensity, value); }
        }


        private double _steelModulus = 2e8;
        /// <summary>
        /// Steel's Modulus
        /// </summary>
        public double SteelModulus
        {
            get { return _steelModulus; }
            set { Set(ref _steelModulus, value); }
        }


        private double _steelPoisson = 0.3;
        /// <summary>
        /// Steel's Poisson
        /// </summary>
        public double SteelPoisson
        {
            get { return _steelPoisson; }
            set { Set(ref _steelPoisson, value); }
        }


        private double _steelDensity = 78.5;
        /// <summary>
        /// steel's density
        /// </summary>
        public double SteelDensity
        {
            get { return _steelDensity; }
            set { Set(ref _steelDensity, value); }
        }


        private double _hat = 2;
        /// <summary>
        /// Highest Astronomical Tide
        /// </summary>
        public double HAT
        {
            get { return _hat; }
            set { Set(ref _hat, value); }
        }


        private double _mhw = 1;
        /// <summary>
        /// Mean High Water
        /// </summary>
        public double MHW
        {
            get { return _mhw; }
            set { Set(ref _mhw, value); }
        }


        private double _msl = 0;
        /// <summary>
        ///  Mean Sea Level
        /// </summary>
        public double MSL
        {
            get { return _msl; }
            set { Set(ref _msl, value); }
        }


        private double _mlw = -1;
        /// <summary>
        /// Mean Low Water
        /// </summary>
        public double MLW
        {
            get { return _mlw; }
            set { Set(ref _mlw, value); }
        }


        private double _lat = -2;
        /// <summary>
        /// Lowest Astronomical Tide
        /// </summary>
        public double LAT
        {
            get { return _lat; }
            set { Set(ref _lat, value); }
        }

        private RelayCommand _writeEnvParameters;

        /// <summary>
        /// Write the Environment related parameter to dgn model through EC
        /// </summary>
        public RelayCommand WriteEnvParameters
        {
            get
            {
                return _writeEnvParameters
                    ?? (_writeEnvParameters = new RelayCommand(WriteEnvParametersExcuteMethod));
            }
        }

        private void WriteEnvParametersExcuteMethod()
        {
            BM.MessageCenter messageCenter = BM.MessageCenter.Instance;
            BDEC.DgnECManager dgnECManager = BDEC.DgnECManager.Manager;

            string ecschemaName = "PDIWT.01.00.ecschema";

            if(!IsECSchemaInActiveModel(ecschemaName))
            {
                if (GetOrganizationECSchemaFile(out FileInfo pdiwtECSchemaFileInfo, ecschemaName) == BD.StatusInt.Error)
                {
                    messageCenter.ShowErrorMessage($"Can't find {ecschemaName}", $"Can't find {ecschemaName}, Setting Parameters failed.", BM.MessageAlert.Balloon);
                    return;
                }

            }
            //messageCenter.ShowInfoMessage($"Find {pdiwtECSchemaFileInfo.FullName}", $"Find {ecschemaName}", BM.MessageAlert.Dialog);
        }

        /// <summary>
        /// Get the ECSchema file based on Schema Name  
        /// </summary>
        /// <param name="ecSchemaFile">[out]The ECSchema's FileInfo</param>
        /// <param name="ecSchemaName"></param>
        /// <param name="schemaExtension">Default ".xml"</param>
        /// <returns>Success/Error</returns>
        private BD.StatusInt GetOrganizationECSchemaFile(out FileInfo ecSchemaFile, string ecSchemaName,string schemaExtension = ".xml")
        {
            if(!BD.ConfigurationManager.IsVariableDefined("PDIWT_ORGANIZATION_ECSCHEMAPATH"))
            {
                BD.ConfigurationManager.DefineVariable("PDIWT_ORGANIZATION_ECSCHEMAPATH", "$(_USTN_ORGANIZATION)ECSchemas/", BD.ConfigurationVariableLevel.Organization);
            }
            string[] folderpaths = BD.ConfigurationManager.GetVariable("PDIWT_ORGANIZATION_ECSCHEMAPATH").Split(';');
            foreach (var path in folderpaths)
            {
                if (File.Exists(path + ecSchemaName + schemaExtension))
                {
                    ecSchemaFile = new FileInfo(path + ecSchemaName + schemaExtension);
                    return BD.StatusInt.Success;
                }               
            }
            ecSchemaFile = null;
            return BD.StatusInt.Error;
        }

        /// <summary>
        /// Verify the Active DgnModel Contains certain ecschema
        /// </summary>
        /// <param name="ecschemaName">The ecschema Name to be verified.</param>
        /// <returns>returns true if it contains</returns>
        bool IsECSchemaInActiveModel(string wholeecschemaName)
        {
            string[] schemaParts = wholeecschemaName.Split('.');
            return BDEC.DgnECManager.IsSchemaContainedWithinFile(BM.Session.Instance.GetActiveDgnFile(), schemaParts[0], Convert.ToInt32(schemaParts[1]),Convert.ToInt32( schemaParts[2]), BDECS.SchemaMatchType.Exact);
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}