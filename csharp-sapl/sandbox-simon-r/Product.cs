using System;

namespace playground
{
	public class Product
	{
		public string Name { get; set; }
		public DateTime ExpiryDate { get; set; }
		public int Price { get; set; }
		public string[] Sizes { get; set; }

		public Product()
		{

		}
	}
}