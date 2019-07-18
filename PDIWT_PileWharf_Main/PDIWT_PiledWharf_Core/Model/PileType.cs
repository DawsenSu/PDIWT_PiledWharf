using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight;

namespace PDIWT_PiledWharf_Core.Model
{
    public class PileType : ObservableObject
    {

        private string _namePath;
        /// <summary>
        /// Name path
        /// </summary>
        public string NamePath
        {
            get { return _namePath; }
            set { Set(ref _namePath, value); }
        }


        private string _value;
        /// <summary>
        /// Property Description
        /// </summary>
        public string Value
        {
            get { return _value; }
            set { Set(ref _value, value); }
        }
    }
}
