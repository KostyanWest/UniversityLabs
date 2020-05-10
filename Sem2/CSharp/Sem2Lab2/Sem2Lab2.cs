using System;
using System.Text;

namespace Sem2Lab2
{
	class Program
	{
		static ulong EnterUInt64 ()
		{
			string str;
			bool isUncorrect = true;
			ulong number = 0;
			do {
				str = Console.ReadLine ();
				try {
					number = Convert.ToUInt64 (str);
					isUncorrect = false;
				} catch {
					Console.WriteLine ("Invalid input, try again.");
				}
			} while (isUncorrect);
			return number;
		}

		static ulong GetPowerOfTwoFromFactorial (ulong factorial)
		{
			ulong tempNumber = factorial;
			ulong numOfOnes = 0;
			while (tempNumber > 0UL) {
				if ((tempNumber & 1UL) == 1UL) {
					numOfOnes++;
				}
				tempNumber >>= 1;
			}
			return factorial - numOfOnes;
		}

		static void Main (string[] args)
		{
			/*
			 * Расчитать маскимальную степень двойки, на которую делится произведение
			 * подряд идущих чисел от a до b (числа целые 64-битные без знака).
			 */
			Console.WriteLine ("Enter number a:");
			ulong a = EnterUInt64 ();
			Console.WriteLine ("Enter number b:");
			ulong b = EnterUInt64 ();
			ulong numA;
			ulong numB;
			if (b > a) {
				numA = a;
				numB = b;
			} else {
				numB = a;
				numA = b;
			}
			if (numA > 0) {
				numA--;
			}
			Console.WriteLine ("Result: {0}", GetPowerOfTwoFromFactorial (numB) - GetPowerOfTwoFromFactorial (numA));
			Console.ReadKey (true);

			/*
			 * С помощью класса DateTime вывести на консоль названия месяцев на французском 
			 * языке. По желанию обобщить на случай, когда язык задаётся с клавиатуры.
			 */
			Console.WriteLine ();
			for (int i = 1; i <= 12; i++) {
				DateTime dateTime = new DateTime (2020, i, 1);
				Console.WriteLine (dateTime.ToString ("MMMM", new System.Globalization.CultureInfo ("fr-FR")));
			}
			Console.ReadKey (true);

			/*
			 * Дана строка из 256 английских букв. Записать через пробел 30 символов этой строки,
			 * стоящих на случайных местах. Желательно сделать только одно обращение к классу Random.
			 */
			Console.WriteLine ();
			StringBuilder sourceString = new StringBuilder (256, 256);
			int index = 0;
			while (index < 256) {
				for (char ch = 'A'; ch < 'Z' && index < 256; ch++) {
					sourceString.Append (ch);
					index++;
				}
				for (char ch = 'a'; ch < 'z' && index < 256; ch++) {
					sourceString.Append (ch);
					index++;
				}
			}
			StringBuilder resultString = new StringBuilder (30, 30);
			Random random = new Random();
			byte[] bytes = new byte[30];
			random.NextBytes (bytes);
			for (int i = 0; i < 30; i++) {
				resultString.Append (sourceString[bytes[i]]);
			}
			Console.WriteLine ("Source string:");
			Console.WriteLine (sourceString);
			Console.WriteLine ("Random 30 characters:");
			Console.WriteLine (resultString);
			Console.ReadKey (true);
		}
	}
}
