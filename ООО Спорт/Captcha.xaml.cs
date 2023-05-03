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

namespace ООО_Спорт
{
	/// <summary>
	/// Логика взаимодействия для Captcha.xaml
	/// </summary>
	public partial class Captcha : Window
	{
		int rand;
		public Captcha()
		{
			InitializeComponent();
		}

		private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (Visibility == Visibility.Visible)
			{
				Random r = new Random();
				rand = r.Next(1000, 10000);
				captcha.Text = rand.ToString();
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			if (captchaTB.Text != rand.ToString())
			{
				MainWindow.wrongCaptcha = true;
			}
			Close();
		}
	}
}
