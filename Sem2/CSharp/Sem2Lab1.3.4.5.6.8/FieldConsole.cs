using System;

namespace Sem2Lab1
{
	internal class FieldConsole : Field
	{
		protected CellConsole cellVoid;

		public FieldConsole (int h, int w, long goal = 2048, EFieldFlags flags = 0) : base (h, w, goal, flags)
		{
			cellVoid = new CellConsole (0);
			Render ();
		}

		protected override void SpawnCell ()
		{
			int k = rand.Next (height * width - count);
			for (int h = 0; h < height; h++) {
				for (int w = 0; w < width; w++) {
					if (cells[h][w] == null) {
						if (k > 0) {
							k--;
						} else {
							int newValue;
							// 10% вероятности
							if (!flagDontSpawn4 && rand.Next(10) == 0) {
								newValue = 4;
							} else {
								newValue = 2;
							}
							cells[h][w] = new CellConsole (newValue);
							if (flagScoreAsSumOfValues) {
								score += newValue;
							}
							count++;
							cells[h][w].ValueChange += CheckValueChange;
							CheckValueChange (newValue);
							return;
						}
					}
				}
			}
		}

		public override void Render ()
		{
			if (isGameOver) {
				return;
			}
			Console.Clear ();
		
			if (flagGoalIsScore) {
				Console.WriteLine ($"\tTOTAL SCORE:\t{score} (goal: {goal})");
				Console.WriteLine ($"\tMAX VALUE:\t{maxValue}");
			} else {
				Console.WriteLine ($"\tTOTAL SCORE:\t{score}");
				Console.WriteLine ($"\tMAX VALUE:\t{maxValue} (goal: {goal})");
			}
			Console.WriteLine ($"\tNUM OF CELLS:\t{count}");


			for (int i = 0; i < width * 8 + 2; i++) {
				Console.Write ('#');
			}
			Console.WriteLine ();
			for (int h = 0; h < height; h++) {
				for (int row = 0; row < 5; row++) {
					Console.Write ('#');
					for (int w = 0; w < width; w++) {
						if (cells[h][w] != null) {
							((CellConsole)cells[h][w]).Render (row);
						} else {
							cellVoid.Render (0);
						}
					}
					Console.WriteLine ('#');
				}
			}
			for (int i = 0; i < width * 8 + 2; i++) {
				Console.Write ('#');
			}


			Console.WriteLine ();
			Console.WriteLine ("Use arrows to move cells.");
			Console.WriteLine ("R - to refresh image.");
			Console.WriteLine ("ESC - to exit.");
		}

		public override void Destroy ()
		{
			base.Destroy ();
			cellVoid?.Destroy ();
			cellVoid = null;
		}
	}
}
