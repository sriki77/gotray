using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoTrayFeed;
using GoTrayUtils;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace GoTray
{
    public sealed class GoTrayDashboardErrorContext
    {
        public bool Error { get; private set; }

        public string ErrorMessage { get; private set; }

        public string ErrorDetails { get; private set; }

        internal GoTrayDashboardErrorContext ShowError(string msg, Exception cause)
        {
            Error = true;
            ErrorMessage = msg;
            ErrorDetails = cause.Message;
            return this;
        }

        internal GoTrayDashboardErrorContext RemoveError()
        {
            Error = false;
            ErrorMessage = "";
            ErrorDetails = "";
            return this;
        }
    }
    /// <summary>
    /// A page that displays a collection of item previews.  In the Split Application this page
    /// is used to display and select one of the available groups.
    /// </summary>
    public sealed partial class GoTrayDashboard : GoTray.Common.LayoutAwarePage
    {
        private readonly GoTrayConfiguration _config;

        public GoTrayDashboard()
        {
            this.InitializeComponent();
            _config = GoTrayConfiguration.TrayConfiguration;
            _config.ConfigChanged += ConfigurationChanged;
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            LoadPipelines(false);
        }

        private async Task LoadPipelines(bool reload)
        {
            try
            {
                InitProgress(reload);
                GoTrayFeedSource feedDataSource = new GoTrayFeedSource(_config.GoServerUrl, _config.GoServerUserName,
                                                                       _config.GoServerPassword);
                IEnumerable<GoProject> projects = await feedDataSource.projects;
                Projects = projects;
                ResetProgress();
            }
            catch (Exception ex)
            {
                Projects = Enumerable.Empty<GoProject>();
                ResetProgress();
                ShowException(ex);
            }
        }

        private void ResetProgress()
        {
            ShowProgress = false;
            ShowReloadProgress = false;
        }

        private void InitProgress(bool reload)
        {
            if (reload)
            {
                ShowReloadProgress = true;
            }
            else
            {
                ShowProgress = true;
            }
        }

        private void ConfigurationChanged(object sender, EventArgs e)
        {
            Projects = Enumerable.Empty<GoProject>();
            ResetException();
            LoadPipelines(true);
        }

        private void ShowException(Exception ex)
        {
            ErrorGrid.DataContext = new GoTrayDashboardErrorContext()
            .ShowError("Failed To Retieve Pipelines From Go Server", ex);
        }

        private void ResetException()
        {
            ErrorGrid.DataContext = new GoTrayDashboardErrorContext().RemoveError();
        }

        private bool ShowProgress
        {
            set { 
                DefaultViewModel["ShowProgress"]=value;
                DefaultViewModel["ReloadProgress"] = false;
                DefaultViewModel["LoadProgress"] = value;
            }
        }

        private bool ShowReloadProgress
        {
            set
            {
                DefaultViewModel["ShowProgress"] = value;
                DefaultViewModel["ReloadProgress"] = value;
                DefaultViewModel["LoadProgress"] = false;
            }
        }

        private IEnumerable<GoProject> Projects
        {
            set { DefaultViewModel["Items"] = value;
            }
        }
    }
}
