using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MailClient.Forms.POP3Indbakke.NS.MVVM
{
    public class PasswordBoxBindingBehavior
    {
        public static DependencyProperty EnableBindingProperty = DependencyProperty.RegisterAttached(
            "EnableBinding", typeof(bool), typeof(PasswordBoxBindingBehavior),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(EnableBindingPropertyChanged)));

        public static void SetEnableBinding(PasswordBox target, bool value)
        {
            target.SetValue(PasswordBoxBindingBehavior.EnableBindingProperty, value);
        }

        public static bool GetEnableBinding(PasswordBox target)
        {
            return (bool)target.GetValue(EnableBindingProperty);
        }


        public static readonly DependencyProperty BoundPasswordProperty = DependencyProperty.RegisterAttached(
            "BoundPassword", typeof(string), typeof(PasswordBoxBindingBehavior), new UIPropertyMetadata(String.Empty));

        public static string GetBoundPassword(PasswordBox target)
        {
            return (string)target.GetValue(BoundPasswordProperty);
        }

        public static void SetBoundPassword(PasswordBox target, string value)
        {
            target.SetValue(BoundPasswordProperty, value);
        }

        private static void EnableBindingPropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox element = target as PasswordBox;

            if (element != null)
            {
                if (e.NewValue != null && (bool)e.NewValue)
                {
                    element.PasswordChanged += Element_PasswordChanged;
                }
                else
                {
                    element.PasswordChanged -= Element_PasswordChanged;
                }
            }

        }

        static void Element_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = (PasswordBox)sender;

            if (GetEnableBinding(passwordBox))
            {
                SetBoundPassword(passwordBox, passwordBox.Password);
            }
        }
    }
}
