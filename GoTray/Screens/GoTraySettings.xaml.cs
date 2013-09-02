using GoTray.Common;
using GoTrayUtils;
using Windows.UI.ApplicationSettings;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace GoTray
{
    public sealed partial class GoTraySettings : LayoutAwarePage
    {
        private readonly GoTrayConfiguration _config;
        private string _password = "";
        private string _url = "";
        private string _username = "";

        public GoTraySettings()
        {
            InitializeComponent();
            Loaded += SettingsLoaded;
            Unloaded += SettingsClosed;
            _config = GoTrayConfiguration.TrayConfiguration;
        }

        private string GoServerPassword
        {
            get { return (string) DefaultViewModel["GoServerPassword"]; }
            set { DefaultViewModel["GoServerPassword"] = value; }
        }

        private string GoServerUserName
        {
            get { return (string) DefaultViewModel["GoServerUserName"]; }
            set { DefaultViewModel["GoServerUserName"] = value; }
        }

        private string GoServerUrl
        {
            get { return (string) DefaultViewModel["GoServerUrl"]; }
            set { DefaultViewModel["GoServerUrl"] = value; }
        }

        private void SettingsLoaded(object sender, RoutedEventArgs e)
        {
            LoadConfiguration();
            GoServerUrl = _url;
            GoServerUserName = _username;
            GoServerPassword = _password;
        }

        private void LoadConfiguration()
        {
            _url = _config.GoServerUrl;
            _username = _config.GoServerUserName;
            _password = _config.GoServerPassword;
        }

        private void SettingsClosed(object sender, object e)
        {
            if (ConfigurationChanged())
            {
                _config.GoServerUrl = GoServerUrl;
                _config.GoServerUserName = GoServerUserName;
                _config.GoServerPassword = GoServerPassword;
                _config.ValuesChanged();
            }
        }

        private bool ConfigurationChanged()
        {
            return !_url.Equals(GoServerUrl) ||
                   !_username.Equals(GoServerUserName) ||
                   !_password.Equals(GoServerPassword);
        }


        private void SettingsBackClicked(object sender, RoutedEventArgs e)
        {
            var parent = Parent as Popup;
            if (parent != null)
            {
                parent.IsOpen = false;
            }

            // If the app is not snapped, then the back button shows the Settings pane again.
            if (ApplicationView.Value
                != ApplicationViewState.Snapped)
            {
                SettingsPane.Show();
            }
        }
    }
}