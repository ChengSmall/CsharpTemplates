using Cheng.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cheng.Bilibili
{

    /// <summary>
    /// bilibili用户基本信息
    /// </summary>
    public class BilibiliSpaceInfo
    {

        #region 构造

        public BilibiliSpaceInfo()
        {
            f_init();
        }

        /// <summary>
        /// 使用json数据初始化data
        /// </summary>
        /// <param name="jsonData"></param>
        /// <exception cref="ArgumentNullException">为null</exception>
        /// <exception cref="NotImplementedException">数据格式错误</exception>
        public BilibiliSpaceInfo(JsonDictionary jsonData)
        {
            if (jsonData is null) throw new ArgumentNullException();
            try
            {
                f_init();
                f_initToJson(jsonData);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException("data格式不正确", ex);
            }
        }

        private void f_init()
        {
            p_liveRoom = new BilibiliLiveRoom();
            p_mid = string.Empty;
            p_name = string.Empty;
            p_sign = string.Empty;
            p_faceUri = string.Empty;
            p_topPhotoUri = string.Empty;
            p_sex = string.Empty;
            p_lvl = 0;
        }

        private void f_initToJson(JsonDictionary json)
        {

            JsonVariable jt;
            jt = json["mid"];
            if (jt.DataType == JsonType.Integer)
            {
                Mid = jt.Integer.ToString();
            }
            else
            {
                Mid = jt.String;
            }

            jt = json["name"];
            Name = jt.String;

            jt = json["sex"];
            Sex = jt.String;

            jt = json["face"];
            FaceUrl = jt.String;

            jt = json["sign"];
            Sign = jt.String;

            jt = json["level"];
            Level = (int)jt.Integer;

            f_ToLive(LiveRoom, json["live_room"].JsonObject);
        }

        static void f_ToLive(BilibiliLiveRoom live, JsonDictionary json)
        {
            live.liveStatus = json["liveStatus"].Integer == 1;

            live.url = json["url"].String;

            live.title = json["title"].String;

            live.roomID = json["roomid"].Integer.ToString();

            live.watchedShowCount = json["watched_show"].JsonObject["num"].Integer;
        }

        #endregion

        #region 参数

        private string p_mid;

        private string p_sex;

        private string p_name;

        private string p_sign;

        private string p_faceUri;

        private string p_topPhotoUri;

        private BilibiliLiveRoom p_liveRoom;

        private int p_lvl;

        #endregion

        #region 功能

        #region 参数访问

        /// <summary>
        /// 用户Uid
        /// </summary>
        public string Mid
        {
            get => p_mid;
            set
            {
                p_mid = value ?? string.Empty;
            }
        }

        /// <summary>
        /// 用户昵称
        /// </summary>
        public string Name
        {
            get => p_name;
            set => p_name = value ?? string.Empty;
        }

        /// <summary>
        /// 性别
        /// </summary>
        public string Sex
        {
            get => p_sex;
            set => p_sex = value ?? string.Empty;
        }

        /// <summary>
        /// 用户个性签名
        /// </summary>
        public string Sign
        {
            get => p_sign;
            set => p_sign = value ?? string.Empty;
        }

        /// <summary>
        /// 用户账户等级
        /// </summary>
        public int Level
        {
            get => p_lvl;
            set
            {
                p_lvl = value;
            }
        }

        /// <summary>
        /// 头像图像的uri
        /// </summary>
        public string FaceUrl
        {
            get => p_faceUri;
            set => p_faceUri = value ?? string.Empty;
        }

        /// <summary>
        /// 主页顶部图片的uri
        /// </summary>
        public string TopPhotoUrl
        {
            get => p_topPhotoUri;
            set => p_topPhotoUri = value ?? string.Empty;
        }

        /// <summary>
        /// 用户直播间基本信息
        /// </summary>
        public BilibiliLiveRoom LiveRoom
        {
            get => p_liveRoom;
        }

        #endregion

        #region 解析器

        /// <summary>
        /// 将json数据转化到实例
        /// </summary>
        /// <param name="json">从bapi获取的json数据</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="NotImplementedException">解析错误</exception>
        /// <exception cref="BilibiliErrorException">消息获取错误</exception>
        public static BilibiliSpaceInfo JsonToData(JsonVariable json)
        {
            if (json is null) throw new ArgumentNullException();

            var jd = json.JsonObject;

            var code = jd["code"].Integer;

            if(code != 0)
            {
                throw new BilibiliErrorException(jd["message"].String, (int)code);
            }
            
            return new BilibiliSpaceInfo(jd["data"].JsonObject);
        }

        #endregion

        #region 派生

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(64);

            sb.Append("Name:");
            sb.Append(Name);
            sb.Append(' ');

            sb.Append("Sex:");
            sb.Append(Sex);
            sb.Append(' ');

            sb.Append("Mid:");
            sb.Append(Mid);
            sb.Append(' ');

            sb.Append('l');
            sb.Append('v');
            sb.AppendLine(Level.ToString());

            sb.Append("Sign:");
            sb.Append(Sign);

            return sb.ToString();
        }

        #endregion

        #endregion

    }

}
