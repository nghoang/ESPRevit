using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Revit.UI;
using System.Windows.Media.Imaging;  

namespace AddPanel
{
    public class AddPanel : Autodesk.Revit.UI.IExternalApplication
    {
        public Autodesk.Revit.UI.Result OnStartup(UIControlledApplication application)
        {
            // add new ribbon panel
            RibbonPanel ribbonPanel = application.CreateRibbonPanel("Unity Tools");

            //Create a push button in the ribbon panel "NewRibbonPanel"
            //the add-in application "HelloWorld" will be triggered when button is pushed

            PushButton pushButton = ribbonPanel.AddItem(new PushButtonData("Connect to Unity",
                "Connect to Unity", @"C:\revitunity\RevitUnityInvoker.dll", "RevitUnityInvoker.ServiceClient")) as PushButton;

            // Set the large image shown on button
            Uri uriImage = new Uri(@"C:\revitunity\logo32x32.png");
            BitmapImage largeImage = new BitmapImage(uriImage);
            pushButton.LargeImage = largeImage;

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
