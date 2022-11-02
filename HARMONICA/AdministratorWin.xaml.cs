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
    /// Логика взаимодействия для AdministratorWin.xaml
    /// </summary>
    public partial class AdministratorWin : Window
    {

        

        public AdministratorWin()
        {
            InitializeComponent();
        }

        private void btnActivate_Click(object sender, RoutedEventArgs e)
        {
            string login;
            string password;

            login = tbLogin.Text.ToString();
            password = pbPass.Password.ToString();


        }
    }
}
