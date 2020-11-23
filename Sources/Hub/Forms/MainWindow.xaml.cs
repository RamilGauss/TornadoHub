using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace Hub
{
    public partial class MainWindow : Window
    {
        public const string APP_NAME = "Tornado Hub";

        private const string HUB_CONFIG_PATH = "HubConfig.json";
        private const string HUB_LOCAL_VERSION_PATH = "LocalHub.json";
        private const string LOCAL_EDITORS_PATH = "LocalEditors.json";

        private LocalHub _localHub;
        private HubConfig _hubConfig;

        private RemoteEditors _remoteEditors;
        private LocalEditors _localEditors;

        public string GetVersion() => _localHub == null ? $"not found {HUB_LOCAL_VERSION_PATH}" : _localHub.ToString();

        public MainWindow()
        {
            LoadHubVersion();
            LoadHubConfig();
            LoadLocalEditors();

            DownloadRemoteHubsJson();
            DownloadRemoteEditorsJson();

            InitializeComponent();

            InitInstallList();
        }

        #region Loaders

        private void LoadLocalEditors()
        {
            var isExist = System.IO.File.Exists(LOCAL_EDITORS_PATH);
            if ( !isExist ) {
                MessageBox.Show("", $"{APP_NAME} {GetVersion()}");
                return;
            }

            var f = System.IO.File.ReadAllText(LOCAL_EDITORS_PATH);
            _localEditors = Newtonsoft.Json.JsonConvert.DeserializeObject<LocalEditors>(f);
        }

        private void DownloadRemoteEditorsJson()
        {
            var client = new WebClient();
            var remoteEditorsJson = client.DownloadString(_hubConfig.RemoteEditorsUrl);

            _remoteEditors = Newtonsoft.Json.JsonConvert.DeserializeObject<RemoteEditors>(remoteEditorsJson);
        }

        private void DownloadRemoteHubsJson()
        {
            var client = new WebClient();
            var remoteHubsJson = client.DownloadString(_hubConfig.RemoteHubsUrl);

            _localEditors = Newtonsoft.Json.JsonConvert.DeserializeObject<LocalEditors>(remoteHubsJson);
        }

        private void LoadHubVersion()
        {
            var isExist = System.IO.File.Exists(HUB_LOCAL_VERSION_PATH);
            if ( !isExist ) {
                MessageBox.Show($"Not found {HUB_LOCAL_VERSION_PATH}.", $"{APP_NAME}");

                Application.Current.Shutdown();
                return;
            }
            var f = System.IO.File.ReadAllText(HUB_LOCAL_VERSION_PATH);
            _localHub = Newtonsoft.Json.JsonConvert.DeserializeObject<LocalHub>(f);
        }

        private void LoadHubConfig()
        {
            var isExist = System.IO.File.Exists(HUB_CONFIG_PATH);
            if ( !isExist ) {
                MessageBox.Show("", $"{APP_NAME} {GetVersion()}");
                return;
            }

            var f = System.IO.File.ReadAllText(HUB_CONFIG_PATH);
            _hubConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<HubConfig>(f);
        }

        private void InitInstallList()
        {
            foreach ( var remoteEditor in _remoteEditors.VersionList ) {

                var item = new InstallItem()
                {
                    Info = $"Tornado Editor {remoteEditor.ToString()}",
                    EditorProperties = remoteEditor
                };

                this.InstallList.Items.Add(item);
            }
        }
        #endregion

        #region Handlers
        private void ClearEventsHandler(object sender, RoutedEventArgs e)
        {

        }

        private void AddButtonHandler(object sender, RoutedEventArgs e)
        {

        }

        private void UninstallButtonHandler(object sender, RoutedEventArgs e)
        {

        }

        private void InfoButtonHandler(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            var installItem = button.DataContext as InstallItem;

            var info = String.Join(", ", installItem.EditorProperties.Info.ToArray());

            MessageBox.Show($"Info: {info}", "TornadoHub");
        }
        #endregion
    }
}
