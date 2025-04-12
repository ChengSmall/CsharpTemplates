using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cheng.GameTemplates.PushingBoxes.XSB
{

    /// <summary>
    /// 一个推箱子关卡
    /// </summary>
    public struct PushBoxLevel
    {

        internal PushBoxLevel(string title, string author, PushBoxScene scene)
        {
            this.title = title;
            this.author = author;
            this.scene = scene;
        }

        /// <summary>
        /// 关卡标题
        /// </summary>
        public readonly string title;

        /// <summary>
        /// 关卡作者
        /// </summary>
        public readonly string author;

        /// <summary>
        /// 关卡场景
        /// </summary>
        public readonly PushBoxScene scene;

    }


}
