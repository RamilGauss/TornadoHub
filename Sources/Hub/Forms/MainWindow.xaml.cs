using System;
using System.Diagnostics;
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

        private HubConfig _hubConfig;

        private RemoteApplications _remoteEditors;
        private RemoteApplications _remoteHubs;
        private LocalApplications _localEditors;
        private LocalApplication _localHub;

        public string GetVersion => _localHub == null ? $"not found {HUB_LOCAL_VERSION_PATH}" : _localHub.ApplicationInfo.Version.ToString();

        public string GetTitle => $"{APP_NAME} {GetVersion}";

        public MainWindow()
        {
            LoadLocalHubVersionInfo();
            LoadHubConfig();
            LoadLocalEditorsInfo();

            DownloadRemoteHubsJson();
            DownloadRemoteEditorsJson();

            InitializeComponent();

            Title = GetTitle;

            FillInstallList();
            FillLaunchList();

            var info = String.Join(",\n", _localHub.ApplicationInfo.Version.Info.ToArray());
            AboutInfoLabel.Content = info;
        }

        #region Loaders

        private void LoadLocalEditorsInfo()
        {
            var isExist = System.IO.File.Exists(LOCAL_EDITORS_PATH);
            if ( !isExist ) {
                MessageBox.Show($"Not found {LOCAL_EDITORS_PATH}.", GetTitle);
                return;
            }

            var f = System.IO.File.ReadAllText(LOCAL_EDITORS_PATH);
            _localEditors = Newtonsoft.Json.JsonConvert.DeserializeObject<LocalApplications>(f);
        }

        private void DownloadRemoteEditorsJson()
        {
            var client = new WebClient();
            var remoteEditorsJson = client.DownloadString(_hubConfig.RemoteEditorsUrl);

            _remoteEditors = Newtonsoft.Json.JsonConvert.DeserializeObject<RemoteApplications>(remoteEditorsJson);
        }

        private void DownloadRemoteHubsJson()
        {
            var client = new WebClient();
            var remoteHubsJson = client.DownloadString(_hubConfig.RemoteHubsUrl);

            _remoteHubs = Newtonsoft.Json.JsonConvert.DeserializeObject<RemoteApplications>(remoteHubsJson);
        }

        private void LoadLocalHubVersionInfo()
        {
            var isExist = System.IO.File.Exists(HUB_LOCAL_VERSION_PATH);
            if ( !isExist ) {
                MessageBox.Show($"Not found {HUB_LOCAL_VERSION_PATH}.", $"{APP_NAME}");

                Application.Current.Shutdown();
                return;
            }
            var f = System.IO.File.ReadAllText(HUB_LOCAL_VERSION_PATH);
            _localHub = Newtonsoft.Json.JsonConvert.DeserializeObject<LocalApplication>(f);
        }

        private void LoadHubConfig()
        {
            var isExist = System.IO.File.Exists(HUB_CONFIG_PATH);
            if ( !isExist ) {
                MessageBox.Show("", GetTitle);
                return;
            }

            var f = System.IO.File.ReadAllText(HUB_CONFIG_PATH);
            _hubConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<HubConfig>(f);
        }

        private void FillInstallList()
        {
            foreach ( var remoteEditor in _remoteEditors.ApplicationInfoList ) {

                var local = _localEditors.ApplicationInfoList.Find(x => 
                    x.ApplicationInfo.Version.Build == remoteEditor.Version.Build);
                if ( local != null ) {
                    continue;
                }

                var item = new InstallItem()
                {
                    Info = $"Tornado Editor {remoteEditor}",
                    EditorProperties = remoteEditor
                };

                InstallList.Items.Add(item);
            }
        }

        private void FillLaunchList()
        {
            foreach ( var localEditor in _localEditors.ApplicationInfoList ) {

                var item = new LaunchItem()
                {
                    Info = $"Tornado Editor {localEditor.ApplicationInfo.Version}",
                    LocalApplication = localEditor
                };

                InstallList.Items.Add(item);
            }
        }
        #endregion

        #region Handlers
        private void ClearEventsHandler(object sender, RoutedEventArgs e)
        {
            //this.EventsStatusBar.
        }

        private void AddButtonHandler(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var installItem = button.DataContext as InstallItem;

            var installForm = new InstallForm();

            var localEditor = new LocalApplication();
            localEditor.ApplicationInfo.Version = installItem.EditorProperties.Version;

            installForm.Setup( () => Install(localEditor), localEditor);

            installForm.ShowDialog();
        }

        private void Install(LocalApplication localEditor)
        {
            var client = new WebClient();
            var arhciveFileName = "temp.zip";

            client.DownloadFile(localEditor.ApplicationInfo.Version.DownloadUrl, arhciveFileName);

            System.IO.Compression.ZipFile.ExtractToDirectory(arhciveFileName, localEditor.InstallationFolder);

            System.IO.File.Delete(arhciveFileName);

            _localEditors.ApplicationInfoList.Add(localEditor);

            var localEditorsJson = Newtonsoft.Json.JsonConvert.SerializeObject(_localEditors);
            System.IO.File.WriteAllText(localEditorsJson, LOCAL_EDITORS_PATH);

            RefreshLaunchList();
            RefreshInstallList();
        }

        private void UninstallButtonHandler(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var launchItem = button.DataContext as LaunchItem;

            System.IO.Directory.Delete(launchItem.LocalApplication.InstallationFolder);

            _localEditors.ApplicationInfoList.Remove(launchItem.LocalApplication);

            var localEditorsJson = Newtonsoft.Json.JsonConvert.SerializeObject(_localEditors);
            System.IO.File.WriteAllText(localEditorsJson, LOCAL_EDITORS_PATH);

            RefreshLaunchList();
            RefreshInstallList();
        }

        private void LaunchButtonHandler(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;

            var button = sender as Button;
            var launchItem = button.DataContext as LaunchItem;

            var launchFullPath = 
                launchItem.LocalApplication.InstallationFolder + launchItem.LocalApplication.ApplicationInfo.LaunchPath;

            Process.Start(launchFullPath);
        }

        private void InfoButtonHandler(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var installItem = button.DataContext as InstallItem;

            var info = $"Tornado Editor {installItem.EditorProperties}\n";
            info += String.Join(",\n", installItem.EditorProperties.Version.Info.ToArray());

            MessageBox.Show($"Info: {info}", GetTitle);
        }
        #endregion

        private void RefreshLaunchList()
        {
            LaunchList.Items.Clear();

            FillLaunchList();
        }

        private void RefreshInstallList()
        {
            InstallList.Items.Clear();

            FillInstallList();
        }

    }
}
