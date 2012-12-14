using System;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace GoTrayUtils
{
    public static class ToastUtil
    {
        public static ToastNotification ShowExceptionToast(String msg,Exception ex)
        {
            ToastTemplateType toastType = ToastTemplateType.ToastImageAndText02;
            XmlDocument toastXML = ToastNotificationManager.GetTemplateContent(toastType);
            XmlNodeList toastText = toastXML.GetElementsByTagName("text");
            XmlNodeList toastImages = toastXML.GetElementsByTagName("image");
            toastText[0].InnerText = msg;
            toastText[1].InnerText = ex.Message;
            ((XmlElement)toastImages[0]).SetAttribute("src", "img/oops.png");
            ToastNotification toast = new ToastNotification(toastXML);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
            return toast;
        }
    }
}
