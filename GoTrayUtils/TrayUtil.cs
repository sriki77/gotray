using System;
using Windows.System;

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