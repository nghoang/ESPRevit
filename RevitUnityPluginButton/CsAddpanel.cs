using System;
using System.Collections.Generic;
using System.Text;
using Autodesk.Revit.UI;
using System.Windows.Media.Imaging;

namespace RevitUnityPluginButton
{
    class CsAddpanel : Autodesk.Revit.UI.IExternalApplication
    {
        public Autodesk.Revit.UI.Result OnStartup(UIControlledApplication application)
        {
            RibbonPanel ribbonPanel = application.CreateRibbonPanel("Unity Toos");
            PushButton pushButton = ribbonPanel.AddItem(new PushButtonData("ServiceClient",
            "ServiceClient", @"C:\Users\user\Desktop\TestProject11\RevitUnity\RevitUnity\RevitUnityInvoker\bin\Debug\RevitUnityInvoker.dll", "RevitUnityInvoker.ServiceClient")) as PushButton;
            // Set the large image shown on button
            //Uri uriImage = new Uri(@"D:\universitylogo32.png");
            //BitmapImage largeImage = new BitmapImage(uriImage);
            //pushButton.LargeImage = largeImage;
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
