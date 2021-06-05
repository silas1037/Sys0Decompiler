using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Sys0Decompiler
{
    using AldUtil = AldArchiveFile.AldUtil;
    using System.Security.Cryptography;
    class AldHeadersCache
    {
        private static AldHeadersCache _defaultInstance = null;
        public static AldHeadersCache DefaultInstance
        {
            get
            {
                if (_defaultInstance == null)
                {
                    _defaultInstance = new AldHeadersCache();
                }
                return _defaultInstance;
            }
        }

        string rootPath;
        public AldHeadersCache()
        {
            Init();
        }

        private void Init()
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appName = Application.ProductName;
            this.rootPath = Path.Combine(Path.Combine(localAppData, appName), "AldHeadersCache");
        }

        private string GetCacheDirectoryName(byte[] sha1Hash)
        {
            if (sha1Hash.Length < 8)
            {
                throw new ArgumentException("sha1Hash");
            }
            long hash8 = BitConverter.ToInt64(sha1Hash, 0);
            string string16 = hash8.ToString("X16");
            return Path.Combine(rootPath, string16);
        }

        public byte[][] GetAldFileHeaders(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return null;
            }

            int fileSize;
            long modificationTimeUtc;
            byte[] sha1Hash;
            GetFileInformation(fileName, out fileSize, out modificationTimeUtc, out sha1Hash);
            string cacheDirectoryName = GetCacheDirectoryName(sha1Hash);

            var headers = ReadAldFileHeadersFromDirectory(cacheDirectoryName, (int)fileSize, modificationTimeUtc, sha1Hash);
            return headers;
        }

        private bool GetFileInformation(string fileName, out int fileSize, out long modificationTimeUtc, out byte[] sha1Hash)
        {
            fileSize = -1;
            modificationTimeUtc = -1;
            sha1Hash = null;

            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                long[] fileAddresses = AldUtil.GetAldFileAddresses(fs);
                if (fileAddresses == null)
                {
                    return false;
                }
                var br = new BinaryReader(fs);
                byte[] aldFileHeader = br.ReadBytes((int)fileAddresses[0]);
                var sha1 = SHA1.Create();
                sha1Hash = sha1.ComputeHash(aldFileHeader);

                fileSize = (int)fs.Length;
                modificationTimeUtc = -1;

                try
                {
                    FileInfo fileInfo = new FileInfo(fileName);
                    modificationTimeUtc = fileInfo.LastWriteTimeUtc.ToBinary();
                }
                catch
                {

                }
            }
            return true;
        }

        private byte[][] ReadAldFileHeadersFromDirectory(string cacheDirectoryName, int fileSize, long modificationTimeUtc, byte[] sha1Hash)
        {
            if (!Directory.Exists(cacheDirectoryName))
            {
                return null;
            }
            string[] fileNames = Directory.GetFiles(cacheDirectoryName, "*.dat");
            foreach (var fileName in fileNames)
            {
                byte[][] headers = ReadAldFileHeaders(fileName, fileSize, modificationTimeUtc, sha1Hash);
                if (headers != null)
                {
                    return headers;
                }
            }
            return null;
        }

        void WriteAldFileHeadersToDirectory(string cacheDirectoryName, int fileSize, long modificaitonTimeUtc, byte[] sha1Hash, byte[][] fileHeaders)
        {
            try
            {
                if (Directory.Exists(cacheDirectoryName))
                {

                }
                else
                {
                    Directory.CreateDirectory(cacheDirectoryName);
                }
                //string 
                string fileName;
                do
                {
                    fileName = Path.Combine(cacheDirectoryName, Path.ChangeExtension(Path.GetRandomFileName(), ".dat"));
                } while (File.Exists(fileName));
                WriteAldFileHeaders(fileName, fileSize, modificaitonTimeUtc, sha1Hash, fileHeaders);
            }
            catch (FileNotFoundException)
            {

            }
            catch (IOException)
            {

            }
            catch (UnauthorizedAccessException)
            {

            }
        }


        void WriteAldFileHeaders(string cacheFileName, int fileSize, long modificationTimeUtc, byte[] sha1Hash, byte[][] fileHeaders)
        {
            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);
            bw.Write((int)0);
            byte[] containerHash = new byte[20];
            bw.Write(containerHash.Length);
            bw.Write(containerHash);
            bw.Write((int)20 + 8 + 4);
            bw.Write(fileSize);
            bw.Write(modificationTimeUtc);
            bw.Write(sha1Hash);
            bw.Write(fileHeaders.Length);
            foreach (var header in fileHeaders)
            {
                bw.Write(header.Length);
                bw.Write(header);
            }
            byte[] bytes = ms.ToArray();
            ms = new MemoryStream(bytes);
            bw = new BinaryWriter(ms);
            bw.Write((int)ms.Length);
            bw.Write(containerHash.Length);
            SHA1 sha1 = SHA1.Create();
            containerHash = sha1.ComputeHash(bytes, 8 + 20, bytes.Length - (8 + 20));
            bw.Write(containerHash);

            if (!Directory.Exists(Path.GetDirectoryName(cacheFileName)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(cacheFileName));
            }
            File.WriteAllBytes(cacheFileName, bytes);
        }

        private byte[][] ReadAldFileHeaders(string cacheFileName, int fileSize, long modificationTimeUtc, byte[] sha1Hash)
        {
            if (sha1Hash.Length != 20)
            {
                throw new ArgumentException("sha1Hash");
            }
            long cacheFileSize;
            byte[] cacheBytes;
            using (var fs = new FileStream(cacheFileName, FileMode.Open, FileAccess.Read))
            {
                cacheFileSize = fs.Length;
                if (cacheFileSize >= 8 * 1024 * 1024)   //8MB limit for cache files
                {
                    return null;
                }
                if (cacheFileSize < 32)
                {
                    return null;
                }
                var br = new BinaryReader(fs);
                cacheBytes = br.ReadBytes((int)fs.Length);
            }
            {
                try
                {
                    var ms = new MemoryStream(cacheBytes);
                    var br = new BinaryReader(ms);

                    int containerFileSize = br.ReadInt32();
                    if (containerFileSize != cacheFileSize)
                    {
                        return null;
                    }

                    int containerHashSize = br.ReadInt32();
                    if (containerHashSize != 20)
                    {
                        return null;
                    }
                    byte[] containerHash = br.ReadBytes(containerHashSize);
                    SHA1 sha1 = SHA1.Create();
                    byte[] bytesHash = sha1.ComputeHash(cacheBytes, (int)ms.Position, (int)(ms.Length - ms.Position));
                    if (!bytesHash.SequenceEqual(containerHash))
                    {
                        return null;
                    }

                    int containerHeaderSize = br.ReadInt32();
                    if (containerHeaderSize != 20 + 8 + 4)
                    {
                        return null;
                    }
                    int aldFileSize = br.ReadInt32();
                    if (aldFileSize != fileSize)
                    {
                        return null;
                    }
                    long dateTimeFromCache = br.ReadInt64();
                    if (modificationTimeUtc != dateTimeFromCache)
                    {
                        return null;
                    }
                    byte[] aldFileHeaderHash = br.ReadBytes(20);
                    if (!aldFileHeaderHash.SequenceEqual(sha1Hash))
                    {
                        return null;
                    }
                    int headerCount = br.ReadInt32();
                    if (headerCount < 0 || headerCount > 65536)
                    {
                        return null;
                    }
                    List<byte[]> headers = new List<byte[]>();
                    for (int i = 0; i < headerCount; i++)
                    {
                        int headerSize = br.ReadInt32();
                        if (headerSize < 0 || headerSize >= 512)
                        {
                            return null;
                        }
                        byte[] header = br.ReadBytes(headerSize);
                        headers.Add(header);
                    }
                    if (br.BaseStream.Position != br.BaseStream.Length)
                    {
                        return null;
                    }
                    return headers.ToArray();
                }
                catch (EndOfStreamException)
                {
                    return null;
                }
            }
        }

        public void SaveAldFileHeaders(string fileName, byte[][] fileHeaders)
        {
            int fileSize;
            long modificationTimeUtc;
            byte[] sha1Hash;
            GetFileInformation(fileName, out fileSize, out modificationTimeUtc, out sha1Hash);

            string cacheDirectoryName = GetCacheDirectoryName(sha1Hash);
            this.WriteAldFileHeadersToDirectory(cacheDirectoryName, fileSize, modificationTimeUtc, sha1Hash, fileHeaders);
        }
    }
}
