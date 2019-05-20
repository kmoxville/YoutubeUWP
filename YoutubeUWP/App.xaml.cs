using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Authentication.Web.Core;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using YoutubeApi;

namespace YoutubeUWP
{
    /// <summary>
    /// Обеспечивает зависящее от конкретного приложения поведение, дополняющее класс Application по умолчанию.
    /// </summary>
    sealed partial class App : Application
    {
        const string AccountsContainer = "accountssettingscontainer";
        const string ProviderIdSubContainer = "providers";
        const string AuthoritySubContainer = "authorities";

        const string AppSpecificProviderId = "youtube";
        const string AppSpecificProviderName = "Youtube";

        public static YoutubeClient YoutubeClient = new YoutubeClient("AIzaSyDlWs-o1VzVEH-HshjzLPl0NTuxueKB4y8");

        public static I18Item Region { get; internal set; } = new I18Item();
        public static ApplicationDataContainer Settings { get; }

        /// <summary>
        /// Инициализирует одноэлементный объект приложения.  Это первая выполняемая строка разрабатываемого
        /// кода; поэтому она является логическим эквивалентом main() или WinMain().
        /// </summary>
        /// 
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;         
        }

        static App()
        {
            Settings = ApplicationData.Current.LocalSettings;
        }

        public static void LogoffAndRemoveAccount(WebAccount account)
        {
            var accountsContainer = Settings.Containers[AccountsContainer];
            //FixME perform any actions needed to log off the app specific account
            accountsContainer.DeleteContainer(account.UserName);
            //account.GetPictureAsync(WebAccountPictureSize.Size208x208).GetResults();
            //account.SignOutAsync().GetResults();
        }

        public static void StoreNewAccountDataLocally(WebAccount account, YoutubeClient.ProviderUserDetails details)
        {
            if (!Settings.Containers.ContainsKey(AccountsContainer))
            {
                Settings.CreateContainer(AccountsContainer, ApplicationDataCreateDisposition.Always);
            }
            ApplicationDataContainer accountsContainer = Settings.Containers[AccountsContainer];         

            var accountContainer = accountsContainer.CreateContainer(details.Email, ApplicationDataCreateDisposition.Always);
            accountsContainer.Values["active"] = details.Email;

            accountContainer.Values["name"] = details.Name;
            accountContainer.Values["lastName"] = details.LastName;
            accountContainer.Values["email"] = details.Email;
            accountContainer.Values["access_token"] = details.AccessToken;
            accountContainer.Values["refresh_token"] = details.RefreshToken;
            accountContainer.Values["accountID"] = details.Email;
            accountContainer.Values["accountProviderID"] = account.WebAccountProvider.Id;
        }

        public static List<WebAccount> GetAllAccounts()
        {
            List<WebAccount> accounts = new List<WebAccount>();

            try
            {
                ApplicationDataContainer AccountListContainer = ApplicationData.Current.LocalSettings.Containers[AccountsContainer];

                foreach (var key in AccountListContainer.Containers.Keys)
                {
                    var accountContainer = (ApplicationDataContainer)AccountListContainer.Containers[key];


                    String accountID = (string)accountContainer.Values["accountID"];
                    String providerID = (string)AccountListContainer.Values["accountProviderID"];

                    WebAccountProvider provider = new WebAccountProvider(AppSpecificProviderId, AppSpecificProviderName, new Uri("ms-appx:///Assets/Images/logo.png"));

                    accounts.Add(new WebAccount(provider, accountID, WebAccountState.None));

                }
            }
            catch (Exception)
            {

            }

            return accounts;
        }     

        /// <summary>
        /// Вызывается при обычном запуске приложения пользователем.  Будут использоваться другие точки входа,
        /// например, если приложение запускается для открытия конкретного файла.
        /// </summary>
        /// <param name="e">Сведения о запросе и обработке запуска.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Не повторяйте инициализацию приложения, если в окне уже имеется содержимое,
            // только обеспечьте активность окна
            if (rootFrame == null)
            {
                // Создание фрейма, который станет контекстом навигации, и переход к первой странице
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Загрузить состояние из ранее приостановленного приложения
                }

                // Размещение фрейма в текущем окне
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // Если стек навигации не восстанавливается для перехода к первой странице,
                    // настройка новой страницы путем передачи необходимой информации в качестве параметра
                    // параметр
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Обеспечение активности текущего окна
                Window.Current.Activate();

                //Extend acryllic
                extendAcrylicIntoTitleBar();
            }
        }

        /// <summary>
        /// Вызывается в случае сбоя навигации на определенную страницу
        /// </summary>
        /// <param name="sender">Фрейм, для которого произошел сбой навигации</param>
        /// <param name="e">Сведения о сбое навигации</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Вызывается при приостановке выполнения приложения.  Состояние приложения сохраняется
        /// без учета информации о том, будет ли оно завершено или возобновлено с неизменным
        /// содержимым памяти.
        /// </summary>
        /// <param name="sender">Источник запроса приостановки.</param>
        /// <param name="e">Сведения о запросе приостановки.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Сохранить состояние приложения и остановить все фоновые операции
            deferral.Complete();
        }

        //
        private void extendAcrylicIntoTitleBar()
        {
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        }
    }
}
