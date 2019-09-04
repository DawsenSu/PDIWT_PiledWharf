using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight;

using System.ComponentModel;
using System.Collections.ObjectModel;

using BD = Bentley.DgnPlatformNET;
using BG = Bentley.GeometryNET;
using BM = Bentley.MstnPlatformNET;
using BDE = Bentley.DgnPlatformNET.Elements;
using PDIWT.Formulas;

namespace PDIWT_PiledWharf_Core.Model
{
    //todo to complement in the future
    public class PileFrame : ObservableObject
    {
        public PileFrame()
        {
            Piles = new List<PileBase>();
            FrameNumber = 0;
        }
        public PileFrame(List<PileBase> piles, long frameNumber)
        {
            Piles = piles;
            FrameNumber = frameNumber;
        }
        public List<PileBase> Piles { get; }

        public long FrameNumber { get; set; }

        public long NumberOfPilesInFrame => Piles.Count;


        public bool Add(PileBase pile)
        {
            if (Piles.Contains(pile))
                return false;
            else
            {
                Piles.Add(pile);
                return true;
            }
        }

        public void Clear() => Piles.Clear();
        /// <summary>
        /// Clone the this PileFrame for next PileFrame with given offset
        /// </summary>
        /// <param name="offset">the offset from next pileframe to current one</param>
        /// <returns>The next pileframe</returns>
        public PileFrame CloneNextWithOffset(double offset)
        {
            long _numberofpiles = NumberOfPilesInFrame;
            long _biggestJointNumbering = 0;
            long _biggestPileNumbering = 0;
            foreach (var _pile in Piles)
            {
                if (_pile.TopJoint.Numbering > _biggestJointNumbering)
                    _biggestJointNumbering = _pile.TopJoint.Numbering;
                if (_pile.BottomJoint.Numbering > _biggestJointNumbering)
                    _biggestJointNumbering = _pile.BottomJoint.Numbering;

                if (_pile.Numbering > _biggestPileNumbering)
                    _biggestPileNumbering = _pile.Numbering;
            }

            List<PileBase> _nextPilesList = new List<PileBase>();
            for (int i = 0; i < _numberofpiles; i++)
            {
                // top joint goes first, then bottom joint
                Joint _newTopJoint = new Joint(_biggestJointNumbering + i + 1,
                                               Piles[i].TopJoint.Point.X + offset,
                                               Piles[i].TopJoint.Point.Y,
                                               Piles[i].TopJoint.Point.Z);
                Joint _newBottomJoint = new Joint(_biggestJointNumbering + i + 2,
                                                  Piles[i].BottomJoint.Point.X + offset,
                                                  Piles[i].BottomJoint.Point.Y,
                                                  Piles[i].BottomJoint.Point.Z);
                PileBase _newPile = new PileBase(_biggestPileNumbering + i + 1,
                                         _biggestJointNumbering + i + 1,
                                         _biggestJointNumbering + i + 2,
                                         new List<Joint> { _newBottomJoint, _newTopJoint });
                _nextPilesList.Add(_newPile);
            }
            return new PileFrame(_nextPilesList, FrameNumber + 1);
        }
        public double GetOffsetFrom(PileFrame pileFrameFrom)
        {
            return pileFrameFrom.Piles.First().TopJoint.Point.Distance(Piles.First().TopJoint.Point);
        }

        public override string ToString()
        {
            StringBuilder _sb = new StringBuilder();
            _sb.Append(FrameNumber.ToString() + "\n");
            foreach (var _pile in Piles)
            {
                _sb.Append(_pile + "; ");
            }
            return _sb.ToString();
        }

    }
}
