using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using System.Runtime.InteropServices;

using Cheng.Memorys;
using Cheng.IO;
using UnityEngine.Rendering;

namespace Cheng.Streams.Parsers.Default.Unitys
{

    [Obsolete("未完成")]
    internal unsafe sealed class UnityParserMeshData : CustomParser
    {

        #region 派生

        public override Type GetObjectType()
        {
            return typeof(Mesh.MeshData);
        }

        public override object ReadObject(Stream stream)
        {
            throw new NotImplementedException();
        }

        public override void WriteObject(object obj, Stream stream)
        {
            throw new NotImplementedException();
        }

        #endregion

    }


}
