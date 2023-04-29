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
using ООО_Спорт.Context;
using ООО_Спорт.Models;

namespace ООО_Спорт
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			if ((loginTB.Text.Length != 0) && (PasswordTB.Text.Length != 0))
			{
				bool flag = true;
				using (EmployeeContext db = new EmployeeContext())
				{
					foreach (User u in db.User)
					{
						if (u.UserLogin == loginTB.Text)
						{
							if (u.UserPassword == PasswordTB.Text)
							{
								Products p = new Products();
								p.ShowDialog();
								flag = false;
								break;
							}
							else
							{
								MessageBox.Show("Пароль неверный");
								flag = false;
								break;
							}
						}
					}
					if (flag)
					{
						MessageBox.Show("Неверный логин");
					}
				}
			}
			else
			{
				MessageBox.Show("Введите логин и пароль");
			}
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			Products p = new Products();
			p.ShowDialog();
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Environment.Exit(0);
		}
	}
}
