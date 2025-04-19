using RevitApplication.ViewModels;
using RevitApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RevitApplication.Views
{
    /// <summary>
    /// Interaction logic for RotationWindow.xaml
    /// </summary>
    public partial class RotationWindow : Window
    {
        public RotationWindow(Autodesk.Revit.UI.UIApplication application)
        {
            InitializeComponent();
            var vm = new RotationViewModel(application);
            vm.CloseAction = () => this.Close();
            DataContext = vm;
        }
    }
}
