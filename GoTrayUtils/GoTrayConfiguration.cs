using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace GoTrayUtils
{
    public sealed class GoTrayConfiguration
    {
        public static readonly GoTrayConfiguration TrayConfiguration = new GoTrayConfiguration();
        private readonly ApplicationDataContainer _roamingSettings;
        private readonly ApplicationDataContainer _localSettings;

        private GoTrayConfiguration()
        {
            _roamingSettings = ApplicationData.Current.RoamingSettings;
            _localSettings = ApplicationData.Current.LocalSettings;
        }

        public string GoServerPassword
        {
            get { return (string) _roamingSettings.Values[GoTrayConstants.GoServerPasswordProperty] ?? ""; }
            set { _roamingSettings.Values[GoTrayConstants.GoServerPasswordProperty] = value; }
        }

        public string GoServerUserName
        {
            get { return (string) _roamingSettings.Values[GoTrayConstants.GoServerUsernameProperty] ?? ""; }
            set { _roamingSettings.Values[GoTrayConstants.GoServerUsernameProperty] = value; }
        }

        public string GoServerUrl
        {
            get { return (string) _roamingSettings.Values[GoTrayConstants.GoServerUrlProperty] ?? ""; }
            set { _roamingSettings.Values[GoTrayConstants.GoServerUrlProperty] = value; }
        }

        private IList<string> UnpinnedPipelines
        {
            get
            {
                string unpinned = (string) _localSettings.Values[GoTrayConstants.UnpinnedPipelines] ?? "";
                return new List<string>(unpinned.Split(','));
            }
            set { _localSettings.Values[GoTrayConstants.UnpinnedPipelines] = string.Join(",", value ?? new List<string>()); }
        }

        public event EventHandler<EventArgs> ConfigChanged;

        public void ValuesChanged()
        {
            ConfigChanged.Invoke(this, new EventArgs());
        }

        public bool IsUnpinnedPipeline(string pipeline)
        {
            return UnpinnedPipelines.Contains(pipeline);
        }

        public void UnpinPipeline(string pipeline)
        {
            IList<string> unpinnedPipelines = UnpinnedPipelines;
            unpinnedPipelines.Add(pipeline);
            UnpinnedPipelines = unpinnedPipelines;
        }

        public void RemoveUnpinnedPipelines()
        {
            UnpinnedPipelines = new List<string>();
        }

    }
}