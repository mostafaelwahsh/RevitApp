using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace RevitApplication
{
    public class ExternalApp : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            application.CreateRibbonTab("Elwahsh");
            //create panel
            RibbonPanel panel = application.CreateRibbonPanel("Elwahsh", "Create");

            //edit panel
            RibbonPanel panel2 = application.CreateRibbonPanel("Elwahsh", "Edit");

            // Path to the DLL
            string path = Assembly.GetExecutingAssembly().Location;

            //Tag Button

            PushButtonData buttonData = new PushButtonData("CreateTags", "Create Tags", path, "RevitApplication.Commands.Window.TagWindowCommand");
            PushButton TagButton = panel.AddItem(buttonData) as PushButton;
            TagButton.ToolTip = "Automatically creates Tags and places them in the view.";

            // Sheets button
            PushButtonData buttonData1 = new PushButtonData("CreateSheets", "Create Sheets", path, "RevitApplication.Creating_Sheets.Sheets");
            PushButton SheetButton = panel.AddItem(buttonData1) as PushButton;
            SheetButton.ToolTip = "Automatically creates sheets and places plan views on them.";


            // Rotation button
            PushButtonData buttonData2 = new PushButtonData("Rotation", "Rotate Elements", path, "RevitApplication.Commands.Window.RotationWindowCommand");
            PushButton RotationButton = panel2.AddItem(buttonData2) as PushButton;
            RotationButton.ToolTip = "Rotate the selected elements to a specific angle.";

            // Debug: Show all embedded resources
            Assembly assembly = Assembly.GetExecutingAssembly();


            #region Image_Of_SheetsButton 
            // Try loading the correct embedded image
            string resourceName1 = "RevitApplication.Resources.icon.png";
            Stream iconStream1 = assembly.GetManifestResourceStream(resourceName1);

            if (iconStream1 != null)
            {
                BitmapImage bitmap1 = new BitmapImage();
                bitmap1.BeginInit();
                bitmap1.StreamSource = iconStream1;
                bitmap1.CacheOption = BitmapCacheOption.OnLoad;
                bitmap1.EndInit();

                SheetButton.LargeImage = bitmap1;
            }
            else
            {
                TaskDialog.Show("Image Error", $"Could not load image: {resourceName1}");
            }
            #endregion


            #region Image_Of_TagButton 
            // Try loading the correct embedded image
            string resourceName = "RevitApplication.Resources.Tag.png";
            Stream iconStream = assembly.GetManifestResourceStream(resourceName);

            if (iconStream != null)
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = iconStream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                TagButton.LargeImage = bitmap;
            }
            else
            {
                TaskDialog.Show("Image Error", $"Could not load image: {resourceName}");
            }
            #endregion

            #region Image_Of_RotationButton
            // Try loading the correct embedded image
            string resourceName2 = "RevitApplication.Resources.Rotate.png";
            Stream iconStream2 = assembly.GetManifestResourceStream(resourceName2);

            if (iconStream2 != null)
            {
                BitmapImage bitmap2 = new BitmapImage();
                bitmap2.BeginInit();
                bitmap2.StreamSource = iconStream2;
                bitmap2.CacheOption = BitmapCacheOption.OnLoad;
                bitmap2.EndInit();

                RotationButton.LargeImage = bitmap2;
            }
            else
            {
                TaskDialog.Show("Image Error", $"Could not load image: {resourceName2}");
            }
            #endregion
            return Result.Succeeded;
        }
    }
}
