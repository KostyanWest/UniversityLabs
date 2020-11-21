using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Sem3Lab3
{
	/// <summary>
	/// Класс для построения строковых KV деревьев на основе XML текста.
	/// </summary>
	/// <remarks>
	/// Строковое KV дерево - список пар "ключ-значение",
	/// где ключ - строка, а значение - строка или такой же список.
	/// </remarks>
	public class XmlParser
	{
		public readonly string text;
		private int lastIndex;
		private Dictionary<string, string> entities;

		/// <summary>
		/// Создаёт новый экземпляр <see cref="XmlParser"/> на основе текста
		/// в формате XML. Может построить строковое KV дерево.
		/// </summary>
		/// <remarks>
		/// Строковое KV дерево - список пар "ключ-значение",
		/// где ключ - строка, а значение - строка или такой же список.
		/// </remarks>
		/// <param name="text">Текст в формате XML.</param>
		public XmlParser (string text)
		{
			this.text = text;
			entities = new Dictionary<string, string> ();
			entities["lt"] = "<";
			entities["gt"] = ">";
			entities["amp"] = "&";
			entities["apos"] = "\'";
			entities["quot"] = "\"";
		}

		/// <summary>
		/// Создаёт строковое KV дерево
		/// на основе хранящегося внутри текста.
		/// </summary>
		/// <remarks>
		/// Строковое KV дерево - список пар "ключ-значение",
		/// где ключ - строка, а значение - строка или такой же список.
		/// </remarks>
		/// <returns>Новое строковое KV дерево.</returns>
		public List<KeyValuePair<string, object>> CreateStringKVTree ()
		{
			lastIndex = 0;
			SkipDeclarations ();
			var element = GetElement ();
			return (List<KeyValuePair<string, object>>)(element.Value);
		}

		/// <summary>
		/// Пропускает <c>&lt;? пролог ?&gt; и &lt;! объявления &gt;</c>.
		/// Переменная <c>lastIndex</c> устанавливается НА открывающем тег
		/// символе '<c>&lt;</c>' корневого элемента.
		/// </summary>
		private void SkipDeclarations ()
		{
			int index = lastIndex;
			while ((index = text.IndexOf ('<', index)) >= 0)
			{
				char ch = text[index + 1];
				if ((ch != '!') && (ch != '?'))
				{
					lastIndex = index;
					return;
				}
				else
				{
					index += 2;
				}
			}
			throw new InvalidDataException (
				$"Не найден корневой элемент файла."
			);
		}

		/// <summary>
		/// Выполняет подстановку <c>&amp;ссылок;</c> на сущности и символы в строке.
		/// </summary>
		/// <param name="str">Исходная строка.</param>
		/// <returns>Строка с выполненной подстановкой ссылок.</returns>
		private string ExpandEntities (string str)
		{
			// Индекс в 'str'
			int index = 0;
			// Смещение 'sb' относительно 'str', равное
			// отрицательному числу удалённых символов из 'sb'
			int shift = 0;
			StringBuilder sb = new StringBuilder (str);
			while ((index = str.IndexOf ('&', index)) >= 0)
			{
				int end = str.IndexOf (';', index);
				int length = end - index + 1;
				if (end < 0)
				{
					throw new InvalidDataException (
						$"Не найден символ окончания ссылки ';', позиция символа '&': {index}."
					);
				}
				if (str[index + 1] == '#')
				{
					char ch = str[index + 2];
					int result = 0;
					if ((ch == 'x') || (ch == 'X'))
					{
						int arrLength = length - 4; // w/o '&' '#' 'x' ';'
						char[] chars = str.ToCharArray (index + 3, arrLength);
						for (int i = 0; i < arrLength; i++)
						{
							result <<= 4;
							result += (chars[i] < 'A') ? chars[i] - '0' :
								(chars[i] < 'a') ? chars[i] - 'A' + 10 : chars[i] - 'a' + 10;
						}
					}
					else
					{
						int arrLength = length - 3; // w/o '&' '#' ';'
						char[] chars = str.ToCharArray (index + 2, arrLength);
						for (int i = 0; i < arrLength; i++)
						{
							result *= 10;
							result += chars[i] - '0';
						}
					}
					sb[index + shift] = (char)result;
					sb.Remove (index + 1 + shift, length - 1); // w/o '&'
					shift -= length - 1;
					index += length;
				}
				else
				{
					if (entities.TryGetValue (str.Substring (index + 1, length), out string insert))
					{
						sb.Remove (index, length);
						sb.Insert (index, insert);
						shift += insert.Length - length;
						index += length;
					}
					else
					{
						throw new InvalidDataException (
							$"Не найдена сущность \"{str.Substring (index + 1, length)}\", " +
							$"место символа '&' {index}."
						);
					}
				}
			}
			return sb.ToString ();
		}

		/// <summary>
		/// Читает элемент с открывающим и закрывающими тегами включительно.
		/// Переменная <c>lastIndex</c> устанавливается НА закрывающем тег символе '<c>&gt;</c>'
		/// закрывающего тега элемента.
		/// </summary>
		/// <remarks>
		/// <para>Переменная <c>lastIndex</c> должна быть НА открывающем тег символе '<c>&lt;</c>'
		/// открывающего тега элемента.</para>
		/// Строковое KV дерево - список пар "ключ-значение",
		/// где ключ - строка, а значение - строка или такой же список.
		/// </remarks>
		/// <returns>
		/// Пара "ключ-значение", где ключ - строка, а значение - строковое KV дерево.
		/// </returns>
		private KeyValuePair<string, object> GetElement ()
		{
			lastIndex++; // перейти за '<'
			int endTag = GetEndTagIndex (out bool isSelfClosing);

			string name = GetElementName (endTag);
			TryGetAttributes (endTag, out List<KeyValuePair<string, object>> list);
			lastIndex = endTag;
			if (isSelfClosing)
			{
				if (list == null)
				{
					list = new List<KeyValuePair<string, object>> ();
				}
				return new KeyValuePair<string, object> (name, list);
			}

			int index;
			lastIndex++; // перейти за '>'
			StringBuilder content = new StringBuilder ();
			while ((index = text.IndexOf ('<', lastIndex)) >= 0)
			{
				if (list == null)
				{
					content.Append (text.Substring (lastIndex, index - lastIndex));
				}
				lastIndex = index;
				switch (text[lastIndex + 1])
				{
					case '/':
						lastIndex = GetEndTagIndex (out _);
						if (list != null)
						{
							return new KeyValuePair<string, object> (name, list);
						}
						else
						{
							return new KeyValuePair<string, object> (name, content.ToString ());
						}
					case '!':
						if (!TrySkipComment ())
						{
							if (TryExtractCdata (out string append) && (list == null))
							{
								content.Append (append);
							}
						}
						break;
					default:
						if (list == null)
						{
							list = new List<KeyValuePair<string, object>> ();
						}
						list.Add (GetElement ());
						lastIndex++; // перейти за '>'
						break;
				}
			}
				throw new InvalidDataException (
					$"Элемент \"{name}\" не имеет закрывающего тега, последне место удачной операции: {lastIndex}."
				);
		}

		/// <summary>
		/// Находит закрывающий тег символ '<c>&gt;</c>'.
		/// </summary>
		/// <param name="isSelfClosing">Показывает, является ли этот тег самозакрывающимся.</param>
		/// <returns>Индекс закрывающего тег символа '<c>&gt;</c>'.</returns>
		private int GetEndTagIndex (out bool isSelfClosing)
		{
			int endTag = text.IndexOf ('>', lastIndex);
			if (endTag < 0)
			{
				throw new InvalidDataException (
					$"Тег не имеет закрывающего символа '>', начало тега: {lastIndex}."
				);
			}
			isSelfClosing = text[endTag - 1] == '/';
			return endTag;
		}

		/// <summary>
		/// Читает имя элемента.
		/// Переменная <c>lastIndex</c> устанавливается ЗА именем элемента.
		/// </summary>
		/// <remarks>
		/// Переменная <c>lastIndex</c> должна быть ЗА открывающим тег символом '<c>&lt;</c>'
		/// открывающего тега элемента.
		/// </remarks>
		/// <param name="endTag">Индекс закрывающего тег символа '<c>&gt;</c>'.</param>
		/// <returns>Строка, содержащая имя элемента.</returns>
		private string GetElementName (int endTag)
		{
			char[] chars = { ' ', '\t', '\v', '\r', '\n', '\f', '/' };
			int index = text.IndexOfAny (chars, lastIndex, endTag - lastIndex);
			if (index < 0)
			{
				index = endTag;
			}
			string name = text.Substring (lastIndex, index - lastIndex);
			name = ExpandEntities (name);
			lastIndex = index;
			return name;
		}

		/// <summary>
		/// Читает имя элемента.
		/// Переменная <c>lastIndex</c> устанавливается ЗА значением последнего атрибута.
		/// </summary>
		/// <remarks>
		/// Переменная <c>lastIndex</c> должна быть ЗА именем элемнта,
		/// ПЕРЕД или НА первом символе первого атрибута.
		/// </remarks>
		/// <param name="endTag">Индекс закрывающего тег символа '<c>&gt;</c>'.</param>
		/// <param name="attributest">Список атрибутов тега в виде списка пар "строка-строка".</param>
		/// <returns>True - если атрибуты были прочитаны, false - если атрибуты не были найдены.</returns>
		private bool TryGetAttributes (int endTag, out List<KeyValuePair<string, object>> attributest)
		{
			attributest = null;
			char[] chars = { '\"', '\'' };
			int index = lastIndex;
			while ((index = text.IndexOf ('=', index, endTag - index)) >= 0)
			{
				try
				{
					string key = text.Substring (lastIndex, index - lastIndex).Trim ();
					key = ExpandEntities (key);
					int start = text.IndexOfAny (chars, index) + 1; // w/o '"'
					int end = text.IndexOf (text[start - 1], start);
					string value = text.Substring (start, end - start);
					value = ExpandEntities (value);
					if (attributest == null)
					{
						attributest = new List<KeyValuePair<string, object>> ();
					}
					attributest.Add (new KeyValuePair<string, object> (key, value));
					lastIndex = end + 1;
					index = lastIndex;
				}
				catch (ArgumentOutOfRangeException ex)
				{
					throw new InvalidDataException (
						$"Ошибка во время чтения атрибутов тега, последне место удачной операции: {lastIndex}.",
						ex
					);
				}
			}
			return attributest != null;
		}

		/// <summary>
		/// Пропускает комментарий.
		/// Переменная <c>lastIndex</c> устанавливается ЗА последовательностью "<c>--&gt;</c>".
		/// </summary>
		/// /// <remarks>
		/// Переменная <c>lastIndex</c> должна быть установлена
		/// НА начале последовательности "<c>&lt;!--</c>".
		/// </remarks>
		/// <returns>True - если комментарий был пропущен, false - если это был не комментарий.</returns>
		private bool TrySkipComment ()
		{
			try
			{
				if (text.Substring (lastIndex, 4) == "<!--")
				{
					int start = lastIndex + 4;
					int end = text.IndexOf ("-->", start);
					if (end >= 0)
					{
						lastIndex = end + 3;
						return true;
					}
					else
					{
						throw new InvalidDataException (
							$"Комментарий не имеет конца, начало комментария: {lastIndex}."
						);
					}
				}
			}
			catch (ArgumentOutOfRangeException) { }
			return false;
		}

		/// <summary>
		/// Извлекает содержимое сегмента CDATA.
		/// Переменная <c>lastIndex</c> устанавливается ЗА последовательностью "<c>]]&gt;</c>".
		/// </summary>
		/// /// <remarks>
		/// Переменная <c>lastIndex</c> должна быть установлена
		/// НА начале последовательности "<c>&lt;![CDATA[</c>".
		/// </remarks>
		/// <param name="cdata">Содержимое сегмента CDATA в случае успеха, иначе - null</param>
		/// <returns>
		/// True - если CDATA успешно извлечена,
		/// false - если это была не секция CDATA.
		/// </returns>
		private bool TryExtractCdata (out string cdata)
		{
			try
			{
				if (text.Substring (lastIndex, 9) == "<![CDATA[")
				{
					int start = lastIndex + 9;
					int end = text.IndexOf ("]]>", start);
					if (end >= 0)
					{
						lastIndex = end + 3;
						cdata = text.Substring (start, end - start);
						return true;
					}
					else
					{
						throw new InvalidDataException (
							$"Раздел CDATA не имеен конца, начало раздела: {lastIndex}."
						);
					}
				}
			}
			catch (ArgumentOutOfRangeException) { }
			cdata = "";
			return false;
		}
	}
}
