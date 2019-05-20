using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Credentials;
using Windows.UI;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using YoutubeApi;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace YoutubeUWP
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ObservableCollection<I18Item> Regions = new ObservableCollection<I18Item>();

        public MainPage()
        {
            this.InitializeComponent();

            //Handling titlebar
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            UpdateTitleBarLayout(coreTitleBar);
            Window.Current.SetTitleBar(TitleBar);
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
            coreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;

            AccountsSettingsPane.GetForCurrentView().AccountCommandsRequested += BuildPaneAsync;

            //Finished initialization
            //Navigate to default page
            NavigationMenuList.SelectedIndex = 0;
        }

        private void BuildPaneAsync(AccountsSettingsPane sender, AccountsSettingsPaneCommandsRequestedEventArgs e)
        {
            var deferral = e.GetDeferral();

            e.HeaderText = "Sign in to get access to live tiles and notifications";

            var googleProvider = new WebAccountProvider("youtube", "Youtube", new Uri("ms-appx:///Assets/Images/logo.png"));
            var providerCommand = new WebAccountProviderCommand(googleProvider, WebAccountProviderCommandInvoked);
            e.WebAccountProviderCommands.Add(providerCommand);

            List<WebAccount> accounts = App.GetAllAccounts();

            foreach (WebAccount account in accounts)
            {
                WebAccountCommand command = new WebAccountCommand(account, WebAccountInvoked, SupportedWebAccountActions.Remove);
                e.WebAccountCommands.Add(command);
            }

            deferral.Complete();
        }

        private void WebAccountInvoked(WebAccountCommand command, WebAccountInvokedArgs args)
        {
            if (args.Action == WebAccountAction.Remove)
            {
                App.LogoffAndRemoveAccount(command.WebAccount);
            }
            else if (args.Action == WebAccountAction.Reconnect)
            {
                // Display user management UI for this account
            }
        }

        private async void WebAccountProviderCommandInvoked(WebAccountProviderCommand command)
        {
            if (command.WebAccountProvider.Id == "youtube")
            {
                // Show user registration/login for your app specific account type.
                // Store the user if registration/login successful
                var account = new WebAccount(command.WebAccountProvider, "App Specific User(" + DateTime.Now + ")", WebAccountState.Connected);
                var details = await App.YoutubeClient.AuthentificateAsync("578338675903-psaaj9eaukg7bvii3a4h17jd6r01vff2.apps.googleusercontent.com", "XcNuCRCoOcFXVoCzk2axCKhN");
                if (details != null)
                    App.StoreNewAccountDataLocally(account, details);
            }
        }

        private async Task LoadRegions()
        {
            I18nRegionsResource.ListRequest I18ListRequest = 
                App.YoutubeClient.I18nRegions.List(I18nRegionsResource.ListRequest.Parts.Snippet);
            var response = await I18ListRequest.ExecuteAsync();
            foreach (var item in response.I18Items)
            {
                Regions.Add(item);
            }
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            UpdateTitleBarLayout(sender);
        }

        private void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreTitleBar)
        {
            if (coreTitleBar.IsVisible)
            {
                TopBarButton.Margin = new Thickness(0, 0, coreTitleBar.SystemOverlayRightInset, 0);
                TopBarButton.VerticalAlignment = VerticalAlignment.Top;
                TopBarButton.Background = new SolidColorBrush(Color.FromArgb(20, 0, 0, 0));
            }
            else
            {
                TopBarButton.Margin = new Thickness(0, 0, 0, 0);
                TopBarButton.VerticalAlignment = VerticalAlignment.Center;               
                TopBarButton.Background = new SolidColorBrush(Colors.Transparent);
            }
        }

        private void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            UpdateTitleBarLayout(coreTitleBar);
        }

        private void hamButton_Click(object sender, RoutedEventArgs e)
        {
            NavView.IsPaneOpen = !NavView.IsPaneOpen;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NavMenuItem item = (NavMenuItem)((ListView)sender).SelectedItem;

            if (item != null 
                && item.DestPage != null
                && item.DestPage != AppFrame.CurrentSourcePageType)
            {
                AppFrame.Navigate(item.DestPage);
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadRegions();
        }

        private void RegionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            App.Region = (I18Item)((ComboBox)sender).SelectedItem;
            ((IRefrashablePage)AppFrame.Content).RefreshAll();
        }

        private void NightModeButton_Toggled(object sender, RoutedEventArgs e)
        {
            var toggleSwitch = (ToggleSwitch)sender;

            if (toggleSwitch.IsOn == true)
                RequestedTheme = ElementTheme.Dark;
            else
                RequestedTheme = ElementTheme.Light;
        }

        private void AccountPanel_LoginButtonClick(object sender, RoutedEventArgs e)
        {
            AccountsSettingsPane.Show();
        }
    }

    public class MainMenuItems : ObservableCollection<NavMenuItem>
    {
        public MainMenuItems()
        {
            Add(new NavMenuItem() { Label = "Home", Symbol = "\ue88a", Role = Role.Main, DestPage = typeof(HomePage) });
            Add(new NavMenuItem() { Label = "Trends", Symbol = "\ue80e", Role = Role.Main });
            Add(new NavMenuItem() { Label = "Subscriptions", Symbol = "\ue064", Role = Role.Main });

            Add(new NavMenuItem() { Label = "LIBRARY", Symbol = "", Role = Role.Alt });

            Add(new NavMenuItem() { Label = "History", Symbol = "\ue889", Role = Role.Main });
            Add(new NavMenuItem() { Label = "Watch Later", Symbol = "\ue924", Role = Role.Main });
            Add(new NavMenuItem() { Label = "Liked Videos", Symbol = "\ue8dc", Role = Role.Main });
        }
    }

    public class NavMenuItemTemplateSelector : DataTemplateSelector
    {
        public NavMenuItemTemplateSelector()
        {

        }

        public DataTemplate MainItemTemplate { get; set; }

        public DataTemplate AltItemTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            DataTemplate ret = MainItemTemplate;
            NavMenuItem _item = item as NavMenuItem;

            if (_item.Role == Role.Alt)
                ret = AltItemTemplate;

            return ret;
        }
    }

}
