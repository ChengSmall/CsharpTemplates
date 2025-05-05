
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

//using UnityEditor;

namespace Cheng.Unitys.Editors
{

    /// <summary>
    /// GUI编辑器常用参数和扩展方法
    /// </summary>
    public static class EditorGUIParser
    {

        #region 常量

#if UNITY_EDITOR

        /// <summary>
        /// UI参数---一行脚本参数的基本高度
        /// </summary>
        public const float OnceHeight = 18;

        /// <summary>
        /// UI参数---脚本参数之间的间隔高度
        /// </summary>
        public const float HeightInterval = 2;

        /// <summary>
        /// UI参数---一个字段参数的总高度间隔
        /// </summary>
        public const float AllFieldHeight = OnceHeight + HeightInterval;

        /// <summary>
        /// UI参数---开关长度
        /// </summary>
        public const float ToggleWidth = 20;

        /// <summary>
        /// UI参数---开关高度
        /// </summary>
        public const float ToggleHeight = 18;

#endif

        #endregion

        #region 扩展函数

        #region Rect

        /// <summary>
        /// 将指定矩形按长度切割，并返回切割的左半边矩形
        /// </summary>
        /// <param name="position">原矩形</param>
        /// <param name="mid">指定左半边切割的百分比，范围[0,1]</param>
        /// <returns>切割完毕后的左半边矩形</returns>
        public static Rect LeftTrim(this Rect position, float mid)
        {
            //float width, height, x, y;

            return new Rect(position.x, position.y, position.width * mid, position.height);
        }

        /// <summary>
        /// 将指定矩形按长度切割，并返回切割的右半边矩形
        /// </summary>
        /// <param name="position">原矩形</param>
        /// <param name="mid">指定右半边切割的百分比，范围[0,1]</param>
        /// <returns>切割完毕后的右半边矩形</returns>
        public static Rect RightTrim(this Rect position, float mid)
        {
            float width, x;

            width = position.width * mid;

            x = position.x + (position.width * (1 - mid));

            return new Rect(x, position.y, width, position.height);
        }

        /// <summary>
        /// 将矩形按长度切割，指定从左向右延申的比例
        /// </summary>
        /// <param name="position">原矩形</param>
        /// <param name="left">左端坐标；从矩形左边开始向右延申的比例，0表示最左端，范围在[0,1]</param>
        /// <param name="length">切割的矩形长度；从矩形左边开始向右延申的距离比，以原矩形长度为1，范围在[0,1]</param>
        /// <returns>切割后的矩形</returns>
        public static Rect LeftTrim(this Rect position, float left, float length)
        {
            float width = position.width;

            //float rewidth = length * width;
            float x = position.x + (width * left);

            return new Rect(x, position.y, length * width, position.height);

        }

        /// <summary>
        /// 将矩形按长度切割，指定从右向左延申的比例
        /// </summary>
        /// <param name="position">原矩形</param>
        /// <param name="right">右端坐标；从矩形右端开始向左延申的比例，0表示最右端，范围在[0,1]</param>
        /// <param name="length">切割的矩形长度；从矩形右端开始向左延申的距离比，以原矩形长度为1，范围在[0,1]</param>
        /// <returns>切割后的矩形</returns>
        public static Rect RightTrim(this Rect position, float right, float length)
        {
            //总长度
            float width = position.width;

            //切割长度
            //float reWidth = length * width;
            float x = position.x + (width * (1 - (right + length)));

            return new Rect(x, position.y, length * width, position.height);

        }

        /// <summary>
        /// 将矩形的长度从右侧缩短
        /// </summary>
        /// <param name="position">原矩形</param>
        /// <param name="shorten">要缩短的值</param>
        /// <returns>缩短后的矩形</returns>
        public static Rect ShortenLengthFormRight(this Rect position, float shorten)
        {
            return new Rect(position.x, position.y, position.width - shorten, position.height);
        }

        /// <summary>
        /// 将矩形的长度从右侧按缩放比例缩短
        /// </summary>
        /// <param name="position">原矩形</param>
        /// <param name="shortenScale">要缩短的以原长度为单位的长度比；0表示不缩放，1表示缩放到0长度</param>
        /// <returns>缩短后的矩形</returns>
        public static Rect ShortenLengthFormRightToScale(this Rect position, float shortenScale)
        {
            //var width = position.width;
            return new Rect(position.x, position.y, position.width * (1 - shortenScale), position.height);
        }

        /// <summary>
        /// 将矩形的长度从左侧缩短
        /// </summary>
        /// <param name="position">原矩形</param>
        /// <param name="shorten">要缩短的值</param>
        /// <returns>缩短后的矩形</returns>
        public static Rect ShortenLengthFormLeft(this Rect position, float shorten)
        {

            return new Rect(position.x + shorten, position.y, position.width - shorten, position.height);

        }

        /// <summary>
        /// 将矩形的长度从左侧按缩放比例缩短
        /// </summary>
        /// <param name="position">原矩形</param>
        /// <param name="shortenScale">要缩短的与原长度的比值；0表示无缩放，1表示缩放到0长度</param>
        /// <returns>缩短后的矩形</returns>
        public static Rect ShortenLengthFormLeftToScale(this Rect position, float shortenScale)
        {
            var width = position.width;
            float sh = shortenScale * width;
            return new Rect(position.x + sh, position.y, width - sh, position.height);

        }

        /// <summary>
        /// 将矩形的左右两端缩短
        /// </summary>
        /// <param name="position">矩形</param>
        /// <param name="leftShorten">左端缩小的长度</param>
        /// <param name="rightShorten">右端缩小的长度</param>
        /// <returns>缩短后的矩形</returns>
        public static Rect ShortenLength(this Rect position, float leftShorten, float rightShorten)
        {

            return new Rect(position.x + leftShorten, position.y, position.width - (leftShorten + rightShorten), position.height);

        }

        /// <summary>
        /// 将矩形的左右两端按比例缩短
        /// </summary>
        /// <param name="position">矩形</param>
        /// <param name="leftShortenScale">左端缩小的长度比例，以表示原矩形长度为单位缩放</param>
        /// <param name="rightShortenScale">右端缩小的长度比例，以表示原矩形长度为单位缩放</param>
        /// <returns></returns>
        public static Rect ShortenLengthToScale(this Rect position, float leftShortenScale, float rightShortenScale)
        {
            //总缩长度
            //float shorten = leftShortenScale + rightShortenScale;
            var width = position.width;

            var lefts = leftShortenScale * width;
            var rights = rightShortenScale * width;

            return new Rect(position.x + lefts, position.y, width - (lefts + rights), position.height);
            //return new Rect(position.x + leftShortenScale, position.y, position.width - (leftShortenScale + rightShortenScale), position.height);

        }

        /// <summary>
        /// 将矩形的左右两端按一半的比例缩短
        /// </summary>
        /// <param name="position">矩形</param>
        /// <param name="leftShortenScale">左端缩小的长度比例，以表示原矩形长度的一半为单位缩放</param>
        /// <param name="rightShortenScale">右端缩小的长度比例，以表示原矩形长度的一半为单位缩放</param>
        /// <returns></returns>
        public static Rect ShortenLengthToScaleMid(this Rect position, float leftShortenScale, float rightShortenScale)
        {
            //总缩长度
            //float shorten = leftShortenScale + rightShortenScale;
            var width = position.width;
            var ws = width / 2;
            var lefts = (leftShortenScale) * (ws);
            var rights = (rightShortenScale) * ws;

            return new Rect(position.x + lefts, position.y, width - (lefts + rights), position.height);
            //return new Rect(position.x + leftShortenScale, position.y, position.width - (leftShortenScale + rightShortenScale), position.height);

        }

        /// <summary>
        /// 将矩形沿纵轴切开
        /// </summary>
        /// <param name="rect">矩形</param>
        /// <param name="mid">要切开的纵轴切线长度比，范围在[0,1]，接近0表示靠左，接近1表示靠右</param>
        /// <param name="spareLength">从中间线向两端平均延申的长度，该长度所在的矩形会被舍弃</param>
        /// <param name="left">切割好的左侧矩形</param>
        /// <param name="right">切割好的右侧矩形</param>
        public static void SectionLength(this Rect rect, float mid, float spareLength, out Rect left, out Rect right)
        {
            float width = rect.width;
            float height = rect.height;

            float x = rect.x, y = rect.y;

            float midX = mid * width;

            float spareB = spareLength / 2;

            //float leftWidth;
            float rightOri;

            //中间忽略分割
            //leftWidth = midX - spareB;
            rightOri = midX + spareB;

            left = new Rect(x, y, (midX - spareB), height);

            right = new Rect(x + rightOri, y, width - rightOri, height);

        }

        /// <summary>
        /// 将矩形沿纵轴切开
        /// </summary>
        /// <param name="rect">矩形</param>
        /// <param name="mid">要切开的纵轴切线长度值，范围在[0,<see cref="Rect.width"/>]，该值表示切开后左侧的长度</param>
        /// <param name="spareLength">从中间线向两端平均延申的长度，该长度所在的矩形会被舍弃</param>
        /// <param name="left">切割好的左侧矩形</param>
        /// <param name="right">切割好的右侧矩形</param>
        public static void SectionLengthByValue(this Rect rect, float mid, float spareLength, out Rect left, out Rect right)

        {
            float width = rect.width;
            float height = rect.height;

            float x = rect.x, y = rect.y;

            float midX = mid;

            float spareB = spareLength / 2;

            //float leftWidth;
            float rightOri;

            //中间忽略分割
            //leftWidth = midX - spareB;
            rightOri = midX + spareB;

            left = new Rect(x, y, (midX - spareB), height);

            right = new Rect(x + rightOri, y, width - rightOri, height);

        }

        /// <summary>
        /// 从指定矩形内裁剪矩形
        /// </summary>
        /// <param name="rect">原矩形</param>
        /// <param name="position">要裁剪的左上角原点比，范围在[0,1]</param>
        /// <param name="size">要裁剪的右下角与原矩形大小的比，范围在[0,1]</param>
        /// <returns>裁剪后的矩形</returns>
        public static Rect Cropping(this Rect rect, Vector2 position, Vector2 size)
        {
            float width = rect.width;
            float height = rect.height;

            //float x = rect.x, y = rect.y;

            //目标左上点
            //float reX = (width * position.x) + rect.x;
            //float reY = (height * position.y) + rect.y;

            var pos_x = position.x;
            var pos_y = position.y;

            return new Rect(((width * pos_x) + rect.x), ((height * pos_y) + rect.y), width * (size.x - pos_x), height * (size.y - pos_y));
        }

        /// <summary>
        /// 将矩形沿横轴切开
        /// </summary>
        /// <param name="rect">矩形</param>
        /// <param name="mid">切割比例，接近0表示切口靠上，接近1表示切口靠下</param>
        /// <param name="spareHeight">中间舍弃的部分高度，0表示不舍弃</param>
        /// <param name="up">切割后上方的矩形</param>
        /// <param name="down">切割后下方的矩形</param>
        public static void SectionHeight(this Rect rect, float mid, float spareHeight, out Rect up, out Rect down)
        {

            var width = rect.width;
            var height = rect.height;

            float x = rect.x;
            float y = rect.y;

            //中间舍弃高度分半
            var spare = spareHeight * 0.5f;

            //上方矩形高度
            var upH = (height * mid) - (spare);

            //var downH = (height * (1 - mid)) - (spareHeight);

            up = new Rect(x, y, width, upH);

            down = new Rect(x, y + (upH + spareHeight), width, ((height * (1 - mid)) - (spareHeight)));
        }

        /// <summary>
        /// 将矩形按竖直方向移动
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="height">要移动的竖直方向的距离，大于0向下移动，小于0向上移动</param>
        /// <returns>移动后的矩形</returns>
        public static Rect MoveHeight(this Rect rect, float height)
        {
            return new Rect(rect.x, rect.y + height, rect.width, rect.height);
        }

        /// <summary>
        /// 将矩形按横向移动
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="width">要移动的横向距离，大于0向右移动，小于0向左移动</param>
        /// <returns>移动后的矩形</returns>
        public static Rect MoveWidth(this Rect rect, float width)
        {
            return new Rect(rect.x + width, rect.y, rect.width, rect.height);
        }

        /// <summary>
        /// 将矩形移动
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="width">要移动的横向距离，大于0向右移动，小于0向左移动</param>
        /// <param name="height">要移动的竖直方向的距离，大于0向下移动，小于0向上移动</param>
        /// <returns>移动后的矩形</returns>
        public static Rect Move(this Rect rect, float width, float height)
        {
            return new Rect(rect.x + width, rect.y + height, rect.width, rect.height);
        }

        /// <summary>
        /// 将矩形区域区域从下方缩短高度
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="shorten">要缩短的高度</param>
        /// <returns>缩短后的矩形</returns>
        public static Rect ShortenHeightToUp(this Rect rect, float shorten)
        {

            return new Rect(rect.x, rect.y, rect.width, rect.height - shorten);
        }

        /// <summary>
        /// 将矩形区域区域从下方缩短高度
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="shortenScale">要缩短的高度的原长度比，范围[0,1]，0表示无缩放，1表示缩短的高度为当前矩形高度</param>
        /// <returns>缩短后的矩形</returns>
        public static Rect ShortenHeightToUpScale(this Rect rect, float shortenScale)
        {
            var h = rect.height;
            return new Rect(rect.x, rect.y, rect.width, h - (shortenScale * h));
        }

        /// <summary>
        /// 将矩形区域区域从上方向下缩短高度
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="shorten">要缩短的高度</param>
        /// <returns>缩短后的矩形</returns>
        public static Rect ShortenHeightToDown(this Rect rect, float shorten)
        {

            return new Rect(rect.x, rect.y + shorten, rect.width, rect.height - shorten);
        }

        /// <summary>
        /// 将矩形区域区域从上方向下缩短高度
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="shortenScale">要缩短的高度的原长度比，范围[0,1]，0表示无缩放，1表示缩短的高度为当前矩形高度</param>
        /// <returns>缩短后的矩形</returns>
        public static Rect ShortenHeightToDownScale(this Rect rect, float shortenScale)
        {
            var h = rect.height;
            var hv = h - (shortenScale * h);
            return new Rect(rect.x, rect.y + hv, rect.width, hv);
        }

        /// <summary>
        /// 以顶部为基准设置矩形区域的高度
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="height">要设置的高度</param>
        /// <returns>新设置的矩形</returns>
        public static Rect SetHeightFromTop(this Rect rect, float height)
        {
            return new Rect(rect.x, rect.y, rect.width, height);
        }

        /// <summary>
        /// 以底部为基准设置矩形区域的高度
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="height">要设置的高度</param>
        /// <returns>新设置的矩形</returns>
        public static Rect SetHeightFromBotton(this Rect rect, float height)
        {            
            return new Rect(rect.x, rect.y + (rect.height - height), rect.width, height);
        }

        /// <summary>
        /// 将矩形区域按横坐标成若干小块
        /// </summary>
        /// <param name="rect">矩形</param>
        /// <param name="count">要切割的块的数量，最小为1</param>
        /// <param name="append">切割后要添加到的集合，向后从上到下依次添加</param>
        /// <exception cref="ArgumentNullException">集合为null</exception>
        /// <exception cref="NotSupportedException">无法使用<see cref="ICollection{T}.Add(T)"/>方法添加集合</exception>
        /// <exception cref="ArgumentOutOfRangeException">切割的块数量小于或等于0</exception>
        public static void AveragecuttingWidth(this Rect rect, int count, ICollection<Rect> append)
        {
            if (append is null) throw new ArgumentNullException();
            if (count <= 0) throw new ArgumentOutOfRangeException();

            if(count == 1)
            {
                append.Add(rect);
                return;
            }

            float height = rect.height;

            var avh = height / count;

            Rect rt = new Rect(rect.x, rect.y, rect.width, avh);

            for (int i = 0; i < count; i++)
            {
                append.Add(rt);
                rt = new Rect(rt.x, rt.y + avh, rt.width, avh);
            }

        }

        /// <summary>
        /// 将矩形区域按横坐标成若干小块
        /// </summary>
        /// <param name="rect">矩形</param>
        /// <param name="appArray">切割后要填充进的数组，切割数量取决于数组元素容量</param>
        /// <exception cref="ArgumentNullException">集合为null</exception>
        public static void AveragecuttingWidth(this Rect rect, Rect[] appArray)
        {
            if (appArray is null) throw new ArgumentNullException();
            //if (count <= 0) throw new ArgumentOutOfRangeException();

            int count = appArray.Length;

            if (count == 1)
            {
                appArray[0] = rect;
                return;
            }

            float height = rect.height;

            var avh = height / count;

            Rect rt = new Rect(rect.x, rect.y, rect.width, avh);

            for (int i = 0; i < count; i++)
            {
                //append.Add(rt);
                appArray[i] = rt;
                rt = new Rect(rt.x, rt.y + avh, rt.width, avh);
            }

        }

        /// <summary>
        /// 将矩形区域按纵坐标平均切分成若干小块
        /// </summary>
        /// <param name="rect">矩形</param>
        /// <param name="count">切分数量</param>
        /// <param name="append">切割后要向其添加的数组</param>
        public static void AveragecuttingHeight(this Rect rect, int count, ICollection<Rect> append)
        {
            if (append is null) throw new ArgumentNullException();

            if (count <= 1)
            {
                append.Add(rect);
                return;
            }

            float width = rect.width;

            var av = width / count;

            Rect rt = new Rect(rect.x, rect.y, av, rect.width);

            for (int i = 0; i < count; i++)
            {
                append.Add(rt);
                rt = new Rect(rt.x + av, rt.y, av, rt.height);
            }

        }

        /// <summary>
        /// 将矩形区域按纵坐标平均切分成若干小块
        /// </summary>
        /// <param name="rect">矩形</param>
        /// <param name="appArray">要填入的数组，数组容量表示要切分的数量</param>
        public static void AveragecuttingHeight(this Rect rect, Rect[] appArray)
        {
            if (appArray is null) throw new ArgumentNullException();
            int count = appArray.Length;
            if (count == 1)
            {
                appArray[0] = rect;
                return;
            }

            float width = rect.width;

            var av = width / count;

            Rect rt = new Rect(rect.x, rect.y, av, rect.width);

            for (int i = 0; i < count; i++)
            {
                appArray[i] = rt;
                rt = new Rect(rt.x + av, rt.y, av, rt.height);
            }
        }

        /// <summary>
        /// 以正中心为轴，将矩形沿纵轴缩短到指定长度
        /// </summary>
        /// <param name="rect">矩形</param>
        /// <param name="length">保持的长度</param>
        /// <returns>新的矩形</returns>
        public static Rect ShortenToLengthByCenter(this Rect rect, float length)
        {
            //var width = rect.width;
            //float height = rect.height;
            //半个
            //float BL = length / 2;
            //float BW = rect.width / 2;
            //x向右移动 半个 width 再向左移动 BL
            float x = rect.x + ((rect.width / 2) - (length / 2));

            return new Rect(x, rect.y, length, rect.height);
        }

        #endregion

        #endregion

    }

}
#if UNITY_EDITOR

#endif