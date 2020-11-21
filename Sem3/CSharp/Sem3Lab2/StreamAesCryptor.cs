using System.IO;
using System.Security.Cryptography;

namespace Sem3Lab2
{
	public class StreamAesCryptorSettings
	{
		public readonly byte[] key;
		public readonly byte[] IV;

		public StreamAesCryptorSettings ()
		{
			Aes aes = Aes.Create ();
			key = aes.Key;
			IV = aes.IV;
			aes.Dispose ();
		}

		public StreamAesCryptorSettings (byte[] key, byte[] IV)
		{
			this.key = key;
			this.IV = IV;
		}
	}

	/// <summary>
	/// Класс, умеющий шифровать/дешифровать поток методом AES, используя хранящиеся в себе ключи шифрования.
	/// Экземпляр класса сможет дешифровать любой зашифрованный им поток.
	/// </summary>
	public class StreamAesCryptor
	{
		private readonly byte[] key;
		private readonly byte[] IV;

		public StreamAesCryptor (StreamAesCryptorSettings settings)
		{
			key = settings.key;
			IV = settings.IV;
		}

		public OneoffCryptoStream Encrypt (Stream input, bool readOrWrite)
		{
			CryptoStreamMode mode = (readOrWrite) ? CryptoStreamMode.Read : CryptoStreamMode.Write;
			using (Aes aes = Aes.Create ())
			{
				aes.Key = key;
				aes.IV = IV;
				ICryptoTransform encryptor = aes.CreateEncryptor (aes.Key, aes.IV);
				try
				{
					return new OneoffCryptoStream (input, encryptor, mode);
				}
				catch
				{
					if (encryptor != null)
					{
						encryptor.Dispose ();
					}
					throw;
				}
			}
		}

		public Stream Decrypt (Stream input, bool readOrWrite)
		{
			CryptoStreamMode mode = (readOrWrite) ? CryptoStreamMode.Read : CryptoStreamMode.Write;
			using (Aes aes = Aes.Create ())
			{
				aes.Key = key;
				aes.IV = IV;
				ICryptoTransform decryptor = aes.CreateDecryptor (aes.Key, aes.IV);
				try
				{
					return new OneoffCryptoStream (input, decryptor, mode);
				}
				catch
				{
					if (decryptor != null)
					{
						decryptor.Dispose ();
					}
					throw;
				}
			}
		}
	}
}
