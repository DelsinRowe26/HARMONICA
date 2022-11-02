using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Логика взаимодействия для Help.xaml
    /// </summary>
    public partial class Help : Window
    {
        public Help()
        {
            InitializeComponent();
        }

        private void btnVK_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://vk.com/al_im.php?invite_chat_id=8589934592511611692&invite_link=AJQ1dyyTfh6pq2gMJMA1JkxH&invite_hash=qatoDCTANSZMRw==");
            Close();
        }

        private void btnTelegram_Click(object sender, RoutedEventArgs e)
        {
            if (HelpUnhelp.HelpUnhelpClick == 1)
            {
                Process.Start("https://t.me/+NluKC6H_m1ZjYmUy");
                Close();
            }
            else if (HelpUnhelp.HelpUnhelpClick == 2)
            {
                Process.Start("https://t.me/+T_XNE1Lrqsg4ZjA6");
                Close();
            }
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(HelpUnhelp.HelpUnhelpClick == 1)
            {
                lbComment.Content = "Please leave your feedback";
                btnVK.Visibility = Visibility.Visible;
                btnTelegram.Visibility = Visibility.Visible;
            }
            else if(HelpUnhelp.HelpUnhelpClick == 2)
            {
                lbComment.Content = "If you want to understand\nwhy you did not feel\nthe effect,\nplease leave a request,\nwe will contact you";
                btnVK.Visibility = Visibility.Hidden;
            }
        }
    }
}
