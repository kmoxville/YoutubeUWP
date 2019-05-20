using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeUWP
{
    public class NavMenuItem : INotifyPropertyChanged
    {
        public string Label { get; set; }
        public string Symbol { get; set; }
        public Role Role { get; set; }

        public Type DestPage { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class FlyoutMenuItem : INotifyPropertyChanged
    {
        public string Symbol { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.

            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum Role
    {
        Main,
        Alt,
        Channel
    }
}
