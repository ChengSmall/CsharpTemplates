using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using System.IO.Compression;

using Cheng.Streams;
using Cheng.Memorys;
using Cheng.Algorithm.Compressions;
using Cheng.Algorithm.Compressions.Systems;


namespace Cheng.DEBUG
{


    public static class ZipDec
    {


        /// <summary>
        /// 解压缩指定目录下每一个zip到压缩文件所在目录
        /// </summary>
        /// <param name="baseDirectory">根文件夹</param>
        /// <param name="isDeleteZip">每当解压完毕一个zip后是否删除</param>
        /// <param name="print">是否打印解压过程</param>
        public static void ToZipALLComp(string baseDirectory, bool isDeleteZip, bool print)
        {

            if (!Directory.Exists(baseDirectory))
            {
                if (print) $"目录\"{baseDirectory}\"不存在".printl();
                return;
            }

            DirectoryInfo direInfo = new DirectoryInfo(baseDirectory);

            var files = direInfo.GetFiles("*.zip", SearchOption.AllDirectories);

            if (files.Length == 0)
            {
                if (print) $"目录\"{baseDirectory}\"没有任何zip压缩文件包".printl();
                return;
            }

            byte[] buffer = new byte[1024 * 16];

            foreach (var fileInfo in files)
            {
                //文件所在目录
                var fileDire = fileInfo.DirectoryName;
                var fullName = fileInfo.FullName;
                if (print) $"解压缩:{fullName}".printl();
                ZipDec.ZipToDirectory(fullName, fileDire, print, buffer);
                if (isDeleteZip)
                {
                    File.Delete(fullName); 
                    if (print) $"删除:{fullName}".printl();
                }
                if (print) "------------------------------------".printl();
            }

        }

        /// <summary>
        /// zip文件解压缩
        /// </summary>
        /// <param name="zipFilePath">zip文件路径</param>
        /// <param name="toDirectory">要解压到的文件夹</param>
        /// <param name="print">是否向控制台打印过程</param>
        /// <param name="buffer">用于解压缩时拷贝数据的缓冲区</param>
        public static void ZipToDirectory(string zipFilePath, string toDirectory, bool print, byte[] buffer)
        {

            if (print) $"正在打开压缩文件:{zipFilePath}".printl();

            using (FileStream file = new FileStream(zipFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if (print) $"初始化压缩文件解析器".printl();
                string dire = toDirectory;

                //ZipArchiveCompress zip = new ZipArchiveCompress(file, ZipArchiveMode.Read);
                //var arc = zip.BaseArchive;

                ZipArchive arc = new ZipArchive(file, ZipArchiveMode.Read, true);

                var ens = arc.Entries;

                if (print) $"开始解压缩文件".printl();

                for (int i = 0; i < ens.Count; i++)
                {

                    var entry = ens[i];

                    //解压到的路径
                    var toPath = Path.Combine(dire, entry.FullName);

                    //解压到的文件夹
                    var toDire = Path.GetDirectoryName(toPath);

                    //创建文件夹
                    if(!Directory.Exists(toDire)) Directory.CreateDirectory(toDire);

                    using (FileStream newFile = new FileStream(toPath, FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        using (var openStream = entry.Open())
                        {
                            openStream.CopyToStream(newFile, buffer);
                        }
                    }

                    if (print) $"将 {entry.FullName} 解压缩到 {toPath}".printl();

                }

                if (print) $"解压缩完毕".printl();

                arc.Dispose();
            }

        }


    }

}
