using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
using ООО_Спорт.Models;
using Word = Microsoft.Office.Interop.Word;

namespace ООО_Спорт
{
	/// <summary>
	/// Логика взаимодействия для Cart.xaml
	/// </summary>
	public partial class Cart : Window
	{
		public static Order order;
		decimal fullCost = 0;
		decimal discountCost = 0;
		int productCount = 0;
		public Cart()
		{
			InitializeComponent();
		}

		private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (Visibility == Visibility.Visible)
			{
				if (order != null)
				{
					order = DatabaseConnection.GetCurrentOrder(order.OrderId);
					pickPointTB.Text = order.OrderPickupPoint;
					if (Products.user.UserName != "Гость")
					{
						UserName.Text = Products.user.UserSurname + " " + Products.user.UserName + " " + Products.user.UserPatronymic;
					}
					else
					{
						UserName.Text = "";
					}
					LoadCart();
				}
			}
		}

		private void LoadCart()
		{
			fullCost = 0;
			discountCost = 0;
			productCount = 0;
			productGrid.Children.Clear();
			productGrid.RowDefinitions.Clear();
			//productGrid.RowDefinitions.Add(new RowDefinition());
			List<Product> products = DatabaseConnection.GetCart(order.OrderId);
			int n = 0;
			foreach (Product product in products)
			{
				LoadProductInCart(product, n);
				n++;
			}
			FinalCost();
		}

		private void LoadProductInCart(Product product, int row)
		{
			productCount++;
			productGrid.RowDefinitions.Add(new RowDefinition());

			StackPanel prod = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(10), HorizontalAlignment = HorizontalAlignment.Center };

			Image image = new Image() { Width = 100, Height = 100 };
			if (File.Exists(Environment.CurrentDirectory.Remove(Environment.CurrentDirectory.Length - 10) + @"/Resources/" + product.ProductPhoto))
			{
				using (FileStream fs = File.OpenRead(Environment.CurrentDirectory.Remove(Environment.CurrentDirectory.Length - 10) + @"/Resources/" + product.ProductPhoto))
				{
					BitmapImage bi = new BitmapImage();
					bi.BeginInit();
					bi.CacheOption = BitmapCacheOption.OnLoad;
					bi.StreamSource = fs;
					bi.EndInit();
					image.Source = bi;
				}
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
				decimal nc = (decimal)(product.ProductCost - product.ProductCost * product.ProductDiscountAmount * 0.01m);
				fullCost += product.ProductCost;
				discountCost += nc;
				TextBlock oldCost = new TextBlock() { Text = Math.Round(product.ProductCost, 2).ToString() + "₽", TextDecorations = TextDecorations.Strikethrough, Margin = new Thickness(0, 0, 10, 0) };
				TextBlock newCost = new TextBlock() { Text = Math.Round(nc, 2).ToString() + "₽" };
				cost.Children.Add(oldCost);
				cost.Children.Add(newCost);
			}
			else
			{
				fullCost += product.ProductCost;
				discountCost += product.ProductCost;
				TextBlock costTB = new TextBlock() { Text = Math.Round(product.ProductCost, 2).ToString() + "₽" };
				cost.Children.Add(costTB);
			}
			inf.Children.Add(nameTB);
			inf.Children.Add(descriptionTB);
			inf.Children.Add(manufactureTB);
			inf.Children.Add(cost);

			Button delete = new Button() { Content = "Удалить", HorizontalAlignment = HorizontalAlignment.Center, Padding = new Thickness(5), Margin = new Thickness(0, 5, 0, 0), VerticalAlignment = VerticalAlignment.Center };
			delete.Click += (o, e) =>
			{
				if (productCount == 1)
				{
					DatabaseConnection.DeleteOrder(order.OrderId, product.ProductArticleNumber);
					order = null;
					productCount--;
					this.Close();
				}
				else
				{
					DatabaseConnection.DeleteOrderProduct(order.OrderId, product.ProductArticleNumber);
					LoadCart();
				}
			};

			prod.Children.Add(image);
			prod.Children.Add(inf);
			prod.Children.Add(delete);

			productGrid.Children.Add(prod);
			Grid.SetRow(prod, row);
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (order != null)
			{
				DatabaseConnection.ChangePickPoint(order.OrderId, pickPointTB.Text);
			}
		}

		private void FinalCost()
		{
			finalCostTB.Text = Math.Round(discountCost, 2).ToString() + "₽";
			try
			{
				finalDiscountTB.Text = Math.Round(100 - 100 * discountCost / fullCost).ToString() + "%";
			}
			catch { }
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var app = new Word.Application();
			Word.Document document = app.Documents.Add();

			Word.Paragraph date = document.Paragraphs.Add();
			date.Range.Text = "Дата заказа: " + DateTime.Now.ToString("D");
			date.Range.InsertParagraphAfter();

			Word.Paragraph Id = document.Paragraphs.Add();
			Id.Range.Text = "Номер заказа: " + order.OrderId;
			Id.Range.InsertParagraphAfter();

			Word.Paragraph orderProducts = document.Paragraphs.Add();
			orderProducts.Range.Text = "Состав заказа:";
			orderProducts.Range.InsertParagraphAfter();
			List<Product> products = DatabaseConnection.GetCart(order.OrderId);
			foreach (Product product in products)
			{
				Word.Paragraph p = document.Paragraphs.Add();
				p.Range.Text = "  - " + product.ProductName;
				p.Range.InsertParagraphAfter();
			}

			Word.Paragraph cost = document.Paragraphs.Add();
			cost.Range.Text = "Сумма заказа: " + Math.Round(discountCost, 2).ToString() + "₽";
			cost.Range.InsertParagraphAfter();

			Word.Paragraph discount = document.Paragraphs.Add();
			discount.Range.Text = "Скидка: " + Math.Round(100 - 100 * discountCost / fullCost).ToString() + "%";
			cost.Range.InsertParagraphAfter();

			Word.Paragraph pickPoint = document.Paragraphs.Add();
			pickPoint.Range.Text = "Пункт выдачи: " + pickPointTB.Text;
			pickPoint.Range.InsertParagraphAfter();

			Word.Paragraph deliveryDate = document.Paragraphs.Add();
			deliveryDate.Range.Text = "Дата доставки: " + order.OrderDeliveryDate.ToString("D");
			deliveryDate.Range.InsertParagraphAfter();

			Word.Paragraph codeText = document.Paragraphs.Add();
			codeText.Range.Text = "Код получения:";
			codeText.Range.InsertParagraphAfter();

			Random r = new Random();
			Word.Paragraph code = document.Paragraphs.Add();
			code.Range.Text = r.Next(100, 1000).ToString();
			code.Range.Font.Bold = 1;
			code.Range.Font.Size = 14;

			app.Visible = true;
			document.SaveAs2(@"D:\Талон.pdf", Word.WdExportFormat.wdExportFormatPDF);
		}
	}
}
