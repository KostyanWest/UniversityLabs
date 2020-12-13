using System;

namespace Sem3Lab4.Models
{
	public class ProductModelProductDescription
	{
		public int ProductModelId { get; set; }
		public int ProductDescriptionId { get; set; }
		public string Culture { get; set; }
		public Guid Rowguid { get; set; }
		public DateTime ModifiedDate { get; set; }
	}
}
