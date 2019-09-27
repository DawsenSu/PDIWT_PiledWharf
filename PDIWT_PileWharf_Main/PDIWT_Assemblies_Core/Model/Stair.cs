using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;


namespace PDIWT_Assemblies_Core.Model
{
    public class Stair : ObservableObject
    {

        private double _height;
        /// <summary>
        /// Unit: m
        /// </summary>
        public double Height
        {
            get { return _height; }
            set
            {
                if (value < 0)
                    Set(ref _height, 0);
                else
                    Set(ref _height, value);
            }
        }

        private double _depth;
        /// <summary>
        /// Unti: m
        /// </summary>
        public double Depth
        {
            get { return _depth; }
            set
            {
                if(value < 0)
                    Set(ref _depth, 0);
                else
                    Set(ref _depth, value);
            }
        }

        private double _width;
        /// <summary>
        /// Unit:m
        /// </summary>
        public double Width
        {
            get { return _width; }
            set { Set(ref _width, value); }
        }
    }
}
