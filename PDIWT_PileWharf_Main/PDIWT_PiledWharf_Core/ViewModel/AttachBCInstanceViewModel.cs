using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDIWT.Resources.Localization.MainModule;
using PDIWT_PiledWharf_Core.Common;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using PDIWT_PiledWharf_Core.Model;

namespace PDIWT_PiledWharf_Core.ViewModel
{
    public class AttachBCInstanceViewModel : ViewModelBase
    {
        public AttachBCInstanceViewModel()
        {
            _soilLayer = new SoilLayer();
        }
        //private string _number;
        ///// <summary>
        ///// Property Description
        ///// </summary>
        //[EC(SchemaName = "PDIWT", ClassName = "SoilLayerBase",PropertyName ="LayerNumber")]
        //public string Number
        //{
        //    get { return _number; }
        //    set { Set(ref _number, value); }
        //}

        //private string _name;
        ///// <summary>
        ///// Property Description
        ///// </summary>
        //[EC(SchemaName = "PDIWT", ClassName = "SoilLayerBase",PropertyName ="LayerName")]
        //public string Name
        //{
        //    get { return _name; }
        //    set { Set(ref _name, value); }
        //}

        ////********* Driven pile with sealed end *************//
        //private double _dpse_qfi;
        ///// <summary>
        ///// Property Description
        ///// </summary>
        //[EC(SchemaName = "PDIWT", ClassName = "BCDriven_SI", PropertyName ="qfi")]
        //public double DPSE_Qfi
        //{
        //    get { return _dpse_qfi; }
        //    set
        //    {
        //        if (value < 0)
        //            Set(ref _dpse_qfi, 0);
        //        else
        //            Set(ref _dpse_qfi, value);
        //    }
        //}


        //private double _dpse_qr;
        ///// <summary>
        ///// Property Description
        ///// </summary>
        //[EC(SchemaName = "PDIWT", ClassName = "BCDriven_SI", PropertyName ="qr")]
        //public double DPSE_Qr
        //{
        //    get { return _dpse_qr; }
        //    set
        //    {
        //        if (value < 0)
        //            Set(ref _dpse_qr, 0);
        //        else
        //            Set(ref _dpse_qr, value);
        //    }
        //}

        ////********* Steel or concrete tube pile *************//

        //private double _tpsp_qfi;
        ///// <summary>
        ///// Property Description
        ///// </summary>
        //[EC(SchemaName = "PDIWT", ClassName = "BCTube_SI", PropertyName ="qfi")]
        //public double TPSP_Qfi
        //{
        //    get { return _tpsp_qfi; }
        //    set
        //    {
        //        if (value < 0)
        //            Set(ref _tpsp_qfi, 0);
        //        else
        //            Set(ref _tpsp_qfi, value);
        //    }
        //}

        //private double _tpsp_yita;
        ///// <summary>
        ///// Property Description
        ///// </summary>
        //[EC(SchemaName = "PDIWT", ClassName = "BCTube_SI", PropertyName ="Yita")]
        //public double TPSP_Yita
        //{
        //    get { return _tpsp_yita; }
        //    set
        //    {
        //        if (value < 0)
        //            Set(ref _tpsp_yita, 0);
        //        else
        //            Set(ref _tpsp_yita, value);
        //    }
        //}

        //private double _tpsp_qr;
        ///// <summary>
        ///// Property Description
        ///// </summary>
        //[EC(SchemaName = "PDIWT", ClassName = "BCTube_SI", PropertyName ="qr")]
        //public double TPSP_Qr
        //{
        //    get { return _tpsp_qr; }
        //    set
        //    {
        //        if (value < 0)
        //            Set(ref _tpsp_qr, 0);
        //        else
        //            Set(ref _tpsp_qr, value);
        //    }
        //}

        ////********* Cast-In Situ Pile *************//

        //private double _cisp_psisi;
        ///// <summary>
        ///// Property Description
        ///// </summary>
        //[EC(SchemaName = "PDIWT", ClassName = "BCCastIn_SI", PropertyName ="Psisi")]
        //public double CISP_Psisi
        //{
        //    get { return _cisp_psisi; }
        //    set
        //    {
        //        if (value < 0)
        //            Set(ref _cisp_psisi, 0);
        //        else
        //            Set(ref _cisp_psisi, value);
        //    }
        //}

        //private double _cisp_qfi;
        ///// <summary>
        ///// Property Description
        ///// </summary>
        //[EC(SchemaName = "PDIWT", ClassName = "BCCastIn_SI", PropertyName ="qfi")]
        //public double CISP_Qfi
        //{
        //    get { return _cisp_qfi; }
        //    set
        //    {

        //        if (value < 0)
        //            Set(ref _cisp_qfi, 0);
        //        else
        //            Set(ref _cisp_qfi, value);
        //    }
        //}

        //private double _cisp_psip;
        ///// <summary>
        ///// Property Description
        ///// </summary>
        //[EC(SchemaName = "PDIWT", ClassName = "BCCastIn_SI", PropertyName ="Psip")]
        //public double CISP_Psip
        //{
        //    get { return _cisp_psip; }
        //    set
        //    {
        //        if (value < 0)
        //            Set(ref _cisp_psip, 0);
        //        else
        //            Set(ref _cisp_psip, value);
        //    }
        //}

        //private double _cisp_qr;
        ///// <summary>
        ///// Property Description
        ///// </summary>
        //[EC(SchemaName = "PDIWT", ClassName = "BCCastIn_SI", PropertyName ="qr")]
        //public double CISP_Qr
        //{
        //    get { return _cisp_qr; }
        //    set
        //    {
        //        if (value < 0)
        //            Set(ref _cisp_qr, 0);
        //        else
        //            Set(ref _cisp_qr, value);
        //    }
        //}

        ////********* Cast-In Situ After Grouting Pile *************//

        //private double _cisagp_betasi;
        ///// <summary>
        ///// Property Description
        ///// </summary>
        //[EC(SchemaName = "PDIWT", ClassName = "BCCastInAfterGrouting_SI", PropertyName ="Betasi")]
        //public double CISAGP_Betasi
        //{
        //    get { return _cisagp_betasi; }
        //    set
        //    {
        //        if (value < 0)
        //            Set(ref _cisagp_betasi, 0);
        //        else
        //            Set(ref _cisagp_betasi, value);
        //    }
        //}

        //private double _cisagp_psisi;
        ///// <summary>
        ///// Property Description
        ///// </summary>
        //[EC(SchemaName = "PDIWT", ClassName = "BCCastInAfterGrouting_SI", PropertyName ="Psisi")]
        //public double CISAGP_Psisi
        //{
        //    get { return _cisagp_psisi; }
        //    set
        //    {
        //        if (value < 0)
        //            Set(ref _cisagp_psisi, 0);
        //        else
        //            Set(ref _cisagp_psisi, value);
        //    }
        //}

        //private double _cisagp_qfi;
        ///// <summary>
        ///// Property Description
        ///// </summary>
        //[EC(SchemaName = "PDIWT", ClassName = "BCCastInAfterGrouting_SI", PropertyName ="qfi")]
        //public double CISAGP_Qfi
        //{
        //    get { return _cisagp_qfi; }
        //    set
        //    {
        //        if (value < 0)
        //            Set(ref _cisagp_qfi, 0);
        //        else
        //            Set(ref _cisagp_qfi, value);
        //    }
        //}

        //private double _cisagp_betap;
        ///// <summary>
        ///// Property Description
        ///// </summary>
        //[EC(SchemaName = "PDIWT", ClassName = "BCCastInAfterGrouting_SI", PropertyName ="Betap")]
        //public double CISAGP_Betap
        //{
        //    get { return _cisagp_betap; }
        //    set
        //    {
        //        if (value < 0)
        //            Set(ref _cisagp_betap, 0);
        //        else
        //            Set(ref _cisagp_betap, value);
        //    }
        //}

        //private double _cisagp_psip;
        ///// <summary>
        ///// Property Description
        ///// </summary>
        //[EC(SchemaName = "PDIWT", ClassName = "BCCastInAfterGrouting_SI", PropertyName ="Psip")]
        //public double CISAGP_Psip
        //{
        //    get { return _cisagp_psip; }
        //    set
        //    {
        //        if (value < 0)
        //            Set(ref _cisagp_psip, 0);
        //        else
        //            Set(ref _cisagp_psip, value);
        //    }
        //}

        //private double _cisagp_qr;
        ///// <summary>
        ///// Property Description
        ///// </summary>
        //[EC(SchemaName = "PDIWT", ClassName = "BCCastInAfterGrouting_SI", PropertyName ="qr")]
        //public double CISAGP_Qr
        //{
        //    get { return _cisagp_qr; }
        //    set
        //    {
        //        if (value < 0)
        //            Set(ref _cisagp_qr, 0);
        //        else
        //            Set(ref _cisagp_qr, value);
        //    }
        //}

        ////********* Pile Lifting *************//

        //private double _xii;
        ///// <summary>
        ///// Property Description
        ///// </summary>
        //[EC(SchemaName = "PDIWT", ClassName = "BCLF_SI", PropertyName = "xii")]
        //public double Xii
        //{
        //    get { return _xii; }
        //    set
        //    {
        //        if (value < 0)
        //            Set(ref _xii, 0);
        //        else
        //            Set(ref _xii, value);
        //    }
        //}


        private SoilLayer _soilLayer;
        /// <summary>
        /// Property Description
        /// </summary>
        public SoilLayer CurrentSoilLayer
        {
            get { return _soilLayer; }
            set { Set(ref _soilLayer, value); }
        }
    }
}
