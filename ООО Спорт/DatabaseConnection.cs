using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Controls;
using ООО_Спорт.Context;
using ООО_Спорт.Models;

namespace ООО_Спорт
{
	internal class DatabaseConnection
	{
		public static int Login(string login, string password)
		{
			using (EmployeeContext db = new EmployeeContext())
			{
				foreach (User u in db.User)
				{
					if (u.UserLogin == login)
					{
						if (u.UserPassword == password)
						{
							Products.user = u;
							return 0;
						}
						else
						{
							return 1;
						}
					}
				}
			}
			return 2;
		}

		public static List<Product> GetProducts()
		{
			using (EmployeeContext db = new EmployeeContext())
			{
				return db.Product.ToList();
			}
		}

		public static List<Product> GetFilteredProducts(int code)
		{
			List<Product> products = new List<Product>();
			if (code == 1)
			{
				using (EmployeeContext db = new EmployeeContext())
				{
					foreach (Product product in db.Product)
					{
						if (product.ProductDiscountAmount < 10)
						{
							products.Add(product);
						}
					}
				}
				return products;
			}
			else if (code == 2)
			{
				using (EmployeeContext db = new EmployeeContext())
				{
					foreach (Product product in db.Product)
					{
						if ((product.ProductDiscountAmount >=10) && (product.ProductDiscountAmount < 15))
						{
							products.Add(product);
						}
					}
				}
				return products;
			}
			else
			{
				using (EmployeeContext db = new EmployeeContext())
				{
					foreach (Product product in db.Product)
					{
						if (product.ProductDiscountAmount >= 15)
						{
							products.Add(product);
						}
					}
				}
				return products;
			}
		}

		public static void CreateProduct(Product product)
		{
			using (EmployeeContext db = new EmployeeContext())
			{
				db.Product.Add(product);
				db.SaveChanges();
			}
		}

		public static void EditProduct(string article, string name, string description, string category, string image, string manufacture, decimal cost, byte discount, int quantity)
		{
			using (EmployeeContext db = new EmployeeContext())
			{
				Product product = db.Product.Find(article);
				product.ProductName = name;
				product.ProductDescription = description;
				product.ProductCategory = category;
				product.ProductPhoto = image;
				product.ProductManufacturer = manufacture;
				product.ProductDiscountAmount = discount;
				product.ProductCost = cost;
				product.ProductQuantityInStock = quantity;
				db.SaveChanges();
			}
		}

		public static void DeleteProduct(string article)
		{
			using (EmployeeContext db = new EmployeeContext())
			{
				db.Product.Remove(db.Product.Find(article));
				db.SaveChanges();
			}
		}

		public static Order CreateOrder(string firstProductArticle)
		{
			Order order = new Order() { OrderPickupPoint = "1", OrderStatus = "Новый", OrderDeliveryDate = DateTime.Now.AddDays(3) };
			using (EmployeeContext db = new EmployeeContext())
			{
				db.Order.Add(order);
				OrderProduct op = new OrderProduct() { OrderId = order.OrderId, Order = order, ProductArticleNumber = firstProductArticle, ProductArticleNumberNavigation = db.Product.Find(firstProductArticle) };
				db.OrderProduct.Add(op);
				db.SaveChanges();
				return order;
			}
		}

		public static void AddProductToOrder(string productArticle, int orderId)
		{
			using (EmployeeContext db = new EmployeeContext())
			{
				if ((from x in db.OrderProduct where x.OrderId == orderId && x.ProductArticleNumber == productArticle select x).Count() == 0)
				{
					OrderProduct op = new OrderProduct() { OrderId = orderId, Order = db.Order.Find(orderId), ProductArticleNumber = productArticle, ProductArticleNumberNavigation = db.Product.Find(productArticle) };
					db.OrderProduct.Add(op);
					db.SaveChanges();
				}
			}
		}

		public static List<Product> GetCart(int orderId)
		{
			List<Product> products = new List<Product>();
			using (EmployeeContext db = new EmployeeContext())
			{
				foreach (OrderProduct op in from x in db.OrderProduct where x.OrderId == orderId select x)
				{
					products.Add(db.Product.Find(op.ProductArticleNumber));
				}
			}
			return products;
		}

		public static void ChangePickPoint(int orderId, string pickPoint)
		{
			using (EmployeeContext db = new EmployeeContext())
			{
				db.Order.Find(orderId).OrderPickupPoint = pickPoint;
				db.SaveChanges();
			}
		}

		public static Order GetCurrentOrder(int orderId)
		{
			using (EmployeeContext db = new EmployeeContext())
			{
				return db.Order.Find(orderId);
			}
		}

		public static void DeleteOrderProduct(int orderId, string productArticle)
		{
			using (EmployeeContext db = new EmployeeContext())
			{
				OrderProduct op = (from x in db.OrderProduct where x.OrderId == orderId && x.ProductArticleNumber == productArticle select x).FirstOrDefault();
				db.OrderProduct.Remove(op);
				db.SaveChanges();
			}
		}

		public static void DeleteOrder(int orderId, string productArticle)
		{
			DeleteOrderProduct(orderId, productArticle);
			using (EmployeeContext db = new EmployeeContext())
			{
				db.Order.Remove(db.Order.Find(orderId));
				db.SaveChanges();
			}
		}
	}
}
