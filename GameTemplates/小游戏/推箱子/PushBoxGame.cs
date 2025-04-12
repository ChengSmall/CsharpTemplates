using Cheng.DataStructure.Cherrsdinates;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cheng.GameTemplates.PushingBoxes
{

    public delegate void PushBoxEvent(PushBoxGame game);

    public delegate void PushBoxEvent<in T>(PushBoxGame game, T args);

    #region

    ///// <summary>
    ///// 玩家移动事件参数
    ///// </summary>
    //public struct PlayerMove
    //{
    //    public PlayerMove(PointInt2 position, MoveType moveType)
    //    {
    //        this.position = position;
    //        this.moveType = moveType;
    //    }

    //    /// <summary>
    //    /// 玩家移动后的位置
    //    /// </summary>
    //    public readonly PointInt2 position;

    //    /// <summary>
    //    /// 此次移动的方向
    //    /// </summary>
    //    public readonly MoveType moveType;

    //}

    #endregion

    /// <summary>
    /// 推箱子游戏操作台
    /// </summary>
    public class PushBoxGame
    {

        #region 构造

        /// <summary>
        /// 空构造
        /// </summary>
        public PushBoxGame()
        {
        }

        /// <summary>
        /// 使用指定场景构造
        /// </summary>
        /// <param name="scene"></param>
        /// <exception cref="ArgumentNullException">场景参数为null</exception>
        public PushBoxGame(PushBoxScene scene)
        {
            this.scene = scene ?? throw new ArgumentNullException();
            InitSceneArg(scene);
        }
        #endregion

        #region 参数

        /// <summary>
        /// 游戏场景
        /// </summary>
        public PushBoxScene scene;

        /// <summary>
        /// 该场景内终点所在的坐标
        /// </summary>
        public PointInt2[] finishs;

        /// <summary>
        /// 角色当前所在位置
        /// </summary>
        public PointInt2 playerPos;

        private List<PointInt2> pos_buffer = new List<PointInt2>();
        #endregion

        #region 功能

        #region 参数

        /// <summary>
        /// 初始化参数时的临时缓冲区
        /// </summary>
        public List<PointInt2> TragetBuffer => pos_buffer;

        #endregion

        #region 游戏

        /// <summary>
        /// 将箱子移动到任意目标点引发的事件
        /// </summary>
        /// <remarks>
        /// 参数表示目标点坐标
        /// </remarks>
        public event PushBoxEvent<PointInt2> BoxMoveToTraget;

        /// <summary>
        /// 将场景人物移动一个位置
        /// </summary>
        /// <param name="playerPosition">人物坐标</param>
        /// <param name="move">要移动的方向</param>
        /// <param name="toMove">移动后的人物位置</param>
        /// <returns>是否成功移动</returns>
        internal bool moveTo(PointInt2 playerPosition, MoveType move, out PointInt2 toMove)
        {
            var scene = this.scene;
            int x = playerPosition.x, y = playerPosition.y;

            SceneGrid p, objTriget, boxPos;
            SceneObject o2, boxPosObj;
            //SceneTarget traget;
            //SceneGround ground;
            bool changeTraget;
            p = scene[x, y];
            //p.GetValue(out obj, out traget, out ground);

            int width = scene.width, height = scene.height;

            int tox, toy;

            int cx = 0, cy = 0;

            toMove = default;
            changeTraget = false;
            if (move == MoveType.Left)
            {

                //获取目的地坐标
                tox = x - 1;
                toy = y;

                #region 推
                if (tox < 0 || toy < 0 || tox >= width || toy >= height) return false;

                //目标格子
                objTriget = scene[tox, toy];
                //目标格子对象
                o2 = objTriget.Object;

                //是墙
                if (o2 == SceneObject.Wall) return false;

                if (o2 == SceneObject.Box)
                {
                    //推箱子坐标
                    cx = tox - 1;
                    cy = toy;

                    if (cx >= 0 && cx < width && cy >= 0 && cy < height)
                    {
                        //推点不越界
                        boxPos = scene[cx, cy];
                        boxPosObj = boxPos.Object;
                        if (boxPosObj != SceneObject.Wall && boxPosObj != SceneObject.Box)
                        {
                            //不是墙和箱子
                            //开推
                            //Console.WriteLine("不是墙和箱子，开推");
                            scene[cx, cy] = boxPos.NewObjectGrid(objTriget);
                            changeTraget = scene[cx, cy].IsTraget;
                        }
                        else
                        {
                            //Console.WriteLine("是墙和箱子，不推");
                            return false;
                        }
                    }
                    else
                    {
                        //箱子后是场景边缘
                        return false;
                    }
                }

                //写入人物坐标
                toMove = new PointInt2(tox, toy);
                //移动

                //if (!changeTraget) changeTraget = (scene[x, y].IsTraget || scene[toMove.x, toMove.y].IsTraget);

                f_moveToPlayer(scene, new PointInt2(x, y), toMove);

                if (changeTraget)
                {
                    BoxMoveToTraget?.Invoke(this, new PointInt2(cx, cy));
                }

                return true;
                #endregion


            }
            else if (move == MoveType.Right)
            {
                tox = x + 1;
                toy = y;

                #region 推
                if (tox < 0 || toy < 0 || tox >= width || toy >= height) return false;

                //目标格子
                objTriget = scene[tox, toy];
                //目标格子对象
                o2 = objTriget.Object;

                //是墙
                if (o2 == SceneObject.Wall) return false;

                if (o2 == SceneObject.Box)
                {
                    //推箱子坐标
                    cx = tox + 1;
                    cy = toy;

                    if (cx >= 0 && cx < width && cy >= 0 && cy < height)
                    {
                        //推点不越界
                        boxPos = scene[cx, cy];
                        boxPosObj = boxPos.Object;
                        if (boxPosObj != SceneObject.Wall && boxPosObj != SceneObject.Box)
                        {
                            //不是墙和箱子
                            //开推
                            //Console.WriteLine("不是墙和箱子，开推");
                            scene[cx, cy] = boxPos.NewObjectGrid(objTriget);
                            changeTraget = scene[cx, cy].IsTraget;
                        }
                        else
                        {
                            //Console.WriteLine("是墙和箱子，不推");
                            return false;
                        }

                    }
                    else
                    {
                        //箱子后是场景边缘
                        return false;
                    }
                }

                //写入人物坐标
                toMove = new PointInt2(tox, toy);

                f_moveToPlayer(scene, new PointInt2(x, y), toMove);

                if (changeTraget)
                {
                    BoxMoveToTraget?.Invoke(this, new PointInt2(cx, cy));
                }

                return true;
                #endregion
            }
            else if (move == MoveType.Down)
            {
                tox = x;
                toy = y - 1;

                #region 推
                if (tox < 0 || toy < 0 || tox >= width || toy >= height) return false;

                //目标格子
                objTriget = scene[tox, toy];
                //目标格子对象
                o2 = objTriget.Object;

                //是墙
                if (o2 == SceneObject.Wall) return false;

                if (o2 == SceneObject.Box)
                {
                    //推箱子坐标
                    cx = tox;
                    cy = toy - 1;

                    if (cx >= 0 && cx < width && cy >= 0 && cy < height)
                    {
                        //推点不越界
                        boxPos = scene[cx, cy];
                        boxPosObj = boxPos.Object;
                        if (boxPosObj != SceneObject.Wall && boxPosObj != SceneObject.Box)
                        {
                            //不是墙和箱子
                            //开推
                            //Console.WriteLine("不是墙和箱子，开推");
                            scene[cx, cy] = boxPos.NewObjectGrid(objTriget);
                            changeTraget = scene[cx, cy].IsTraget;
                        }
                        else
                        {
                            //Console.WriteLine("是墙和箱子，不推不动");
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }

                }

                //写入人物坐标
                toMove = new PointInt2(tox, toy);
                //移动

                f_moveToPlayer(scene, new PointInt2(x, y), toMove);

                if (changeTraget)
                {
                    BoxMoveToTraget?.Invoke(this, new PointInt2(cx, cy));
                }
                return true;
                #endregion
            }
            else if (move == MoveType.Up)
            {
                tox = x;
                toy = y + 1;

                #region 推
                if (tox < 0 || toy < 0 || tox >= width || toy >= height) return false;

                //目标格子
                objTriget = scene[tox, toy];
                //目标格子对象
                o2 = objTriget.Object;

                //是墙
                if (o2 == SceneObject.Wall) return false;

                if (o2 == SceneObject.Box)
                {
                    //推箱子坐标
                    cx = tox;
                    cy = toy + 1;

                    if (cx >= 0 && cx < width && cy >= 0 && cy < height)
                    {
                        //推点不越界
                        boxPos = scene[cx, cy];
                        boxPosObj = boxPos.Object;
                        if (boxPosObj != SceneObject.Wall && boxPosObj != SceneObject.Box)
                        {
                            //不是墙和箱子
                            //开推
                            scene[cx, cy] = boxPos.NewObjectGrid(objTriget);
                            changeTraget = scene[cx, cy].IsTraget;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }


                //写入人物坐标
                toMove = new PointInt2(tox, toy);
                //移动

                f_moveToPlayer(scene, new PointInt2(x, y), toMove);

                if (changeTraget)
                {
                    BoxMoveToTraget?.Invoke(this, new PointInt2(cx, cy));
                }
                return true;
                #endregion
            }

            return false;
        }

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="origin">源人物</param>
        /// <param name="toMove">移动到的地点</param>
        static void f_moveToPlayer(PushBoxScene scene, PointInt2 origin, PointInt2 toMove)
        {
            var obj = scene[origin.x, origin.y];
            var toObj = scene[toMove.x, toMove.y];

            obj.GetValue(out var O, out var T, out var G);

            //人物设置在移动区域，保持地面和终点枚举
            scene[toMove.x, toMove.y] = new SceneGrid(O, toObj.Traget, toObj.Ground);
            //设置源人物区域为空，保持地面和终点枚举
            scene[origin.x, origin.y] = new SceneGrid(SceneObject.None, T, G);
        }

        /// <summary>
        /// 将场景人物移动一个位置
        /// </summary>
        /// <param name="move">要移动的方向</param>
        /// <returns>是否成功移动</returns>
        public bool MoveTo(MoveType move)
        {
            bool flag = moveTo(playerPos, move, out var tom);
            if (flag) playerPos = tom;
            return flag;
        }

        /// <summary>
        /// 判断是否胜利
        /// </summary>
        public bool IsVictory
        {
            get
            {
                var arr = finishs;

                int length = arr.Length;

                PointInt2 p;
                for (int i = 0; i < length; i++)
                {
                    p = arr[i];
                    if(scene[p.x, p.y].Object != SceneObject.Box)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// 从场景初始化参数
        /// </summary>
        /// <param name="scene">要初始化的场景，场景内仅限一个玩家</param>
        /// <exception cref="ArgumentNullException">场景实例为null</exception>
        public void InitSceneArg(PushBoxScene scene)
        {
            if (scene is null) throw new ArgumentNullException("场景对象未引用实例");

            List<PointInt2> ts = pos_buffer;

            int x, y;

            SceneGrid g;

            for(x = 0; x < scene.width; x++)
            {

                for(y = 0; y < scene.height; y++)
                {
                    g = scene[x, y];
                    if(g.Object == SceneObject.Player)
                    {
                        playerPos = new PointInt2(x, y);
                    }

                    if (g.IsTraget) ts.Add(new PointInt2(x, y));

                }

            }

            finishs = ts.ToArray();
            ts.Clear();
        }

        /// <summary>
        /// 重新从场景参数初始化参数
        /// </summary>
        /// <exception cref="ArgumentNullException">场景实例为null</exception>
        public void InitSceneArg()
        {
            InitSceneArg(scene);
        }

        #endregion

        #endregion

    }

}
