using Microsoft.Win32;
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
    /// Interaction logic for ReprocessView.xaml
    /// </summary>
    public partial class ReprocessView : UserControl
    {
        public ReprocessView()
        {
            InitializeComponent();
        }
        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    OpenFileDialog openFileDialog = new OpenFileDialog();
            //    if (openFileDialog.ShowDialog() == true)
            //    {
            //        FilePath.Text = openFileDialog.FileName;

            //    }

            //}
            //catch (Exception)
            //{
            //    // handle exception here;
            //}
        }

        private void dgPending_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
