using System;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace GoTrayUtils
{
    public static class ToastUtil
    {
        public static ToastNotification ShowExceptionToast(String msg, Exception ex)
        {
            var toastType = ToastTemplateType.ToastText02;
            XmlDocument toastXML = ToastNotificationManager.GetTemplateContent(toastType);
            XmlNodeList toastText = toastXML.GetElementsByTagName("text");
            toastText[0].InnerText = msg;
            toastText[1].InnerText = ex.Message;
            var toast = new ToastNotification(toastXML);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
            return toast;
        }
    }
}