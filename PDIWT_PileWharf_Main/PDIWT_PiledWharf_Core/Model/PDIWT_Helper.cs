using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PDIWT_PiledWharf_Core.Model
{
    public class PDIWT_Helper
    {
        static public  bool EnumTextBoxHasError(DependencyObject framework)
        {
            foreach (var _item in LogicalTreeHelper.GetChildren(framework))
            {
                if (_item is DependencyObject)
                    if (((_item is TextBox) && Validation.GetHasError((TextBox)_item)) || EnumTextBoxHasError((DependencyObject)_item))
                        return true;

            }
            return false;
        }
    }
}
