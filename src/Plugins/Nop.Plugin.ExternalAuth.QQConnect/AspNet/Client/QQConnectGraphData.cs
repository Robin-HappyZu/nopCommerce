using System;
using System.Runtime.Serialization;

namespace Nop.Plugin.ExternalAuth.QQConnect.AspNet.Client
{
    [DataContract]
    public class QQConnectGraphData
    {
        /// <summary>
        /// 用户在QQ空间的昵称。
        /// </summary>
        [DataMember(Name = "nickname", IsRequired = true)]
        public string Nickname { get; set; }
        /// <summary>
        /// 大小为30×30像素的QQ空间头像URL。
        /// </summary>
        [DataMember(Name = "figureurl", IsRequired = true)]
        public string Figureurl { get; set; }
        /// <summary>
        /// 大小为50×50像素的QQ空间头像URL。
        /// </summary>
        [DataMember(Name = "figureurl_1", IsRequired = true)]
        public string Figureurl_1 { get; set; }
        /// <summary>
        /// 大小为100×100像素的QQ空间头像URL。
        /// </summary>
        [DataMember(Name = "figureurl_2", IsRequired = true)]
        public string Figureurl_2 { get; set; }
        /// <summary>
        /// 大小为40×40像素的QQ头像URL。
        /// </summary>
        [DataMember(Name = "figureurl_qq_1", IsRequired = true)]
        public string Figureurl_qq_1 { get; set; }
        /// <summary>
        /// 大小为100×100像素的QQ头像URL。需要注意，不是所有的用户都拥有QQ的100x100的头像，但40x40像素则是一定会有。
        /// </summary>
        [DataMember(Name = "figureurl_qq_2")]
        public string Figureurl_qq_2 { get; set; }
        /// <summary>
        /// 性别。 如果获取不到则默认返回"男"
        /// </summary>
        [DataMember(Name = "gender", IsRequired = true)]
        public string Gender { get; set; }
        /// <summary>
        /// 标识用户是否为黄钻用户（0：不是；1：是）。
        /// </summary>
        [DataMember(Name = "is_yellow_vip", IsRequired = true)]
        public string Is_Yellow_Vip { get; set; }
        /// <summary>
        /// 标识用户是否为黄钻用户（0：不是；1：是）
        /// </summary>
        [DataMember(Name = "vip", IsRequired = true)]
        public string Vip { get; set; }
        /// <summary>
        /// 黄钻等级
        /// </summary>
        [DataMember(Name = "yellow_vip_level", IsRequired = true)]
        public string Yellow_Vip_Level { get; set; }
        /// <summary>
        /// 黄钻等级
        /// </summary>
        [DataMember(Name = "level", IsRequired = true)]
        public string Level { get; set; }
        /// <summary>
        /// 标识是否为年费黄钻用户（0：不是； 1：是）
        /// </summary>
        [DataMember(Name = "is_yellow_year_vip", IsRequired = true)]
        public string Is_Yellow_Year_Vip { get; set; }
    }
}
