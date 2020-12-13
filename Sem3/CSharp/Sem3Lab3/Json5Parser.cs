using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Sem3Lab3
{
	/// <summary>
	/// Содержит статические методы для анализа JSON5 текста.
	/// Экземпляры класса могут строить строковые KV деревья.
	/// </summary>
	/// <remarks>
	/// Строковое KV дерево - список пар "ключ-значение",
	/// где ключ - строка, а значение - строка или такой же список.
	/// </remarks>
	public class Json5Parser
	{
		public readonly string text;
		private int lastIndex;

		/// <summary>
		/// Создаёт новый экземпляр <see cref="Json5Parser"/> на основе текста
		/// в формате JSON5. Может построить строковое KV дерево.
		/// </summary>
		/// <remarks>
		/// Строковое KV дерево - список пар "ключ-значение",
		/// где ключ - строка, а значение - строка или такой же список.
		/// </remarks>
		/// <param name="text">Текст в формате JSON5.</param>
		public Json5Parser (string text)
		{
			this.text = RemoveComments (text);
			GetString (); // получить первый символ разметки
			if (this.text[lastIndex - 1] != '{')
			{
				lastIndex = 0;
				this.text = "{" + this.text + "}";
			}
		}

		/// <summary>
		/// Определяет, является ли символ в тексте экранированным.
		/// </summary>
		/// <param name="text">Исходный текст.</param>
		/// <param name="index">Позиция символа, который нужно проверить.</param>
		/// <returns>True - символ экранирован, false - не экранирован.</returns>
		public static bool IsEscape (string text, int index)
		{
			bool isEscape = false;
			index--;
			while ((index >= 0) && (text[index] == '\\'))
			{
				isEscape = !isEscape;
				index--;
			}
			return isEscape;
		}

		/// <summary>
		/// Удаляет из текста все <c>//однострочные</c>
		/// и <c>/*многострочные*/</c> комментарии.
		/// </summary>
		/// <remarks>
		/// Знаки комментариев внутри строк <c>"строка"</c> и <c>'строка'</c>
		/// комментариями не считаются.
		/// </remarks>
		/// <param name="text">Исходный текст.</param>
		/// <returns>Текст без комментариев.</returns>
		public static string RemoveComments (string text)
		{
			// Индекс в 'str'
			int index = 0;
			// Смещение 'sb' относительно 'str', равное
			// отрицательному числу удалённых символов из 'sb'
			int shift = 0;
			StringBuilder sb = new StringBuilder (text);
			char[] chars = { '\"', '\'', '/' };
			while ((index = text.IndexOfAny (chars, index)) >= 0)
			{
				int lastIndex = index;
				char ch = text[index];
				switch (ch)
				{
					case '\"':
						// no break
					case '\'':
						do
						{
							index = text.IndexOf (ch, index + 1);
							if (index < 0)
							{
								throw new InvalidDataException (
									$"Не найдено окончание строки, начало строки: {lastIndex}."
								);
							}
						}
						while (IsEscape (text, index));
						index++;
						break;
					case '/':
						ch = text[index + 1];
						int length = 1;
						if (ch == '/')
						{
							int end = text.IndexOf ('\n', index + 2);
							length = end - index; // w/o '\n'
						}
						if (ch == '*')
						{
							int end = text.IndexOf ("*/", index + 2);
							length = end - index + 2; // with "*/"
						}
						if (length < 0)
						{
							throw new InvalidDataException (
								$"Не найдено окончание комментария, начало комментария: {lastIndex}."
							);
						}
						sb.Remove (index + shift, length);
						shift -= length;
						index += length;
						break;
				}
			}
			return sb.ToString ();
		}

		/// <summary>
		/// Выполняет подстановку экранированных символов в строке.
		/// </summary>
		/// <param name="str">Исходная строка.</param>
		/// <returns>Строка с выполненной заменой экранированных символов.</returns>
		public static string ExpandEscape (string str)
		{
			// Индекс в 'str'
			int index = 0;
			// Смещение 'sb' относительно 'str', равное
			// отрицательному числу удалённых символов из 'sb'
			int shift = 0;
			StringBuilder sb = new StringBuilder (str);
			while ((index = str.IndexOf ('\\', index)) >= 0)
			{
				char ch = str[index + 1];
				if ((ch == 'u') || (ch == 'U'))
				{
					char[] chars = str.ToCharArray (index + 2, 4);
					int result = 0;
					for (int i = 0; i < 4; i++)
					{
						result <<= 4;
						result += (chars[i] < 'A') ? chars[i] - '0' :
							(chars[i] < 'a') ? chars[i] - 'A' + 10 : chars[i] - 'a' + 10;
					}
					sb[index + shift] = (char)result;
					sb.Remove (index + 1 + shift, 5);
					shift -= 5;
					index += 6;
				}
				else
				{
					switch (ch)
					{
						case 'b':
							sb[index + shift] = '\b';
							break;
						case 't':
							sb[index + shift] = '\t';
							break;
						case 'n':
							sb[index + shift] = '\n';
							break;
						case 'f':
							sb[index + shift] = '\f';
							break;
						case 'r':
							sb[index + shift] = '\r';
							break;
						case '\"':
							sb[index + shift] = '\"';
							break;
						case '\'':
							sb[index + shift] = '\'';
							break;
						case '\\':
							break;
						case '\r':
							if (str[index + 2] == '\n')
							{
								sb.Remove (index + shift, 2);
								shift -= 2;
								index++;
								break;
							}
							else
							{
								goto case '\n';
							}
						case '\n':
							sb.Remove (index + shift, 1);
							shift--;
							break;
					}
					sb.Remove (index + 1 + shift, 1);
					shift--;
					index += 2;
				}
			}
			return sb.ToString ();
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
			GetString (); // перейти за символ '{'
			return GetObject ();
		}

		/// <summary>
		/// Читает строку с <c>lastIndex</c> до следующего символа разметки.
		/// Переменная <c>lastIndex</c> устанавливается ЗА найденым символом.
		/// </summary>
		/// <remarks>
		/// Символы разметки : <c>" ' : , { } [ ]</c> .
		/// </remarks>
		/// <returns>
		/// Если строка была в кавычках - возвращает строку внутри кавычек,
		/// иначе с <c>lastIndex</c> ДО символа разметки
		/// без начальных и конечных символов-разделителей.
		/// </returns>
		private string GetString ()
		{
			char[] chars = { '\"', '\'', ':', ',', '{', '}', '[', ']' };
			int index = lastIndex;
			if ((index = text.IndexOfAny (chars, index)) >= 0)
			{
				string result;
				char ch = text[index];
				if (ch == '\"' || ch == '\'')
				{
					int end = index;
					do
					{
						end = text.IndexOf (ch, end + 1);
					}
					while (IsEscape (text, end));
					result = text.Substring (index + 1, end - index - 1);
					result = ExpandEscape (result);
					lastIndex = text.IndexOfAny (chars, end + 1) + 1;
				}
				else
				{
					result = text.Substring (lastIndex, index - lastIndex).Trim ();
					lastIndex = index + 1;
				}
				return result;
			}
			else
			{
				throw new InvalidDataException (
					$"Неожиданное окончание файла, последнее место удачной операции: {lastIndex}."
				);
			}
		}

		/// <summary>
		/// Считывает значение для ключа - строку, объект или массив.
		/// Переменная <c>lastIndex</c> устанавливается ЗА символом разметки после значения.
		/// </summary>
		/// <remarks>
		/// Символы разметки : <c>" ' : , { } [ ]</c> .
		/// </remarks>
		/// <returns>Значение для ключа.</returns>
		private object GetValue ()
		{
			string value = GetString ();
			switch (text[lastIndex - 1])
			{
				case ',':
				// no break
				case '}':
				// no break
				case ']':
					return value;
				case '{':
					var obj = GetObject ();
					GetString (); // перейти к следующему символу разметки
					return obj;
				case '[':
					var array = GetArray ();
					GetString (); // перейти к следующему символу разметки
					return array;
				default:
					return "bad_value_end";
			}
		}

		/// <summary>
		/// Считывает объект.
		/// Переменная <c>lastIndex</c> устанавливается ЗА символом конца объекта '<c>}</c>'.
		/// </summary>
		/// <remarks>
		/// Переменная <c>lastIndex</c> должна быть ЗА символом начала объекта '<c>{</c>'.
		/// </remarks>
		/// <returns>Объект, являющийся значением для ключа.</returns>
		private List<KeyValuePair<string, object>> GetObject ()
		{
			List<KeyValuePair<string, object>> list = new List<KeyValuePair<string, object>> ();
			while (true)
			{
				string key = GetString ();
				if (text[lastIndex - 1] == ':')
				{
					object value = GetValue ();
					list.Add (new KeyValuePair<string, object> (key, value));
					switch (text[lastIndex - 1])
					{
						case ',':
							break;
						case '}':
							return list;
						default:
							throw new InvalidDataException (
								$"Неожиданный символ '{text[lastIndex - 1]}' при чтении значения ключа \"{key}\", " +
								$"последнее место удачной операции: {lastIndex}."
							);
					}
				}
				else if (text[lastIndex - 1] == '}')
				{
					return list;
				}
				else
				{
					throw new InvalidDataException (
						$"Неожиданный символ '{text[lastIndex - 1]}' при чтении ключа объекта, " +
						$"последнее место удачной операции: {lastIndex}."
					);
				}
			}
		}

		/// <summary>
		/// Считывает массив.
		/// Переменная <c>lastIndex</c> устанавливается ЗА символом конца массива '<c>]</c>'.
		/// </summary>
		/// <remarks>
		/// Переменная <c>lastIndex</c> должна быть ЗА символом начала массива '<c>[</c>'.
		/// </remarks>
		/// <returns>Массив, являющийся значением для ключа.</returns>
		private List<KeyValuePair<string, object>> GetArray ()
		{
			List<KeyValuePair<string, object>> list = new List<KeyValuePair<string, object>> ();
			string key = "element";
			while (true)
			{
				object value = GetValue ();
				switch (text[lastIndex - 1])
				{
					case ',':
						list.Add (new KeyValuePair<string, object> (key, value));
						break;
					case ']':
						if (!(value is string s) || s != "")
						{
							list.Add (new KeyValuePair<string, object> (key, value));
						}
						return list;
					default:
						throw new InvalidDataException (
							$"Неожиданный символ '{text[lastIndex - 1]}' при чтении эдемента массива, " +
							$"последнее место удачной операции: {lastIndex}."
						);
				}
			}
		}
	}
}
