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
    /// Логика взаимодействия для ChoiceView.xaml
    /// </summary>
    public partial class ChoiceView : Window
    {

        public static string View = " ";
        //public static string Session = " ";
        private int click = 0;

        public ChoiceView()
        {
            InitializeComponent();
        }

        private void btnAudioGid_Click(object sender, RoutedEventArgs e)
        {
            View = "AudioGid";
            click = 1;
            Close();
        }

        private void btnText_Click(object sender, RoutedEventArgs e)
        {
            View = "Text";
            click = 1;
            Close();
        }

        private void btnStraightaway_Click(object sender, RoutedEventArgs e)
        {
            View = "Straightaway";
            click = 1;
            Close();
        }

        private void ChoiseView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
 
        }
    }
}
