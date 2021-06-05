using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace Sys0Decompiler
{
    public partial class ArchiveFileCollection
    {
        public string ArchiveFileName
        {
            get
            {
                string firstFileName = knownFileName;
                var firstArchiveFile = this.ArchiveFiles.FirstOrDefault();
                if (firstArchiveFile != null)
                {
                    firstFileName = firstArchiveFile.ArchiveFileName;
                }
                return firstFileName;
            }
        }
        public ArchiveFileType FileType
        {
            get
            {
                if (ArchiveFiles != null && ArchiveFiles.Count >= 1)
                {
                    return ArchiveFiles.FirstOrDefault().FileType;
                }
                return ArchiveFileType.Invalid;
            }
        }
        string knownFileName = "";
        public List<ArchiveFile> ArchiveFiles = new List<ArchiveFile>();
        public List<ArchiveFileEntry> FileEntries = new List<ArchiveFileEntry>();
        public Dictionary<int, ArchiveFileEntry> FileEntriesByNumber = new Dictionary<int, ArchiveFileEntry>();
        public Dictionary<string, ArchiveFileEntry> FileEntriesByName = new Dictionary<string, ArchiveFileEntry>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Recreates the FileEntries collection from the archive files contained within this file collection
        /// </summary>
        public void Refresh()
        {
            this.FileEntries.Clear();
            foreach (var archiveFile in this.ArchiveFiles.ToArray())
            {
                int fileLetter = archiveFile.FileLetter;
                for (int i = 0; i < archiveFile.FileEntries.Count; i++)
                {
                    var entry = archiveFile.FileEntries[i];
                    if (entry.FileLetter != fileLetter)
                    {
                        archiveFile.FileEntries.RemoveAt(i);
                        i--;
                        var destArchiveFile = GetArchiveFileByLetter(entry.FileLetter);
                        destArchiveFile.FileEntries.Add(entry);
                    }
                }
            }

            UpdateIndexes();
            foreach (var archiveFile in this.ArchiveFiles)
            {
                this.FileEntries.AddRange(archiveFile.FileEntries);
            }
        }

        public void UpdateIndexes()
        {
            this.FileEntriesByNumber.Clear();
            this.FileEntriesByName.Clear();
            foreach (var archiveFile in this.ArchiveFiles)
            {
                for (int i = 0; i < archiveFile.FileEntries.Count; i++)
                {
                    var entry = archiveFile.FileEntries[i];
                    entry.Index = i;
                    FileEntriesByNumber[entry.FileNumber] = entry;
                    FileEntriesByName[entry.FileName] = entry;
                }
            }
        }

        partial void ReadMultipleFiles(string firstArchiveFileName, ref bool success);

        public ArchiveFile GetArchiveFileByLetter(int fileLetter)
        {
            return GetArchiveFileByLetter(fileLetter, true);
        }

        public ArchiveFile GetArchiveFileByLetter(int fileLetter, bool create)
        {
            string firstFileName = this.ArchiveFileName;
            if (String.IsNullOrEmpty(firstFileName))
            {
                //throw new InvalidOperationException();
            }

            var archiveFile = this.ArchiveFiles.Where(f => f.FileLetter == fileLetter).FirstOrDefault();
            if (archiveFile == null && create)
            {
                var archiveType = typeof(AldArchiveFile);
                var firstFile = this.ArchiveFiles.FirstOrDefault();
                if (firstFile != null)
                {
                    archiveType = firstFile.GetType();
                }
                archiveFile = (ArchiveFile)Activator.CreateInstance(archiveType);
                archiveFile.FileType = this.FileType;
                archiveFile.FileLetter = fileLetter;
                archiveFile.Parent = this;
                archiveFile.ArchiveFileName = GetArchiveFileName(firstFileName, fileLetter);
                this.ArchiveFiles.Add(archiveFile);
                this.ArchiveFiles.Sort((f1, f2) => f1.FileLetter - f2.FileLetter);
            }
            return archiveFile;
        }

        public string GetArchiveFileName(string fileName, int fileLetter)
        {
            string outputFileName = null;
            GetAldArchiveFileName(fileName, fileLetter, ref outputFileName);
            if (outputFileName != null)
            {
                return outputFileName;
            }
            return fileName;
        }

        partial void GetAldArchiveFileName(string fileName, int fileLetter, ref string outputFileName);

        public void SaveFile(string fileName)
        {
            ArchiveFile aFile = this.ArchiveFiles.FirstOrDefault();
            if (aFile.FileType == ArchiveFileType.AldFile || aFile.FileType == ArchiveFileType.DatFile)
            {
                aFile = GetArchiveFileByLetter(1);
            }
            foreach (var archiveFile in this.ArchiveFiles)
            {
                archiveFile.UpdateFileHeaders();
            }

            var aldFile = aFile as AldArchiveFile;
            if (aldFile != null)
            {
                aldFile.BuildIndexBlock(GetEntries());
            }
            string[] newFileNames;
            string[] tempFileNames;
            GetOutputAndTempFileNames(fileName, aFile.FileType, out newFileNames, out tempFileNames);

            for (int i = 0; i < this.ArchiveFiles.Count; i++)
            {
                var archiveFile = this.ArchiveFiles[i];
                string newFileName = newFileNames[i];
                string tempFile = tempFileNames[i];
                if (archiveFile.FileType == ArchiveFileType.DatFile)
                {
                    //build full index block for every file for DAT files
                    var aldArchiveFile = archiveFile as AldArchiveFile;
                    if (aldArchiveFile != null)
                    {
                        aldArchiveFile.BuildIndexBlock(GetEntries());
                    }
                }
                archiveFile.SaveToFile(tempFile);
            }

            for (int i = 0; i < this.ArchiveFiles.Count; i++)
            {
                var archiveFile = this.ArchiveFiles[i];
                string newFileName = newFileNames[i];
                string tempFile = tempFileNames[i];
                archiveFile.CommitTempFile(newFileName, tempFile);
            }
        }

        private void GetOutputAndTempFileNames(string fileName, ArchiveFileType fileType, out string[] newFileNames, out string[] tempFileNames)
        {
            newFileNames = ArchiveFiles.Select(f => GetArchiveFileName(fileName, f.FileLetter)).ToArray();
            tempFileNames = ArchiveFiles.Select(f => f.GetTempFileName(GetArchiveFileName(fileName, f.FileLetter))).ToArray();
        }

        private IEnumerable<ArchiveFileEntry> GetEntries()
        {
            foreach (var archiveFile in this.ArchiveFiles)
            {
                foreach (var entry in archiveFile.FileEntries)
                {
                    yield return entry;
                }
            }
        }
    }
}
