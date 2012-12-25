using System;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace GoTrayUtils
{
    public sealed class GoTrayConfiguration
    {
        public event EventHandler<EventArgs> ConfigChanged; 
        public static readonly GoTrayConfiguration TrayConfiguration = new GoTrayConfiguration();
        private readonly ApplicationDataContainer _roamingSettings;

        private GoTrayConfiguration()
        {
            _roamingSettings = ApplicationData.Current.RoamingSettings;
        }

        public void ValuesChanged()
        {
            ConfigChanged.Invoke(this,new EventArgs());
        }

        public string GoServerPassword
        {
            get { return (string) _roamingSettings.Values[GoTrayConstants.GoServerPasswordProperty] ?? ""; }
            set { _roamingSettings.Values[GoTrayConstants.GoServerPasswordProperty] = value; }
        }

        public string GoServerUserName
        {
            get { return (string)_roamingSettings.Values[GoTrayConstants.GoServerUsernameProperty] ?? ""; }
            set { _roamingSettings.Values[GoTrayConstants.GoServerUsernameProperty] = value;}
        }

        public string GoServerUrl
        {
            get { return (string)_roamingSettings.Values[GoTrayConstants.GoServerUrlProperty] ?? "";  }
            set { _roamingSettings.Values[GoTrayConstants.GoServerUrlProperty] = value;}
        }

    }
}
