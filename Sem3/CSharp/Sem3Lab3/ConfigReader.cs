using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sem3Lab3
{
	/// <summary>
	/// Статический класс для создания экземпляров классов, используя конфигурационные файлы.
	/// </summary>
	public static class ConfigReader
	{
		/// <summary>
		/// Создаёт экземпляр класса, используя первый подходящий конфигурационный файл
		/// из найденых по условиям поиска.
		/// </summary>
		/// <param name="directory">Путь директории, в которой нужно искать файлы.</param>
		/// <param name="filePattern">Шаблон имени файла, по которому будут отбираться файлы.</param>
		/// <param name="log">
		/// Метод, предназначеный для ведения лога. Оставьте <c>null</c>, чтобы не вести лог.
		/// </param>
		/// <returns>Новый экземпляр класса T.</returns>
		public static T GetOptions<T> (string directory, string filePattern, Action<string> log)
		{
			foreach (string file in Directory.GetFiles (directory, filePattern))
			{
				List<KeyValuePair<string, object>> settings;
				string extension = Path.GetExtension (file).ToLower ();
				switch (extension)
				{
					case ".json":
						using (StreamReader reader = new StreamReader (file, Encoding.UTF8, true))
						{
							try
							{
								Json5Parser parser = new Json5Parser (reader.ReadToEnd ());
								settings = parser.CreateStringKVTree ();
							}
							catch (Exception ex)
							{
								log?.Invoke ($"Json5Parser:\n{ex}");
								throw;
							}
						}
						break;
					case ".xml":
						using (StreamReader reader = new StreamReader (file, Encoding.UTF8, true))
						{
							try
							{
								XmlParser parser = new XmlParser (reader.ReadToEnd ());
								settings = parser.CreateStringKVTree ();
							}
							catch (Exception ex)
							{
								log?.Invoke ($"XmlParser:\n{ex}");
								throw;
							}
						}
						break;
					default:
						continue;
				}
				try
				{
					object result = ClassConstructor.ConstructFromStringKVTree (typeof (T), settings);
					return (T)result;
				}
				catch (Exception ex)
				{
					log?.Invoke ($"ClassConstructor:\n{ex}");
					throw;
				}
			}
			throw new FileNotFoundException ("Не найден подходящий конфигурационный файл.");
		}
	}
}
