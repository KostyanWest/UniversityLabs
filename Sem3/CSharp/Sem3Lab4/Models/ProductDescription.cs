using System;

namespace Sem3Lab4.Models
{
	public class ProductDescription
	{
		public int ProductDescriptionId { get; set; }
		public string Description { get; set; }
		public Guid Rowguid { get; set; }
		public DateTime ModifiedDate { get; set; }
	}
}
