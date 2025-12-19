using System;
using System.Collections.Generic;

using Cheng.Unitys;
using UnityEngine;
using Cheng.Algorithm.Randoms;

namespace Cheng.Algorithm.Randoms.Extends
{

    /// <summary>
    /// 随机数生成器Unity扩展方法
    /// </summary>
    public static class UnityRandom
    {

        #region 

        /// <summary>
        /// 获取一个范围在[0,1)的随机向量
        /// </summary>
        /// <param name="random"></param>
        /// <returns>三个范围在[0,1)的随机值向量</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static Vector3 NextVector3(this BaseRandom random)
        {
            if (random is null) throw new ArgumentNullException();
            return new Vector3(random.NextFloat(), random.NextFloat(), random.NextFloat());
        }

        /// <summary>
        /// 获取一个范围在[0,1)的随机向量
        /// </summary>
        /// <param name="random"></param>
        /// <returns>2个范围在[0,1)的随机值向量</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static Vector2 NextVector2(this BaseRandom random)
        {
            if (random is null) throw new ArgumentNullException();
            return new Vector2(random.NextFloat(), random.NextFloat());
        }

        /// <summary>
        /// 获取一个完全随机的颜色
        /// </summary>
        /// <param name="random"></param>
        /// <returns>完全随机的颜色</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static Color32 NextColor(this BaseRandom random)
        {
            if (random is null) throw new ArgumentNullException();
            return new Color32((byte)random.Next(0, 256), (byte)random.Next(0, 256), (byte)random.Next(0, 256), (byte)random.Next(0, 256));
        }

        /// <summary>
        /// 获取一个完全随机的不透明颜色
        /// </summary>
        /// <param name="random"></param>
        /// <returns>完全随机的不透明颜色</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static Color32 NextColorOpaque(this BaseRandom random)
        {
            if (random is null) throw new ArgumentNullException();
            return new Color32((byte)random.Next(0, 256), (byte)random.Next(0, 256), (byte)random.Next(0, 256), byte.MaxValue);
        }

        /// <summary>
        /// 返回一个随机向量，表示一个半径为1的圆上的随机点
        /// </summary>
        /// <param name="random"></param>
        /// <returns>一个随机的半径为1的圆上的点</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static Vector2 NextOnUnitCircle(this BaseRandom random)
        {
            if (random is null) throw new ArgumentNullException();
            double radian = random.NextDouble(0, System.Math.PI * 2);

            return new Vector2((float)(Math.Cos(radian)), (float)(Math.Sin(radian)));
            //v.x = (float)(1 * Math.Cos(radian));
            //v.y = (float)(1 * Math.Sin(radian));
        }

        /// <summary>
        /// 返回一个随机向量，表示一个半径为1的圆内的随机点
        /// </summary>
        /// <param name="random"></param>
        /// <returns>一个随机的半径为1的圆内的点</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static Vector2 NextInsideUnitCircle(this BaseRandom random)
        {
            if (random is null) throw new ArgumentNullException();
            double radian = random.NextDouble(0, System.Math.PI * 2);

            var length = random.NextDouble();
            return new Vector2((float)(length * Math.Cos(radian)), (float)(length * Math.Sin(radian)));
            //v.x = (float)(1 * Math.Cos(radian));
            //v.y = (float)(1 * Math.Sin(radian));
        }

        /// <summary>
        /// 返回一个随机向量，表示指定范围内的随机点
        /// </summary>
        /// <param name="random"></param>
        /// <param name="min">起始坐标，该参数表示返回的向量的分量不会小于该值</param>
        /// <param name="max">最大值，该参数表示返回的向量的分量不会大于该值</param>
        /// <returns>表示指定范围内的随机点</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentException">参数不正确</exception>
        public static Vector3 NextPosition(this BaseRandom random, Vector3 min, Vector3 max)
        {
            if (random is null) throw new ArgumentNullException();
            return new Vector3(random.NextFloat(min.x, max.x + Vector3.kEpsilon), random.NextFloat(min.y, max.y + Vector3.kEpsilon), random.NextFloat(min.z, max.z + Vector3.kEpsilon));
        }

        /// <summary>
        /// 返回一个随机向量，表示在[0,1]范围内的随机点
        /// </summary>
        /// <param name="random"></param>
        /// <returns>[0,1]范围内的随机点</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static Vector3 NextPosition(this BaseRandom random)
        {
            if (random is null) throw new ArgumentNullException();
            return new Vector3(random.NextFloat(0, 1 + Vector2.kEpsilon), random.NextFloat(0, 1 + Vector2.kEpsilon), random.NextFloat(0, 1 + Vector2.kEpsilon));
        }

        /// <summary>
        /// 返回一个随机向量，表示在矩形上的随机点
        /// </summary>
        /// <param name="random"></param>
        /// <param name="rect">矩形</param>
        /// <returns>矩形<paramref name="rect"/>上的随机点</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentException">参数不正确</exception>
        public static Vector2 NextPosition(this BaseRandom random, Rect rect)
        {
            if (random is null) throw new ArgumentNullException();
            //float x = rect.x;
            //float y = rect.y;

            //float width = rect.width;
            //float height = rect.height;
            return new Vector2((float)(rect.x + (rect.width * random.NextDouble())), (float)(rect.y + (rect.height * random.NextDouble())));
        }

        /// <summary>
        /// 返回射线上的随机点
        /// </summary>
        /// <param name="random"></param>
        /// <param name="ray">射线</param>
        /// <param name="maxlength">随机点距离射线原点的最大长度</param>
        /// <returns>射线上的随机点</returns>
        public static Vector3 NextPosition(this BaseRandom random, Ray ray, float maxlength)
        {
            if (random is null) throw new ArgumentNullException();
            return ray.GetPoint(random.NextFloat(0, maxlength + Vector3.kEpsilon));
        }

        /// <summary>
        /// 返回射线上的随机点
        /// </summary>
        /// <param name="random"></param>
        /// <param name="ray">射线</param>
        /// <param name="maxlength">随机点距离射线原点的最大长度</param>
        /// <returns>射线上的随机点</returns>
        public static Vector2 NextPosition(this BaseRandom random, Ray2D ray, float maxlength)
        {
            if (random is null) throw new ArgumentNullException();
            return ray.GetPoint(random.NextFloat(0, maxlength + Vector2.kEpsilon));
        }

        /// <summary>
        /// 使用HSV颜色模型返回随机颜色
        /// </summary>
        /// <param name="random"></param>
        /// <param name="Hmin">色相最小值</param>
        /// <param name="Hmax">色相最大值</param>
        /// <param name="Smin">饱和度最小值</param>
        /// <param name="Smax">饱和度最大值</param>
        /// <param name="Vmin">亮度最小值</param>
        /// <param name="Vmax">亮度最大值</param>
        /// <returns>指定范围内的随机颜色</returns>
        public static Color NextColorHSV(this BaseRandom random, float Hmin, float Hmax, float Smin, float Smax, float Vmin, float Vmax)
        {
            if (random is null) throw new ArgumentNullException();
            return Color.HSVToRGB(random.NextFloat(Hmin, Hmax + 1e-5f), random.NextFloat(Smin, Smax + 1e-5f), random.NextFloat(Vmin, Vmax + 1e-5f));
        }

        /// <summary>
        /// 将现有颜色随机修改透明度并返回
        /// </summary>
        /// <param name="random"></param>
        /// <param name="color">颜色</param>
        /// <returns>随机透明度的<paramref name="color"/></returns>
        public static Color NextColorAlpha(this BaseRandom random, Color color)
        {
            if (random is null) throw new ArgumentNullException();
            color.a = (random.Next(0, 256) / 255f);
            return color;
        }

        /// <summary>
        /// 将现有颜色随机修改透明度并返回
        /// </summary>
        /// <param name="random"></param>
        /// <param name="color">颜色</param>
        /// <param name="min">随机透明度最小值</param>
        /// <param name="max">随机透明度最大值</param>
        /// <returns>范围内随机透明度的<paramref name="color"/></returns>
        public static Color NextColorAlpha(this BaseRandom random, Color color, float min, float max)
        {
            if (random is null) throw new ArgumentNullException();
            color.a = Mathf.Clamp01(random.NextFloat(min, max + 1e-5f));
            return color;
        }

        #endregion

    }

}
