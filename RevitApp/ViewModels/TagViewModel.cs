using Autodesk.Revit.DB;
using RevitApplication.Commands;
using RevitApplication.Commands.Window;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace RevitApplication
{
    internal class TagViewModel : INotifyPropertyChanged
    {
        #region Fields
        private string _selectedCategory;
        private string _selectedOrientation;
        private Boolean _leader;
        #endregion
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Constructor
        public TagViewModel()
        {
            DoneCommand = new Command(Execute);
        }

        #endregion

        #region Properities
        public Command DoneCommand { get; set; }
        public List<string> Categories { get; set; } = new List<string>
        {
            "Structural Columns",
            
            "Footings"

        };
        public List<string> TagOrientation { get; set; } = new List<string>
        {
            "Horizontal",
            "Vertical",
            "Any Model Direction"

        };
        public string SelectedCategory { get { return _selectedCategory; } set { _selectedCategory = value; OnPropertyChanged(nameof(SelectedCategory)); } }
        public string SelectedOrientation { get { return _selectedOrientation; } set { _selectedOrientation = value; OnPropertyChanged(nameof(SelectedOrientation)); } }
        public Boolean Leader { get { return _leader; } set { _leader = value; OnPropertyChanged(nameof(Leader)); } }

        public Action CloseAction { get; internal set; }





        #endregion

        #region Methods
        public BuiltInCategory GetBuiltInCategory()
        {
            switch (SelectedCategory)
            {
                case "Structural Columns":
                    return Autodesk.Revit.DB.BuiltInCategory.OST_StructuralColumns;
                case "Footings":
                    return Autodesk.Revit.DB.BuiltInCategory.OST_StructuralFoundation;
                default:
                    return Autodesk.Revit.DB.BuiltInCategory.OST_StructuralColumns;

            }
        }
        public TagOrientation GetTagOrientation()
        {
            switch (SelectedOrientation)
            {
                case "Horizontal":
                    return Autodesk.Revit.DB.TagOrientation.Horizontal;
                case "Vertical":
                    return Autodesk.Revit.DB.TagOrientation.Vertical;
                case "Any Model Direction":
                    return Autodesk.Revit.DB.TagOrientation.AnyModelDirection;
                default:
                    return Autodesk.Revit.DB.TagOrientation.AnyModelDirection;

            }
        }
        
        #endregion
        private void Execute(object obj)
        {
            if (SelectedCategory != null)
            {
                //EnsureTagFamilyLoaded(WindowCommand.doc);
                var elements = new FilteredElementCollector(TagWindowCommand.doc)
                    .OfCategory(GetBuiltInCategory()).WhereElementIsNotElementType()
                    .WhereElementIsNotElementType()
                    .ToElements();

                using (Transaction tr = new Transaction(TagWindowCommand.doc, "Creating Tags"))
                {
                    tr.Start();

                    foreach (var item in elements)
                    {
                        Reference reference = new Reference(item);
                        LocationPoint location = item.Location as LocationPoint;

                        if (location != null)
                        {
                            XYZ point = location.Point;

                             IndependentTag.Create(
                                TagWindowCommand.doc,
                                TagWindowCommand.doc.ActiveView.Id,
                                reference,
                                Leader,
                               TagMode.TM_ADDBY_CATEGORY,
                                GetTagOrientation(),
                                point
                            );
                        }
                    }

                    tr.Commit();
                }
            }
            
            CloseAction?.Invoke();
        }
        

    }
}
