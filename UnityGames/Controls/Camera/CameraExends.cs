using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;


namespace Cheng.Unitys.Cameras
{

    /// <summary>
    /// 摄像机控件扩展
    /// </summary>
    public static unsafe class CameraExends
    {

        #region RectTransform

        /// <summary>
        /// 将2D摄像机下指定像素坐标转化到矩形变换中的内部坐标
        /// </summary>
        /// <remarks>请按需填入参数，否则后果自负</remarks>
        /// <param name="renderCamera2D">指定的2D正交摄像机</param>
        /// <param name="pixelPos">指定摄像机渲染区域的像素坐标</param>
        /// <param name="transform">要转化到的2D变换</param>
        /// <param name="buffer4">数据缓冲区，容量至少为4个元素</param>
        /// <returns>像素坐标位置的矩形变换内部坐标，以变换左下角为原点，向右上延申变换坐标</returns>
        public static Vector2 PixelToTransform(this Camera renderCamera2D, Vector2 pixelPos, RectTransform transform, Vector3[] buffer4)
        {
            //return default;

            //获取世界空间4角
            transform.GetWorldCorners(buffer4);

            //目标点摄像机视口空间
            Vector3 target_cameraViewPos;

            target_cameraViewPos = new Vector3(pixelPos.x / renderCamera2D.pixelWidth, pixelPos.y / renderCamera2D.pixelHeight);

            //目标点世界空间坐标
            Vector3 target_worlPos;
            target_worlPos = renderCamera2D.ViewportToWorldPoint(target_cameraViewPos);

            //变换左下和右上角所在世界坐标
            var transPos_world_leftDown = buffer4[0];
            var transPos_world_rightUp = buffer4[2];

            //变换在世界空间坐标系下的大小
            var transPos_world_size = transPos_world_rightUp - transPos_world_leftDown;

            //世界空间下-目标点与变换左下角的相对向量
            var target_world_size = target_worlPos - transPos_world_leftDown;

            //变换坐标系-目标点与变换大小的01标准坐标
            Vector2 st_size = new Vector2(target_world_size.x / transPos_world_size.x,
                target_world_size.y / transPos_world_size.y);

            //变换坐标系下的变换大小
            var trans_size = transform.sizeDelta;

            return trans_size * st_size;

        }

        /// <summary>
        /// 将2D摄像机下指定像素坐标转化到矩形变换中的内部坐标
        /// </summary>
        /// <remarks>请按需填入参数，否则后果自负</remarks>
        /// <param name="renderCamera2D">指定的2D正交摄像机</param>
        /// <param name="pixelPos">指定摄像机渲染区域的像素坐标</param>
        /// <param name="transform">要转化到的2D变换</param>
        /// <param name="anchor">指定变换坐标原点，范围在[0,1]的标准化坐标</param>
        /// <param name="buffer4">数据缓冲区，容量至少为4个元素</param>
        /// <returns>像素坐标位置的矩形变换内部坐标，以标准化坐标<paramref name="anchor"/>为原点，向右上延申的变换坐标</returns>
        public static Vector2 PixelToTransform(this Camera renderCamera2D, Vector2 pixelPos, RectTransform transform, Vector2 anchor, Vector3[] buffer4)
        {

            //获取世界空间4角
            transform.GetWorldCorners(buffer4);

            //目标点摄像机视口空间
            Vector3 target_cameraViewPos;

            target_cameraViewPos = new Vector3(pixelPos.x / renderCamera2D.pixelWidth, pixelPos.y / renderCamera2D.pixelHeight);

            //目标点世界空间坐标
            Vector3 target_worlPos;
            target_worlPos = renderCamera2D.ViewportToWorldPoint(target_cameraViewPos);

            //变换左下和右上角所在世界坐标
            var transPos_world_leftDown = buffer4[0];
            var transPos_world_rightUp = buffer4[2];

            //变换在世界空间坐标系下的大小
            var transPos_world_size = transPos_world_rightUp - transPos_world_leftDown;

            //世界空间下-目标点与变换左下角的相对向量
            var target_world_size = target_worlPos - transPos_world_leftDown;

            //变换坐标系-目标点与变换大小的01标准坐标
            Vector2 st_size = new Vector2(target_world_size.x / transPos_world_size.x,
                target_world_size.y / transPos_world_size.y);

            //变换坐标系下的变换大小
            var trans_size = transform.sizeDelta;

            return trans_size * (st_size - anchor);
        }

        /// <summary>
        /// 将2D摄像机下指定像素坐标转化到矩形变换的标准化坐标
        /// </summary>
        /// <param name="renderCamera2D">指定的2D正交摄像机</param>
        /// <param name="pixelPos">指定摄像机渲染区域的像素坐标</param>
        /// <param name="transform">要转化到的2D变换</param>
        /// <param name="buffer4">数据缓冲区，容量至少为4个元素</param>
        /// <returns>像素坐标位置的矩形变换标准化坐标，范围在[0,1]，以左下角为原点</returns>
        public static Vector2 PixelToStandardized(this Camera renderCamera2D, Vector2 pixelPos, RectTransform transform, Vector3[] buffer4)
        {
            //获取世界空间4角
            transform.GetWorldCorners(buffer4);

            //目标点世界空间坐标
            Vector3 target_worlPos;
            target_worlPos = renderCamera2D.ViewportToWorldPoint(new Vector3(pixelPos.x / renderCamera2D.pixelWidth, pixelPos.y / renderCamera2D.pixelHeight));

            //变换左下和右上角所在世界坐标
            var transPos_world_leftDown = buffer4[0];
            var transPos_world_rightUp = buffer4[2];

            //变换在世界空间坐标系下的大小
            var transPos_world_size = transPos_world_rightUp - transPos_world_leftDown;

            //世界空间下-目标点与变换左下角的相对向量
            var target_world_size = target_worlPos - transPos_world_leftDown;

            //变换坐标系-目标点与变换大小的01标准坐标
            return new Vector2(target_world_size.x / transPos_world_size.x,
                target_world_size.y / transPos_world_size.y);
        }

        #endregion

        #region 像素转换空间

        /// <summary>
        /// 按屏幕像素坐标获取摄像机所渲染的世界位置
        /// </summary>
        /// <param name="camera">摄像机</param>
        /// <param name="pixelPos">摄像机渲染的画面所在的像素坐标，从左下角开始（0,0）到右上角的像素长宽<paramref name="screen"/>；z数据单独表示为指定空间坐标与当前摄像机的距离，采用unity场景单位</param>
        /// <param name="screen">指定屏幕渲染的像素单位的长宽，x表示长度，y表示宽度</param>
        /// <returns>从<paramref name="pixelPos"/>转换到在unity场景中的坐标位置</returns>
        public static Vector3 ViewportScenePosition(this Camera camera, Vector3 pixelPos, Vector2 screen)
        {
            //获取归一化相对位置
            float gx = pixelPos.x / (screen.x - 1);
            float gy = pixelPos.y / (screen.y - 1);
            return camera.ViewportToWorldPoint(new Vector3(gx, gy, pixelPos.z));
        }

        /// <summary>
        /// 按屏幕像素坐标获取摄像机所渲染的世界位置
        /// </summary>
        /// <param name="camera">摄像机</param>
        /// <param name="pixelPos">摄像机渲染的画面所在的像素坐标，从左下角开始（0,0）到右上角的屏幕分辨率像素长宽；z数据单独表示为指定空间坐标与当前摄像机的距离，采用unity场景单位</param>
        /// <returns>从<paramref name="pixelPos"/>转换到在unity场景中的坐标位置</returns>
        public static Vector3 ViewportScenePosition(this Camera camera, Vector3 pixelPos)
        {
            return ViewportScenePosition(camera, pixelPos, new Vector2(Screen.width, Screen.height));
        }

        #endregion

    }

}
