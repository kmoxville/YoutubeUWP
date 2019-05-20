using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using YoutubeApi;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace YoutubeUWP
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class HomePage : Page, IRefrashablePage
    {
        public ObservableCollection<VideoItem> Videos = new ObservableCollection<VideoItem>();
        private string NextPageToken;
        private double DefaultPageHeight;
        private VideosResource.ListRequest ChannelsListRequest;
        private bool firstTimeScrolled = true;
        private ScrollViewer scrollViewer;

        public bool IsLoading { get; private set; }

        public HomePage()
        {
            this.InitializeComponent();
            this.Loaded += HomePage_Loaded;          
        }

        private async void HomePage_Loaded(object sender, RoutedEventArgs e)
        {
            ChannelsListRequest = App.YoutubeClient.Videos.List(VideosResource.ListRequest.Parts.Snippet + 
                VideosResource.ListRequest.Parts.ContentDetails +
                VideosResource.ListRequest.Parts.Statistics,
                new VideosResource.ListRequest.Filters(VideosResource.ListRequest.Filters.Charts.MostPopular));
            ChannelsListRequest.MaxResults = 50;
            await LoadItems();

            scrollViewer = MainGrid.GetFirstDescendantOfType<ScrollViewer>();
            if (scrollViewer == null)
                return;

            scrollViewer.IsScrollInertiaEnabled = true;
            scrollViewer.ViewChanged += ScrollViewer_ViewChanged;
            scrollViewer.ViewChanged += ScrollViewer_FirstTimeChanged;                  
        }

        private void ScrollViewer_FirstTimeChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var sv = scrollViewer;
            if (firstTimeScrolled == true)
            {
                firstTimeScrolled = false;             
                sv.ViewChanged -= ScrollViewer_FirstTimeChanged;
                DefaultPageHeight = sv.ScrollableHeight;
            }
        }

        private async void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var sv = scrollViewer;

            if (sv.VerticalOffset == 0)
            {
                GoToTopButton.Visibility = Visibility.Collapsed;
                return;
            }
            else
                GoToTopButton.Visibility = Visibility.Visible;

            if ((sv.ScrollableHeight - sv.VerticalOffset) < (DefaultPageHeight / 3)
                && IsLoading != true)
            {
                await LoadMoreItems();
            }
        }

        private async Task LoadMoreItems()
        {
            if (ChannelsListRequest == null || NextPageToken == null)
                return;

            ChannelsListRequest.PageToken = NextPageToken;
            await LoadItems();
        }

        private async Task LoadItems()
        {
            if (IsLoading == true)
                return;

            IsLoading = true;
            //ChannelsListRequest.Hl = App.Region.Gl;
            ChannelsListRequest.RegionCode = App.Region.Gl;
            var response = await ChannelsListRequest.ExecuteAsync();

            foreach (var item in response.VideoItems)
            {
                Videos.Add(item);
            }

            NextPageToken = response.NextPageToken;
            IsLoading = false;
        }

        private void GoToTopButton_Click(object sender, RoutedEventArgs e)
        {
            scrollViewer.ChangeView(null, 0, null);
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshAll();
        }

        public async void RefreshAll()
        {
            ChannelsListRequest.PageToken = null;
            Videos.RemoveAll();
            await LoadItems();
        }
    }

}
