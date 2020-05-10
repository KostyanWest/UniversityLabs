using System;
using System.Runtime.InteropServices;

namespace Sem2Lab4
{
	class Program
	{
		delegate double MathFunc (double d);

		[DllImport ("Sem2Lab4dll.dll", CallingConvention = CallingConvention.StdCall)]
		static extern double IntegralLeft (MathFunc func, double start, double end, int steps);
		[DllImport ("Sem2Lab4dll.dll", CallingConvention = CallingConvention.StdCall)]
		static extern double IntegralRight (MathFunc func, double start, double end, int steps);
		[DllImport ("Sem2Lab4dll.dll", CallingConvention = CallingConvention.Cdecl)]
		static extern double IntegralCenter (MathFunc func, double start, double end, int steps);
		[DllImport ("Sem2Lab4dll.dll", CallingConvention = CallingConvention.Cdecl)]
		static extern double IntegralTrapec (MathFunc func, double start, double end, int steps);

		static double SomeMathFunc (double x)
		{
			return Math.Sin (x * x * 0.8 + 0.3) / (0.7 + Math.Cos (x * 1.2 + 0.3));
		}

		static void Main (string[] args)
		{
			MathFunc mathFunc = SomeMathFunc;
			double start = 4.0;
			double end = 9.0;
			int steps = 100;
			Console.WriteLine ("Integral results:");
			Console.WriteLine (IntegralLeft (mathFunc, start, end, steps));
			Console.WriteLine (IntegralRight (mathFunc, start, end, steps));
			Console.WriteLine (IntegralCenter (mathFunc, start, end, steps));
			Console.WriteLine (IntegralTrapec (mathFunc, start, end, steps));
			Console.ReadKey (true);
		}
	}
}
