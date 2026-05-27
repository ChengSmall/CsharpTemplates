using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cheng.GameTemplates.PushingBoxes
{
    
    /// <summary>
    /// 所在格子物品
    /// </summary>
    public enum SceneObject : byte
    {
        /// <summary>
        /// 无对象
        /// </summary>
        None = 0,
        /// <summary>
        /// 箱子
        /// </summary>
        Box = 1,
        /// <summary>
        /// 玩家
        /// </summary>
        Player = 2,
        /// <summary>
        /// 墙体
        /// </summary>
        Wall = 3,
    }

    /// <summary>
    /// 表示是否存在目标点的枚举
    /// </summary>
    public enum SceneTarget : byte
    {
        /// <summary>
        /// 没有
        /// </summary>
        None = 0,
        /// <summary>
        /// 存在
        /// </summary>
        Exist = 1
    }

    /// <summary>
    /// 表示是否存在地面
    /// </summary>
    public enum SceneGround : byte
    {
        /// <summary>
        /// 没有地面
        /// </summary>
        None = 0,
        /// <summary>
        /// 存在地面
        /// </summary>
        Exist = 1
    }

    /// <summary>
    /// 玩家位移操作
    /// </summary>
    public enum MoveType : byte
    {
        /// <summary>
        /// 无操作
        /// </summary>
        None = 0,
        /// <summary>
        /// 向左移动
        /// </summary>
        Left =  1,
        /// <summary>
        /// 向右移动
        /// </summary>
        Right = 2,
        /// <summary>
        /// 向上移动
        /// </summary>
        Up =    3,
        /// <summary>
        /// 向下移动
        /// </summary>
        Down =  4
    }

    /// <summary>
    /// 场景中的格子
    /// </summary>
    public struct SceneGrid : IEquatable<SceneGrid>
    {
        /*
         * 物品 前2bit
         * 目标点 第3bit => 0b00000100
         * 地面 第4bit => 0b00001000
         * 后4bit为预留数值
         * 0表示空对象
         */

        #region 构造

        /// <summary>
        /// 使用唯一id构建格子对象
        /// </summary>
        /// <param name="id">id</param>
        public SceneGrid(byte id)
        {
            typeID = id;
        }

        /// <summary>
        /// 使用类型枚举实例化有地面的格子
        /// </summary>
        /// <param name="obj">格子对象</param>
        /// <param name="traget">是否存在目标点</param>
        public SceneGrid(SceneObject obj, SceneTarget traget)
        {
            typeID = (byte)(((byte)obj & 0b11) | (((byte)((byte)traget & 1)) << 2) | 0b1000);
        }

        /// <summary>
        /// 使用类型枚举实例化一个无目标点且有地面的格子
        /// </summary>
        /// <param name="obj">格子对象</param>
        public SceneGrid(SceneObject obj)
        {
            typeID = (byte)(((byte)obj & 0b11) | 0b1000);
        }

        /// <summary>
        /// 使用终点枚举实例化一个无物品且有地面的格子
        /// </summary>
        /// <param name="traget">终点枚举</param>
        public SceneGrid(SceneTarget traget)
        {
            typeID = (byte)((((byte)((byte)traget & 1)) << 2) | 0b1000);
        }

        /// <summary>
        /// 使用枚举实例化
        /// </summary>
        /// <param name="obj">格子对象</param>
        /// <param name="traget">是否存在目标点</param>
        /// <param name="ground">是否拥有地面</param>
        public SceneGrid(SceneObject obj, SceneTarget traget, SceneGround ground)
        {
            typeID = (byte)(((byte)obj & 0b11) | (((byte)((byte)traget & 1)) << 2) | (((byte)ground & 1) << 3));
        }

        #endregion

        #region 参数

        /// <summary>
        /// 格子类型id
        /// </summary>
        public readonly byte typeID;

        /// <summary>
        /// 空格子
        /// </summary>
        public static readonly SceneGrid EmptyGrid = new SceneGrid();

        #endregion

        #region 功能

        #region 参数访问

        /// <summary>
        /// 获取格子内的对象
        /// </summary>
        public SceneObject Object
        {
            get
            {
                return (SceneObject)(typeID & 0b11);
            }
        }

        /// <summary>
        /// 该格子的目标点枚举
        /// </summary>
        public SceneTarget Traget
        {
            get
            {
                return (SceneTarget)((typeID >> 2) & 1);
            }
        }

        /// <summary>
        /// 该格子的地面枚举
        /// </summary>
        public SceneGround Ground
        {
            get
            {
                return (SceneGround)((typeID >> 3) & 1);
            }
        }

        /// <summary>
        /// 该格子是否为目标点
        /// </summary>
        public bool IsTraget
        {
            get
            {
                return (((byte)(typeID >> 2)) & 1) == 1;
            }
        }

        /// <summary>
        /// 获取该对象的值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="traget">目标点枚举</param>
        public void GetValue(out SceneObject obj, out SceneTarget traget)
        {
            obj = (SceneObject)(typeID & 0b11);
            traget = (SceneTarget)((typeID >> 2) & 1);
        }

        /// <summary>
        /// 获取该对象的值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="traget">目标点枚举</param>
        /// <param name="ground">地面枚举</param>
        public void GetValue(out SceneObject obj, out SceneTarget traget, out SceneGround ground)
        {
            obj = (SceneObject)(typeID & 0b11);
            traget = (SceneTarget)((typeID >> 2) & 1);
            ground = (SceneGround)((typeID >> 3) & 1);
        }

        /// <summary>
        /// 判断该格子是什么都没有的空格子
        /// </summary>
        public bool IsEmpty
        {
            get => typeID == 0;
        }

        /// <summary>
        /// 使用新的对象和该实例内的终点参数返回一个新实例
        /// </summary>
        /// <param name="obj">新对象</param>
        /// <returns>新对象实例</returns>
        public SceneGrid NewObjectGrid(SceneObject obj)
        {
            return new SceneGrid(obj, Traget, Ground);
        }

        /// <summary>
        /// 使用指定实例的对象参数和该实例的终点参数返回一个新值
        /// </summary>
        /// <param name="grid">指定实例</param>
        /// <returns>新格子对象</returns>
        public SceneGrid NewObjectGrid(SceneGrid grid)
        {
            return new SceneGrid((byte)((grid.typeID & 0b1111_1011) | (typeID & 0b100)));
        }

        #endregion

        #region 判断

        /// <summary>
        /// 比较相等
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static bool operator ==(SceneGrid d1, SceneGrid d2)
        {
            return d1.typeID == d2.typeID;
        }

        /// <summary>
        /// 比较不相等
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static bool operator !=(SceneGrid d1, SceneGrid d2)
        {
            return d1.typeID != d2.typeID;
        }

        #endregion

        #region 派生

        public bool Equals(SceneGrid other)
        {
            return typeID == other.typeID;
        }

        public override bool Equals(object obj)
        {
            if(obj is SceneGrid d) return typeID == d.typeID;
            return false;
        }

        public override int GetHashCode()
        {
            return typeID;
        }

        /// <summary>
        /// 返回此格子的文本样式
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (typeID == 0) return "  ";

            GetValue(out var obj, out var trg, out var gr);

            if (gr == SceneGround.None) return "  ";

            if(obj == SceneObject.None)
            {
                if(trg == SceneTarget.Exist)
                {
                    return "×";
                }
                return "□";
            }
            else if (obj == SceneObject.Box)
            {
                if (trg == SceneTarget.Exist)
                {
                    return "田";
                }
                return "曰";
              
            }
            else if (obj == SceneObject.Player)
            {
                if (trg == SceneTarget.Exist) return "火";

                return "人";
            }
            else if (obj == SceneObject.Wall)
            {
                return "■";
            }

            return "□";
        }

        #endregion

        #region 集成参数

        /// <summary>
        /// 获取一个玩家站在地面上的格子
        /// </summary>
        public static SceneGrid PlayerGrid
        {
            get => new SceneGrid(SceneObject.Player);
        }

        /// <summary>
        /// 获取一个地面格子
        /// </summary>
        public static SceneGrid GroundObject
        {
            get => new SceneGrid(SceneObject.None);
        }

        /// <summary>
        /// 获取一个终点格子
        /// </summary>
        public static SceneGrid TragetGrid
        {
            get => new SceneGrid(SceneTarget.Exist);
        }

        /// <summary>
        /// 获取一个玩家在目标点的格子
        /// </summary>
        public static SceneGrid TragetPlayerGrid
        {
            get => new SceneGrid(SceneObject.Player, SceneTarget.Exist);
        }

        /// <summary>
        /// 获取一个箱子在目标点的格子
        /// </summary>
        public static SceneGrid TragetBoxGrid
        {
            get => new SceneGrid(SceneObject.Box, SceneTarget.Exist);
        }

        /// <summary>
        /// 获取一个箱子格子
        /// </summary>
        public static SceneGrid BoxGrid
        {
            get => new SceneGrid(SceneObject.Box);
        }
        
        /// <summary>
        /// 获取一个墙壁格子
        /// </summary>
        public static SceneGrid WallGrid
        {
            get => new SceneGrid(SceneObject.Wall);
        }

        #endregion

        #endregion

    }


}
