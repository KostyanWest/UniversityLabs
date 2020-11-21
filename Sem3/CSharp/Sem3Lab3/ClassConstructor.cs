using System;
using System.Collections.Generic;
using System.Reflection;

namespace Sem3Lab3
{
	/// <summary>
	/// Статический класс для создания классов из строковых KV деревьев.
	/// </summary>
	/// <remarks>
	/// Строковое KV дерево - список пар "ключ-значение",
	/// где ключ - строка, а значение - строка или такой же список.
	/// </remarks>
	public static class ClassConstructor
	{
		/// <summary>
		/// Создаёт экземпляр класса из строкового KV дерева.
		/// Для создания выбирается самый подходящий конструктор класса,
		/// который определяется наибольшим соответствием ИМЁН переменных
		/// конструктора с КЛЮЧАМИ строкового KV дерева.
		/// Проверка типов происходит на этапе создания экземпляра класса,
		/// несоответствие типов приводит к генерации исключения.
		/// </summary>
		/// /// <remarks>
		/// Строковое KV дерево - список пар "ключ-значение",
		/// где ключ - строка, а значение - строка или такой же список.
		/// </remarks>
		/// <param name="type">Тип создаваемого класса.</param>
		/// <param name="stringTree">Строковое KV дерево.</param>
		/// <returns>Новый экземпляр класса.</returns>
		public static object ConstructFromStringKVTree (Type type, object stringTree)
		{
			if (TryConstructBasicType (type, stringTree, out object obj))
			{
				return obj;
			}
			else if (type.IsArray)
			{
				return ConstructArray (type.GetElementType (), stringTree);
			}
			else if (type.IsEnum)
			{
				return ConstructEnum (type, stringTree);
			}
			else
			{
				return ConstructCustomType (type, stringTree);
			}
		}

		/// <summary>
		/// Создаёт экземпляр класса базового типа - bool, int, float, string и др.
		/// </summary>
		/// <param name="type">Тип создаваемого класса.</param>
		/// <param name="stringTree">Строковое KV дерево.</param>
		/// <param name="obj">Новый экземпляр класса.</param>
		/// <returns>
		/// True - если экземпляр успешно создан, false - если <paramref name="type"/>
		/// не является базовым типом.
		/// </returns>
		private static bool TryConstructBasicType (Type type, object stringTree, out object obj)
		{
			bool result = true;
			if (type == typeof (string))
			{
				obj = (string)stringTree;
			}
			else if (type == typeof (bool))
			{
				obj = bool.Parse ((string)stringTree);
			}
			else if (type == typeof (byte))
			{
				obj = byte.Parse ((string)stringTree);
			}
			else if (type == typeof (sbyte))
			{
				obj = sbyte.Parse ((string)stringTree);
			}
			else if (type == typeof (short))
			{
				obj = short.Parse ((string)stringTree);
			}
			else if (type == typeof (int))
			{
				obj = int.Parse ((string)stringTree);
			}
			else if (type == typeof (long))
			{
				obj = long.Parse ((string)stringTree);
			}
			else if (type == typeof (char))
			{
				obj = char.Parse ((string)stringTree);
			}
			else if (type == typeof (float))
			{
				obj = float.Parse ((string)stringTree);
			}
			else if (type == typeof (double))
			{
				obj = double.Parse ((string)stringTree);
			}
			else if (type == typeof (decimal))
			{
				obj = decimal.Parse ((string)stringTree);
			}
			else if (type == typeof (ushort))
			{
				obj = ushort.Parse ((string)stringTree);
			}
			else if (type == typeof (uint))
			{
				obj = uint.Parse ((string)stringTree);
			}
			else if (type == typeof (ulong))
			{
				obj = ulong.Parse ((string)stringTree);
			}
			else
			{
				obj = null;
				result = false;
			}
			return result;
		}

		/// <summary>
		/// Создаёт массив элементов.
		/// </summary>
		/// <param name="elementType">Тип элементов массива.</param>
		/// <param name="stringTree">Строковое KV дерево.</param>
		/// <returns>Новый массив.</returns>
		private static object ConstructArray (Type elementType, object stringTree)
		{
			if (stringTree is List<KeyValuePair<string, object>> list)
			{
				Array array = Array.CreateInstance (elementType, list.Count);
				for (int i = 0; i < list.Count; i++)
				{
					array.SetValue (ConstructFromStringKVTree (elementType, list[i].Value), i);
				}
				return array;
			}
			else
			{
				throw new ArgumentException (
					$"Ошибка при создании массива из \"{elementType.FullName}\", " +
					$"\"stringTree\" должен быть \"List<KeyValuePair<string, object>>\", но оказался {stringTree.GetType ().FullName}."
				);
			}
		}

		/// <summary>
		/// Создаёт экземпляр перечисления.
		/// </summary>
		/// <param name="enumType">Тип перечисления.</param>
		/// <param name="stringTree">Строковое KV дерево.</param>
		/// <returns>Новый экземпляр перечисления.</returns>
		private static object ConstructEnum (Type enumType, object stringTree)
		{
			if (stringTree is string str)
			{
				object e = Enum.Parse (enumType, str);
				if (Enum.IsDefined (enumType, e))
				{
					return e;
				}
			}
			throw new ArgumentException (
				$"Ошибка при создании перечисления \"{enumType.FullName}\", " +
				$"значение \"{stringTree}\" не является значением данного перечисления."
			);
		}

		/// <summary>
		/// Создаёт экземпляр небазового типа.
		/// </summary>
		/// <param name="type">Тип создаваемого класса.</param>
		/// <param name="stringTree">Строковое KV дерево.</param>
		/// <returns>Новый экземпляр класса.</returns>
		private static object ConstructCustomType (Type type, object stringTree)
		{
			if (stringTree is List<KeyValuePair<string, object>> list)
			{
				Dictionary<string, object> pairs = new Dictionary<string, object> (list.Count);
				foreach (var pair in list)
				{
					pairs[pair.Key] = pair.Value;
				}
				ConstructorInfo constructor = GetSuitableConstructor (type, pairs);
				ParameterInfo[] parameters = constructor.GetParameters ();
				object[] objs = new object[parameters.Length];
				for (int i = 0; i < objs.Length; i++)
				{
					objs[i] = ConstructFromStringKVTree (parameters[i].ParameterType, pairs[parameters[i].Name]);
				}
				return constructor.Invoke (objs);
			}
			else
			{
				throw new ArgumentException (
					$"Ошибка при создании \"{type.FullName}\", " +
					$"\"stringTree\" должен быть \"List<KeyValuePair<string, object>>\", но оказался {stringTree.GetType ().FullName}."
				);
			}
		}

		/// <summary>
		/// Выбирает самый подходящий конструктор класса,
		/// который определяется наибольшим соответствием ИМЁН переменных
		/// конструктора с КЛЮЧАМИ строкового KV дерева.
		/// </summary>
		/// <param name="type">Тип создаваемого класса.</param>
		/// <param name="pairs">
		/// Словарь, ключами которого являются имена переменных,
		/// а ключами - строковые KV деревья.
		/// </param>
		/// <returns>Самый подходящий конструктор класса</returns>
		private static ConstructorInfo GetSuitableConstructor (Type type, Dictionary<string, object> pairs)
		{
			ConstructorInfo[] constructors = type.GetConstructors ();
			if (constructors.Length > 0)
			{
				int maxMatches = -1;
				int maxMatchesI = -1;
				for (int i = 0; i < constructors.Length; i++)
				{
					ParameterInfo[] parameters = constructors[i].GetParameters ();
					int matches = 0;
					for (int j = 0; j < parameters.Length; j++)
					{
						if (pairs.ContainsKey (parameters[j].Name))
						{
							matches++;
						}
					}
					if ((matches == parameters.Length) && (matches > maxMatches))
					{
						maxMatches = matches;
						maxMatchesI = i;
					}
				}
				if (maxMatchesI > -1)
				{
					return constructors[maxMatchesI];
				}
				else
				{
					throw new ArgumentException (
						$"Ошибка при создании \"{type.FullName}\", нет подходящего конструктора."
					);
				}
			}
			else
			{
				throw new ArgumentException (
					$"Ошибка при создании \"{type.FullName}\", тип не имеет конструкторов."
				);
			}
		}
	}
}
