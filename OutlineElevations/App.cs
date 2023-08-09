using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Utilities;

namespace OutlineElevations
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.DB.Macros.AddInId("ab29e823-b124-4cd7-81a4-20406504de48")]
    public class ThisApplication : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication uiApp)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication uiApp)
        {

            try
            {
                #region GAS ADDIN BOILERPLATE
                // Assembly that contains the invoke method
                string exeConfigPath = Utils.GetExeConfigPath("OutlineElevations.dll");

                // Finds and creates the tab, finds and creates the panel
                RibbonPanel DefaultPanel = Utils.GetRevitPanel(uiApp, GlobalVars.PANEL_NAME, GlobalVars.TAB_NAME);
                #endregion

                // Button configuration
                string OutlineElevationsName = "Outline\nElevations";
                PushButtonData OutlineElevationsData = new PushButtonData(OutlineElevationsName, OutlineElevationsName, exeConfigPath, "OutlineElevations.ThisCommand");
                OutlineElevationsData.LargeImage = Utils.RetriveImage("OutlineElevations.Resources.CropReg32x32.ico", Assembly.GetExecutingAssembly()); // Pushbutton image
                OutlineElevationsData.ToolTip = "";
                DefaultPanel.AddItem(OutlineElevationsData); // Add pushbutton

                return Result.Succeeded;
            }


            catch (Exception ex)
            {
                Utils.CatchDialog(ex);
                return Result.Failed;
            }
        }
    }
}
