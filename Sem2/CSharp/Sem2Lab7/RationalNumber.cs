using System;
using System.Text.RegularExpressions;

namespace Sem2Lab7
{
	public class RationalNumber : IEquatable<RationalNumber>, IComparable, IComparable<RationalNumber>,
				IConvertible, IFormattable
	{
		private long numerator = 0;
		private long denominator = 1;

		public RationalNumber ()
		{
		}

		public RationalNumber (long number)
		{
			Numerator = number;
		}

		public RationalNumber (double number)
		{
			if (Math.Abs (number) < long.MaxValue) {
				throw new ArgumentOutOfRangeException ();
			}
			long newNumerator = 0;
			long newDenominator = 1;
			if (number < 0.0) {
				newDenominator = -newDenominator;
				number = -number;
			}
			try {
				while (number > 0.00001) {
					long n = (long)number;
					number -= n;
					checked {
						newNumerator += n;
						long tempN = newNumerator * 2;
						long tempM = newDenominator * 2;
						number *= 2;
						newNumerator = tempN;
						newDenominator = tempM;
					}
				}
			} catch (OverflowException) { }
			long gcf = GCF (newNumerator, newDenominator);
			Numerator = newNumerator / gcf;
			Denominator = newDenominator / gcf;
		}
		public RationalNumber (decimal number)
		{
			if (Math.Abs (number) < long.MaxValue) {
				throw new ArgumentOutOfRangeException ();
			}
			long newNumerator = 0;
			long newDenominator = 1;
			if (number < 0.0M) {
				newDenominator = -newDenominator;
				number = -number;
			}
			try {
				while (number > 0.00001M) {
					long n = (long)number;
					number -= n;
					checked {
						newNumerator += n;
						long tempN = newNumerator * 10;
						long tempM = newDenominator * 10;
						number *= 10;
						newNumerator = tempN;
						newDenominator = tempM;
					}
				}
			} catch (OverflowException) { }
			long gcf = GCF (newNumerator, newDenominator);
			Numerator = newNumerator / gcf;
			Denominator = newDenominator / gcf;
		}
		public RationalNumber (long n, long m)
		{
			long gcf = GCF (n, m);
			Numerator = n / gcf;
			Denominator = m / gcf;
		}
		public void Deconstruct (out long n, out long m)
		{
			n = Numerator;
			m = Denominator;
		}

		public long Numerator
		{
			get => numerator;
			set => numerator = value;
		}
		public long Denominator
		{
			get => denominator;
			set
			{
				if (value == 0L) {
					throw new DivideByZeroException ();
				}
				if (value < 0L) {
					if (numerator == long.MinValue) {
						numerator = long.MaxValue;
					} else {
						numerator = -numerator;
					}
					if (value == long.MaxValue) {
						denominator = long.MaxValue;
					} else {
						denominator = -value;
					}
				} else {
					denominator = value;
				}
			}
		}

		// НОД
		public static long GCF (long a, long b)
			=> (b != 0) ? GCF (b, a % b) : a;

		public static (long n1, long n2, long m) ToSameDenominator (long n1, long m1, long n2, long m2)
		{
			long newN1;
			long newN2;
			long newM;

			long gcf = GCF (m1, m2);
			double dNewM = (double)m1 / gcf * m2;
			double dNewN1 = n1 * (dNewM / m1);
			double dNewN2 = n2 * (dNewM / m2);
			if (Math.Abs (dNewN1) < long.MaxValue && Math.Abs (dNewN2) < long.MaxValue && dNewM < long.MaxValue) {
				newM = m1 / gcf * m2;
				newN1 = n1 * (newM / m1);
				newN2 = n2 * (newM / m2);
			} else {
				double k = long.MaxValue / Math.Max (Math.Max (Math.Abs (dNewN1),
							Math.Abs (dNewN2)), Math.Abs (dNewM)) * 0.95;
				newN1 = (long)(dNewN1 * k);
				newN2 = (long)(dNewN2 * k);
				newM = (long)(dNewM * k);
				if (newM <= 0) {
					throw new OverflowException ("ToSameDenominator: The numbers are too large");
				}
			}
			return (newN1, newN2, newM);
		}

		// приведение типов
		public static implicit operator RationalNumber (long number)
			=> new RationalNumber (number);

		public static explicit operator long (RationalNumber rn)
			=> rn.Numerator / rn.Denominator;

		public static explicit operator RationalNumber (double number)
			=> new RationalNumber (number);

		public static explicit operator double (RationalNumber rn)
			=> (double)(rn.Numerator) / rn.Denominator;
		
		public static explicit operator float (RationalNumber rn)
			=> (float)(rn.Numerator) / rn.Denominator;

		public static explicit operator RationalNumber (decimal number)
			=> new RationalNumber (number);

		public static explicit operator decimal (RationalNumber rn)
			=> (decimal)(rn.Numerator) / rn.Denominator;

		// IConvertable
		public TypeCode GetTypeCode ()
			=> TypeCode.Object;

		public bool ToBoolean (IFormatProvider provider)
			=> Numerator != 0;

		public byte ToByte (IFormatProvider provider)
			=> (byte)(Numerator / Denominator);

		public char ToChar(IFormatProvider provider)
			=> throw new InvalidCastException ("ToChar: Conversion not supported.");

		public DateTime ToDateTime(IFormatProvider provider)
			=> throw new InvalidCastException ("ToDateTime: Conversion not supported.");

		public decimal ToDecimal (IFormatProvider provider)
			=> (decimal)(Numerator) / Denominator;

		public double ToDouble (IFormatProvider provider)
			=> (double)(Numerator) / Denominator;

		public short ToInt16 (IFormatProvider provider)
			=> (short)(Numerator / Denominator);

		public int ToInt32 (IFormatProvider provider)
			=> (int)(Numerator / Denominator);

		public long ToInt64 (IFormatProvider provider)
			=> Numerator / Denominator;

		public sbyte ToSByte (IFormatProvider provider)
			=> (sbyte)(Numerator / Denominator);

		public float ToSingle (IFormatProvider provider)
			=> (float)(Numerator) / Denominator;

		public string ToString (IFormatProvider provider)
			=> ToString (null, provider);

		public ushort ToUInt16 (IFormatProvider provider)
			=> (ushort)(Numerator / Denominator);

		public uint ToUInt32 (IFormatProvider provider)
			=> (uint)(Numerator / Denominator);

		public ulong ToUInt64 (IFormatProvider provider)
			=> (ulong)(Numerator / Denominator);

		public object ToType (Type conversionType, IFormatProvider provider)
		{
			switch (Type.GetTypeCode (conversionType)) {
				case TypeCode.Boolean:
					return ToBoolean (provider);
				case TypeCode.Byte:
					return ToByte (provider);
				case TypeCode.Char:
					return ToChar (provider);
				case TypeCode.DateTime:
					return ToDateTime (provider);
				case TypeCode.Decimal:
					return ToDecimal (provider);
				case TypeCode.Double:
					return ToDouble (provider);
				case TypeCode.Empty:
					throw new NullReferenceException ("ToType: The target type is null.");
				case TypeCode.Int16:
					return ToInt16 (provider);
				case TypeCode.Int32:
					return ToInt32 (provider);
				case TypeCode.Int64:
					return ToInt64 (provider);
				case TypeCode.Object:
					if (conversionType == typeof (RationalNumber)) {
						return this;
					} else {
						throw new InvalidCastException ($"ToType: Cannot convert from Temperature to {conversionType.Name}.");
					}
				case TypeCode.SByte:
					return ToSByte (provider);
				case TypeCode.Single:
					return ToSingle (provider);
				case TypeCode.String:
					return ToString (provider);
				case TypeCode.UInt16:
					return ToUInt16 (provider);
				case TypeCode.UInt32:
					return ToUInt32 (provider);
				case TypeCode.UInt64:
					return ToUInt64 (provider);
				default:
					throw new InvalidCastException ("ToType: Conversion not supported.");
			}
		}

		// переопределение математический операций
		public static RationalNumber operator + (RationalNumber rn)
			=> rn;

		public static RationalNumber operator - (RationalNumber rn)
			=> new RationalNumber (rn.Numerator, -rn.Denominator);

		public static RationalNumber operator + (RationalNumber rn1, RationalNumber rn2)
		{
			(long newN1, long newN2, long newDenominator) =
				ToSameDenominator (rn1.Numerator, rn1.Denominator, rn2.Numerator, rn2.Denominator);
			long newNumerator;

			double dNewN = (double)newN1 + newN2;
			if (Math.Abs (dNewN) < long.MaxValue) {
				newNumerator = newN1 + newN2;
			} else {
				double k = long.MaxValue / Math.Abs (dNewN) * 0.9;
				newNumerator = (long)(dNewN * k);
				newDenominator = (long)(newDenominator * k);
				if (newDenominator <= 0) {
					throw new OverflowException ("operator+: The numbers are too large");
				}
			}
			return new RationalNumber (newNumerator, newDenominator);
		}

		public static RationalNumber operator - (RationalNumber rn1, RationalNumber rn2)
		{
			(long newN1, long newN2, long newDenominator) =
				ToSameDenominator (rn1.Numerator, rn1.Denominator, rn2.Numerator, rn2.Denominator);
			long newNumerator;

			double dNewN = (double)newN1 - newN2;
			if (Math.Abs (dNewN) < long.MaxValue) {
				newNumerator = newN1 - newN2;
			} else {
				double k = long.MaxValue / Math.Abs (dNewN) * 0.9;
				newNumerator = Math.Max((long)(dNewN * k), long.MaxValue);
				newDenominator = (long)(newDenominator * k);
				if (newDenominator <= 0) {
					throw new OverflowException ("operator-: The numbers are too large");
				}
			}
			return new RationalNumber (newNumerator, newDenominator);
		}

		public static RationalNumber operator * (RationalNumber rn1, RationalNumber rn2)
		{
			long newNumerator;
			long newDenominator;
			(long n1, long m1) = rn1;
			(long n2, long m2) = rn2;

			double dNewN = (double)n1 * n2;
			double dNewM = (double)m1 * m2;
			if (Math.Abs(dNewN) < long.MaxValue && Math.Abs (dNewM) < long.MaxValue) {
				newNumerator = n1 * n2;
				newDenominator = m1 * m2;
			} else {
				double k = long.MaxValue / Math.Max (Math.Abs (dNewN), Math.Abs (dNewM)) * 0.9;
				newNumerator = (long)(dNewN * k);
				newDenominator = (long)(dNewM * k);
				if (newDenominator <= 0) {
					throw new OverflowException ("operator*: The numbers are too large");
				}
			}
			return new RationalNumber (newNumerator, newDenominator);
		}

		public static RationalNumber operator / (RationalNumber rn1, RationalNumber rn2)
		{
			long newNumerator;
			long newDenominator;
			(long n1, long m1) = rn1;
			(long n2, long m2) = rn2;

			double dNewN = (double)n1 * m2;
			double dNewM = (double)m1 * n2;
			if (Math.Abs (dNewN) < long.MaxValue && Math.Abs (dNewM) < long.MaxValue) {
				newNumerator = n1 * m2;
				newDenominator = m1 * n2;
			} else {
				double k = long.MaxValue / Math.Max (Math.Abs (dNewN), Math.Abs (dNewM)) * 0.9;
				newNumerator = (long)(dNewN * k);
				newDenominator = (long)(dNewM * k);
				if (newDenominator <= 0) {
					throw new OverflowException ("operator/: The numbers are too large");
				}
			}
			return new RationalNumber (newNumerator, newDenominator);
		}

		public static RationalNumber operator % (RationalNumber rn1, RationalNumber rn2)
		{
			(long newN1, long newN2, long newDenominator) =
				ToSameDenominator (rn1.Numerator, rn1.Denominator, rn2.Numerator, rn2.Denominator);
			long newNumerator = newN1 % newN2;
			return new RationalNumber (newNumerator, newDenominator);
		}

		// сравнение эквивалентности
		public override int GetHashCode ()
			=> (int)(Numerator * 31 + Denominator);

		public override bool Equals (object other)
		{
			if (other is RationalNumber)
				return Equals ((RationalNumber)other);
			else
				return false;
		}

		public bool Equals (RationalNumber other)
			=> (Numerator == other.Numerator && Denominator == other.Denominator);

		public static bool operator == (RationalNumber rn1, RationalNumber rn2)
			=> (double)rn1 == (double)rn2;

		public static bool operator != (RationalNumber rn1, RationalNumber rn2)
			=> (double)rn1 != (double)rn2;

		// сравнение порядка
		public int CompareTo (object other)
		{
			if (other is RationalNumber)
				return CompareTo ((RationalNumber)other);
			else
				throw new InvalidOperationException ("CompareTo: Not a RationalNumber");
		}

		public int CompareTo (RationalNumber other)
		{
			double n1 = (double)Numerator / Denominator;
			double n2 = (double)other.Numerator / other.Denominator;
			return n1.CompareTo (n2);
		}

		public static bool operator > (RationalNumber rn1, RationalNumber rn2)
			=> rn1.CompareTo(rn2) > 0;

		public static bool operator < (RationalNumber rn1, RationalNumber rn2)
			=> rn1.CompareTo (rn2) < 0;

		public static bool operator >= (RationalNumber rn1, RationalNumber rn2)
			=> rn1.CompareTo (rn2) >= 0;

		public static bool operator <= (RationalNumber rn1, RationalNumber rn2)
			=> rn1.CompareTo (rn2) <= 0;

		// представление в виде строки
		public override string ToString ()
			=> ToString(null, null);

		public string ToString (string format, IFormatProvider provider)
		{
			if (string.IsNullOrEmpty (format)) format = "G";
			switch (format) {
				case "G":
				case "g":
					return string.Format ("({0} / {1})", Numerator, Denominator);
				case ":":
				case "/":
				case "\\":
					return string.Format ("({0} {2} {1})", Numerator, Denominator, format);
				default:
					format = Regex.Replace (format, @"N(:|,).*?N",
						delegate (Match match) { return "{0" + match.Value.Substring (1, match.Length - 2) + "}"; });
					format = Regex.Replace (format, @"M(:|,).*?M",
						delegate (Match match) { return "{1" + match.Value.Substring (1, match.Length - 2) + "}"; });
					return string.Format (format, Numerator, Denominator);
			}
		}

		public static bool TryParse (string str, out RationalNumber rn)
		{
			Match match = Regex.Match (str, @"(?x) (?<=^\s*[(\[]?\s*) -?\d+ \s*[/\\:]\s* -?\d+ (?=\s*[)\]]?\s*$)");
			if (match.Success) {
				string[] splits = match.Value.Split ('/', '\\', ':');
				if (long.TryParse (splits[0], out long n) && long.TryParse (splits[1], out long m)) {
					rn = new RationalNumber (n, m);
					return true;
				}
			}
			rn = null;
			return false;
		}

		public static RationalNumber Parse (string str)
		{
			if (TryParse (str, out RationalNumber rn))
				return rn;
			else
				throw new FormatException ("Parse: Input string was not in a correct format.");
		}
	}
}
