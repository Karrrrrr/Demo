using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
		bool captchaFlag = false;
		public static bool wrongCaptcha = false;
		Timer timer;
		bool enterButton = false;
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			if (enterButton)
			{
				MessageBox.Show("Вход заблокирован. Подождите немного, чтобы попробовать ещё раз");
			}
			else
			{
				if (captchaFlag)
				{
					captchaFlag = false;
					Captcha captcha = new Captcha();
					captcha.ShowDialog();
				}
				if ((loginTB.Text.Length != 0) && (PasswordTB.Text.Length != 0))
				{
					int code = DatabaseConnection.Login(loginTB.Text, PasswordTB.Text);
					if (code == 0)
					{
						if (wrongCaptcha)
						{
							MessageBox.Show("CAPTCHA введена неверно. Вход заблокирован на 10 секунд");
							wrongCaptcha = false;
							enterButton = true;
							timer = new Timer(TimerTick, null, 0, 10000);
						}
						else
						{
							Products p = new Products();
							p.ShowDialog();
						}
					}
					else if (code == 1)
					{
						if (wrongCaptcha)
						{
							MessageBox.Show("Неверный пароль. Вход заблокирован на 10 секунд");
							wrongCaptcha = false;
							enterButton = true;
							timer = new Timer(TimerTick, null, 0, 10000);
						}
						else
						{
							MessageBox.Show("Неверный пароль");
						}
						captchaFlag = true;
					}
					else
					{
						if (wrongCaptcha)
						{
							MessageBox.Show("Неверный логин. Вход заблокирован на 10 секунд");
							wrongCaptcha = false;
							enterButton = true;
							timer = new Timer(TimerTick, null, 10000, 10000);
						}
						else
						{
							MessageBox.Show("Неверный логин");
						}
						captchaFlag = true;
					}
				}
				else
				{
					MessageBox.Show("Введите логин и пароль");
				}
			}
		}

		private void TimerTick(object state)
		{
			enterButton = false;
			timer.Change(Timeout.Infinite, Timeout.Infinite);
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			Products.user = new User() { UserName = "Гость", UserSurname = "", UserPatronymic = "" };
			Products p = new Products();
			p.ShowDialog();
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Environment.Exit(0);
		}
	}
}
