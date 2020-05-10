using System;

namespace Sem2Lab1
{
	internal class CellConsole : Cell
	{
		private static readonly ConsoleColor[] colors =
		{
			ConsoleColor.Cyan,
			ConsoleColor.Green,
			ConsoleColor.Yellow,
			ConsoleColor.Red,
			ConsoleColor.Magenta
		};
		private static readonly int colorsNum = colors.Length;
		private ConsoleColor color = ConsoleColor.DarkRed;

		public CellConsole (long initValue)
		{
			Value = initValue;
		}

		protected override void ChangeColor ()
		{
			if (Value < 4) {
				color = ConsoleColor.Gray;
			} else if (Value < 8) {
				color = ConsoleColor.White;
			} else {
				long value = Value >> 4;
				int power = 0;
				while (value > 0) {
					value >>= 1;
					power++;
				}
				color = colors[power % colorsNum];
			}
		}

		public void Render (int row)
		{
			// for cellVoid
			if (Value <= 0) {
				Console.Write ("        ");
				return;
			}

			ConsoleColor oldColor = Console.ForegroundColor;
			Console.ForegroundColor = color;
			switch (row) {
				case 0:
					Console.Write ("/^^^^^^\\");
					break;
				case 1:
				case 3:
					Console.Write ("|      |");
					break;
				case 2:
					if (Value < 10) {
						Console.Write ("|  " + Value + "   |");
					} else if (Value < 100) {
						Console.Write ("|  " + Value + "  |");
					} else if (Value < 1000) {
						Console.Write ("| " + Value + "  |");
					} else if (Value < 10000) {
						Console.Write ("| " + Value + " |");
					} else if (Value < 100000) {
						Console.Write ("|" + Value + " |");
					} else if (Value < 1000000) {
						Console.Write ("|" + Value + "|");
					} else {
						Console.Write ("|TooLng|");
					}
					break;
				case 4:
					Console.Write ("\\______/");
					break;
			}
			Console.ForegroundColor = oldColor;
		}
	}
}
