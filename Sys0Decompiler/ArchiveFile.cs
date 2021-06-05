using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Reflection;

namespace Sys0Decompiler
{
    public abstract partial class ArchiveFile
    {
        public abstract string[] SupportedExtensions { get; }

        private static Type[] _descendentTypes = null;
        private static Type[] GetDescenedentTypes()
        {
            if (_descendentTypes != null)
            {
                return _descendentTypes;
            }

            var myType = typeof(ArchiveFile);

            _descendentTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => 
                {
                    var constructor = t.GetConstructor(Type.EmptyTypes);
                    return constructor != null && myType.IsAssignableFrom(t) && t != myType && constructor.IsPublic;
                }
                    ).ToArray();
            return _descendentTypes;
        }

        private static Dictionary<string, List<Type>> _ArchiveTypes;

        public static string[] GetFileExtensions()
        {
            if (_ArchiveTypes == null)
            {
                BuildArchiveTypes();
            }
            return _ArchiveTypes.Keys.ToArray();
        }

        static string _fileFilter;

        public static string GetFileFilter()
        {
            if (_fileFilter == null)
            {
                _fileFilter = GetFileExtensions().Select(ext => "*" + ext).Join(";");
            }
            return _fileFilter;
        }

        public static string GetFileFilter(string currentExtension)
        {
            return "*" + currentExtension + GetFileFilter();
        }

        public static Type[] GetArchiveTypes(string extension)
        {
            if (_ArchiveTypes == null)
            {
                BuildArchiveTypes();
            }
            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }
            extension = extension.ToLowerInvariant();
            return _ArchiveTypes.GetOrNull(extension).ToArray();
        }

        private static void BuildArchiveTypes()
        {
            _ArchiveTypes = new Dictionary<string, List<Type>>();
            var descendentTypes = GetDescenedentTypes();
            foreach (var type in descendentTypes)
            {
                var archiveFile = (ArchiveFile)Activator.CreateInstance(type);
                var supportedExtensions = archiveFile.SupportedExtensions;
                for (int i = 0; i < supportedExtensions.Length; i++)
                {
                    string ext = supportedExtensions[i];
                    if (!ext.StartsWith("."))
                    {
                        ext = "." + ext;
                    }
                    ext = ext.ToLowerInvariant();

                    var list = _ArchiveTypes.GetOrAddNew(ext);
                    list.Add(type);
                }
            }
        }

        private ArchiveFileType GetArchiveFileType(string ext, int version)
        {
            //do we implement a static method for getting a file type from an extension?
            var typeInfo = this.GetType();
            var getArchiveFileTypeMethod = typeInfo.GetMethod("GetArchiveFileType",
                 BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static);
            if (getArchiveFileTypeMethod != null)
            {
                object result = getArchiveFileTypeMethod.Invoke(null, new object[] { ext, version });
                if (result is ArchiveFileType)
                {
                    return (ArchiveFileType)result;
                }
            }
            return ArchiveFileType.Invalid;
        }

        //public static ArchiveFile CreateArchiveFile(ArchiveFileType archiveFileType)
        //{

        //}

        public static bool IsArchiveFileExtension(string ext)
        {
            ext = ext.ToLowerInvariant();
            var types = GetArchiveTypes(ext);
            if (types.Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static ArchiveFile CreateArchiveFile(string ext, int version)
        {
            ext = ext.ToLowerInvariant();
            var types = GetArchiveTypes(ext);

            if (types != null)
            {
                foreach (var type in types)
                {
                    var archiveFile = (ArchiveFile)Activator.CreateInstance(type);
                    var archiveFileType = archiveFile.GetArchiveFileType(ext, version);
                    if (archiveFileType != ArchiveFileType.Invalid)
                    {
                        archiveFile.FileType = archiveFileType;
                    }

                    return archiveFile;
                }
            }
            return null;
        }

        public static ArchiveFile ReadArchiveFile(string fileName)
        {
            //try the preferred types first
            string ext = Path.GetExtension(fileName).ToLowerInvariant();
            var types = GetArchiveTypes(ext);

            if (types != null)
            {
                foreach (var type in types)
                {
                    var archiveFile = (ArchiveFile)Activator.CreateInstance(type);
                    archiveFile.ArchiveFileName = fileName;
                    if (archiveFile.ReadFile(fileName))
                    {
                        archiveFile.InitializeIndexAndParents();
                        return archiveFile;
                    }
                }
            }

            //try every archive type
            var descendentTypes = GetDescenedentTypes();
            foreach (var type in descendentTypes)
            {
                var archiveFile = (ArchiveFile)Activator.CreateInstance(type);
                archiveFile.ArchiveFileName = fileName;
                if (archiveFile.ReadFile(fileName))
                {
                    archiveFile.InitializeIndexAndParents();
                    return archiveFile;
                }
            }
            return null;
        }

        public ArchiveFileType FileType
        {
            get
            {
                return fileType;
            }
            internal set
            {
                fileType = value;
            }
        }

        ArchiveFileType fileType;

        public string ArchiveFileName;
        public ArchiveFileEntryCollection FileEntries = new ArchiveFileEntryCollection();
        public int FileLetter;

        public abstract bool ReadFile(string fileName);
        //{
        //    ReadFileImpl(fileName);
        //}

        public abstract void SaveToFile(string fileName);
        //{
        //    SaveToFileImpl(fileName);
        //}

        //partial void ReadFileImpl(string fileName);
        //partial void SaveToFileImpl(string outputFileName);

        public static int PadToLength(int value, int padSize)
        {
            return (value + (padSize - 1)) & ~(padSize - 1);
        }

        public static long PadToLength(long value, long padSize)
        {
            return (value + (padSize - 1)) & ~(padSize - 1);
        }

        public static void SetStreamLength(Stream stream, int newSize)
        {
            stream.Position = stream.Length;
            if (newSize < stream.Length)
            {
                stream.SetLength(newSize);
            }
            else
            {
                if (stream.Length < newSize)
                {
                    stream.WriteZeroes(newSize - (int)stream.Length);
                }
            }
        }

        public static void SetStreamLength(Stream stream, long newSize)
        {
            stream.Position = stream.Length;
            if (newSize < stream.Length)
            {
                stream.SetLength(newSize);
            }
            else
            {
                if (stream.Length < newSize)
                {
                    stream.WriteZeroes(newSize - stream.Length);
                }
            }
        }

        public void SaveFileAndCommit()
        {
            SaveFileAndCommit(this.ArchiveFileName);
        }

        public virtual void SaveFileAndCommit(string newFileName)
        {
            string tempFile = GetTempFileName(newFileName);
            SaveToFile(tempFile);
            CommitTempFile(newFileName, tempFile);
        }

        public virtual string GetTempFileName(string newFileName)
        {
            string tempFile = Path.ChangeExtension(newFileName, ".$$$");
            return tempFile;
        }

        public void CommitTempFile()
        {
            CommitTempFile(this.ArchiveFileName, GetTempFileName(this.ArchiveFileName));
        }

        public virtual void CommitTempFile(string newFileName, string tempFile)
        {
            this.ArchiveFileName = newFileName;
            if (File.Exists(newFileName))
            {
                File.Delete(newFileName);
            }
            File.Move(tempFile, newFileName);
        }

        public void SaveTempFile()
        {
            SaveToFile(GetTempFileName(this.ArchiveFileName));
        }

        public void UpdateFileHeaders()
        {
            for (int i = 0; i < this.FileEntries.Count; i++)
            {
                var entry = this.FileEntries[i];
                entry.UpdateFileHeader();
            }
        }

        public void InitializeIndexAndParents()
        {
            for (int i = 0; i < this.FileEntries.Count; i++)
            {
                var entry = this.FileEntries[i];
                entry.UpdateFileHeader();
                entry.Index = i;
                entry.Parent = this;
            }
        }

        private ArchiveFileCollection _parent;
        public ArchiveFileCollection Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                _parent = value;
            }
        }
    }
}
