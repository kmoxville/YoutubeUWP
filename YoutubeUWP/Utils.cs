
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace YoutubeUWP
{
    public static class ExtensionMethods
    {
        public static void RemoveAll<T>(
            this ObservableCollection<T> coll)
        {
            var list = coll.ToList();
            foreach (var itemToRemove in list)
            {
                coll.Remove(itemToRemove);
            }
        }
    }
}