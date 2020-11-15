﻿using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace Sem3Lab2
{
	public class DirectoryFileExtractorSettings
	{
		public readonly DirectoryInfo sourceDirectory;
		public readonly DirectoryInfo targetDirectory;
		public readonly string searchExtension;
		public readonly GZipFileSettings compressorSettings;
		public readonly AesFileSettings cryptorSettings;

		public DirectoryFileExtractorSettings
		(
			DirectoryInfo sourceDirectory,
			DirectoryInfo targetDirectory,
			string searchExtension,
			GZipFileSettings compressorSettings,
			AesFileSettings cryptorSettings
		)
		{
			this.sourceDirectory = sourceDirectory;
			this.targetDirectory = targetDirectory;
			this.searchExtension = searchExtension;
			this.compressorSettings = compressorSettings;
			this.cryptorSettings = cryptorSettings;
		}
	}

	public class DirectoryFileExtractor
	{
		private FileSystemWatcher watcher;
		private ConcurrentQueue<FileSystemEventArgs> queue;
		private GZipFile compressor;
		private AesFile cryptor;
		private Action<string> log;
		private Thread workThread;
		public bool IsRunning { get; private set; }
		public bool IsBusy
		{
			get => workThread?.IsAlive ?? false;
		}
		public DirectoryInfo SourceDirectory { get; private set; }
		public DirectoryInfo TargetDirectory { get; private set; }
		public string SearchExtension { get; private set; }

		public DirectoryFileExtractor (DirectoryFileExtractorSettings settings, Action<string> log)
		{
			Setup (settings, log);
		}

		public void Setup (DirectoryFileExtractorSettings settings, Action<string> log)
		{
			if (!IsRunning && !IsBusy)
			{
				SourceDirectory = settings.sourceDirectory;
				if (!SourceDirectory.Exists)
				{
					SourceDirectory.Create ();
				}
				TargetDirectory = settings.targetDirectory;
				SearchExtension = settings.searchExtension;

				watcher?.Dispose ();
				watcher = new FileSystemWatcher
				{
					IncludeSubdirectories = true,
					Path = SourceDirectory.FullName
				};
				queue = new ConcurrentQueue<FileSystemEventArgs> ();

				compressor = new GZipFile (settings.compressorSettings);
				cryptor = new AesFile (settings.cryptorSettings);
				this.log = log;
			}
			else
			{
				throw new InvalidOperationException ("Изменять параметры во время работы нельзя");
			}
		}

		public void StartContinue ()
		{
			if (!IsRunning && !IsBusy)
			{
				IsRunning = true;
				watcher.Created += OnFileCreate;
				watcher.EnableRaisingEvents = true;
				workThread = new Thread (new ThreadStart (Cycle));
				workThread.Start ();
				log?.Invoke ("Служба запущена/возобновлена.");
			}
		}

		public void Pause ()
		{
			if (IsRunning)
			{
				watcher.EnableRaisingEvents = false;
				watcher.Created -= OnFileCreate;
				IsRunning = false;
				log?.Invoke ("Служба приостановлена.");
			}
		}

		public void Stop ()
		{
			Pause ();
			try
			{
				if (IsBusy)
				{
					workThread?.Abort ();
				}
			}
			catch (ThreadStateException) { }
			log?.Invoke ("Служба остановлена.");
		}

		private void Cycle ()
		{
			while (IsRunning)
			{
				if (!queue.IsEmpty)
				{
					if (queue.TryDequeue (out FileSystemEventArgs args))
					{
						string path = args.FullPath;
						if (Path.GetExtension (path) == SearchExtension)
						{
							FileInfo file = new FileInfo (path);
							if (file.Exists)
							{
								try
								{
									Extract (file);
									log?.Invoke ($"Success:\n{path}");
								}
								catch (Exception ex)
								{
									if (ex.HResult == -2147024864)
									{
										//"Процесс не может получить доступ к файлу"
										queue.Enqueue (args);
									}
									else
									{
										log?.Invoke ($"Exception:\n{ex}\n\nFailure:\n{path}");
									}
								}
							}
						}
					}
				}
				else
				{
					Thread.Sleep (500);
				}
			}
		}

		private void Extract (FileInfo file)
		{
			using (DisposableFileInfo zippedFile = compressor.Compress (file))
			{
				using (DisposableFileInfo encryptedFile = cryptor.Encrypt (zippedFile))
				{
					using (DisposableFileInfo movedFile = Move (encryptedFile))
					{
						using (DisposableFileInfo decryptedFile = cryptor.Decrypt (movedFile))
						{
							using (DisposableFileInfo unzippedFile = compressor.Decompress (decryptedFile))
							{
								Store (unzippedFile);
							}
						}
					}
				}
			}
		}

		private FileInfo Move (FileInfo file)
		{
			FileInfo newFile = new FileInfo (Path.Combine (TargetDirectory.FullName, file.Name));
			try
			{
				Directory.CreateDirectory (newFile.DirectoryName);
				if (newFile.Exists)
				{
					newFile.Delete ();
				}
				File.Move (file.FullName, newFile.FullName);
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

		private FileInfo Store (FileInfo file)
		{
			FileInfo newFile = new FileInfo (Path.Combine (file.DirectoryName, "archive", file.Name));
			try
			{
				Directory.CreateDirectory (newFile.DirectoryName);
				if (newFile.Exists)
				{
					newFile.Delete ();
				}
				File.Move (file.FullName, newFile.FullName);
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

		private void OnFileCreate (object sender, FileSystemEventArgs e)
		{
			try
			{
				log?.Invoke ($"Found\n{e.FullPath}");
				queue.Enqueue (e);
			}
			catch (Exception ex)
			{
				log?.Invoke ($"Exception:\n{ex}");
			}
		}
	}
}
