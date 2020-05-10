using System;
using System.Runtime.InteropServices;

namespace Sem2Lab1
{
	class CellDesktop : Cell, IDisposable
	{
		[DllImport ("gdi32.dll", CallingConvention = CallingConvention.Winapi)]
		protected static extern IntPtr CreatePen (int style, int width, uint color);
		[DllImport ("gdi32.dll", CallingConvention = CallingConvention.Winapi)]
		protected static extern IntPtr CreateSolidBrush (uint color);
		[DllImport ("gdi32.dll", CallingConvention = CallingConvention.Winapi)]
		protected static extern IntPtr SelectObject (IntPtr hdc, IntPtr hObj);
		[DllImport ("gdi32.dll", CallingConvention = CallingConvention.Winapi)]
		protected static extern bool DeleteObject (IntPtr hObj);
		[DllImport ("gdi32.dll", CallingConvention = CallingConvention.Winapi)]
		protected static extern bool Rectangle (IntPtr hdc, int left, int top, int right, int bottom);
		[DllImport ("User32.dll", CallingConvention = CallingConvention.Winapi)]
		protected static extern int DrawTextExA (IntPtr hdc, string str, int lenght, ref Rect rect, uint format, IntPtr dtp);

		protected bool disposed = true;
		protected IntPtr hPen = IntPtr.Zero;
		protected IntPtr hBrush = IntPtr.Zero;
		protected string valueStr;

		public CellDesktop (long initValue)
		{
			Value = initValue;
			hPen = CreatePen (0, 5, 0x00ffffff);
			disposed = false;
		}

		~CellDesktop ()
		{
			Dispose ();
		}

		protected override void ChangeColor ()
		{
			long value = Value;
			int power = 0;
			while (value > 0) {
				value >>= 1;
				power++;
			}

			// составление цвета
			int h = 216 + 12 * power;
			int s = 100 + 10 * power;
			s = Math.Clamp (s, 0, 200);
			int r = Math.Abs ((h % 360) - 180);
			r = (200 - s) + (int)((r - 60) * s / 60f);
			r = Math.Clamp (r, 200 - s, 200);
			int g = Math.Abs (((h + 60) % 360) - 180);
			g = (200 - s) + (int)((g - 60) * s / 60f);
			g = Math.Clamp (g, 200 - s, 200);
			int b = Math.Abs (((h + 120) % 360) - 180);
			b = (200 - s) + (int)((b - 60) * s / 60f);
			b = Math.Clamp (b, 200 - s, 200);

			uint color = (uint)((b << 16) | (g << 8) | r);
			if (hBrush != IntPtr.Zero) {
				DeleteObject (hBrush);
			}
			hBrush = CreateSolidBrush (color);

			if (Value > 0) {
				valueStr = Value.ToString ();
			} else {
				valueStr = "";
			}
		}

		public void Render (IntPtr hdc, ref Rect rect)
		{
			if (disposed) {
				throw new ObjectDisposedException ("CellDesktop");
			}

			IntPtr hOldPen = SelectObject (hdc, hPen);
			IntPtr hOldBrush = SelectObject (hdc, hBrush);

			Rectangle (hdc, rect.left, rect.top, rect.right, rect.bottom);
			// DT_CENTER | DT_SINGLELINE | DT_VCENTER | DT_NOCLIP 0x00000001 | 0x00000020 | 0x00000004 | 0x00000100
			DrawTextExA (hdc, valueStr, valueStr.Length, ref rect, 0x00000001 | 0x00000020 | 0x00000004, IntPtr.Zero);

			SelectObject (hdc, hOldPen);
			SelectObject (hdc, hOldBrush);
		}

		public virtual void Dispose ()
		{
			disposed = true;
			if (hPen != IntPtr.Zero) {
				DeleteObject (hPen);
			}
			if (hBrush != IntPtr.Zero) {
				DeleteObject (hBrush);
			}
		}

		public override void Destroy ()
		{
			base.Destroy ();
			Dispose ();
		}
	}
}
