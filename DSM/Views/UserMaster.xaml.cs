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

namespace DSM.Views
{
    /// <summary>
    /// Interaction logic for UserMaster.xaml
    /// </summary>
    public partial class UserMaster : UserControl
    {
        public UserMaster()
        {
            InitializeComponent();
        }

        private void useronpress(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.A && e.Key <= Key.Z)
            {
            }
            else
            {
                e.Handled = true;
            }
        }
    }
}
