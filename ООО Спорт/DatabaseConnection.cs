using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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

		
	}
}
