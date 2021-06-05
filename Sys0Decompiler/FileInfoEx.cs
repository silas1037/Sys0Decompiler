using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace Sys0Decompiler
{
    class FileInfoEx
    {
        long fileSize = -1;
        FileInfo fileInfo;
        public FileInfoEx(FileInfo fileInfo)
        {
            this.fileInfo = fileInfo;
        }

        public FileInfoEx(string fileName)
            : this(new FileInfo(fileName))
        {

        }

        public string Name
        {
            get
            {
                return this.fileInfo.Name;
            }
        }

        public long Length
        {
            get
            {
                long length = this.fileInfo.Length;
                try
                {
                    if (length == 0 && 0 != (this.Attributes & FileAttributes.ReparsePoint))
                    {
                        if (this.fileSize != -1)
                        {
                            return this.fileSize;
                        }

                        using (var fileStream = new FileStream(this.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            this.fileSize = fileStream.Length;
                            return this.fileSize;
                        }
                    }
                }
                catch
                {
                    return 0;
                }



                return this.fileInfo.Length;
            }
        }

        public string DirectoryName
        {
            get
            {
                return this.fileInfo.DirectoryName;
            }
        }

        public DirectoryInfo Directory
        {
            get
            {
                return this.fileInfo.Directory;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return this.fileInfo.IsReadOnly;
            }
            set
            {
                this.fileInfo.IsReadOnly = value;
            }
        }

        public bool Exists
        {
            get
            {
                return this.fileInfo.Exists;
            }
        }

        public string FullName
        {
            get
            {
                return this.fileInfo.FullName;
            }
        }

        public string Extension
        {
            get
            {
                return this.fileInfo.Extension;
            }
        }

        public DateTime CreationTime
        {
            get
            {
                return this.fileInfo.CreationTime;
            }
            set
            {
                this.fileInfo.CreationTime = value;
            }
        }

        public DateTime CreationTimeUtc
        {
            get
            {
                return this.fileInfo.CreationTimeUtc;
            }
            set
            {
                this.fileInfo.CreationTimeUtc = value;
            }
        }

        public DateTime LastAccessTime
        {
            get
            {
                return this.fileInfo.LastAccessTime;
            }
            set
            {
                this.fileInfo.LastAccessTime = value;
            }
        }

        public DateTime LastAccessTimeUtc
        {
            get
            {
                return this.fileInfo.LastAccessTimeUtc;
            }
            set
            {
                this.fileInfo.LastAccessTimeUtc = value;
            }
        }

        public DateTime LastWriteTime
        {
            get
            {
                return this.fileInfo.LastWriteTime;
            }
            set
            {
                this.fileInfo.LastWriteTime = value;
            }
        }

        public DateTime LastWriteTimeUtc
        {
            get
            {
                return this.fileInfo.LastWriteTimeUtc;
            }
            set
            {
                this.fileInfo.LastWriteTimeUtc = value;
            }
        }

        public FileAttributes Attributes
        {
            get
            {
                return this.fileInfo.Attributes;
            }
            set
            {
                this.fileInfo.Attributes = value;
            }
        }

        //public FileSecurity GetAccessControl()
        //{
        //    return this.fileInfo.GetAccessControl();
        //}

        //public FileSecurity GetAccessControl(AccessControlSections includeSections)
        //{
        //    return this.fileInfo.GetAccessControl(includeSections);
        //}

        //public void SetAccessControl(FileSecurity fileSecurity)
        //{
        //    this.fileInfo.SetAccessControl(fileSecurity);
        //}

        //public void Decrypt()
        //{
        //    this.fileInfo.Decrypt();
        //}

        //public void Encrypt()
        //{
        //    this.fileInfo.Encrypt();
        //}

        public void Refresh()
        {
            this.fileSize = -1;
            this.fileInfo.Refresh();
        }
    }
}
