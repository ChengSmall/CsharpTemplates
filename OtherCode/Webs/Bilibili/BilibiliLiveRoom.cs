using Cheng.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cheng.Bilibili
{

    /// <summary>
    /// Bilibili直播间基本信息
    /// </summary>
    public class BilibiliLiveRoom
    {

        #region

        public BilibiliLiveRoom()
        {
        }

        #endregion

        #region 参数

        /// <summary>
        /// 直播间uri
        /// </summary>
        public string url;

        /// <summary>
        /// 直播间房间号
        /// </summary>
        public string roomID;

        /// <summary>
        /// 直播间标题
        /// </summary>
        public string title;

        /// <summary>
        /// 直播间状态，true表示正在直播，false表示没有正在直播
        /// </summary>
        public bool liveStatus;

        /// <summary>
        /// 直播间人数
        /// </summary>
        public long watchedShowCount;

        #endregion

    }
}