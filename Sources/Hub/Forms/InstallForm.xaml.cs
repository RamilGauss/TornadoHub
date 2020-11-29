using System;
using System.Windows;

namespace Hub
{
    public partial class InstallForm : Window
    {
        private LocalApplication _localEditor;
        private Action _onInstall;

        public InstallForm()
        {
            InitializeComponent();
        }

        public void Setup(Action onInstall, LocalApplication localEditor)
        {
            _onInstall = onInstall;
            _localEditor = localEditor;

            this.PathTextBox.Text = _localEditor.InstallationFolder;

            Title = $"Installing Tornado Editor {_localEditor.ApplicationInfo.Version}";
        }

        private void SelectPathButtonHandler(object sender, RoutedEventArgs e)
        {
            this.Topmost = false;
            try {
                System.IO.Directory.CreateDirectory(_localEditor.InstallationFolder);
            }
            catch(System.Exception ex) {

            }

            var selectFolder = new WPFFolderBrowser.WPFFolderBrowserDialog(Title);
            selectFolder.InitialDirectory = _localEditor.InstallationFolder;

            var selectResult = selectFolder.ShowDialog();

            this.Topmost = true;
            this.Topmost = false;

            if ( selectResult.Value ) {
                _localEditor.InstallationFolder = selectFolder.FileName;
                this.PathTextBox.Text = _localEditor.InstallationFolder;
            }
        }

        private void DoInstallButtonHandler(object sender, RoutedEventArgs e)
        {


            // Additional utilities installations

            var version = _localEditor.ApplicationInfo.Version;
            var path = $"{_localEditor.InstallationFolder}/{version.MajorVersion}.{version.MinorVersion}.{version.Build}";


            _onInstall?.Invoke();

            Close();
        }

        private void DoCancelButtonHandler(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
