using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Sys0Decompiler
{
    public class ArchiveFileEntryCollection : Collection<ArchiveFileEntry>
    {
        //public ArchiveFile Parent;
        
        //public ArchiveFileEntryCollection(ArchiveFile parent)
        //{
        //    this.Parent = parent;
        //}


        //protected override void InsertItem(int index, ArchiveFileEntry item)
        //{
        //    base.InsertItem(index, item);
        //    if (item != null)
        //    {
        //        item.Parent = this.Parent;
        //    }
        //}

        //protected override void RemoveItem(int index)
        //{
        //    base.RemoveItem(index);
        //}
    }

    public partial class ArchiveFileEntry : IWithIndex, IWithParent<ArchiveFile>
    {
        public ArchiveFileEntry Clone()
        {
            var clone = (ArchiveFileEntry)MemberwiseClone();
            if (clone.FileHeader != null)
            {
                clone.FileHeader = (byte[])clone.FileHeader.Clone();
            }
            return clone;
        }

        public ArchiveFileEntry()
        {

        }
        public ArchiveFile Parent
        {
            get;
            set;
        }
        public string FileName
        {
            get;
            set;
        }
        public int FileLetter
        {
            get;
            set;
        }
        public int Index
        {
            get;
            set;
        }
        public int FileNumber
        {
            get;
            set;
        }
        public long FileSize
        {
            get;
            set;
        }
        public long FileAddress
        {
            get;
            set;
        }
        public long HeaderAddress
        {
            get;
            set;
        }
        public byte[] FileHeader
        {
            get;
            set;
        }
        public byte[] GetFileData()
        {
            return GetFileData(false);
        }

        public byte[] GetFileData(bool doNotConvert)
        {
            var ms = new MemoryStream();
            WriteDataToStream(ms, doNotConvert);
            return ms.ToArray();
        }

        public Stream GetFileStream()
        {
            if (this.ReplacementBytes != null)
            {
                return new MemoryStream(this.ReplacementBytes);
            }
            if (File.Exists(this.ReplacementFileName))
            {
                return new FileStream(this.ReplacementFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }
            if (this.Parent != null && File.Exists(Parent.ArchiveFileName))
            {
                var fs = new FileStream(this.Parent.ArchiveFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                fs.Position = this.FileAddress;
                return fs;
            }
            return null;
        }

        public object Tag
        {
            get;
            set;
        }

        public object ExtraInformation
        {
            get;
            set;
        }

        public override string ToString()
        {
            return "FileNumber: " + this.FileNumber + " FileName: " + this.FileName + " Index: " + this.Index;
        }

        static Encoding shiftJis = Encoding.GetEncoding("shift_jis");

        public void UpdateFileHeader()
        {
            UpdateFileHeaderImpl();
        }

        partial void UpdateFileHeaderImpl();

        public void WriteDataToStream(Stream stream)
        {
            WriteDataToStream(stream, false);
        }

        partial void CheckForSubImages(ref bool hasSubImages);

        public void WriteDataToStream(Stream stream, bool doNotConvert)
        {
            bool hasSubImages = false;
            CheckForSubImages(ref hasSubImages);

            if (hasSubImages)
            {
                WriteDataToStreamSubImages(stream, doNotConvert);
                return;
            }
            else
            {
                WriteDataToStream2(stream, doNotConvert);
            }
        }

        partial void WriteDataToStreamSubImages(Stream stream, bool doNotConvert);

        private void WriteDataToStream2(Stream stream, bool doNotConvert)
        {
            long streamStartPosition = stream.Position;
            bool handled = false;
            if (GetReplacementFileData != null)
            {
                var eventArgs = new GetReplacementFileDataEventArgs();
                eventArgs.OutputStream = stream;
                GetReplacementFileData(this, eventArgs);
                handled = eventArgs.Handled;
            }
            if (handled)
            {

            }
            else if (!String.IsNullOrEmpty(ReplacementFileName))
            {
                WriteReplacementFile(stream, doNotConvert);
            }
            else if (ReplacementBytes != null)
            {
                stream.Write(this.ReplacementBytes, 0, this.ReplacementBytes.Length);
            }
            else
            {
                bool wroteOriginalSubImage = false;
                TryWriteOriginalSubImage(stream, ref wroteOriginalSubImage);

                if (wroteOriginalSubImage)
                {
                    return;
                }

                if (Parent != null)
                {
                    //write the original section of the archive file
                    using (var fs = new FileStream(Parent.ArchiveFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        var br = new BinaryReader(fs);
                        fs.Position = this.FileAddress;
                        fs.WriteToStream(stream, FileSize);
                    }
                }
                else
                {
                    bool wroteModifiedSubImage = false;
                    TryWriteModifiedSubImage(stream, doNotConvert, ref wroteModifiedSubImage);
                    if (wroteModifiedSubImage)
                    {
                        return;
                    }
                }
            }
        }

        partial void TryWriteOriginalSubImage(Stream stream, ref bool wroteOriginalSubImage);
        partial void TryWriteModifiedSubImage(Stream stream, bool doNotConvert, ref bool wroteModifiedSubImage);

        private void WriteReplacementFile(Stream stream, bool doNotConvert)
        {
            bool wroteReplacementImage = false;

            if (doNotConvert || !wroteReplacementImage)
            {
                using (var fs = new FileStream(ReplacementFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    fs.WriteToStream(stream);
                }
                return;
            }
        }

        partial void GetParentFileEntry(ref ArchiveFileEntry parentEntry);
        partial void FixFileExtension(ref string extension);

        private byte[] GetOriginalBytes()
        {
            return GetOriginalBytes((int)this.FileSize);
        }

        private byte[] GetOriginalBytes(int byteCount)
        {
            return CallFunctionOnFile((fs) =>
            {
                var br = new BinaryReader(fs);
                return br.ReadBytes(byteCount);
            });
        }

        private T CallFunctionOnFile<T>(Func<Stream, T> actionToTake)
        {
            try
            {
                if (this.FileSize > 0 && this.FileAddress > 0 && File.Exists(Parent.ArchiveFileName))
                {
                    using (var fs = new FileStream(Parent.ArchiveFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        fs.Position = this.FileAddress;
                        if (fs.Position >= 0 && fs.Position < fs.Length)
                        {
                            return actionToTake(fs);
                        }
                        else
                        {
                            if (Debugger.IsAttached)
                            {
                                Debugger.Break();
                            }
                            return default(T);
                        }
                    }
                }
                else
                {
                    return default(T);
                }
            }
            catch
            {
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
                return default(T);
            }
        }

        public class GetReplacementFileDataEventArgs : EventArgs
        {
            public Stream OutputStream;
            public bool Handled;
        }

        public event EventHandler<GetReplacementFileDataEventArgs> GetReplacementFileData;
        string _replacementFileName;
        public string ReplacementFileName
        {
            get
            {
                return _replacementFileName;
            }
            set
            {
                if (_replacementFileName != value)
                {
                    _replacementFileName = value;
                    ClearSubImages();
                }
            }
        }

        partial void SetReplacementFileNameJpegCheck();
        partial void ClearSubImages();

        byte[] _replacementBytes;
        public byte[] ReplacementBytes
        {
            get
            {
                return _replacementBytes;
            }
            set
            {
                _replacementBytes = value;
            }
        }

        #region IWithParent Members

        object IWithParent.Parent
        {
            get
            {
                return Parent;
            }
            set
            {
                Parent = value as ArchiveFile;
            }
        }

        #endregion

        public bool HasReplacementData()
        {
            return (!String.IsNullOrEmpty(this.ReplacementFileName)) || (this.GetReplacementFileData != null) || (this.ReplacementBytes != null);
        }
    }
}
