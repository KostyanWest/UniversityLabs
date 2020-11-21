using System.IO;
using System.Text;

namespace Sem3Lab2
{
	public class AesFileSettings
	{
		public readonly string cryptedName;
		public readonly int maxPathLength;
		public readonly StreamAesCryptorSettings streamSettings;

		public AesFileSettings ()
		{
			cryptedName = "tmp.bin";
			maxPathLength = 260;
			streamSettings = new StreamAesCryptorSettings ();
		}

		public AesFileSettings (string cryptedName)
		{
			this.cryptedName = cryptedName;
			maxPathLength = 260;
			streamSettings = new StreamAesCryptorSettings ();
		}

		public AesFileSettings (string cryptedName, int maxPathLength)
		{
			this.cryptedName = cryptedName;
			this.maxPathLength = maxPathLength;
			streamSettings = new StreamAesCryptorSettings ();
		}

		public AesFileSettings (string cryptedName, int maxPathLength, StreamAesCryptorSettings streamSettings)
		{
			this.cryptedName = cryptedName;
			this.maxPathLength = maxPathLength;
			this.streamSettings = streamSettings;
		}
	}

	/// <summary>
	/// Класс предназначен для шифрования/дешифрования файлов методом AES.
	/// </summary>
	public class AesFile
	{
		private readonly string cryptedName;
		private readonly int maxPathLength;
		private readonly StreamAesCryptor cryptor;

		public AesFile (AesFileSettings settings)
		{
			cryptedName = settings.cryptedName;
			maxPathLength = settings.maxPathLength;
			cryptor = new StreamAesCryptor (settings.streamSettings);
		}

		public FileInfo Encrypt (FileInfo file)
		{
			FileInfo newFile = new FileInfo (Path.Combine (file.DirectoryName, cryptedName));
			try
			{
				using (FileStream input = new FileStream (file.FullName, FileMode.Open, FileAccess.Read))
				{
					using (FileStream output = new FileStream (newFile.FullName, FileMode.Create, FileAccess.Write))
					{
						using (Stream crypto = cryptor.Encrypt (output, false))
						{
							// Encrypt file name
							byte[] buffer = Encoding.Unicode.GetBytes (file.Name.PadRight (maxPathLength, ' '));
							crypto.Write (buffer, 0, buffer.Length);
							// Encrypt file body
							input.CopyTo (crypto);
						}
					}
				}
				return newFile;
			}
			catch
			{
				if (newFile.Exists)
				{
					newFile.Delete ();
				}
				throw;
			}
		}

		public FileInfo Decrypt (FileInfo file)
		{
			FileInfo newFile;
			using (FileStream input = new FileStream (file.FullName, FileMode.Open, FileAccess.Read))
			{
				using (Stream crypto = cryptor.Decrypt (input, true))
				{
					// Decrypt file name
					byte[] buffer = new byte[maxPathLength * sizeof (char)];
					int offset = 0;
					while (offset < buffer.Length)
					{
						offset += crypto.Read (buffer, offset, buffer.Length - offset);
					}
					newFile = new FileInfo (Path.Combine (file.DirectoryName, Encoding.Unicode.GetString (buffer).TrimEnd (' ')));
					// Decrypt file body
					try
					{
						using (FileStream output = new FileStream (newFile.FullName, FileMode.Create, FileAccess.Write))
						{
							crypto.CopyTo (output);
						}
					}
					catch
					{
						if (newFile.Exists)
						{
							newFile.Delete ();
						}
						throw;
					}
				}
			}
			return newFile;
		}
	}
}
