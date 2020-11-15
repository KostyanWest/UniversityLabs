using System;
using System.IO;

namespace Sem3Lab2
{
	public class DisposableFileInfo : IDisposable
	{
		public readonly FileInfo fileInfo;

		public DisposableFileInfo (FileInfo fileInfo)
		{
			this.fileInfo = fileInfo;
		}

		public static implicit operator DisposableFileInfo (FileInfo fileInfo)
			=> new DisposableFileInfo (fileInfo);

		public static implicit operator FileInfo (DisposableFileInfo disposableFileInfo)
			=> disposableFileInfo.fileInfo;

		public void Dispose ()
		{
			if (fileInfo.Exists)
			{
				fileInfo.Delete ();
			}
		}
	}
}
