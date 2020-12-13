using System;

namespace Sem3Lab4.Models
{
	public class ProductModel
	{
		public int ProductModelId { get; set; }
		public string Name { get; set; }
		public string CatalogDescription { get; set; }
		public Guid Rowguid { get; set; }
		public DateTime ModifiedDate { get; set; }
	}
}
