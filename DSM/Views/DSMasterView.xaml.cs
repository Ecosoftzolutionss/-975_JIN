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
using System.Text.RegularExpressions;

namespace DSM.Views
{
    /// <summary>
    /// Interaction logic for DSMasterView.xaml
    /// </summary>
    public partial class DSMasterView : UserControl
    {
        public DSMasterView()
        {
            InitializeComponent();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {

        }

        private void txtNickName_TextChanged(object sender, TextChangedEventArgs e)
        {


        }

        private void onkeypressvalidate(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.A && e.Key <= Key.Z)
            {
            }
            else
            {
                e.Handled = true;
            }



            //string s = @"$KUH% I*$)OFNlkfn$";
            //var withoutSpecial = new string(s.Where(c => Char.IsLetterOrDigit(c)
            //                                            || Char.IsWhiteSpace(c)).ToArray());

            //if (s != withoutSpecial)
            //{
            //    MessageBox.Show("String contains special   chars");
            //}

            //string st = "0123456789" + (char)8;
            //if (st.IndexOf(e.KeyChar) == -1)
            //{
            //    MessageBox.Show("please enter digits only");
            //    e.Handled = true;
            //}
        }

        private void dsonpress(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.A && e.Key <= Key.Z)
            {
            }
            else
            {
                e.Handled = true;
            }

        }

        private void deptonpress(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.A && e.Key <= Key.Z)
            {
            }
            else
            {
                e.Handled = true;
            }

        }

        private void designationonpress(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.A && e.Key <= Key.Z)
            {
            }
            else
            {
                e.Handled = true;
            }

        }

        private void pinonpress(object sender,KeyEventArgs e)
        {
            //if (e.Key >= Key.A && e.Key <= Key.Z) //&& e.Key >= Key.D0 && e.Key <= Key.D9) 
            //{
            //}
            //else if (e.Key >= Key.D0 && e.Key <= Key.D9)
            //{ }
            //else
            //{ 
            //    e.Handled = true;
            //}


            //Regex regex = new Regex("[^0-9]+");
            //e.Handled = regex.IsMatch(e.Text);

            //string st = "0123456789" + (char)8;
            //if (st.IndexOf(e.KeyChar) == -1)
            //{
            //    MessageBox.Show("please enter digits only");
            //    e.Handled = true;
            //}
        }

    }
}
