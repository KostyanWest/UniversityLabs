using System;
using System.IO;
using System.Security.Cryptography;

namespace Sem3Lab2
{
	public class OneoffCryptoStream : CryptoStream
	{
		private readonly IDisposable disposable;

		public OneoffCryptoStream (Stream stream, ICryptoTransform cryptor, CryptoStreamMode mode)
			: base (stream, cryptor, mode)
		{
			disposable = cryptor;
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			if (disposing && disposable != null)
			{
				disposable.Dispose ();
			}
		}
	}
}
