using System;

namespace Sem3Lab4.Models
{
	public class ProductCategory
	{
		public int ProductCategoryId { get; set; }
		public int? ParentProductCategoryId { get; set; }
		public string Name { get; set; }
		public Guid Rowguid { get; set; }
		public DateTime ModifiedDate { get; set; }
	}
}
