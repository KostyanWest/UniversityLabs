namespace Sem3Lab4.ServiceLayer
{
	public interface IXmlGeneratorService
	{
		void StartGenerateXml (string rootElementName);
		void OpenElement (string elementName);
		void CloseElement ();
		void AddElement (string elementName, object element);
		string EndGenerateXml ();
		void DeclareType (string elementName, System.Type elementType);
		string GenerateXsd ();
	}
}
