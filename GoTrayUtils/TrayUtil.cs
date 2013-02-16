using System;
using Windows.Data.Xml.Dom;
using Windows.System;
using Windows.UI.Notifications;
using Windows.UI.Xaml;

namespace GoTrayUtils
{
    public static class TrayUtil
    {
        public static void GoToGitHub()
        {
            Launcher.LaunchUriAsync(new Uri("https://github.com/sriki77/gotray"));

        }
    }
}
