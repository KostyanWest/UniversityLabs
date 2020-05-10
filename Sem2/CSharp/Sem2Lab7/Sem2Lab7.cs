using System;

namespace Sem2Lab7
{
	class Program
	{
		static void Main (string[] args)
		{
			RationalNumber rn1 = RationalNumber.Parse ("639/15");
			RationalNumber rn2 = RationalNumber.Parse ("(  876 / 1435 )");
			RationalNumber rn3 = RationalNumber.Parse ("7621185 /-1313");
			Console.WriteLine ("rn1 = {0}\nrn2 = {1:\\}\nrn3 = {2::}\n", rn1, rn2, rn3);

			Console.WriteLine ("rn1 + rn2 = {0:(N,6N : M,-6M)}", rn1 + rn2);
			Console.WriteLine ("rn1 - rn2 = {0:(N,6N : M,-6M)}", rn1 - rn2);
			Console.WriteLine ("rn1 * rn2 = {0:(N,6N : M,-6M)}", rn1 * rn2);
			Console.WriteLine ("rn1 / rn2 = {0:(N,6N : M,-6M)}", rn1 / rn2);
			Console.WriteLine ("rn1 % rn2 = {0:(N,6N : M,-6M)}", rn1 % rn2);
			Console.WriteLine ();

			try { 
				RationalNumber rnTemp = rn1 * rn2;
				double dTemp = rnTemp.ToDouble (null);
				double d3 = (double)rn3;
				for (int i = 1; ; i++) {
					Console.Write ("rn1 * rn2 * rn3^{0} = ", i);
					rnTemp *= rn3;
					dTemp *= d3;
					Console.WriteLine ("{0}\t[{1}]\t[{2}]", rnTemp, (double)rnTemp, dTemp);
				}
			} catch (OverflowException ex) {
				Console.WriteLine (ex.Message);
			}

			Console.ReadKey (true);
		}
	}
}
