using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoTray.Common;
using GoTrayFeed;
using GoTrayUtils;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace GoTray
{
    public sealed partial class GoTrayDashboard : LayoutAwarePage
    {
        private readonly GoTrayConfiguration _config;
        private readonly DispatcherTimer _refreshTimer = new DispatcherTimer();

        public GoTrayDashboard()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            _config = GoTrayConfiguration.TrayConfiguration;
            _config.ConfigChanged += ConfigurationChanged;
        }

        private bool ShowProgress
        {
            set
            {
                DefaultViewModel["ShowProgress"] = value;
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

        private IEnumerable<Pipeline> Projects
        {
            get { return (IEnumerable<Pipeline>) DefaultViewModel["Items"]; }
            set { DefaultViewModel["Items"] = value; }
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
#pragma warning disable 4014
            LoadPipelines(false);
#pragma warning restore 4014
        }

        private async Task LoadPipelines(bool reload)
        {
            try
            {
                InitProgress(reload);
                var feedDataSource = new GoTrayFeedSource(_config.GoServerUrl, _config.GoServerUserName,
                                                          _config.GoServerPassword);
                IEnumerable<Pipeline> pipelines = await feedDataSource.pipelines;
                Projects = FilterUnpinnedPipelines(pipelines);
                ResetProgress();
            }
            catch (Exception ex)
            {
                Projects = Enumerable.Empty<Pipeline>();
                ResetProgress();
                ShowException(ex);
            }
            DefaultViewModel["LastUpdatedTime"] = "Last Updated: " + DateTime.Now.ToString("HH:mm:ss tt");
            StartAutoRefresh();
        }

        private void StartAutoRefresh()
        {
            _refreshTimer.Interval = new TimeSpan(0, 0, 30);
            _refreshTimer.Tick += (sender, o) => SilentRefresh();
            _refreshTimer.Start();
        }

        private async void SilentRefresh()
        {
            try
            {
                var feedDataSource = new GoTrayFeedSource(_config.GoServerUrl, _config.GoServerUserName,
                                                          _config.GoServerPassword);
                IEnumerable<Pipeline> projects = await feedDataSource.pipelines;
                Projects = FilterUnpinnedPipelines(projects);
            }
            catch (Exception ex)
            {
                Projects = Enumerable.Empty<Pipeline>();
                ShowException(ex);
            }
            DefaultViewModel["LastUpdatedTime"] = "Last Updated: " + DateTime.Now.ToString("HH:mm:ss tt");
        }

        private IEnumerable<Pipeline> FilterUnpinnedPipelines(IEnumerable<Pipeline> projects)
        {
            return projects.Where(project => !_config.IsUnpinnedPipeline(project.PipelineName));
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
            Projects = Enumerable.Empty<Pipeline>();
            ResetException();
#pragma warning disable 4014
            LoadPipelines(true);
#pragma warning restore 4014
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

        private void PiplineClicked(object sender, ItemClickEventArgs e)
        {
            //Frame.Navigate(typeof (PipelineDetails),e.ClickedItem);
            //Frame.Navigating +=delegate(object o, NavigatingCancelEventArgs args) {  };   
        }

        
        private void UnpinPipeline(object sender, RoutedEventArgs e)
        {
            DashboardAppBar.IsOpen = false;
            try
            {
                IList<Pipeline> projectsOnUI = new List<Pipeline>(Projects);
                IList<object> selectedItems = PipelinesGridView.SelectedItems;
                foreach (object item in selectedItems)
                {
                    var project = item as Pipeline;
                    _config.UnpinPipeline(project.PipelineName);
                    projectsOnUI.Remove(project);
                }
                Projects = projectsOnUI;
            }
            catch (Exception exception)
            {
                ToastUtil.ShowExceptionToast("Failed to unpin pipelines", exception);
            }
        }


        private void ResetPipelines(object sender, RoutedEventArgs e)
        {
            DashboardAppBar.IsOpen = false;
            _config.RemoveUnpinnedPipelines();
            ReloadPipelines(sender, e);
        }

        private void ReloadPipelines(object sender, RoutedEventArgs e)
        {
            DashboardAppBar.IsOpen = false;
            SilentRefresh();
        }

        private void GoToGitHub(object sender, RoutedEventArgs e)
        {
            DashboardAppBar.IsOpen = false;
            TrayUtil.GoToGitHub();
        }
    }
}