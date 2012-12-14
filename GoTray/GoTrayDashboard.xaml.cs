using System;
using System.Collections.Generic;
using GoTrayFeed;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234233

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
    }
    /// <summary>
    /// A page that displays a collection of item previews.  In the Split Application this page
    /// is used to display and select one of the available groups.
    /// </summary>
    public sealed partial class GoTrayDashboard : GoTray.Common.LayoutAwarePage
    {
        public GoTrayDashboard()
        {
            this.InitializeComponent();
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
        protected override async void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            try
            {
                DefaultViewModel["ShowProgress"] = true;
                GoTrayFeedSource feedDataSource = (GoTrayFeedSource)Application.Current.Resources["GoTrayFeedSource"];
                IEnumerable<GoProject> projects = await feedDataSource.projects;
                DefaultViewModel["Items"] = projects;
                DefaultViewModel["ShowProgress"] = false;
            }
            catch (Exception ex)
            {
                DefaultViewModel["ShowProgress"] = false;
                ShowException(ex);
            }
        }

        private void ShowException(Exception ex)
        {
            ErrorGrid.DataContext = new GoTrayDashboardErrorContext()
            .ShowError("Failed To Retieve Pipelines From Go Server", ex);
        }
    }
}
