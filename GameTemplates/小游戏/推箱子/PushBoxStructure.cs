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
        None = 0,
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
         * 目标点 第3bit => 0b00000X00
         * 地面 第4bit => 0b0000X000
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
            p_typeID = id;
        }
        /// <summary>
        /// 使用类型枚举实例化有地面的格子
        /// </summary>
        /// <param name="obj">格子对象</param>
        /// <param name="traget">是否存在目标点</param>
        public SceneGrid(SceneObject obj, SceneTarget traget)
        {
            p_typeID = (byte)(((byte)obj & 0b11) | (((byte)((byte)traget & 1)) << 2) | 0b1000);
        }
        /// <summary>
        /// 使用类型枚举实例化一个无目标点且有地面的格子
        /// </summary>
        /// <param name="obj">格子对象</param>
        public SceneGrid(SceneObject obj)
        {
            p_typeID = (byte)(((byte)obj & 0b11) | 0b1000);
        }
        /// <summary>
        /// 使用终点枚举实例化一个无物品且有地面的格子
        /// </summary>
        /// <param name="traget">终点枚举</param>
        public SceneGrid(SceneTarget traget)
        {
            p_typeID = (byte)((((byte)((byte)traget & 1)) << 2) | 0b1000);
        }
        /// <summary>
        /// 使用枚举实例化
        /// </summary>
        /// <param name="obj">格子对象</param>
        /// <param name="traget">是否存在目标点</param>
        /// <param name="ground">是否拥有地面</param>
        public SceneGrid(SceneObject obj, SceneTarget traget, SceneGround ground)
        {
            p_typeID = (byte)(((byte)obj & 0b11) | (((byte)((byte)traget & 1)) << 2) | (((byte)ground & 1) << 3));
        }
        #endregion

        #region 参数

        private byte p_typeID;

        /// <summary>
        /// 空格子
        /// </summary>
        public static readonly SceneGrid EmptyGrid = new SceneGrid();

        #endregion

        #region 功能

        #region 参数访问

        /// <summary>
        /// 获取格子类型id
        /// </summary>
        public byte TypeID
        {
            get => p_typeID;
        }
        /// <summary>
        /// 获取格子内的对象
        /// </summary>
        public SceneObject Object
        {
            get
            {
                return (SceneObject)(p_typeID & 0b11);
            }
        }
        /// <summary>
        /// 该格子的目标点枚举
        /// </summary>
        public SceneTarget Traget
        {
            get
            {
                return (SceneTarget)((p_typeID >> 2) & 1);
            }
        }
        /// <summary>
        /// 该格子的地面枚举
        /// </summary>
        public SceneGround Ground
        {
            get
            {
                return (SceneGround)((p_typeID >> 3) & 1);
            }
        }

        /// <summary>
        /// 该格子是否为目标点
        /// </summary>
        public bool IsTraget
        {
            get
            {
                return (((byte)(p_typeID >> 2)) & 1) == 1;
            }
        }
        /// <summary>
        /// 获取该对象的值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="traget">目标点枚举</param>
        public void GetValue(out SceneObject obj, out SceneTarget traget)
        {
            obj = (SceneObject)(p_typeID & 0b11);
            traget = (SceneTarget)((p_typeID >> 2) & 1);
        }
        /// <summary>
        /// 获取该对象的值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="traget">目标点枚举</param>
        /// <param name="ground">地面枚举</param>
        public void GetValue(out SceneObject obj, out SceneTarget traget, out SceneGround ground)
        {
            obj = (SceneObject)(p_typeID & 0b11);
            traget = (SceneTarget)((p_typeID >> 2) & 1);
            ground = (SceneGround)((p_typeID >> 3) & 1);
        }

        /// <summary>
        /// 判断该格子是什么都没有的空格子
        /// </summary>
        public bool IsEmpty
        {
            get => p_typeID == 0;
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
            return new SceneGrid((byte)((grid.p_typeID & 0b1111_1011) | (p_typeID & 0b100)));
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
            return d1.p_typeID == d2.p_typeID;
        }
        /// <summary>
        /// 比较不相等
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static bool operator !=(SceneGrid d1, SceneGrid d2)
        {
            return d1.p_typeID != d2.p_typeID;
        }
        #endregion

        #region 派生

        public bool Equals(SceneGrid other)
        {
            return p_typeID == other.p_typeID;
        }
        public override bool Equals(object obj)
        {
            if(obj is SceneGrid d) return p_typeID == d.p_typeID;
            return false;
        }

        public override int GetHashCode()
        {
            return p_typeID;
        }
        /// <summary>
        /// 返回此格子的文本样式
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (p_typeID == 0) return "  ";

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
                    return "目";
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

        #endregion

    }



}
