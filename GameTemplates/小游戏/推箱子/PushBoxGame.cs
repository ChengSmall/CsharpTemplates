using Cheng.DataStructure.Cherrsdinates;
using Cheng.DataStructure.Collections;
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
            f_init();
        }

        /// <summary>
        /// 使用指定场景初始化
        /// </summary>
        /// <param name="scene"></param>
        /// <exception cref="ArgumentNullException">场景参数为null</exception>
        public PushBoxGame(PushBoxScene scene)
        {
            this.scene = scene ?? throw new ArgumentNullException();
            f_init();
            InitSceneArg(scene);
        }

        private void f_init()
        {
            pos_buffer = new List<PointInt2>();
            //p_targetOnBox = new Dictionary<PointInt2, bool>();
            pos_targetPoses = new ArrayReadOnly<PointInt2>(pos_buffer);
        }
        #endregion

        #region 参数

        /// <summary>
        /// 游戏场景
        /// </summary>
        public PushBoxScene scene;

        //private PointInt2[] p_targetPos;
        //private Dictionary<PointInt2, bool> p_targetOnBox;

        /// <summary>
        /// 角色当前所在位置
        /// </summary>
        private PointInt2 playerPos;

        private List<PointInt2> pos_buffer;
        
        private ArrayReadOnly<PointInt2> pos_targetPoses;

        /// <summary>
        /// 箱子处于目标点上的数量
        /// </summary>
        private int p_boxOnTargetCount;

        /// <summary>
        /// 所有目标点数量
        /// </summary>
        private int p_targetCount;
        #endregion

        #region 功能

        #region 参数

        /// <summary>
        /// 初始化参数时的临时缓冲区
        /// </summary>
        public List<PointInt2> TragetBuffer => pos_buffer;

        /// <summary>
        /// 玩家当前所在位置
        /// </summary>
        public PointInt2 PlayerPosition
        {
            get => playerPos;
        }

        /// <summary>
        /// 该场景所有目标点的坐标
        /// </summary>
        public ArrayReadOnly<PointInt2> Targets
        {
            get => pos_targetPoses;
        }

        #endregion

        #region 事件

        /// <summary>
        /// 将箱子移动到任意目标点引发的事件
        /// </summary>
        /// <remarks>
        /// 参数表示目标点坐标
        /// </remarks>
        public event PushBoxEvent<PointInt2> BoxMoveToTraget;

        /// <summary>
        /// 将箱子推离目标点时引发的事件
        /// </summary>
        /// <remarks>参数表示目标点所在的位置</remarks>
        public event PushBoxEvent<PointInt2> BoxPushAwayByTarget;

        /// <summary>
        /// 游戏胜利
        /// </summary>
        /// <remarks>
        /// <para>所有的箱子都处于目标点位时引发的事件</para>
        /// <para>只有在每次移动后才会更新事件，你可以用<see cref="IsVictory"/>直接判断</para>
        /// </remarks>
        public event PushBoxEvent Winning;

        #endregion

        #region 封装

        /// <summary>
        /// 成功移动后的回调
        /// </summary>
        /// <param name="origin">移动前的坐标</param>
        /// <param name="moveTo">移动到的坐标</param>
        /// <param name="moveType">移动方向</param>
        private void feInvoke_MoveToGrid(PointInt2 origin, PointInt2 moveTo, MoveType moveType)
        {
            bool susumeiIsBox = false;
            var moveToGrid = scene[moveTo.x, moveTo.y];

            if (moveToGrid.IsTraget)
            {
                //移动到目标点
                //检查前放有箱子
                var susumeiPos = PositionMoveTo(moveTo, moveType);
                if (!CheckPosOutArg(susumeiPos))
                {
                    //未超出范围
                    susumeiIsBox = scene[susumeiPos.x, susumeiPos.y].Object == SceneObject.Box;
                }
            }

            if (susumeiIsBox)
            {
                //将目标点的箱子推离目标点
                p_boxOnTargetCount--;
                BoxPushAwayByTarget?.Invoke(this, moveTo);
            }



        }

        /// <summary>
        /// 将箱子移动到任意目标点引发的回调
        /// </summary>
        /// <param name="point"></param>
        private void feInvoke_BoxMoveToTarget(PointInt2 point)
        {
            p_boxOnTargetCount++;
            BoxMoveToTraget?.Invoke(this, point);

            if(p_boxOnTargetCount == p_targetCount)
            {
                Winning?.Invoke(this);
            }

        }

        /// <summary>
        /// 将场景人物移动一个位置
        /// </summary>
        /// <param name="playerPosition">人物坐标</param>
        /// <param name="move">要移动的方向</param>
        /// <param name="toMove">移动后的人物位置</param>
        /// <returns>是否成功移动</returns>
        private bool moveTo(PointInt2 playerPosition, MoveType move, out PointInt2 toMove)
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
                    feInvoke_BoxMoveToTarget(new PointInt2(cx, cy));
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
                    feInvoke_BoxMoveToTarget(new PointInt2(cx, cy));
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
                    feInvoke_BoxMoveToTarget(new PointInt2(cx, cy));
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
                    feInvoke_BoxMoveToTarget(new PointInt2(cx, cy));
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

        #endregion

        #region 游戏

        /// <summary>
        /// 指定坐标移动后的坐标
        /// </summary>
        /// <param name="position">坐标</param>
        /// <param name="moveType">移动方位</param>
        /// <returns>移动后的坐标</returns>
        public static PointInt2 PositionMoveTo(PointInt2 position, MoveType moveType)
        {
            int x, y;
            switch (moveType)
            {
                case MoveType.Left:
                    x = position.x - 1;
                    y = position.y;
                    break;
                case MoveType.Right:
                    x = position.x + 1;
                    y = position.y;
                    break;
                case MoveType.Up:
                    x = position.x;
                    y = position.y + 1;
                    break;
                case MoveType.Down:
                    x = position.x;
                    y = position.y - 1;
                    break;
                default:
                    x = position.x;
                    y = position.y;
                    break;
            }
            return new PointInt2(x, y);
        }

        /// <summary>
        /// 检查坐标超出场景
        /// </summary>
        /// <param name="position"></param>
        /// <returns>超出场景true，没有超出false</returns>
        public bool CheckPosOutArg(PointInt2 position)
        {
            return (position.x < 0 || position.y < 0 || position.x >= scene.width || position.y > scene.height);
        }

        /// <summary>
        /// 检查此坐标向某处移动后是否超出场景范围
        /// </summary>
        /// <param name="position">坐标</param>
        /// <param name="moveType">移动方位</param>
        /// <returns>超出返回true</returns>
        public bool CheckPosOutArg(PointInt2 position, MoveType moveType)
        {
            int x, y;
            switch (moveType)
            {
                case MoveType.Left:
                    x = position.x - 1;
                    y = position.y;
                    break;
                case MoveType.Right:
                    x = position.x + 1;
                    y = position.y;
                    break;
                case MoveType.Up:
                    x = position.x;
                    y = position.y + 1;
                    break;
                case MoveType.Down:
                    x = position.x;
                    y = position.y - 1;
                    break;
                default:
                    x = position.x;
                    y = position.y;
                    break;
            }

            return (x < 0 || y < 0 || x >= scene.width || y >= scene.height);

        }

        /// <summary>
        /// 将场景人物移动一个位置
        /// </summary>
        /// <param name="move">要移动的方向</param>
        /// <returns>是否成功移动</returns>
        public bool MoveTo(MoveType move)
        {
            var nowPos = playerPos;
            bool flag = moveTo(nowPos, move, out var tom);
            if (flag)
            {
                playerPos = tom;
                feInvoke_MoveToGrid(nowPos, tom, move);
            }
            return flag;
        }

        /// <summary>
        /// 判断是否胜利
        /// </summary>
        public bool IsVictory
        {
            get
            {
                return p_boxOnTargetCount == p_targetCount;
                //var arr = pos_buffer;
                //int length = arr.Count;
                //PointInt2 p;
                //for (int i = 0; i < length; i++)
                //{
                //    p = arr[i];
                //    if(scene[p.x, p.y].Object != SceneObject.Box)
                //    {
                //        return false;
                //    }
                //}
                //return true;
            }
        }

        /// <summary>
        /// 从场景初始化参数
        /// </summary>
        /// <param name="scene">要初始化的场景，场景内仅限一个玩家</param>
        /// <exception cref="ArgumentNullException">场景实例为null</exception>
        public void InitSceneArg(PushBoxScene scene)
        {
            this.scene = scene;
            InitSceneArg();
        }

        /// <summary>
        /// 重新从<see cref="scene"/>参数初始化参数
        /// </summary>
        /// <remarks>
        /// 从场景参数<see cref="scene"/>提取数据并初始化游戏控制台，在进行未初始化时，任何其它操作都可能造成未知错误
        /// </remarks>
        /// <exception cref="ArgumentNullException">场景实例为null</exception>
        public void InitSceneArg()
        {
            var scene = this.scene;

            if (scene is null) throw new ArgumentNullException("场景对象未引用实例");

            List<PointInt2> ts = pos_buffer;
            ts.Clear();

            p_targetCount = 0;
            p_boxOnTargetCount = 0;
            int x, y;
            SceneGrid g;
            for (x = 0; x < scene.width; x++)
            {

                for (y = 0; y < scene.height; y++)
                {
                    g = scene[x, y];
                    g.GetValue(out var obj, out var target);
                    if (obj == SceneObject.Player)
                    {
                        //此处是玩家位置
                        playerPos = new PointInt2(x, y);
                    }

                    if (target == SceneTarget.Exist)
                    {
                        //此处是目标点
                        ts.Add(new PointInt2(x, y));
                        p_targetCount++;
                        if (obj == SceneObject.Box)
                        {
                            //目标点上有箱子
                            p_boxOnTargetCount++;
                        }
                    }
                }
            }

        }

        #endregion

        #endregion

    }

}
