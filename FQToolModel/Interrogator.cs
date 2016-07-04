using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace FQToolModel
{

    /// <summary>
    /// 摊位
    /// </summary>
    public class Interrogator
    {
        /// <summary>
        /// 购买
        /// </summary>
        public Point Buy { get; set; }
        /// <summary>
        /// 下一页的下字
        /// </summary>
        public Point Next { get; set; }
        /// <summary>
        /// 搜索框
        /// </summary>
        public Point QueryTextBox { get; set; }
        /// <summary>
        /// 第一行价格位置
        /// </summary>
        public Point FirstRow { get; set; }
        /// <summary>
        /// 第二行价格位置
        /// </summary>
        public Point TwoRow { get; set; }
        /// <summary>
        /// 参照点
        /// </summary>
        public Point Refer { get; set; }
        /// <summary>
        /// 武器
        /// </summary>
        public Point Arms { get; set; }
        /// <summary>
        /// 防具
        /// </summary>
        public Point Armor { get; set; }
        /// <summary>
        /// 加工
        /// </summary>
        public Point Process { get; set; }
        /// <summary>
        /// 宠物
        /// </summary>
        public Point Pet { get; set; }
        /// <summary>
        /// 道具
        /// </summary>
        public Point Props { get; set; }
        /// <summary>
        /// 战魂
        /// </summary>
        public Point FightingSpirit { get; set; }

        /// <summary>
        /// 下一页
        /// </summary>
        public Point NextPage { get; set; }
        /// <summary>
        /// 价格Icon
        /// </summary>
        public Point Doller { get; set; }
        /// <summary>
        /// 搜索按钮
        /// </summary>
        public Point BtnQuery { get; set; }

        /// <summary>
        /// 价格对照集合
        /// </summary>
        public List<PriceMapping> ListPriceMappings { get; set; }
        /// <summary>
        /// 灰色下一页
        /// </summary>
        public string NpImgStr { get; set; }
    }
}
