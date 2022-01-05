using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using DokanNet;
using Microsoft.Win32;
using FileAccess = DokanNet.FileAccess;

namespace RegistryFS
{
    internal class RFS : IDokanOperations
    {
        #region DokanOperations member

        //private readonly Dictionary<string, RegistryKey> TopDirectory;
        private List<string> TopDirectory;

        public RFS()
        {
            var ftpWrapper = new FtpWrapper("ftp://archive.ubuntu.com/");
            TopDirectory = new List<string>();
            ftpWrapper.getContents(ref TopDirectory);

            /*foreach (var item in list)
            {
                yield return new ShellFolder(this, new StringKeyShellItemId(item));
            }*/
            /*TopDirectory = new Dictionary<string, RegistryKey>
            {
                ["ClassesRoot"] = Registry.ClassesRoot,
                ["CurrentUser"] = Registry.CurrentUser,
                ["CurrentConfig"] = Registry.CurrentConfig,
                ["LocalMachine"] = Registry.LocalMachine,
                ["Users"] = Registry.Users
            };*/
        }

        public void Cleanup(string filename, IDokanFileInfo info)
        {
        }

        public void CloseFile(string filename, IDokanFileInfo info)
        {
        }

        public NtStatus CreateFile(
            string filename,
            FileAccess access,
            FileShare share,
            FileMode mode,
            FileOptions options,
            FileAttributes attributes,
            IDokanFileInfo info)
        {
            if (filename.Contains("testFIle")) {
                if (info.IsDirectory)
                {
                    return DokanResult.NotADirectory;
                }
                if (mode == FileMode.Open)
                {
                    return DokanResult.Success;
                }
                else if (mode == FileMode.OpenOrCreate || mode == FileMode.Create)
                {
                    return DokanResult.AlreadyExists;
                }
                return DokanResult.AlreadyExists; 
            }
            else return DokanResult.Success;
                    
           if (info.IsDirectory && mode == FileMode.CreateNew)
                 return DokanResult.AlreadyExists;
            if (!info.IsDirectory /*&& mode == FileMode.Open*/)
            {
                return DokanResult.AlreadyExists;
            }
            return DokanResult.Success;
           
        }

        public NtStatus DeleteDirectory(string filename, IDokanFileInfo info)
        {
            return DokanResult.Error;
        }

        public NtStatus DeleteFile(string filename, IDokanFileInfo info)
        {
            return DokanResult.Error;
        }

        private RegistryKey GetRegistoryEntry(string name)
        {
            /*Console.WriteLine($"GetRegistoryEntry : {name}");
            var top = name.IndexOf('\\', 1) - 1;
            if (top < 0)
                top = name.Length - 1;

            var topname = name.Substring(1, top);
            var sub = name.IndexOf('\\', 1);

            if (TopDirectory.ContainsKey(topname))
            {
                if (sub == -1)
                    return TopDirectory[topname];
                else
                    return TopDirectory[topname].OpenSubKey(name.Substring(sub + 1));
            }*/
            return null;
        }

        public NtStatus FlushFileBuffers(
            string filename,
            IDokanFileInfo info)
        {
            return DokanResult.Error;
        }

        public NtStatus FindFiles(
            string filename,
            out IList<FileInformation> files,
            IDokanFileInfo info)
        {
            files = new List<FileInformation>();

             if (filename == "\\")
             {
                 foreach (var name in TopDirectory)
                 {
                     var finfo = new FileInformation
                     {
                         FileName = name,
                         Attributes = FileAttributes.Directory,
                         LastAccessTime = DateTime.Now,
                         LastWriteTime = null,
                         CreationTime = null
                     };
                     files.Add(finfo);
                 }
                 return DokanResult.Success;
             }
             else
             {
                 var finfo = new FileInformation
                 {
                     FileName = "testFIle.txt",
                     Attributes = FileAttributes.Normal,
                     LastAccessTime = DateTime.Now,
                     LastWriteTime = null,
                     CreationTime = null
                 };
                 files.Add(finfo);
            }
            /*else
            {
                var key = GetRegistoryEntry(filename);
                if (key == null)
                    return DokanResult.Error;
                foreach (var name in key.GetSubKeyNames())
                {
                    var finfo = new FileInformation
                    {
                        FileName = name,
                        Attributes = FileAttributes.Directory,
                        LastAccessTime = DateTime.Now,
                        LastWriteTime = null,
                        CreationTime = null
                    };
                    files.Add(finfo);
                }
                foreach (var name in key.GetValueNames())
                {
                    var finfo = new FileInformation
                    {
                        FileName = name,
                        Attributes = FileAttributes.Normal,
                        LastAccessTime = DateTime.Now,
                        LastWriteTime = null,
                        CreationTime = null
                    };
                    files.Add(finfo);
                }
                return DokanResult.Success;
            }*/
            return DokanResult.Success;
        }

  

        public NtStatus GetFileInformation(
            string filename,
            out FileInformation fileinfo,
            IDokanFileInfo info)
        {
            fileinfo = new FileInformation {FileName = filename};
            if (filename.Contains("testFIle"))
            {
                fileinfo.Attributes = FileAttributes.Normal;
                fileinfo.LastAccessTime = DateTime.Now;
                fileinfo.LastWriteTime = null;
                fileinfo.CreationTime = null;
                fileinfo.Length = 49;//3135716;
                return DokanResult.Success;
            }
            else if (filename == "\\")
            {
                fileinfo.Attributes = FileAttributes.Directory;
                fileinfo.LastAccessTime = DateTime.Now;
                fileinfo.LastWriteTime = null;
                fileinfo.CreationTime = null;

                return DokanResult.Success;
            }


            fileinfo.Attributes = FileAttributes.Directory;
            fileinfo.LastAccessTime = DateTime.Now;
            fileinfo.LastWriteTime = null;
            fileinfo.CreationTime = null;

            return DokanResult.Success;

        }

        public NtStatus LockFile(
            string filename,
            long offset,
            long length,
            IDokanFileInfo info)
        {
            return DokanResult.Success;
        }

        public NtStatus MoveFile(
            string filename,
            string newname,
            bool replace,
            IDokanFileInfo info)
        {
            return DokanResult.Error;
        }

        public NtStatus ReadFile(
            string filename,
            byte[] buffer,
            out int readBytes,
            long offset,
            IDokanFileInfo info)
        {
            if (info.IsDirectory)
            {
                readBytes = 0;
                return DokanResult.NotImplemented;
            }
            else
            {
                if (filename.Contains("desktop.ini"))
                {
                    readBytes = 0;
                    return DokanResult.NotImplemented;
                }
                /*string text = "M";

                //ASCII エンコード
                byte[] data = System.Text.Encoding.ASCII.GetBytes(text);

                Array.Copy(data, buffer, text.Length);
                readBytes = text.Length;*/
                try
                {
                    System.IO.FileStream fs = new System.IO.FileStream(
                        @"C:\jerryscript\test.txt",
                            System.IO.FileMode.Open,
                            System.IO.FileAccess.Read);
                    //ファイルを読み込むバイト型配列を作成する
                    byte[] raw = new byte[fs.Length];
                    //ファイルの内容をすべて読み込む
                    fs.Read(raw, 0, (int)fs.Length);
                    //閉じる
                    fs.Close();
                    if (raw == null)
                    {
                        readBytes = 0;
                        return DokanResult.Success;
                    }

                    // 超過チェック
                    if (offset >= raw.Length)
                    {
                        readBytes = 0;
                        return DokanResult.Error;
                    }

                    // offsetから読み込むバイト数
                    long read_try_byte = (raw.Length - offset < buffer.Length) ? raw.Length - offset : buffer.Length;

                    // コピー
                    int i = 0;
                    for (i = 0; i < read_try_byte; i++)
                    {
                        buffer[i] = raw[offset + i];
                    }

                    readBytes = (int)i;

                    return DokanResult.Success;
                }
                 catch (Exception)
                {
                    readBytes = 0;
                    return DokanResult.Error;
                }
                return DokanResult.Success;
            }


        }

        public NtStatus SetEndOfFile(string filename, long length, IDokanFileInfo info)
        {
            return DokanResult.Error;
        }

        public NtStatus SetAllocationSize(string filename, long length, IDokanFileInfo info)
        {
            return DokanResult.Error;
        }

        public NtStatus SetFileAttributes(
            string filename,
            FileAttributes attr,
            IDokanFileInfo info)
        {
            return DokanResult.Error;
        }

        public NtStatus SetFileTime(
            string filename,
            DateTime? ctime,
            DateTime? atime,
            DateTime? mtime,
            IDokanFileInfo info)
        {
            return DokanResult.Error;
        }

        public NtStatus UnlockFile(string filename, long offset, long length, IDokanFileInfo info)
        {
            return DokanResult.Success;
        }

        public NtStatus Mounted(string mountPoint, IDokanFileInfo info)
        {
            return DokanResult.Success;
        }

        public NtStatus Unmounted(IDokanFileInfo info)
        {
            return DokanResult.Success;
        }

        public NtStatus GetDiskFreeSpace(
            out long freeBytesAvailable,
            out long totalBytes,
            out long totalFreeBytes,
            IDokanFileInfo info)
        {
            freeBytesAvailable = 512*1024*1024;
            totalBytes = 1024*1024*1024;
            totalFreeBytes = 512*1024*1024;
            return DokanResult.Success;
        }

        public NtStatus WriteFile(
            string filename,
            byte[] buffer,
            out int writtenBytes,
            long offset,
            IDokanFileInfo info)
        {
            writtenBytes = 0;
            return DokanResult.Error;
        }

        public NtStatus GetVolumeInformation(out string volumeLabel, out FileSystemFeatures features,
            out string fileSystemName, out uint maximumComponentLength, IDokanFileInfo info)
        {
            volumeLabel = "RFS";
            features = FileSystemFeatures.None;
            fileSystemName = string.Empty;
            maximumComponentLength = 256;
            return DokanResult.Error;
        }

        public NtStatus GetFileSecurity(string fileName, out FileSystemSecurity security, AccessControlSections sections,
            IDokanFileInfo info)
        {
            security = null;
            return DokanResult.Error;
        }

        public NtStatus SetFileSecurity(string fileName, FileSystemSecurity security, AccessControlSections sections,
            IDokanFileInfo info)
        {
            return DokanResult.Error;
        }

        public NtStatus EnumerateNamedStreams(string fileName, IntPtr enumContext, out string streamName,
            out long streamSize, IDokanFileInfo info)
        {
            streamName = string.Empty;
            streamSize = 0;
            return DokanResult.NotImplemented;
        }

        public NtStatus FindStreams(string fileName, out IList<FileInformation> streams, IDokanFileInfo info)
        {
            streams = new FileInformation[0];
            return DokanResult.NotImplemented;
        }

        public NtStatus FindFilesWithPattern(string fileName, string searchPattern, out IList<FileInformation> files,
            IDokanFileInfo info)
        {
            files = new List<FileInformation>();

            if (fileName == "\\")
            {
                var topDirArray = TopDirectory.ToArray();
                IEnumerable<string> matched = topDirArray.Where(o => o.Contains(searchPattern));
                foreach (var name in matched)
                {
                    var finfo = new FileInformation
                    {
                        FileName = name,
                        Attributes = FileAttributes.Directory,
                        LastAccessTime = DateTime.Now,
                        LastWriteTime = null,
                        CreationTime = null
                    };
                    files.Add(finfo);
                }
                return DokanResult.Success;
            }
            else
            {
                if (searchPattern.Contains("testFIle.txt"))
                {
                    var finfo = new FileInformation
                    {
                        FileName = "testFIle.txt",
                        Attributes = FileAttributes.Normal,
                        LastAccessTime = DateTime.Now,
                        LastWriteTime = null,
                        CreationTime = null,
                        Length = 49,
                    };
                    files.Add(finfo);
                }
                else
                {
                    return DokanResult.FileNotFound;
                }
                return DokanResult.Success;
            }
        }

        #endregion DokanOperations member
    }

    internal class Program
    {
        private static void Main()
        {
            try
            {
                var rfs = new RFS();
                rfs.Init();
                rfs.Mount("C:\\jerryscript\\mount", DokanOptions.DebugMode | DokanOptions.StderrOutput);
                rfs.Shutdown();
                Console.WriteLine(@"Success");
            }
            catch (DokanException ex)
            {
                Console.WriteLine(@"Error: " + ex.Message);
            }
        }
    }
}