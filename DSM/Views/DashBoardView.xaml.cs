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
using System.Diagnostics;

namespace DSM.Views
{
    /// <summary>
    /// Interaction logic for DashBoardView.xaml
    /// </summary>
    public partial class DashBoardView : UserControl
    {
        public DashBoardView()
        {
            InitializeComponent();
        }

        //private void Hyperlink_Click(object sender, RoutedEventArgs e)
        //{
        //    Process p = new Process();
        //    p.StartInfo.FileName = @"C:\PSTools\PsExec.exe";
        //    //p.StartInfo.FileName = @"\\server\IT\SOMS V2.0\YSI\SOMS v2.0 YSI 68-UOM-MOBIS\SOMS(SalesOrderManagementSystem)\bin\Debug\SOMS(SalesOrderManagementSystem).exe";
        //    //p.StartInfo.FileName = @"C:\PsExec.exe";
        //    //p.StartInfo.Arguments = @"-s -d -i 1 \\server calc.exe ";
        //    //p.StartInfo.Arguments = @"-s -d -i 1 \\server C:\Users\Administrator\Desktop\Test\SOMS(SalesOrderManagementSystem).exe";
        //    p.StartInfo.Arguments = @"-s -d -i 1 \\server C:\Users\Administrator\Desktop\SLL\DSM.exe";
        //    p.Start();
        //}

        private void DgPending_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void DgCurrentInv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void DgPending_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TxtSearchText_TextChanged(object sender, TextChangedEventArgs e)
        {
           textblock.Text = "";

        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {

        }
        


        private void DgCurrentInv_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }
        

        private void Minimize_btn(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);

            // Minimize
            window.WindowState = WindowState.Minimized;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            App.Current.MainWindow.Close();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //if(Global.UserType== "Api")
            //{
            //    currinvgrid.Visibility = Visibility.Hidden;
            //}
        }

        private void DgPending_SelectionChanged_2(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
