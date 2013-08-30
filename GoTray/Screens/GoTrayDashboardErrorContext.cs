using System;

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
}