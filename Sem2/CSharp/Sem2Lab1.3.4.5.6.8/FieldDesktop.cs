using System;
using System.Runtime.InteropServices;

namespace Sem2Lab1
{
	[StructLayout (LayoutKind.Sequential)]
	public struct Rect
	{
		public int left;
		public int top;
		public int right;
		public int bottom;
	}

	internal class FieldDesktop : Field, IDisposable
	{
		[DllImport ("User32.dll", CallingConvention = CallingConvention.Winapi)]
		protected static extern IntPtr GetDesktopWindow ();
		[DllImport ("User32.dll", CallingConvention = CallingConvention.Winapi)]
		protected static extern bool GetClientRect (IntPtr hwnd, ref Rect rect);
		[DllImport ("User32.dll", CallingConvention = CallingConvention.Winapi)]
		protected static extern IntPtr GetDC (IntPtr hwnd);
		[DllImport ("User32.dll", CallingConvention = CallingConvention.Winapi)]
		protected static extern bool ReleaseDC (IntPtr hdc);
		[DllImport ("gdi32.dll", CallingConvention = CallingConvention.Winapi)]
		protected static extern uint SetTextColor (IntPtr hdc, uint color);
		[DllImport ("gdi32.dll", CallingConvention = CallingConvention.Winapi)]
		protected static extern int SetBkMode (IntPtr hdc, int mode);
		[DllImport ("gdi32.dll", CallingConvention = CallingConvention.Winapi)]
		protected static extern IntPtr CreateFontA (int cHeight, int cWidth, int cEscapement, int cOrientation, int cWeight, uint bItalic,
							uint bUnderline, uint bStrikeOut, uint iCharSet, uint iOutPrecision, uint iClipPrecision,
							uint iQuality, uint iPitchAndFamily, IntPtr pszFaceName);
		[DllImport ("User32.dll", CallingConvention = CallingConvention.Winapi)]
		protected static extern int DrawTextExA (IntPtr hdc, string str, int lenght, ref Rect rect, uint format, IntPtr dtp);
		[DllImport ("gdi32.dll", CallingConvention = CallingConvention.Winapi)]
		protected static extern IntPtr SelectObject (IntPtr hdc, IntPtr hObj);
		[DllImport ("gdi32.dll", CallingConvention = CallingConvention.Winapi)]
		protected static extern bool DeleteObject (IntPtr hObj);

		const float RECT_MARGIN = 0.9f;
		const float CELL_MARGIN = 0.8f;

		protected bool disposed = true;
		protected CellDesktop cellVoid;
		protected IntPtr hdc = IntPtr.Zero;
		protected IntPtr hFont = IntPtr.Zero;
		protected Rect cellRect;
		protected float cellSize;


		public FieldDesktop (int h, int w, long goal = 2048, EFieldFlags flags = 0) : base (h, w, goal, flags)
		{
			IntPtr hwnd = GetDesktopWindow ();
			Rect rect = new Rect ();
			GetClientRect (hwnd, ref rect);


			Rect tempRect = new Rect {
				left = 0,
				right = (int)((rect.right - rect.left) * RECT_MARGIN),
				top = 0,
				bottom = (int)((rect.bottom - rect.top) * RECT_MARGIN)
			};

			cellSize = Math.Min (tempRect.right / (float)width, tempRect.bottom / (float)height);
			tempRect.right = (int)(cellSize * width);
			tempRect.bottom = (int)(cellSize * height);
			cellRect = new Rect {
				left = (int)Math.Round (cellSize * (1.0f - CELL_MARGIN) / 2) + (rect.right + rect.left - tempRect.right) / 2,
				right = (int)Math.Round (cellSize * (1.0f - (1.0f - CELL_MARGIN) / 2)) + (rect.right + rect.left - tempRect.right) / 2,
				top = (int)Math.Round (cellSize * (1.0f - CELL_MARGIN) / 2) + (rect.bottom + rect.top - tempRect.bottom) / 2,
				bottom = (int)Math.Round (cellSize * (1.0f - (1.0f - CELL_MARGIN) / 2)) + (rect.bottom + rect.top - tempRect.bottom) / 2
			};

			int fontSize = (int)(cellSize * CELL_MARGIN / 4);

			hdc = GetDC (IntPtr.Zero);
			hFont = CreateFontA (fontSize, 0, 0, 0, 1000, 0, 0, 0, 0, 0, 0, 0, 0, IntPtr.Zero);
			cellVoid = new CellDesktop (0);
			disposed = false;
			Render ();
		}

		~FieldDesktop ()
		{
			Dispose ();
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
							if (!flagDontSpawn4 && rand.Next (10) == 0) {
								newValue = 4;
							} else {
								newValue = 2;
							}
							cells[h][w] = new CellDesktop (newValue);
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
			if (disposed) {
				throw new ObjectDisposedException ("FieldDesktop");
			}
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

			IntPtr hOldFont = SelectObject (hdc, hFont);
			uint oldColor = SetTextColor (hdc, 0x00ffffff);
			int oldBM = SetBkMode (hdc, 1);

			for (int h = 0; h < height; h++) {
				for (int w = 0; w < width; w++) {
					Rect rect = new Rect {
						left = cellRect.left + (int)(cellSize * w),
						right = cellRect.right + (int)(cellSize * w),
						top = cellRect.top + (int)(cellSize * h),
						bottom = cellRect.bottom + (int)(cellSize * h),
					};
					if (cells[h][w] != null) {
						((CellDesktop)cells[h][w]).Render (hdc, ref rect);
					} else {
						cellVoid.Render (hdc, ref rect);
					}
				}
			}
			// фикс бага с текстом
			Rect fakeRect = new Rect ();
			cellVoid.Render (hdc, ref fakeRect);

			SelectObject (hdc, hOldFont);
			SetTextColor (hdc, oldColor);
			SetBkMode (hdc, oldBM);

			Console.WriteLine ();
			Console.WriteLine ("Use arrows to move cells.");
			Console.WriteLine ("R - to refresh image.");
			Console.WriteLine ("ESC - to exit.");
		}

		public virtual void Dispose ()
		{
			disposed = true;
			for (int h = 0; h < height; h++) {
				for (int w = 0; w < width; w++) {
					((CellDesktop)cells[h][w])?.Dispose ();
				}
			}
			if (hFont != IntPtr.Zero) {
				DeleteObject (hFont);
			}
			if (hdc != IntPtr.Zero) {
				ReleaseDC (hdc);
			}
		}

		public override void Destroy ()
		{
			base.Destroy ();
			cellVoid?.Destroy ();
			cellVoid = null;
			Dispose ();
		}
	}
}
