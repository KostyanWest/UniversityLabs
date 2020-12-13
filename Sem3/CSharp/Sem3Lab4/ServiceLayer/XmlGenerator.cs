using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Sem3Lab4.ServiceLayer
{
	public class XmlGeneratorSettings
	{
		public readonly string xmlHeader;
		public readonly string schemaLink;

		public XmlGeneratorSettings (string xmlHeader, string schemaLink)
		{
			this.xmlHeader = xmlHeader;
			this.schemaLink = schemaLink;
		}
	}

	public class XmlGenerator : IXmlGeneratorService
	{
		private StringBuilder xmlText;
		private Stack<string> xmlParents;
		private StringBuilder xsdText;
		private Stack<string> xsdParents;

		private string xmlHeader = "<?xml version=\"1.0\" encoding=\"UTF - 8\" standalone=\"yes\"?>\n";
		private string schemaLink = "https://docs.microsoft.com/ru-ru/dotnet/csharp/language-reference/builtin-types/built-in-types";

		public XmlGenerator (XmlGeneratorSettings settings)
		{
			xmlHeader = settings.xmlHeader;
			schemaLink = settings.schemaLink;
		}

		public void StartGenerateXml (string rootElement)
		{
			xmlText = new StringBuilder (xmlHeader);
			xmlParents = new Stack<string> ();
			xmlParents.Push ("");
			xsdText = new StringBuilder (xmlHeader);
			xsdParents = new Stack<string> ();
			xsdParents.Push ("");
			xsdText.Append ($"xsd:schema xmlns:xsd=\"{schemaLink}\">\n");
			xsdParents.Push ("schema");
			xsdParents.Push ("\t");
			OpenElement (rootElement);
		}

		public void OpenElement (string elementName)
		{
			OpenElementXml (elementName);
			OpenElementXsd (elementName);
		}

		private void OpenElementXml (string elementName)
		{
			string tabShift = xmlParents.Peek ();
			xmlText.Append ($"{tabShift}<{elementName}>\n");
			xmlParents.Push (elementName);
			xmlParents.Push (tabShift + "\t");
		}

		private void OpenElementXsd (string elementName)
		{
			string tabShift = xsdParents.Peek ();
			xsdText.Append ($"{tabShift}<xsd:all name=\"{elementName}\">\n");
			xsdParents.Push ("all");
			xsdParents.Push (tabShift + "\t");
		}

		public void CloseElement ()
		{
			CloseElementXml ();
			CloseElementXsd ();
		}

		private void CloseElementXml ()
		{
			xmlParents.Pop ();
			string elementName = xmlParents.Pop ();
			string tabShift = xmlParents.Peek ();
			xmlText.Append ($"{tabShift}</{elementName}>\n");
		}

		private void CloseElementXsd ()
		{
			xsdParents.Pop ();
			string elementName = xsdParents.Pop ();
			string tabShift = xsdParents.Peek ();
			xsdText.Append ($"{tabShift}</xsd:{elementName}>\n");
		}

		public void AddElement (string elementName, object element)
		{
			OpenElementXml (elementName);
			string tabShift = xmlParents.Peek ();
			foreach (PropertyInfo property in element.GetType ().GetProperties ())
			{
				xmlText.Append ($"{tabShift}<{property.Name}>{property.GetValue (element) ?? "null"}</{property.Name}>\n");
			}
			CloseElementXml ();
		}

		public void DeclareType (string elementName, Type elementType)
		{
			OpenElementXsd (elementName);
			string tabShift = xsdParents.Peek ();
			foreach (PropertyInfo property in elementType.GetProperties ())
			{
				xsdText.Append ($"{tabShift}<xsd:element name=\"{property.Name}\" type=\"xsd:{property.PropertyType}\">\n");
			}
			CloseElementXsd ();
		}

		public string EndGenerateXml ()
		{
			while (xmlParents.Count > 1)
			{
				CloseElementXml ();
			}
			return xmlText.ToString ();
		}

		public string GenerateXsd ()
		{
			while (xsdParents.Count > 1)
			{
				CloseElementXsd ();
			}
			return xsdText.ToString ();
		}
	}
}
