using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace YoutubeUWP
{
    class FlyoutListViewItem : ListViewItem
    {
        public static readonly DependencyProperty SymbolProperty =
            DependencyProperty.Register("Symbol", typeof(string), typeof(FlyoutListViewItem), null);

        public string Symbol
        {
            get { return (string)GetValue(SymbolProperty); }
            set { SetValue(SymbolProperty, value); }
        }
    }
}
