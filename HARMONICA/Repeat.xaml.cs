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
    /// Логика взаимодействия для Repeat.xaml
    /// </summary>
    public partial class Repeat : Window
    {

        public static string repeat = "Yes";
        public static int click = 0;

        public Repeat()
        {
            InitializeComponent();
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            repeat = "Yes";
            click = 1;
            Close();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            repeat = "No";
            click = 1;
            Close();
        }

        private void Repeat_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(click == 0)
            {
                e.Cancel = true;
            }
        }
    }
}
