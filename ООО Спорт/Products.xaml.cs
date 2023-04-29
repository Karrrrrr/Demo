using System;
using System.Collections.Generic;
using System.Data;
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
using ООО_Спорт.Context;
using ООО_Спорт.Models;

namespace ООО_Спорт
{
	/// <summary>
	/// Логика взаимодействия для Products.xaml
	/// </summary>
	public partial class Products : Window
	{
		public Products()
		{
			InitializeComponent();
		}

		private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (Visibility == Visibility.Visible)
			{
				ProductsGrid.Children.Clear();
				int n = 1;
				List<Product> p = new List<Product>();
				using (EmployeeContext db = new EmployeeContext())
				{
					p = db.Product.ToList();
				}
				foreach (Product product in p)
				{
					ProductsGrid.RowDefinitions.Add(new RowDefinition());

					StackPanel prod = new StackPanel() { Orientation = Orientation.Horizontal };
					Image image = new Image() { Source = new BitmapImage(new Uri("picture.png", UriKind.Relative)) };

					StackPanel inf = new StackPanel() { Orientation = Orientation.Vertical, VerticalAlignment = VerticalAlignment.Center };
					TextBlock nameTB = new TextBlock() { Text = product.ProductName, FontWeight = FontWeights.Bold };
					TextBlock descriptionTB = new TextBlock() { Text = product.ProductDescription };
					TextBlock manufactureTB = new TextBlock() { Text = product.ProductManufacturer };
					StackPanel cost = new StackPanel() { Orientation = Orientation.Horizontal };
					if (product.ProductDiscountAmount > 0)
					{
						TextBlock oldCost = new TextBlock() { Text = product.ProductCost.ToString(), TextDecorations = TextDecorations.Strikethrough };
						TextBlock newCost = new TextBlock() { Text = (product.ProductCost - product.ProductCost * product.ProductDiscountAmount * 0.01m).ToString() };
						cost.Children.Add(oldCost);
						cost.Children.Add(newCost);
					}
					else
					{
						TextBlock costTB = new TextBlock() { Text = product.ProductCost.ToString() };
						cost.Children.Add(costTB);
					}
					inf.Children.Add(nameTB);
					inf.Children.Add(descriptionTB);
					inf.Children.Add(manufactureTB);
					inf.Children.Add(cost);

					TextBlock discountTB = new TextBlock() { Text = product.ProductDiscountAmount.ToString() + "%", Padding = new Thickness(10), VerticalAlignment = VerticalAlignment.Center };
					if (product.ProductDiscountAmount > 15)
					{
						discountTB.Background = (Brush)new BrushConverter().ConvertFromString("#7fff00");
					}

					prod.Children.Add(image);
					prod.Children.Add(inf);
					prod.Children.Add(discountTB);

					ProductsGrid.Children.Add(prod);
					Grid.SetRow(prod, n);
					n++;
				}
			}
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			//Environment.Exit(0);
		}
	}
}
