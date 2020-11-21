using System.IO;
using System.IO.Compression;

namespace Sem3Lab2
{
	public class GZipFileSettings
	{
		public readonly CompressionLevel compressionLevel;
		public readonly string fileExtension;

		public GZipFileSettings ()
		{
			compressionLevel = CompressionLevel.Optimal;
			fileExtension = ".zip";
		}

		public GZipFileSettings (CompressionLevel compressionLevel)
		{
			this.compressionLevel = compressionLevel;
			fileExtension = ".zip";
		}

		public GZipFileSettings (string fileExtension)
		{
			compressionLevel = CompressionLevel.Optimal;
			this.fileExtension = fileExtension;
		}

		public GZipFileSettings (CompressionLevel compressionLevel, string fileExtension)
		{
			this.compressionLevel = compressionLevel;
			this.fileExtension = fileExtension;
		}
	}

	/// <summary>
	/// Класс предназначен для архивации/деархивации файлов.
	/// </summary>
	public class GZipFile
	{
		private readonly CompressionLevel compressionLevel;
		private readonly string fileExtension;

		public GZipFile (GZipFileSettings settings)
		{
			compressionLevel = settings.compressionLevel;
			fileExtension = settings.fileExtension;
		}

		public FileInfo Compress (FileInfo file)
		{
			FileInfo newFile = new FileInfo (file.FullName + fileExtension);
			try
			{
				using (FileStream input = new FileStream (file.FullName, FileMode.Open, FileAccess.Read))
				{
					using (FileStream output = new FileStream (newFile.FullName, FileMode.Create, FileAccess.Write))
					{
						using (GZipStream zip = new GZipStream (output, compressionLevel))
						{
							input.CopyTo (zip);
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

		public FileInfo Decompress (FileInfo file)
		{
			FileInfo newFile = new FileInfo (
				file.FullName.Remove (
					file.FullName.Length - fileExtension.Length,
					fileExtension.Length
				)
			);
			try
			{
				using (FileStream input = new FileStream (file.FullName, FileMode.Open, FileAccess.Read))
				{
					using (FileStream output = new FileStream (newFile.FullName, FileMode.Create, FileAccess.Write))
					{
						using (GZipStream zip = new GZipStream (input, CompressionMode.Decompress))
						{
							zip.CopyTo (output);
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
	}
}
