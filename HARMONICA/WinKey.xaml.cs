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
using System.IO;

namespace HARMONICA
{
	/// <summary>
	/// Логика взаимодействия для WinKey.xaml
	/// </summary>
	public partial class WinKey : Window
	{

		private static string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		private static string path2;
		Stream MyStream;

		public WinKey()
		{
			InitializeComponent();
		}

		private void btnActivate_Click(object sender, RoutedEventArgs e)
		{
			switch (pbKey.Password.ToString())
			{
				case "nBs1pVRH":
					FileStream fs = new FileStream(path2, FileMode.Append);
					StreamWriter sw = new StreamWriter(fs);
					sw.WriteLineAsync(pbKey.Password.ToString());
					sw.Close();
					fs.Close();
					MessageBox.Show("Количество запусков увеличено на 7");
					Close();
					break;
				case "q7|cM**D":
					FileStream fs1 = new FileStream(path2, FileMode.Append);
					StreamWriter sw1 = new StreamWriter(fs1);
					sw1.WriteLineAsync(pbKey.Password.ToString());
					sw1.Close();
					fs1.Close();
					MessageBox.Show("Количество запусков увеличено на 7");
					Close();
					break;
				case "$99aJKQI":
					FileStream fs2 = new FileStream(path2, FileMode.Append);
					StreamWriter sw2 = new StreamWriter(fs2);
					sw2.WriteLineAsync(pbKey.Password.ToString());
					sw2.Close();
					fs2.Close();
					MessageBox.Show("Количество запусков увеличено на 7");
					Close();
					break;
				case "SYf6f3NJ":
					FileStream fs3 = new FileStream(path2, FileMode.Append);
					StreamWriter sw3 = new StreamWriter(fs3);
					sw3.WriteLineAsync(pbKey.Password.ToString());
					sw3.Close();
					fs3.Close();
					MessageBox.Show("Количество запусков увеличено на 7");
					Close();
					break;
				case "|*~0TILx":
					FileStream fs4 = new FileStream(path2, FileMode.Append);
					StreamWriter sw4 = new StreamWriter(fs4);
					sw4.WriteLineAsync(pbKey.Password.ToString());
					sw4.Close();
					fs4.Close();
					MessageBox.Show("Количество запусков увеличено на 7");
					Close();
					break;
				case "{1fpjgIa":
					FileStream fs5 = new FileStream(path2, FileMode.Append);
					StreamWriter sw5 = new StreamWriter(fs5);
					sw5.WriteLineAsync(pbKey.Password.ToString());
					sw5.Close();
					fs5.Close();
					MessageBox.Show("Количество запусков увеличено на 7");
					Close();
					break;
				case "s4DwaSG6":
					FileStream fs6 = new FileStream(path2, FileMode.Append);
					StreamWriter sw6 = new StreamWriter(fs6);
					sw6.WriteLineAsync(pbKey.Password.ToString());
					sw6.Close();
					fs6.Close();
					MessageBox.Show("Количество запусков увеличено на 7");
					Close();
					break;
				case "yCRYymE9":
					FileStream fs7 = new FileStream(path2, FileMode.Append);
					StreamWriter sw7 = new StreamWriter(fs7);
					sw7.WriteLineAsync(pbKey.Password.ToString());
					sw7.Close();
					fs7.Close();
					MessageBox.Show("Количество запусков увеличено на 7");
					Close();
					break;
				case "h?d3?NR2":
					FileStream fs8 = new FileStream(path2, FileMode.Append);
					StreamWriter sw8 = new StreamWriter(fs8);
					sw8.WriteLineAsync(pbKey.Password.ToString());
					sw8.Close();
					fs8.Close();
					MessageBox.Show("Количество запусков увеличено на 7");
					Close();
					break;
				case "eFcu8H1r":
					FileStream fs9 = new FileStream(path2, FileMode.Append);
					StreamWriter sw9 = new StreamWriter(fs9);
					sw9.WriteLineAsync(pbKey.Password.ToString());
					sw9.Close();
					fs9.Close();
					MessageBox.Show("Количество запусков увеличено на 7");
					Close();
					break;
				default:
					MessageBox.Show("Количество запусков не увеличино");
					Close();
					break;
			}
			
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			path2 = path + @"\ReSelf - Mental detox Katarsis\Key.tmp";
		}
	}
}
