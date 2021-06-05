using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace Sys0Decompiler
{
    public partial class ArchiveFileEntry : IWithIndex, IWithParent<ArchiveFile>
    {
        partial void UpdateFileHeaderImpl()
        {
            var fileType = ArchiveFileType.Invalid;
            if (Parent != null)
            {
                fileType = Parent.FileType;
            }

            if (fileType == ArchiveFileType.AldFile)
            {
                if (FileHeader == null)
                {
                    var fileNameBytes = shiftJis.GetBytes(this.FileName ?? " ");
                    int headerSize = ArchiveFile.PadToLength(16 + fileNameBytes.Length, 16);
                    if (headerSize == 16) headerSize = 32;

                    FileHeader = new byte[headerSize];
                    var ms = new MemoryStream(FileHeader);
                    var bw = new BinaryWriter(ms);
                    bw.Write((int)headerSize);
                    bw.Write((int)0);
                    bw.Write((uint)0x8E5C4430); //???
                    bw.Write((int)0x01C9F639); //???
                    bw.Write((int)0);
                    bw.Write((int)0);
                    bw.Write((int)0);
                    bw.Write((int)0);
                }

                {
                    var ms = new MemoryStream(FileHeader);
                    var bw = new BinaryWriter(ms);

                    byte[] fileNameBytes = shiftJis.GetBytes(this.FileName);
                    ms.Position = 4;
                    bw.Write((int)this.FileSize);
                    ms.Position = 16;
                    int maxFileNameLength = FileHeader.Length - 16;

                    if (fileNameBytes.Length < maxFileNameLength)
                    {
                        fileNameBytes = fileNameBytes.Concat(Enumerable.Repeat((byte)0, maxFileNameLength - fileNameBytes.Length)).ToArray();
                    }
                    else if (fileNameBytes.Length > maxFileNameLength)
                    {
                        fileNameBytes = fileNameBytes.Take(maxFileNameLength).ToArray();
                    }
                    bw.Write(fileNameBytes);
                }
            }

            //if (fileType == ArchiveFileType.SofthouseCharaVfsFile)
            //{
            //    if (FileHeader == null)
            //    {
            //        FileHeader = new byte[32];
            //        FileHeader[0x0D] = 0x20;
            //        FileHeader[0x0E] = 0x00;
            //        //???
            //        FileHeader[0x0F] = 0x81;
            //        FileHeader[0x10] = 0x38;
            //        FileHeader[0x11] = 0x2A;
            //        FileHeader[0x12] = 0x0D;
            //    }
            //    var ms = new MemoryStream(FileHeader);
            //    var bw = new BinaryWriter(ms);

            //    var fileNameBytes = shiftJis.GetBytes(this.FileName ?? " ");
            //    if (fileNameBytes.Length > 12)
            //    {
            //        fileNameBytes = fileNameBytes.Take(12).ToArray();
            //    }
            //    bw.Write(fileNameBytes);
            //    bw.Write((byte)0);
            //    ms.Position = 19;
            //    bw.Write((int)this.FileAddress);
            //    bw.Write((int)this.FileSize);
            //    bw.Write((int)this.FileSize);
            //}
        }
    }
}
