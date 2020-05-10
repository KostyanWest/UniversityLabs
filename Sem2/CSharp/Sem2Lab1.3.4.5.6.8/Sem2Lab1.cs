using System;

namespace Sem2Lab1
{
	internal class Program
	{
		static void Main (string[] args)
		{
			EFieldFlags flags = EFieldFlags.ffDontSpawn4 | EFieldFlags.ffDrawOnDesktop;
			ISimpleGame game = Field.Create (4, 4, 2048, flags);
			ConsoleKey key = 0;
			bool isInGame = true;
			game.GameEnd += delegate { isInGame = false; };

			while (isInGame && key != ConsoleKey.Escape) {
				key = Console.ReadKey (true).Key;
				game.PressKey (key);
			}

			// пауза перед выходом
			while (key != ConsoleKey.Escape) {
				key = Console.ReadKey (true).Key;
			}
		}
	}
}
