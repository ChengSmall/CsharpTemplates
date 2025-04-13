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

        internal PushBoxLevel(Exception exception)
        {
            this.exception = exception;
            title = null;
            author = null;
            scene = null;
        }

        internal PushBoxLevel(string title, string author, PushBoxScene scene)
        {
            this.title = title;
            this.author = author;
            this.scene = scene;
            exception = null;
        }

        /// <summary>
        /// 关卡标题
        /// </summary>
        public string title;

        /// <summary>
        /// 关卡作者
        /// </summary>
        public string author;

        /// <summary>
        /// 关卡场景
        /// </summary>
        public PushBoxScene scene;

        /// <summary>
        /// 关卡在读取或解析时出错的错误，没有错误则是null
        /// </summary>
        public Exception exception;

        /// <summary>
        /// 克隆该关卡实例
        /// </summary>
        /// <returns>相同标题和场景的不同实例</returns>
        public PushBoxLevel Clone()
        {
            var lvl = new PushBoxLevel(title, author, scene?.Clone());
            lvl.exception = this.exception;
            return lvl;
        }

    }


}
