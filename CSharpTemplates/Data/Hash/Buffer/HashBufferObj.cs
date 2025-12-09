using System;

namespace Cheng.Algorithm.HashCodes
{

    /// <summary>
    /// 缓存哈希值对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct HashBufferObj<T> : IEquatable<T>, IEquatable<HashBufferObj<T>> where T : IEquatable<T>
    {

        /// <summary>
        /// 初始化实例
        /// </summary>
        /// <param name="obj">要初始化哈希缓存的对象</param>
        /// <exception cref="ArgumentNullException">对象是null</exception>
        public HashBufferObj(T obj)
        {
            if (obj == null) throw new ArgumentNullException();
            this.obj = obj;
            hash = obj.GetHashCode();
        }

        /// <summary>
        /// 对象
        /// </summary>
        public readonly T obj;

        /// <summary>
        /// hash缓存
        /// </summary>
        public readonly int hash;

        public override bool Equals(object obj)
        {
            return obj.Equals(obj);
        }

        public bool Equals(T other)
        {
            return obj.Equals(other);
        }

        public override int GetHashCode()
        {
            return hash;
        }

        public bool Equals(HashBufferObj<T> other)
        {
            return obj.Equals(other.obj);
        }

        public static bool operator ==(HashBufferObj<T> a, HashBufferObj<T> b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(HashBufferObj<T> a, HashBufferObj<T> b)
        {
            return !a.Equals(b);
        }

        public static implicit operator T(HashBufferObj<T> hashObj)
        {
            return hashObj.obj;
        }

        public static implicit operator HashBufferObj<T>(T hashObj)
        {
            return new HashBufferObj<T>(hashObj);
        }

    }

}