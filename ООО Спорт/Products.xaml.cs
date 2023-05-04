using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
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
		List<Product> filteredProducts = new List<Product>();
		List<Product> searchedProducts = new List<Product>();
		public static User user;
		int productsCount = 0;
		public Products()
		{
			InitializeComponent();
		}

		private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (Visibility == Visibility.Visible)
			{
				if (user.UserRole == 2)
				{
					CreateButton.Visibility = Visibility.Visible;
				}
				else
				{
					CreateButton.Visibility = Visibility.Hidden;
				}
				filteredProducts = DatabaseConnection.GetProducts();
				searchedProducts = filteredProducts.ToList();
				productsCount = filteredProducts.Count;
				LoadProducts(filteredProducts);
				if (nameTB != null)
				{
					nameTB.Text = user.UserSurname + " " + user.UserName + " " + user.UserPatronymic;
				}
			}
		}

		private void LoadProducts(List<Product> products)
		{
			ProductsGrid.Children.Clear();
			int n = 1;
			foreach (Product product in products)
			{
				ProductsGrid.RowDefinitions.Add(new RowDefinition());

				StackPanel prod = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(10) };

				Image image = new Image() { Width = 100, Height = 100 };
				if (File.Exists(Environment.CurrentDirectory.Remove(Environment.CurrentDirectory.Length - 10) + @"/Resources/" + product.ProductArticleNumber + ".jpg") || File.Exists(Environment.CurrentDirectory.Remove(Environment.CurrentDirectory.Length - 10) + @"/Resources/" + product.ProductArticleNumber + ".png"))
				{
					image.Source = new BitmapImage(new Uri("Resources/" + product.ProductPhoto, UriKind.Relative));
				}
				else
				{
					image.Source = new BitmapImage(new Uri("Resources/picture.png", UriKind.Relative));
				}

				StackPanel inf = new StackPanel() { Orientation = Orientation.Vertical, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(10), Width = 300 };
				TextBlock nameTB = new TextBlock() { Text = product.ProductName, FontWeight = FontWeights.Bold };
				TextBlock descriptionTB = new TextBlock() { Text = product.ProductDescription, TextWrapping = TextWrapping.Wrap };
				TextBlock manufactureTB = new TextBlock() { Text = product.ProductManufacturer };
				StackPanel cost = new StackPanel() { Orientation = Orientation.Horizontal };
				if (product.ProductDiscountAmount > 0)
				{
					TextBlock oldCost = new TextBlock() { Text = product.ProductCost.ToString(), TextDecorations = TextDecorations.Strikethrough, Margin = new Thickness(0, 0, 10, 0) };
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

				prod.MouseUp += (o, e) =>
				{
					if (user.UserRole == 2)
					{
						CreateEditProduct.isCreate = false;
						CreateEditProduct.product = product;
						CreateEditProduct cep = new CreateEditProduct();
						cep.ShowDialog();
					}
				};

				ProductsGrid.Children.Add(prod);
				Grid.SetRow(prod, n);
				Grid.SetColumn(prod, 1);
				n++;
			}
			if (countTB != null)
			{
				countTB.Text = "Количество выведенных товаров: " + products.Count().ToString() + " из " + productsCount.ToString();
			}
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			//Environment.Exit(0);
		}

		private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Sort(searchedProducts);
		}

		private void FilterCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (FilterCB.SelectedIndex == 0)
			{
				filteredProducts = DatabaseConnection.GetProducts();
			}
			else if (FilterCB.SelectedIndex == 1)
			{
				filteredProducts = DatabaseConnection.GetFilteredProducts(1);
			}
			else if (FilterCB.SelectedIndex == 2)
			{
				filteredProducts = DatabaseConnection.GetFilteredProducts(2);
			}
			else
			{
				filteredProducts = DatabaseConnection.GetFilteredProducts(3);
			}
			Search();
		}

		private void Sort(List<Product> p)
		{
			if (SortCB.SelectedIndex == 0)
			{
				LoadProducts(p);
			}
			else if (SortCB.SelectedIndex == 1)
			{
				LoadProducts(p.OrderBy(x => x.ProductCost).ToList());
			}
			else
			{
				LoadProducts(p.OrderByDescending(x => x.ProductCost).ToList());
			}
		}

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			Search();
		}

		private void Search()
		{
			if ((SearchTB == null) || (SearchTB.Text == ""))
			{
				Sort(filteredProducts);
			}
			else
			{
				searchedProducts.Clear();
				foreach (Product product in filteredProducts)
				{
					if (product.ProductName.Contains(SearchTB.Text))
					{
						searchedProducts.Add(product);
					}
				}
				Sort(searchedProducts);
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			CreateEditProduct.isCreate = true;
			CreateEditProduct cep = new CreateEditProduct();
			cep.ShowDialog();
        }

		private void Window_Activated(object sender, EventArgs e)
		{
			filteredProducts = DatabaseConnection.GetProducts();
			searchedProducts = filteredProducts.ToList();
			productsCount = filteredProducts.Count;
			LoadProducts(filteredProducts);
		}
	}
}
