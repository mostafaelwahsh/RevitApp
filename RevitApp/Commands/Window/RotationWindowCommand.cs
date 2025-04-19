using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RevitApplication.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitApplication.Commands.Window
{
    [Transaction(TransactionMode.Manual)]
    public class RotationWindowCommand : IExternalCommand
    {
        public static Document doc;
        public static List<ElementId> SelectedElementIds = new List<ElementId>();

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                // Step 1: Let the user pick structural columns and footings
                IList<Reference> pickedRefs = uidoc.Selection.PickObjects(
                    ObjectType.Element,
                    new StructuralElementSelectionFilter(),
                    "Select Structural Columns and Footings"
                );

                SelectedElementIds = pickedRefs.Select(r => r.ElementId).ToList();

                // Step 2: Open the WPF Window
                RotationWindow window = new RotationWindow(commandData.Application);
                window.ShowDialog();

                return Result.Succeeded;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
    public class StructuralElementSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element e)
        {
            return e.Category != null &&
                   (e.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralColumns ||
                    e.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFoundation);
        }

        public bool AllowReference(Reference reference, XYZ position) => true;
    }
}
