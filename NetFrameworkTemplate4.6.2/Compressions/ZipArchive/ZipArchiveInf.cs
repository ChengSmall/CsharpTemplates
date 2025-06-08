using System;
using System.IO.Compression;

namespace Cheng.Algorithm.Compressions.Systems
{

    internal sealed class ZipArchiveInf : DataInformation
    {
        public ZipArchiveInf(ZipArchiveEntry entry)
        {
            p_entry = entry;
        }

        internal ZipArchiveEntry p_entry;

        public override string DataPath
        {
            get
            {
                if (p_entry?.Archive is null)
                {
                    return null;
                }
                return p_entry.FullName;
            }
        }

        public override string DataName
        {
            get
            {
                if (p_entry?.Archive is null)
                {
                    return null;
                }
                return p_entry.Name;
            }
        }

        public override long BeforeSize
        {
            get
            {
                if ((p_entry?.Archive) is null) return -1;
                try
                {
                    return p_entry.Length;
                }
                catch (Exception)
                {
                    return -1;
                }
            }
        }

        public override long CompressedSize
        {
            get
            {
                if ((p_entry?.Archive) is null) return -1;
                try
                {
                    return p_entry.CompressedLength;
                }
                catch (Exception)
                {
                    return -1;
                }
            }
        }

        public override DateTime? ModifiedTime
        {
            get
            {
                if ((p_entry?.Archive) == null) return null;
                try
                {
                    var time = p_entry.LastWriteTime;
                    return time.DateTime;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

    }

}
