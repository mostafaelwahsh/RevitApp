using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using RevitApplication.Commands.Window;
using RevitApplication.Commands;
using System;
using System.ComponentModel;

namespace RevitApplication.ViewModels
{
    internal class RotationViewModel : INotifyPropertyChanged
    {
        private double _rotationAngle;
        private UIApplication _uiApp;


        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Constructor
        public RotationViewModel(UIApplication uiApp)
        {
            _uiApp = uiApp;
            ApplyCommand = new Command(Execute);
        }

        #endregion

        #region Properities
        public Command ApplyCommand { get; set; }

        public double RotationAngle
        {
            get => _rotationAngle;
            set { _rotationAngle = value; OnPropertyChanged(); }
        }

        public Action CloseAction { get; internal set; }





        #endregion




        private void Execute(object obj)
        {
            UIDocument uidoc = _uiApp.ActiveUIDocument;
            Document doc = uidoc.Document;

            using (Transaction tx = new Transaction(doc, "Rotate Selected Elements"))
            {
                tx.Start();

                foreach (ElementId id in RotationWindowCommand.SelectedElementIds)
                {
                    Element el = doc.GetElement(id);
                    if (el.Location is LocationPoint location)
                    {
                        XYZ basePoint = location.Point;
                        Line axis = Line.CreateBound(basePoint, basePoint + XYZ.BasisZ);
                        double angleRadians = RotationAngle * Math.PI / 180.0;

                        ElementTransformUtils.RotateElement(doc, el.Id, axis, angleRadians);
                    }
                }

                tx.Commit();
            }
            CloseAction?.Invoke();
        }
        public void OnPropertyChanged()
        {
            OnPropertyChanged(nameof(RotationAngle));
        }
    }
}

