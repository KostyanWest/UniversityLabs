using System;

namespace Sem2Lab1
{
	public enum EDirection
	{
		toDown,
		toLeft,
		toUp,
		toRight
	}

	[Flags]
	public enum EFieldFlags
	{
		ffScoreAsSumOfValues = 1,   // счёт - это сумма значений всех клеток
		ffGoalIsScore = 1 << 1,     // цель - достичь определённого счёта
		ffDontSpawn4 = 1 << 2,      // не генерировать клетки со значениями 4
		ffDrawOnDesktop = 1 << 3    // выводить игру на рабочий стол
	}

	public abstract class Field : ISimpleGame
	{
		public static readonly int MAX_SIZE = 8;

		public readonly int width;
		public readonly int height;
		public readonly long goal;

		protected bool flagScoreAsSumOfValues;
		protected bool flagGoalIsMaxValue;
		protected bool flagGoalIsScore;
		protected bool flagDontSpawn4;

		protected Cell[][] cells;
		protected long score = 0;
		protected long maxValue = 0;
		protected int count = 0;
		protected readonly Random rand;
		protected bool isGameOver = false;

		event EventHandler ISimpleGame.GameEnd
		{
			add { Win += value; Lose += value; }
			remove { Win -= value; Lose -= value; }
		}
		public event EventHandler Win;
		public event EventHandler Lose;

		protected Field (int h, int w, long goal = 2048, EFieldFlags flags = 0)
		{
			if ((h > 0) && (h <= MAX_SIZE) && (w > 0) && (w <= MAX_SIZE)) {
				// общие параметры
				height = h;
				width = w;
				cells = new Cell[h][];
				for (int i = 0; i < h; i++) {
					cells[i] = new Cell[w];
				}
				this.goal = goal;
				// правила
				flagScoreAsSumOfValues = (flags & EFieldFlags.ffScoreAsSumOfValues) != 0;
				if (goal >= 0) {
					flagGoalIsScore = (flags & EFieldFlags.ffGoalIsScore) != 0;
					flagGoalIsMaxValue = !flagGoalIsScore;
				} else {
					flagGoalIsMaxValue = flagGoalIsScore = false;
				}
				flagDontSpawn4 = (flags & EFieldFlags.ffDontSpawn4) != 0;
				// специальное
				rand = new Random ();
				SpawnCell ();
			} else {
				throw new ArgumentOutOfRangeException ();
			}
		}

		public Cell this[int h, int w]
		{
			get { return cells[h][w]; }
		}

		public static Field Create (int h, int w, long goal = 2048, EFieldFlags flags = 0)
		{
			if ((flags & EFieldFlags.ffDrawOnDesktop) == 0) {
				return new FieldConsole (h, w, goal, flags);
			} else {
				return new FieldDesktop (h, w, goal, flags);
			}
		}

		protected virtual void OnWin ()
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine ("\tYOU WIN !!!");
			Console.ResetColor ();
			isGameOver = true;
			Win?.Invoke (this, EventArgs.Empty);
		}

		protected virtual void OnLose ()
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine ("\tYOU LOSE !!!");
			Console.ResetColor ();
			isGameOver = true;
			Lose?.Invoke (this, EventArgs.Empty);
		}

		void ISimpleGame.PressKey (ConsoleKey key)
		{
			switch (key) {
				case ConsoleKey.DownArrow:
					Move (EDirection.toDown);
					break;
				case ConsoleKey.LeftArrow:
					Move (EDirection.toLeft);
					break;
				case ConsoleKey.UpArrow:
					Move (EDirection.toUp);
					break;
				case ConsoleKey.RightArrow:
					Move (EDirection.toRight);
					break;
				case ConsoleKey.R:
					Render ();
					break;
			}
		}

		protected abstract void SpawnCell ();

		public abstract void Render ();

		public virtual bool Move (EDirection direction)
		{
			if (isGameOver) {
				return false;
			}
			int h, h_step, h_step_next, w, w_step, w_step_next;
			// перебираем клетки в порядке, противоположном ходу движения
			switch (direction) {
				case EDirection.toDown:
					h = height - 1;
					h_step = -1;
					h_step_next = height;
					w = 0;
					w_step = 0;
					w_step_next = 1;
					break;
				case EDirection.toLeft:
					h = 0;
					h_step = 0;
					h_step_next = 1;
					w = 0;
					w_step = 1;
					w_step_next = -width;
					break;
				case EDirection.toUp:
					h = 0;
					h_step = 1;
					h_step_next = -height;
					w = 0;
					w_step = 0;
					w_step_next = 1;
					break;
				case EDirection.toRight:
					h = 0;
					h_step = 0;
					h_step_next = 1;
					w = width - 1;
					w_step = -1;
					w_step_next = width;
					break;
				default:
					throw new ArgumentOutOfRangeException ();
			}
			// стоящие перед самым краем поля клетки не трогаем (+step)
			h += h_step;
			h_step_next += h_step;
			w += w_step;
			w_step_next += w_step;
			// главный цикл
			bool hasChanged = false;
			while (h >= 0 && h < height && w >= 0 && w < width) {
				if (cells[h][w] != null) {
					// шаги со знаком минус - смена направления
					(int h_new, int w_new) = MoveCell (h, -h_step, w, -w_step);
					// было ли движение
					if (h_new != h || w_new != w) {
						hasChanged = true;
					}
				}
				// след. шаг
				h += h_step;
				w += w_step;
				// меняем ли линию?
				if (!(h >= 0 && h < height && w >= 0 && w < width)) {
					h += h_step_next;
					w += w_step_next;
				}
			}
			// было ли действие?
			if (hasChanged) {
				SpawnCell ();
				Render ();
				// победа? поражение?
				CheckGameStatus ();
			}
			return hasChanged;
		}

		protected (int h_new, int w_new) MoveCell (int h, int h_step, int w, int w_step)
		{
			ref Cell cell = ref cells[h][w];
			h += h_step;
			w += w_step;
			while (h >= 0 && h < height && w >= 0 && w < width) {
				// встретили ли препятствие
				if (cells[h][w] != null) {
					// можно ли сложить
					if (cell.CanMergeWith (cells[h][w])) {
						cells[h][w].Value += cell.Value;
						cell.Destroy ();
						cell = null;
						count--;
						// забираем подходящую клетку
						return MoveCell (h, h_step, w, w_step);
					}
					break;
				}
				h += h_step;
				w += w_step;
			}
			// возвращение на последнюю свободную клетку
			h -= h_step;
			w -= w_step;
			if (cell != cells[h][w]) {
				cells[h][w] = cell;
				cell = null;
			}
			return (h, w);
		}

		protected void CheckValueChange (long newValue)
		{
			if (newValue > maxValue) {
				maxValue = newValue;
			}
		}

		protected void CheckValueChange (object sender, ValueChangeEventArgs e)
		{
			if (e.newValue > maxValue) {
				maxValue = e.newValue;
			}
			if (!flagScoreAsSumOfValues) {
				score += e.newValue;
			}
		}

		protected void CheckGameStatus ()
		{
			if (CanWin()) {
				OnWin ();
			} else {
				if (!CanMove ()) {
					OnLose ();
				}
			}
		}

		protected bool CanWin ()
		{
			if (flagGoalIsScore) {
				if (score >= goal) {
					return true;
				} else {
					return false;
				}
			} else {
				if (maxValue >= goal) {
					return true;
				} else {
					return false;
				}
			}
		}

		protected bool CanMove ()
		{
			if (count == height * width) {
				// все cells не null
				for (int h = 0; h < height; h++) {
					long value = cells[h][0].Value;
					for (int w = 1; w < width; w++) {
						long value2 = cells[h][w].Value;
						if (value == value2) {
							return true;
						} else {
							value = value2;
						}
					}
				}
				for (int w = 0; w < height; w++) {
					long value = cells[0][w].Value;
					for (int h = 1; h < width; h++) {
						long value2 = cells[h][w].Value;
						if (value == value2) {
							return true;
						} else {
							value = value2;
						}
					}
				}
				return false;
			} else {
				return true;
			}
		}

		public virtual void Destroy ()
		{
			for (int h = 0; h < height; h++) {
				for (int w = 0; w < width; w++) {
					if (cells[h][w] != null) {
						cells[h][w].Destroy ();
						cells[h][w] = null;
					}
				}
			}
			Win = null;
			Lose = null;
		}
	}
}
