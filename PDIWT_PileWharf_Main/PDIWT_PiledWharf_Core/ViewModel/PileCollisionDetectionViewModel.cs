using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

using System.ComponentModel;
using System.Collections.ObjectModel;
using PDIWT.Formulas;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Controls.Primitives;
using PDIWT.Resources.Localization.MainModule;
using BD = Bentley.DgnPlatformNET;
using BM = Bentley.MstnPlatformNET;
using BDE = Bentley.DgnPlatformNET.Elements;
using BDEC = Bentley.DgnPlatformNET.DgnEC;
using BES = Bentley.ECObjects.Schema;
using BEI = Bentley.ECObjects.Instance;
using BMW = Bentley.MstnPlatformNET.WPF;
using BG = Bentley.GeometryNET;

using System.Windows.Controls;
using PDIWT_PiledWharf_Core.Model;
using PDIWT_PiledWharf_Core.Model.Tools;
using PDIWT_PiledWharf_Core_Cpp;
namespace PDIWT_PiledWharf_Core.ViewModel
{
    public class PileCollisionDetectionViewModel : ViewModelBase
    {
        public PileCollisionDetectionViewModel()
        {
            _minimumDistance = 1;
            _collisionPiles = new DataTable("CollisionPiles");
            _collisionPiles.Columns.Add(new DataColumn("Number", typeof(int))); //0
            _collisionPiles.Columns.Add(new DataColumn("FirstPile", typeof(PileBase))); //1
            _collisionPiles.Columns.Add(new DataColumn("SecondPile", typeof(PileBase))); //2
            _collisionPiles.Columns.Add(new DataColumn("MinimumDistance", typeof(double))); //3
            _collisionPiles.Columns.Add(new DataColumn("MinimumSegment", typeof(BG.DSegment3d))); //4
            //_collisionPiles.Columns.Add(new DataColumn("Trans", typeof(TransientSegmentElement))); //5
        }

        readonly BM.MessageCenter _mc = BM.MessageCenter.Instance;

        private double _minimumDistance;
        /// <summary>
        /// minimum distance between piles, unit:m
        /// </summary>
        public double MinimumDistance
        {
            get { return _minimumDistance; }
            set
            {
                if (value < 0)
                    Set(ref _minimumDistance, 0);
                else
                    Set(ref _minimumDistance, value);
            }
        }

        private DataTable _collisionPiles;
        /// <summary>
        /// Property Description
        /// </summary>
        public DataTable CollisionPiles
        {
            get { return _collisionPiles; }
            set { Set(ref _collisionPiles, value); }
        }

        private RelayCommand _windowClosing;

        /// <summary>
        /// Gets the WindowClosing.
        /// </summary>
        public RelayCommand WindowClosing
        {
            get
            {
                return _windowClosing
                    ?? (_windowClosing = new RelayCommand(ExecuteWindowClosing));
            }
        }

        private void ExecuteWindowClosing()
        {
            try
            {
                BD.ElementAgendaDisplayable _disappear = new BD.ElementAgendaDisplayable();
                BD.DgnModel _activeModel = BM.Session.Instance.GetActiveDgnModel();

                foreach (DataRow _row in CollisionPiles.Rows)
                {
                    _disappear.Insert(_activeModel.FindElementById((BD.ElementId)(_row[1] as PileBase).Numbering), false);
                    _disappear.Insert(_activeModel.FindElementById((BD.ElementId)(_row[2] as PileBase).Numbering), false);
                }
                _disappear.ClearHilite();

                CollisionPiles.Clear();
            }
            catch(Exception e)
            {
                _mc.ShowErrorMessage(e.Message, e.ToString(), false);
            }

        }

        private RelayCommand<SelectionChangedEventArgs> _collisionPileSelectionChanged;

        /// <summary>
        /// Gets the CollisionPileSelectionChanged.
        /// </summary>
        public RelayCommand<SelectionChangedEventArgs> CollisionPileSelectionChanged
        {
            get
            {
                return _collisionPileSelectionChanged
                    ?? (_collisionPileSelectionChanged = new RelayCommand<SelectionChangedEventArgs>(ExecuteCollisionPileSelectionChanged));
            }
        }

        private void ExecuteCollisionPileSelectionChanged(SelectionChangedEventArgs parameter)
        {
            try
            {
                BD.ElementAgendaDisplayable _display = new BD.ElementAgendaDisplayable();
                BD.ElementAgendaDisplayable _disappear = new BD.ElementAgendaDisplayable();
                BD.DgnModel _activeModel = BM.Session.Instance.GetActiveDgnModel();
                foreach (var _item in parameter.AddedItems)
                {
                    DataRowView _dataRow = _item as DataRowView;
                    _display.Insert(_activeModel.FindElementById((BD.ElementId)(_dataRow.Row[1] as PileBase).Numbering), false);
                    _display.Insert(_activeModel.FindElementById((BD.ElementId)(_dataRow.Row[2] as PileBase).Numbering), false);
                    //BDE.LineElement _line = new BDE.LineElement(_activeModel, null, (BG.DSegment3d)_dataRow[4]);
                    //_dataRow.Row[5] = new TransientSegmentElement();
                    //IntPtr _ptr = _line.GetNativeElementRef();
                    //((TransientSegmentElement)_dataRow.Row[5]).Show(ref _ptr);

                }
                foreach (var _item in parameter.RemovedItems)
                {
                    DataRowView _dataRow = _item as DataRowView;
                    _disappear.Insert(_activeModel.FindElementById((BD.ElementId)(_dataRow.Row[1] as PileBase).Numbering), false);
                    _disappear.Insert(_activeModel.FindElementById((BD.ElementId)(_dataRow.Row[2] as PileBase).Numbering), false);
                    //if(_dataRow[5] != null)
                    //    ((TransientSegmentElement)_dataRow.Row[5]).Free();
                }
                _display.Hilite();
                _disappear.ClearHilite();
            }
            catch(Exception e)
            {
                _mc.ShowErrorMessage("Selection Change fail", e.ToString(), false);
            }
        }



        private RelayCommand _calculate;

        /// <summary>
        /// Gets the Calculate.
        /// </summary>
        public RelayCommand Calculate
        {
            get
            {
                return _calculate
                    ?? (_calculate = new RelayCommand(ExecuteCalculate));
            }
        }

        private void ExecuteCalculate()
        {
            try
            {
                CollisionPiles.Clear();
                BD.DgnModel _activeDgnModel = BM.Session.Instance.GetActiveDgnModel();
                BDEC.DgnECManager _dgnECManger = BDEC.DgnECManager.Manager;
                double _uorpermeter = BM.Session.Instance.GetActiveDgnModel().GetModelInfo().UorPerMeter;
                List<PileBase> _piles = new List<PileBase>();

                BD.ScanCriteria _sc = new BD.ScanCriteria();
                _sc.SetModelRef(_activeDgnModel);
                _sc.SetModelSections(BD.DgnModelSections.GraphicElements);
                BD.BitMask _meshBitMask = new BD.BitMask(false);
                _meshBitMask.Capacity = 400;
                _meshBitMask.ClearAll();
                _meshBitMask.SetBit(1, true);
                _sc.SetElementTypeTest(_meshBitMask);
                _sc.Scan((_element, _model) =>
                {
                    if(ECSChemaReader.IsElementAttachedECInstance(_element, "IfcPort", "IfcPile"))
                    {
                        _piles.Add(PileBase.ObtainFromPileCell((BDE.CellHeaderElement)_element));
                    }
                    return BD.StatusInt.Success;
                });
                if (_piles.Count == 0)
                {
                    _mc.ShowInfoMessage("No Piles", "", false);
                    return; 
                }

                int _index = 1;
                for (int i = 0; i < _piles.Count; i++)
                {
                    for (int j = i+1; j < _piles.Count; j++)
                    {
                        _piles[i].ClosestDistanceFromAnotherPile(_piles[j], out double _pile2pileMinDistance, out BG.DSegment3d _transientDS);
                        if(_pile2pileMinDistance / _uorpermeter <= MinimumDistance)
                        {
                            DataRow _newRow = CollisionPiles.NewRow();
                            _newRow[0] = _index;
                            _newRow[1] = _piles[i];
                            _newRow[2] = _piles[j];
                            _newRow[3] = _pile2pileMinDistance / _uorpermeter;
                            _newRow[4] = _transientDS;
                            CollisionPiles.Rows.Add(_newRow);
                            _index++;
                        }
                    }
                }
                if (CollisionPiles.Rows.Count == 0)
                    _mc.ShowInfoMessage("No pile collision", "", false);
                else
                    _mc.ShowInfoMessage("Collision Detection finished", "", false);
            }
            catch (Exception e)
            {
                _mc.ShowErrorMessage("Calculate Pile Distance Fail", e.ToString(), false);
            }
        }
    }
}
