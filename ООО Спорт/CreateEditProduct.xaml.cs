using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using ООО_Спорт.Models;

namespace ООО_Спорт
{
	/// <summary>
	/// Логика взаимодействия для CreateEditProduct.xaml
	/// </summary>
	public partial class CreateEditProduct : Window
	{
		public static bool isCreate = true;
		string image;
		public static Product product;
		public CreateEditProduct()
		{
			InitializeComponent();
		}

		private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (Visibility == Visibility.Visible)
			{
				if (isCreate)
				{
					Title = "Создание товара";
					ArticleTB.IsEnabled = true;
					DeleteButton.Visibility = Visibility.Hidden;
				}
				else
				{
					Title = "Изменение товара";
					ArticleTB.IsEnabled = false;
					DeleteButton.Visibility = Visibility.Visible;

					ArticleTB.Text = product.ProductArticleNumber;
					NameTB.Text = product.ProductName;
					if (product.ProductCategory == "Одежда")
					{
						CategoryCB.SelectedIndex = 1;
					}
					else
					{
						CategoryCB.SelectedIndex = 0;
					}
					QuantityTB.Text = product.ProductQuantityInStock.ToString();
					DiscountTB.Text = product.ProductDiscountAmount.ToString();
					PriceTB.Text = product.ProductCost.ToString().Trim('0').Trim(',');
					if ((product.ProductPhoto != null) && (product.ProductPhoto != ""))
					{
						ImageButton.Content = product.ProductPhoto;
					}
					else
					{
						ImageButton.Content = "Выбрать изображение";
					}
					DescriptionTB.Text = product.ProductDescription;
					SupplierTB.Text = product.ProductManufacturer;
				}
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			if ((ArticleTB.Text != "") && (NameTB.Text != "") && (QuantityTB.Text != "") && (UnitTB.Text != "") && (QuantityInPackTB.Text != "") && (MinQuantityTB.Text != "") && (SupplierTB.Text != "") && (MaxDiscountTB.Text != "") && (DiscountTB.Text != "") && (PriceTB.Text != "") && (DescriptionTB.Text != ""))
			{
				decimal price = 0;
				if (decimal.TryParse(PriceTB.Text, out price))
				{
					if (price > 0)
					{
						if (!PriceTB.Text.Contains(",") || (PriceTB.Text.Split(',')[1].Length < 3))
						{
							int minQuantity = 0;
							if (int.TryParse(MinQuantityTB.Text, out minQuantity))
							{
								if (minQuantity >= 0)
								{
									int quantity = 0;
									if (int.TryParse(QuantityTB.Text, out quantity))
									{
										if (quantity >= 0)
										{
											int quantityInPack = 0;
											if (int.TryParse(QuantityInPackTB.Text, out quantityInPack))
											{
												if (quantityInPack > 0)
												{
													int maxDiscount = 0;
													if (int.TryParse(MaxDiscountTB.Text, out maxDiscount))
													{
														if (maxDiscount > 0)
														{
															byte discount = 0;
															if (byte.TryParse(DiscountTB.Text, out discount))
															{
																if (discount <= maxDiscount)
																{
																	if (isCreate)
																	{
																		Product product = new Product() { ProductArticleNumber = ArticleTB.Text, ProductName = NameTB.Text, ProductQuantityInStock = quantity, ProductManufacturer = SupplierTB.Text, ProductDiscountAmount = discount, ProductCost = price, ProductDescription = DescriptionTB.Text, ProductStatus = "Спортмастер" };
																		if (CategoryCB.SelectedIndex == 0)
																		{
																			product.ProductCategory = "Спортивный инвентарь";
																		}
																		else
																		{
																			product.ProductCategory = "Одежда";
																		}
																		if (ImageButton.Content.ToString() != "Выбрать изображение")
																		{
																			product.ProductPhoto = product.ProductArticleNumber + "." + image.Split('.').Last();
																			File.Copy(image, Environment.CurrentDirectory.Remove(Environment.CurrentDirectory.Length - 10) + @"/Resources/" + product.ProductPhoto);
																		}
																		DatabaseConnection.CreateProduct(product);
																		Close();
																	}
																	else
																	{
																		string category;
																		if (CategoryCB.SelectedIndex == 0)
																		{
																			category = "Спортивный инвентарь";
																		}
																		else
																		{
																			category = "Одежда";
																		}
																		string thisImage = "";
																		if (ImageButton.Content.ToString() != "Выбрать изображение")
																		{
																			thisImage = ArticleTB.Text + "." + image.Split('.').Last();
																			File.Copy(image, Environment.CurrentDirectory.Remove(Environment.CurrentDirectory.Length - 10) + @"/Resources/" + thisImage, true);
																		}
																		DatabaseConnection.EditProduct(ArticleTB.Text, NameTB.Text, DescriptionTB.Text, category, thisImage, SupplierTB.Text, price, discount, quantity);
																		Close();
																	}
																}
																else
																{
																	MessageBox.Show("Скидка не может быть больше максимальной скидки");
																}
															}
															else
															{
																MessageBox.Show("Скидка должна быть числом");
															}
														}
														else
														{
															MessageBox.Show("Максимальная скидка не может быть отрицательной");
														}
													}
													else
													{
														MessageBox.Show("Максимальная скидка должна быть числом");
													}
												}
												else
												{
													MessageBox.Show("Количество товара в упаковне не может быть отрицательным или раавным 0");
												}
											}
											else
											{
												MessageBox.Show("Количество товара в упаковке должно быть числом");
											}
										}
										else
										{
											MessageBox.Show("Количество товара не может быть отрицательным");
										}
									}
									else
									{
										MessageBox.Show("Количество товара должно быть числом");
									}
								}
								else
								{
									MessageBox.Show("Минимальное количество не может быть отрицательным");
								}
							}
							else
							{
								MessageBox.Show("Минимальное количество должно быть числом");
							}
						}
						else
						{
							MessageBox.Show("В поле Цена слишком много цифр после запятой");
						}
					}
					else
					{
						MessageBox.Show("Цена не может быть отрицательной или равной 0");
					}
				}
				else
				{
					MessageBox.Show("Цена должна быть числом");
				}
			}
			else
			{
				MessageBox.Show("Заполните все поля");
			}
		}

		private void ImageButton_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog() { Filter = "Картинки(*.jpg;*.png)|*.JPG;*.PNG", CheckFileExists = true, AddExtension = true, Title = "Выберите изображение", Multiselect = false };
			ofd.ShowDialog();
			if (ofd.FileName != "")
			{
				ImageButton.Content = ofd.FileName.Split('\\').Last();
				image = ofd.FileName;
			}
		}

		private void DeleteButton_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
