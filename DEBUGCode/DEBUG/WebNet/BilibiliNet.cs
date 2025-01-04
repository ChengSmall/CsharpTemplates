using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cheng.DEBUG
{


    public class BilibiliNet
    {

        #region web接口

        /// <summary>
        /// 获取Bilibili弹幕网视频信息的uri头地址
        /// </summary>
        public const string ViewInformationHand = "https://api.bilibili.com/x/web-interface/wbi/view";

        /// <summary>
        /// 使用bv号获取访问视频基本信息的uri
        /// </summary>
        /// <param name="bvID">视频bv号</param>
        /// <returns>用于访问该BV号下视频的基本信息的uri</returns>
        public static string GetViewMessageUriByBV(string bvID)
        {
            return (ViewInformationHand + "?bvid=") + bvID;
        }

        /// <summary>
        /// 使用av号获取访问视频基本信息的uri
        /// </summary>
        /// <param name="avID">视频AV号，不要加"AV"前缀</param>
        /// <returns>用于访问该AV号下视频的基本信息的uri</returns>
        public static string GetViewMessageUriByAV(string avID)
        {
            return (ViewInformationHand + "?aid=") + avID;
        }

        #endregion

    }

}
