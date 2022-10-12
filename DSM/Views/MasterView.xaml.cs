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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DSM.Views
{
    /// <summary>
    /// Interaction logic for MasterView.xaml
    /// </summary>
    public partial class MasterView : UserControl
    {
        public MasterView()
        {
            InitializeComponent();
        }

        private void Minimize_btn(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);

            // Minimize
            window.WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            App.Current.MainWindow.Close();
        }

        private void btnmin_Click(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);

            // Minimize
            window.WindowState = WindowState.Minimized;
           
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
