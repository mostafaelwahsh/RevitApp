using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Linq;

namespace RevitApplication.Creating_Sheets
{
    [Transaction(TransactionMode.Manual)]
    public class Sheets : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;


            var titleBlockType = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_TitleBlocks)
                .WhereElementIsElementType()
                .FirstOrDefault();

            if (titleBlockType == null)
            {
                TaskDialog.Show("Error", "No title block found.");
                return Result.Failed;
            }


            var allPlans = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewPlan))
                .Cast<ViewPlan>()
                .Where(v => !v.IsTemplate)
                .ToList();


            var floorPlans = allPlans
                .Where(v => v.ViewType == ViewType.FloorPlan)
                .ToList();

            var ceilingPlans = allPlans
                .Where(v => v.ViewType == ViewType.CeilingPlan)
                .ToList();

            if (floorPlans.Count == 0 && ceilingPlans.Count == 0)
            {
                TaskDialog.Show("Info", "All floor and ceiling plan views are already placed on sheets.");
                return Result.Cancelled;
            }


            try
            {

                // Get all existing sheet names
                var existingSheetNames = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewSheet))
                .Cast<ViewSheet>()
                .Select(s => s.Name)
                .ToList(); // keep it as List for partial matching

                // Check if any sheet name starts with FLR- or CLG-
                bool anySheetExists = existingSheetNames.Any(name => name.StartsWith("FLR-"))
                                   || existingSheetNames.Any(name => name.StartsWith("CLG-"));

                if (anySheetExists)
                {
                    TaskDialog dialog = new TaskDialog("Sheets Already Exist");
                    dialog.MainInstruction = "Some sheets already exist.";
                    dialog.MainContent = "Do you want to create them again anyway?";
                    dialog.CommonButtons = TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No;
                    TaskDialogResult result = dialog.Show();

                    if (result == TaskDialogResult.No)
                        return Result.Cancelled;
                }


                using (Transaction tr = new Transaction(doc, "Create Sheets for Floor and Ceiling Plans"))
                {
                    tr.Start();

                    int floorCounter = 1;
                    foreach (var view in floorPlans)
                    {
                        ViewSheet sheet = ViewSheet.Create(doc, titleBlockType.Id);
                        var outline = sheet.Outline;
                        sheet.Name = $"FLR-{floorCounter} - {view.Name}";


                        double xu = (outline.Max.U + outline.Min.U) / 2;
                        double xv = (outline.Max.V + outline.Min.V) / 2;
                        XYZ point = new XYZ(xu, xv, 0);
                        if (Viewport.CanAddViewToSheet(doc, sheet.Id, view.Id))
                        {
                            Viewport.Create(doc, sheet.Id, view.Id, point);
                        }
                        floorCounter++;
                    }

                    int ceilingCounter = 1;
                    foreach (var view in ceilingPlans)
                    {
                        ViewSheet sheet = ViewSheet.Create(doc, titleBlockType.Id);
                        var outline = sheet.Outline;
                        sheet.Name = $"CLG-{ceilingCounter} - {view.Name}";


                        double xu = (outline.Max.U + outline.Min.U) / 2;
                        double xv = (outline.Max.V + outline.Min.V) / 2;
                        XYZ point = new XYZ(xu, xv, 0);
                        if (Viewport.CanAddViewToSheet(doc, sheet.Id, view.Id))
                        {
                            Viewport.Create(doc, sheet.Id, view.Id, point);
                        }
                        ceilingCounter++;
                    }


                    tr.Commit();
                }

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", ex.Message);
                return Result.Failed;
            }
        }
    }
}
