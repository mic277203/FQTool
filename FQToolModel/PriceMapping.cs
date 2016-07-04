using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FQToolModel
{
    /// <summary>
    /// 价格对照
    /// </summary>
    public class PriceMapping
    {
        /// <summary>
        /// 价格图片MD5值
        /// </summary>
        public string Md5Str { get; set; }
        /// <summary>
        /// 价格实际值
        /// </summary>
        public int Price { get; set; }
    }
}
