using System;
using System.IO;
using System.Text;
using Sem3Lab4.DataAccess;
using Sem3Lab4.Models;
using Sem3Lab4.ApplicationInsights;

namespace Sem3Lab4.ServiceLayer
{
	public class DataTransferSettings
	{
		public string xmlPath;
		public string xsdPath;
		public int selectCount;
		public AccessorSettings accessorSettings;
		public AccessorSettings reporterSettings;
		public XmlGeneratorSettings xmlGeneratorSettings;

		public DataTransferSettings (
			string xmlPath,
			string xsdPath,
			int selectCount,
			AccessorSettings accessorSettings,
			AccessorSettings reporterSettings,
			XmlGeneratorSettings xmlGeneratorSettings
			)
		{
			this.xmlPath = xmlPath;
			this.xsdPath = xsdPath;
			this.selectCount = selectCount;
			this.accessorSettings = accessorSettings;
			this.reporterSettings = reporterSettings;
			this.xmlGeneratorSettings = xmlGeneratorSettings;
		}
	}

	public class DataTransfer
	{
		private string xmlPath;
		private string xsdPath;
		public int selectCount;
		private AccessorSettings accessorSettings;
		private AccessorSettings reporterSettings;
		private XmlGeneratorSettings xmlGeneratorSettings;

		public DataTransfer (DataTransferSettings settings)
		{
			xmlPath = settings.xmlPath;
			xsdPath = settings.xsdPath;
			selectCount = settings.selectCount;
			accessorSettings = settings.accessorSettings;
			reporterSettings = settings.reporterSettings;
			xmlGeneratorSettings = settings.xmlGeneratorSettings;
		}

		public void Transfer ()
		{
			try
			{
				Transfer5Tables ();
			}
			catch (Exception ex)
			{
				new Reporter (reporterSettings).Report (ex);
			}
		}

		private void Transfer5Tables ()
		{
			using (Accessor accessor = new Accessor (accessorSettings))
			{
				GetTableParams tableParams = new GetTableParams () {
					ProcedureName = "uspGetProducts",
					Count = selectCount,
				};
				var products = accessor.GetTable<Product> (tableParams);

				tableParams.ProcedureName = "uspGetProductCategories";
				var productCategories = accessor.GetTable<ProductCategory> (tableParams);

				tableParams.ProcedureName = "uspGetProductModels";
				var productModels = accessor.GetTable<ProductModel> (tableParams);

				tableParams.ProcedureName = "uspGetProductModelProductDescriptions";
				var productModelProductDescriptions = accessor.GetTable<ProductModelProductDescription> (tableParams);

				tableParams.ProcedureName = "uspGetProductDescriptions";
				var productDescriptions = accessor.GetTable<ProductDescription> (tableParams);

				accessor.Dispose ();

				XmlGenerator generator = new XmlGenerator (xmlGeneratorSettings);
				generator.StartGenerateXml ("Data");

				generator.OpenElement ("Products");
				generator.DeclareType ("Product", typeof (Product));
				foreach (var product in products)
				{
					generator.AddElement ("Product", product);
				}
				generator.CloseElement ();

				generator.OpenElement ("ProductCategories");
				generator.DeclareType ("ProductCategory", typeof (ProductCategory));
				foreach (var productCategory in productCategories)
				{
					generator.AddElement ("ProductCategory", productCategory);
				}
				generator.CloseElement ();

				generator.OpenElement ("ProductModels");
				generator.DeclareType ("ProductModel", typeof (ProductModel));
				foreach (var productModel in productModels)
				{
					generator.AddElement ("ProductModel", productModel);
				}
				generator.CloseElement ();

				generator.OpenElement ("ProductModelProductDescriptions");
				generator.DeclareType ("ProductModelProductDescription", typeof (ProductModelProductDescription));
				foreach (var productModelProductDescription in productModelProductDescriptions)
				{
					generator.AddElement ("ProductModelProductDescription", productModelProductDescription);
				}
				generator.CloseElement ();

				generator.OpenElement ("ProductDescriptions");
				generator.DeclareType ("ProductDescription", typeof (ProductDescription));
				foreach (var productDescription in productDescriptions)
				{
					generator.AddElement ("ProductDescription", productDescription);
				}
				generator.CloseElement ();

				using (StreamWriter writer = new StreamWriter (xmlPath, false, Encoding.UTF8))
				{
					writer.Write (generator.EndGenerateXml ());
				}

				using (StreamWriter writer = new StreamWriter (xsdPath, false, Encoding.UTF8))
				{
					writer.Write (generator.GenerateXsd ());
				}
			}
		}
	}
}
