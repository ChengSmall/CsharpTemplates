using Cheng.DataStructure.Collections;
using System;

namespace Cheng.GameTemplates.PushingBoxes
{

    /// <summary>
    /// 推箱子的游戏场景
    /// </summary>
    public sealed class PushBoxScene : ITwoDimensionalArray<SceneGrid>
    {

        #region 构造

        /// <summary>
        /// 实例化一个空场景
        /// </summary>
        public PushBoxScene()
        {
            this.width = 0;
            this.height = 0;
            p_grids = new SceneGrid[0, 0];
        }

        /// <summary>
        /// 实例化一个指定大小的推箱子场景
        /// </summary>
        /// <param name="width">长度格子数</param>
        /// <param name="height">高度格子数</param>
        public PushBoxScene(int width, int height)
        {
            if (width < 0 || height < 0) throw new ArgumentOutOfRangeException();

            this.width = width;
            this.height = height;
            p_grids = new SceneGrid[width, height];
        }

        /// <summary>
        /// 使用已有的二维数组实例化场景
        /// </summary>
        /// <param name="grids">格子数组</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public PushBoxScene(SceneGrid[,] grids)
        {
            p_grids = grids ?? throw new ArgumentNullException();
            width = grids.GetLength(0);
            height = grids.GetLength(1);
        }

        /// <summary>
        /// 拷贝构造一个游戏场景实例
        /// </summary>
        /// <param name="scene">要拷贝的数据</param>
        /// <exception cref="ArgumentNullException"></exception>
        public PushBoxScene(PushBoxScene scene)
        {
            if (scene is null) throw new ArgumentNullException();
            p_grids = (SceneGrid[,])scene.p_grids.Clone();
            width = scene.width;
            height = scene.height;
        }

        #endregion

        #region 参数

        private SceneGrid[,] p_grids;

        /// <summary>
        /// 场景长度
        /// </summary>
        public readonly int width;

        /// <summary>
        /// 场景高度
        /// </summary>
        public readonly int height;

        #endregion

        #region 功能

        #region 参数访问

        public int Count => p_grids.Length;

        public int Width => width;

        public int Height => height;

        public SceneGrid this[int x, int y]
        {
            get
            {
                if (x < 0 || x >= width || y < 0 || y >= height) throw new ArgumentOutOfRangeException();
                return p_grids[x, y];
            }
            set
            {
                if (x < 0 || x >= width || y < 0 || y >= height) throw new ArgumentOutOfRangeException();
                p_grids[x, y] = value;
            }
        }

        /// <summary>
        /// 获取场景内部数组
        /// </summary>
        public SceneGrid[,] BaseGrids
        {
            get => p_grids;
        }
        
        /// <summary>
        /// 判断该场景是一个没有格子空场景
        /// </summary>
        public bool IsEmptyScene
        {
            get
            {
                return width == 0 || height == 0;
            }
        }

        #endregion

        #region 参数设置

        /// <summary>
        /// 将字节数组转化为游戏场景
        /// </summary>
        /// <param name="buffer">字节数组</param>
        /// <param name="width">场景长度</param>
        /// <returns>场景</returns>
        public static PushBoxScene ToScene(byte[] buffer, int width)
        {

            int height = buffer.Length / width;

            PushBoxScene s = new PushBoxScene(width, height);
            int x, y;

            for(y = 0; y < height; y++)
            {
                for(x = 0; x < width; x++)
                {
                    s[x, y] = new SceneGrid(buffer[(y * width) + x]);
                }
            }

            return s;
        }

        /// <summary>
        /// 将所有格子设置为指定参数
        /// </summary>
        /// <param name="grid"></param>
        public void SetAll(SceneGrid grid)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    p_grids[x, y] = grid;
                }
            }
        }

        /// <summary>
        /// 将场景数组所有元素实例设置为默认值
        /// </summary>
        public void Clear()
        {
            Array.Clear(p_grids, 0, p_grids.Length);
        }

        /// <summary>
        /// 将所有格子设置为空地面
        /// </summary>
        public void ResetEmpty()
        {
            var grid = SceneGrid.GroundObject;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    p_grids[x, y] = grid;
                }
            }
        }

        /// <summary>
        /// 返回一个新的相同场景
        /// </summary>
        /// <returns></returns>
        public PushBoxScene Clone()
        {
            return new PushBoxScene(this);
        }

        #endregion

        #region 计算

        /// <summary>
        /// 获取当前场景内的箱子和目标点数量
        /// </summary>
        /// <param name="boxCount">场景内箱子数量</param>
        /// <param name="tragetCount">场景内目标点数量</param>
        public void GetObjectCount(out int boxCount, out int tragetCount)
        {
            tragetCount = 0;
            boxCount = 0;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var grid = p_grids[x, y];
                    if (grid.IsTraget)
                    {
                        tragetCount++;
                    }
                    if (grid.Object == SceneObject.Box)
                    {
                        boxCount++;
                    }
                }
            }
        }

        #endregion

        #endregion

    }

}
