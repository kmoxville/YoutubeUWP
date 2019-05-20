using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пользовательский элемент управления" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234236

namespace YoutubeUWP
{
    public sealed partial class AccountPanel : UserControl
    {
        public AccountPanel()
        {
            this.InitializeComponent();
            IsAuthorized = false;
        }

        public static readonly DependencyProperty IsAuthorizedProperty =
            DependencyProperty.Register("IsAuthorized", typeof(bool), typeof(AccountPanel), null);

        public bool IsAuthorized
        {
            get { return (bool)GetValue(IsAuthorizedProperty); }
            set { SetValue(IsAuthorizedProperty, value); }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginButtonClick?.Invoke(sender, e);
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            LogoutButtonClick?.Invoke(sender, e);
        }

        private void SwitchButton_Click(object sender, RoutedEventArgs e)
        {
            SwitchButtonClick?.Invoke(sender, e);
        }

        public event RoutedEventHandler LoginButtonClick;
        public event RoutedEventHandler LogoutButtonClick;
        public event RoutedEventHandler SwitchButtonClick;
    }
}
