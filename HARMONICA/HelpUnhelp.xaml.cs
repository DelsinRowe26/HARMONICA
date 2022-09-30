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

namespace HARMONICA
{
    /// <summary>
    /// Логика взаимодействия для HelpUnhelp.xaml
    /// </summary>
    public partial class HelpUnhelp : Window
    {

        public static int HelpUnhelpClick = 0;

        public HelpUnhelp()
        {
            InitializeComponent();
        }

        private async void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            HelpUnhelpClick = 1;
            Help help = new Help();
            help.ShowDialog();
            Close();
        }

        private async void btnUnhelp_Click(object sender, RoutedEventArgs e)
        {
            HelpUnhelpClick = 2;
            Help help = new Help();
            help.ShowDialog();
            Close();
        }
    }
}
