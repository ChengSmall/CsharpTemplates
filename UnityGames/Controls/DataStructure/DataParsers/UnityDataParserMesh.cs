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

    /// <summary>
    /// Unity数据解析器---网格<see cref="Mesh"/>
    /// </summary>
    public unsafe sealed class UnityDataParserMesh : CustomParser
    {

        #region 构造

        public UnityDataParserMesh()
        {
            p_buffer = new byte[cp_bufSize];
            p_int16Buf = new List<ushort>();
            p_ver3Buf = new List<Vector3>();
            p_colorBuf = new List<Color32>();
            p_ver4Buf = new List<Vector4>();
        }

        #endregion

        #region 参数

        private const int cp_bufSize = 128;

        private byte[] p_buffer;

        private List<ushort> p_int16Buf;

        private List<Vector3> p_ver3Buf;

        private List<Color32> p_colorBuf;

        private List<Vector4> p_ver4Buf;

        #endregion

        #region 功能

        #region 

        /*

        Vertices (顶点): OK
        mesh.vertices: 定义了网格的顶点位置。

        Normals (法线): OK
        mesh.normals: 定义了每个顶点的法线向量，用于光照计算。

        Tangents (切线): OK
        mesh.tangents: 定义了每个顶点的切线向量，用于光照和纹理映射。
        mesh.binormals: 定义了每个顶点的副法线向量（有时也称为副切线向量）。

        UV 坐标: OK
        mesh.uv: 定义了每个顶点的第一组UV坐标，用于纹理映射。
        mesh.uv2, mesh.uv3, mesh.uv4 等（如果存在）: 定义了额外的UV坐标集。

        Colors (颜色): OK
        mesh.colors: 定义了每个顶点的颜色。

        Triangles (三角形): OK
        mesh.triangles: 定义了构成网格的三角形的顶点索引。

        Bounds (边界框): OK
        mesh.bounds: 定义了网格的轴对齐包围盒（AABB），包含最小点和最大点。

        SubMeshes (子网格): OK
        mesh.subMeshCount: 子网格的数量。
        mesh.GetTriangles(int submeshIndex): 获取指定子网格的三角形索引数组。

        如果有额外的信息（如材质索引），你可能还需要考虑 Mesh.SetSubMesh 方法中的参数。

        Bone Weights (骨骼权重) 和 Bone Indices (骨骼索引):
        mesh.boneWeights: 定义了每个顶点的骨骼权重。
        mesh.boneIndices: 定义了每个顶点对应的骨骼索引。

         */

        #endregion

        #region 读

        #region 封装

#if DEBUG
        /// <summary>
        /// 读取数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
#endif
        private T[] f_readArr<T>(Stream stream) where T : unmanaged
        {
            int length;
            if (!stream.ReadValue(p_buffer, out length)) return null;

            T[] arr = new T[length];

            for (int i = 0; i < length; i++)
            {
                if (!stream.ReadValue<T>(p_buffer, out arr[i])) return null;

            }

            return arr;
        }

#if DEBUG
        /// <summary>
        /// 读取集合并添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <param name="list"></param>
        /// <returns></returns>
#endif
        private bool f_readList<T>(Stream stream, IList<T> list) where T : unmanaged
        {
            int length;
            if (!stream.ReadValue(p_buffer, out length)) return false;

            //T[] arr = new T[length];

            for (int i = 0; i < length; i++)
            {
                T temp;
                if (!stream.ReadValue<T>(p_buffer, out temp)) return false;
                list.Add(temp);
            }

            return true;
        }

#if DEBUG
        /// <summary>
        /// 读取一个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <param name="reValue">要读取到的引用</param>
        /// <returns>是否读取完整</returns>
#endif
        private bool f_readValue<T>(Stream stream, out T reValue) where T : unmanaged
        {
            var rs = stream.ReadBlock(p_buffer, 0, sizeof(T));

            if (rs != sizeof(T))
            {
                reValue = default;
                return false;
            }

            reValue = p_buffer.ToStructure<T>();
            return true;
        }

        #endregion

        public override object ReadObject(Stream stream)
        {

            Mesh mesh = null;
            try
            {
                mesh = new Mesh();

                if (ReadStreamToMesh(stream, mesh, out bool empty))
                {
                    if (empty)
                    {
                        Mesh.Destroy(mesh);
                        return null;
                    }
                    else
                    {
                        return mesh;
                    }
                }

                //读取subMeshCount
                Mesh.Destroy(mesh);
                return null;
            }
            catch (Exception)
            {
                if ((object)mesh != null) Mesh.Destroy(mesh);
                throw;
            }

        }

        /// <summary>
        /// 将指定流的数据读取到<paramref name="mesh"/>，无安全检查
        /// </summary>
        /// <param name="stream">可读的流</param>
        /// <param name="mesh">已引用了实例，并可读写数据的mesh</param>
        /// <param name="isEmpty">是否为空实例</param>
        /// <returns>是否成功读取</returns>
        public bool ReadStreamToMesh(Stream stream, Mesh mesh, out bool isEmpty)
        {
            isEmpty = false;
            int ri;
            ri = stream.ReadByte();

            //空
            if (ri != byte.MaxValue)
            {
                isEmpty = true;
                return true;
            }
            int i;

            //读取subMeshCount

            if (!f_readValue(stream, out int subMeshCount)) return false;

            //读取顶点数据
            if (!f_readList(stream, p_ver3Buf)) return false;
            mesh.SetVertices(p_ver3Buf);
            p_ver3Buf.Clear();

            //写入子网格数
            mesh.subMeshCount = subMeshCount;

            //const MeshUpdateFlags meshUpdataFlags = MeshUpdateFlags.DontRecalculateBounds;
            //循环读取子网格数据

            for (i = 0; i < subMeshCount; i++)
            {
                //读取子网格顶点
                if (!f_readList(stream, p_int16Buf)) return false;

                //读取子网格数据
                if (!f_readValue(stream, out int baseVertex)) return false;

                mesh.SetTriangles(p_int16Buf, i, false, baseVertex);
                //mesh.SetSubMesh(i, resd, meshUpdataFlags);

                p_int16Buf.Clear();
            }


            //获取并写入颜色
            if (!f_readList(stream, p_colorBuf)) return false;
            mesh.SetColors(p_colorBuf);
            p_colorBuf.Clear();

            //获取并写入法线数据
            if (!f_readList(stream, p_ver3Buf)) return false;
            mesh.SetNormals(p_ver3Buf);
            p_ver3Buf.Clear();

            //获取并写入切线数据
            f_readList(stream, p_ver4Buf);
            mesh.SetTangents(p_ver4Buf);
            p_ver4Buf.Clear();

            //循环写入UV数据

            for (i = 0; i < 7; i++)
            {
                if (!f_readList(stream, p_ver3Buf)) return false;

                mesh.SetUVs(i, p_ver3Buf);
                p_ver3Buf.Clear();
            }

            //获取并写入网格边界bounds
            if (!f_readValue(stream, out Bounds bs)) return false;
            mesh.bounds = bs;


            return true;
        }

        #endregion

        #region 写

        /// <summary>
        /// 将网格写入流
        /// </summary>
        /// <param name="mesh">能读写数据的mesh</param>
        /// <param name="stream">可写的流</param>
        public void WriteMeshToStream(Mesh mesh, Stream stream)
        {
            if (mesh == null)
            {
                stream.WriteByte(0);
                return;
            }

            stream.WriteByte(byte.MaxValue);

            var smCount = mesh.subMeshCount;
            //写入subMeshCount 子网格数
            f_writeValue(stream, smCount);

            //获取并写入顶点数据
            mesh.GetVertices(p_ver3Buf);
            f_writerList(p_ver3Buf, stream);

            p_ver3Buf.Clear();

            //循环写入网格数据

            int i;
            for (i = 0; i < smCount; i++)
            {

                //获取子网格顶点
                mesh.GetTriangles(p_int16Buf, i);
                //写入子网格顶点
                f_writerList(p_int16Buf, stream);
                p_int16Buf.Clear();

                //获取子网格数据
                var tsm = mesh.GetSubMesh(i);
                //写入子网格数据
                f_writeValue(stream, tsm.baseVertex);
                //f_writeValue(stream, tsm);
            }


            //获取并写入颜色
            mesh.GetColors(p_colorBuf);
            f_writerList(p_colorBuf, stream);

            p_colorBuf.Clear();

            //获取并写入法线数据
            mesh.GetNormals(p_ver3Buf);
            f_writerList(p_ver3Buf, stream);

            p_ver3Buf.Clear();

            //获取并写入切线数据
            mesh.GetTangents(p_ver4Buf);
            f_writerList(p_ver4Buf, stream);

            p_ver4Buf.Clear();

            //循环写入UV数据

            for (i = 0; i < 7; i++)
            {
                //循环写入1-8的uv
                mesh.GetUVs(i, p_ver3Buf);

                f_writerList(p_ver3Buf, stream);

                p_ver3Buf.Clear();
            }


            //获取并写入网格边界
            var bounds = mesh.bounds;
            f_writeValue(stream, bounds);
        }

        public override void WriteObject(object obj, Stream stream)
        {

            WriteMeshToStream(obj as Mesh, stream);
        }

        #region 封装

#if DEBUG
        /// <summary>
        /// 写入值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <param name="value"></param>
#endif
        private void f_writeValue<T>(Stream stream, T value) where T : unmanaged
        {
            value.ToByteArray(p_buffer);
            stream.Write(p_buffer, 0, sizeof(T));
        }

#if DEBUG
        /// <summary>
        /// 写入数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr">要写入的数组</param>
        /// <param name="stream">要写入的流</param>
#endif
        private void f_writeArr<T>(T[] arr, Stream stream) where T : unmanaged
        {
            //数组元素
            int length = arr.Length;
            stream.WriteValue(p_buffer, length);

            for (int i = 0; i < length; i++)
            {
                stream.WriteValue(p_buffer, arr[i]);
            }
        }

#if DEBUG
        /// <summary>
        /// 写入集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="stream"></param>
#endif
        private void f_writerList<T>(IList<T> list, Stream stream) where T : unmanaged
        {
            int length = list.Count;
            stream.WriteValue(p_buffer, length);

            for (int i = 0; i < length; i++)
            {
                stream.WriteValue(p_buffer, list[i]);
            }
        }

        #endregion

        #endregion

        public override Type GetObjectType()
        {
            return typeof(Mesh);
        }

        #endregion

    }


}
#if DEBUG
#endif