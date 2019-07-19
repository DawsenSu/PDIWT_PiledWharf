using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace PDIWT_PiledWharf_Core.Model
{
    public class NotifyErrorBehavior : Behavior<UIElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.AddHandler(Validation.ErrorEvent, new RoutedEventHandler(HasErrorChanged));
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.RemoveHandler(Validation.ErrorEvent, new RoutedEventHandler(HasErrorChanged));
        }

        private static void HasErrorChanged(object sender, RoutedEventArgs e)
        {
            DependencyObject d = e.OriginalSource as DependencyObject;
            HasErrorHelper.SetHasError(d, Validation.GetHasError(d));            
        }
    }

    public class HasErrorHelper
    {
        public static bool GetHasError(DependencyObject obj)
        {
            return (bool)obj.GetValue(HasErrorProperty);
        }

        public static void SetHasError(DependencyObject obj, bool value)
        {
            obj.SetValue(HasErrorProperty, value);
        }

        // Using a DependencyProperty as the backing store for HasError.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HasErrorProperty =
            DependencyProperty.RegisterAttached("HasError", typeof(bool), typeof(HasErrorHelper), new PropertyMetadata(false));
    }
}
