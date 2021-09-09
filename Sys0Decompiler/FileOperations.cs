using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;

namespace Sys0Decompiler
{
	/// <summary>
	/// Settings for FileOperations when running in Console Mode
	/// </summary>
	public class ConsoleModeArguments
    {
        /// <summary>
        /// When opening an archive file, use this filename
        /// </summary>
        public string InputArchiveFileName = "";
        /// <summary>
        /// When saving an archive file, use this filename
        /// </summary>
        public string OutputArchiveFileName = "";
        /// <summary>
        /// When importing multiple files from a directory, use this as the input directory
        /// </summary>
        public string ImportDirectory = "";
        /// <summary>
        /// When exporting multiple files to a directory, use this as the output directory
        /// </summary>
        public string ExportDirectory = "";
        /// <summary>
        /// The filter to use when importing multiple files, such as "*.png" or "*.png;*.swf".
        /// </summary>
        public string ImportFileFilter = "*.*";
        /// <summary>
        /// The file extension to use on imported images when they are imported
        /// </summary>
        public string NewImageExtension = null;
        /// <summary>
        /// The file extension to use on imported flash files when they are imported
        /// </summary>
        public string NewFlashExtension = ".aff";
        /// <summary>
        /// Whether or not to keep directory names when importing (for AFA files)
        /// </summary>
        public bool KeepDirectoryNamesWhenImporting = false;
        /// <summary>
        /// A prefix to put at the beginning of files, such as a common direcory (often "Patch\")
        /// </summary>
        public string ImportFilePrefix = "";
        /// <summary>
        /// If set, only imports files if their modification date is after this minimum date
        /// </summary>
        public DateTime minDate = DateTime.MinValue;
        /// <summary>
        /// When creating an archive file, use this version (currently valid for 1 or 2)
        /// </summary>
        public int Version = 1;
    }

    public partial class FileOperations
    {
        //This part of the parital class FileOperations contains prompts and user interface stuff
        
        /// <summary>
        /// Whether the program is running in Console Mode or not, and should read arguments from the ConsoleModeArguments object instead of opening dialog boxes
        /// </summary>
        public bool ConsoleMode;
        /// <summary>
        /// The arguments for Console Mode
        /// </summary>
        public ConsoleModeArguments consoleModeArguments = new ConsoleModeArguments();
    }

    public partial class FileOperations
    {
        public ArchiveFileCollection loadedArchiveFiles;
        public bool DoNotConvertImageFiles = false;
        public bool IncludeDirectoriesWhenExportingFiles = true;
        /// <summary>
        /// When adding new files, allow files which match existing filenames to be added
        /// </summary>
        public bool DuplicateFileNamesAllowed = true;

        public void NewFile(ArchiveFileType desiredFileType, string filenameOverride="")
        {
            ArchiveFile aFile = null;

            loadedArchiveFiles = new ArchiveFileCollection();
            switch (desiredFileType)
            {
                case ArchiveFileType.Afa1File:
                case ArchiveFileType.Afa2File:
                case ArchiveFileType.AldFile:
                case ArchiveFileType.AlkFile:
                case ArchiveFileType.DatFile:
                    aFile = new AldArchiveFile();
                    aFile.FileLetter = 1;
                    break;
                case ArchiveFileType.Invalid:
                    throw new ArgumentException("desiredFileType");
            }
            aFile.FileType = desiredFileType;
            loadedArchiveFiles.ArchiveFiles.Add(aFile);
            aFile.ArchiveFileName = "new";
            aFile.Parent = loadedArchiveFiles;

			if(filenameOverride == "")
			{
				switch(aFile.FileType)
				{
					case ArchiveFileType.AldFile:
						aFile.ArchiveFileName += "GA.ald";
						break;
					case ArchiveFileType.AlkFile:
						aFile.ArchiveFileName += ".alk";
						break;
					case ArchiveFileType.DatFile:
						aFile.ArchiveFileName = "ACG.DAT";
						break;
					case ArchiveFileType.Afa1File:
					case ArchiveFileType.Afa2File:
						aFile.ArchiveFileName += ".afa";
						break;
					case ArchiveFileType.HoneybeeArcFile:
						aFile.ArchiveFileName += ".arc";
						break;
					case ArchiveFileType.SofthouseCharaVfs11File:
					case ArchiveFileType.SofthouseCharaVfs20File:
						aFile.ArchiveFileName += ".vfs";
						break;
				}
			}
			else
			{
				aFile.ArchiveFileName = filenameOverride;
			}
        }

        private static string GetDefaultImageFileType(ArchiveFileType fileType)
        {
            string newImageExtension;
            switch (fileType)
            {
                case ArchiveFileType.DatFile:
                    newImageExtension = ".vsp";
                    break;
                case ArchiveFileType.Afa1File:
                case ArchiveFileType.Afa2File:
                case ArchiveFileType.AldFile:
                case ArchiveFileType.AlkFile:
                case ArchiveFileType.BunchOfFiles:
                    newImageExtension = ".qnt";
                    break;
                case ArchiveFileType.SofthouseCharaVfs11File:
                    newImageExtension = ".iph";
                    break;
                case ArchiveFileType.SofthouseCharaVfs20File:
                    newImageExtension = ".agf";
                    break;
                case ArchiveFileType.HoneybeeArcFile:
                    newImageExtension = ".png";
                    break;
                default:
                    newImageExtension = ".png";
                    break;
            }
            return newImageExtension;
        }

        public bool ImportNewFiles(ArchiveFile archiveFile, IEnumerable<string> fileNames, string basePath, bool keepDirectoryNames)
        {
            if (loadedArchiveFiles == null)
            {
                return false;
            }

            int newFileLetter = archiveFile.FileLetter;
            if (fileNames == null || fileNames.FirstOrDefault() == null)
            {
                return false;
            }

            string prefix = "";
            if (this.ConsoleMode)
            {
                prefix = this.consoleModeArguments.ImportFilePrefix;
            }
            else
            {
                var fileType = this.loadedArchiveFiles.FileType;
            }

            string basePathUpper = basePath.ToUpperInvariant();

            List<ArchiveFileEntry> entriesToAdd = new List<ArchiveFileEntry>();
            foreach (var fileName in fileNames)
            {
                var entry = new ArchiveFileEntry();
                entry.FileAddress = 0;
                entry.FileHeader = null;

                if (keepDirectoryNames)
                {
                    string fileNameWithDirectory = RemovePathPrefix(basePathUpper, Path.GetFullPath(fileName));
                    entry.FileName = prefix + fileNameWithDirectory;
                }
                else
                {
                    entry.FileName = prefix + Path.GetFileName(fileName);
                }

                entry.FileNumber = GetFileNumber(entry.FileName);
                entry.FileLetter = newFileLetter;
                var fileInfo = new FileInfo(fileName);
                entry.FileSize = (int)fileInfo.Length;
                entry.HeaderAddress = 0;
                entry.Index = -1;
                entry.Parent = null;
                //Todo: check MIN-DATE here...
                bool okay = true;
                if (this.ConsoleMode && this.consoleModeArguments.minDate > DateTime.MinValue)
                {
                    if (fileInfo.LastWriteTime.Date < this.consoleModeArguments.minDate)
                    {
                        okay = false;
                    }
                }
                entry.ReplacementFileName = fileName;
                if (okay)
                {
                    entriesToAdd.Add(entry);
                }
            }
            SortFileEntries(entriesToAdd);
            if (archiveFile == null)
            {
                archiveFile = this.loadedArchiveFiles.ArchiveFiles.FirstOrDefault();
                if (archiveFile == null)
                {
                    return false;
                }
            }
            int oldCount = archiveFile.FileEntries.Count;
            if (this.DuplicateFileNamesAllowed)
            {
                archiveFile.FileEntries.AddRange(entriesToAdd);
            }
            else
            {
                Dictionary<string, ArchiveFileEntry> filenameToEntryUppercase = new Dictionary<string, ArchiveFileEntry>();
                foreach (var entry in this.loadedArchiveFiles.FileEntries)
                {
                    filenameToEntryUppercase[entry.FileName.ToUpperInvariant()] = entry;
                }

                foreach (var entry in entriesToAdd)
                {
                    string key = entry.FileName.ToUpperInvariant();
                    if (filenameToEntryUppercase.ContainsKey(key))
                    {
                        var existingEntry = filenameToEntryUppercase[key];
                        existingEntry.ReplacementFileName = entry.ReplacementFileName;
                    }
                    else
                    {
                        archiveFile.FileEntries.Add(entry);
                        filenameToEntryUppercase[key] = entry;
                    }
                }
            }
            for (int i = oldCount; i < archiveFile.FileEntries.Count; i++)
            {
                var entry = archiveFile.FileEntries[i];
                entry.Parent = archiveFile;
                entry.Index = i;
            }

            //archiveFile.FileEntries.AddRange(entriesToAdd);
            loadedArchiveFiles.Refresh();
            return true;
        }

        private static void SortFileEntries(List<ArchiveFileEntry> entriesToAdd)
        {
            if (entriesToAdd == null)
            {
                throw new ArgumentNullException("entriesToAdd");
            }
            entriesToAdd.Sort(CompareArchiveFileEntry);
        }

        static string RemovePathPrefix(string pathPrefixUpper, string path)
        {
            if (path.ToUpperInvariant().StartsWith(pathPrefixUpper))
            {
                return path.Substring(pathPrefixUpper.Length).TrimStart('/', '\\');
            }
            return path;
        }

        public static string[] OnlyNewestFiles(string[] fileNames)
        {
            HashSet<string> BaseNamesSeen = new HashSet<string>();
            HashSet<string> ConflictingNames = new HashSet<string>();

            foreach (var fileName in fileNames)
            {
                string baseName = Path.GetFileName(fileName).ToUpperInvariant();
                if (BaseNamesSeen.Contains(baseName))
                {
                    ConflictingNames.Add(baseName);
                }
                else
                {
                    BaseNamesSeen.Add(baseName);
                }
            }

            List<string> Output = new List<string>();
            Dictionary<string, List<string>> ConflictingFiles = new Dictionary<string, List<string>>();
            foreach (var fileName in fileNames)
            {
                string baseName = Path.GetFileName(fileName).ToUpperInvariant();
                if (ConflictingNames.Contains(baseName))
                {
                    ConflictingFiles.GetOrAddNew(baseName).Add(fileName);
                }
                else
                {
                    Output.Add(fileName);
                }
            }

            foreach (var pair in ConflictingFiles)
            {
                string baseName = pair.Key;
                List<string> names = pair.Value;

                string latestFileName = names.OrderByDescending(fileName => new FileInfo(fileName).LastWriteTimeUtc).FirstOrDefault();
                Output.Add(latestFileName);
            }

            return Output.OrderBy(f => Path.GetFileName(f)).ToArray();
        }

        static int CompareArchiveFileEntry(ArchiveFileEntry me, ArchiveFileEntry other)
        {
            if (me.FileName != null)
            {
                return me.FileName.CompareTo(other.FileName);
            }
            else if (other.FileName == null)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }

        private static int GetFileNumber(string fileName)
        {
            int maxIndex = -1;
            int maxLength = 0;

            int currentIndex = -1;
            int currentLength = 0;
            //find the longest number in the filename
            for (int i = 0; i < fileName.Length; i++)
            {
                char c = fileName[i];
                if (char.IsNumber(c))
                {
                    if (currentLength == 0)
                    {
                        currentIndex = i;
                        currentLength = 1;
                    }
                    else
                    {
                        currentLength++;
                    }
                    if (currentLength > maxLength)
                    {
                        maxIndex = currentIndex;
                        maxLength = currentLength;
                    }
                }
                else
                {
                    currentIndex = -1;
                    currentLength = 0;
                }
            }

            int number = 0;
            if (maxIndex >= 0)
            {
                string substr = fileName.Substring(maxIndex, maxLength);
                if (int.TryParse(substr, out number))
                {

                }
                else
                {
                    number = 0;
                }
            }
            return number;
        }

        public bool SaveAs(string fileName)
        {
            if (Debugger.IsAttached)
            {
                loadedArchiveFiles.SaveFile(fileName);
                return true;
            }
            else
            {
                try
                {
                    loadedArchiveFiles.SaveFile(fileName);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
