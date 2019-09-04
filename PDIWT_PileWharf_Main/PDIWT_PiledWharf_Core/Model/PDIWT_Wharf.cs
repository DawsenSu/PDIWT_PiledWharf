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

    public class Wharf : ObservableObject
    {
        public Wharf()
        {
            PileFrameList = new List<PileFrame>();
            FrameSpans = new List<double>();
        }
        public Wharf(List<PileFrame> pileFrames)
        {
            PileFrameList = pileFrames;
            FrameSpans = new List<double>();
            for (int i = 0; i < pileFrames.Count - 1; i++)
            {
                FrameSpans.Add(pileFrames[i].GetOffsetFrom(pileFrames[i + 1]));
            }
        }

        public Wharf(PileFrame firstPileFrame, List<double> spans)
        {
            PileFrameList = new List<PileFrame>() { firstPileFrame };
            List<double> _distanceFromFirstFrame = new List<double>();
            for (int i = 0; i < spans.Count; i++)
            {
                if (i == 0)
                    _distanceFromFirstFrame.Add(spans[i]);
                else
                    _distanceFromFirstFrame.Add(_distanceFromFirstFrame[i - 1] + spans[i]);
            }
            foreach (var _distance in _distanceFromFirstFrame)
            {
                PileFrameList.Add(firstPileFrame.CloneNextWithOffset(_distance));
            }
        }

        List<PileFrame> PileFrameList { get; }
        public List<double> FrameSpans { get; }

        public long NumberOfFrames => PileFrameList.Count;
        public long NumberOfPiles
        {
            get
            {
                long _numberOfPiles = 0;
                foreach (var _frame in PileFrameList)
                {
                    _numberOfPiles += _frame.NumberOfPilesInFrame;
                }
                return _numberOfPiles;
            }
        }

        public long NumberOfJoints => 2 * NumberOfPiles;

        public override string ToString()
        {
            StringBuilder _sb = new StringBuilder();
            _sb.AppendFormat("Wharf Information: Contains {0} Frames, {1} Piles, {2} Joints\n", NumberOfFrames, NumberOfPiles, NumberOfJoints);
            foreach (var _frame in PileFrameList)
            {
                _sb.Append(_frame);
            }
            return _sb.ToString();
        }
    }
}
