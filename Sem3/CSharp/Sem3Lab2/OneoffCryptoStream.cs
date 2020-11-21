using System;
using System.IO;
using System.Security.Cryptography;

namespace Sem3Lab2
{
	/// <summary>
	/// Улучшенный поток CryptoStream, который сам вызывает метод <see cref="IDisposable.Dispose()"/>
	/// у <see cref="ICryptoTransform"/> после своего использования.
	/// </summary>
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
