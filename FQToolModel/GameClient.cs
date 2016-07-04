using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FQToolModel
{
    /// <summary>
    /// 游戏对象
    /// </summary>
    public class GameClient
    {
        /// <summary>
        /// 游戏句柄
        /// </summary>
        public int Handle { get; set; }

        /// <summary>
        /// 摆摊对象
        /// </summary>
        public Interrogator IG { get; set; }

        /// <summary>
        /// 游戏位置
        /// </summary>
        public Point GamePosint { get; set; }
    }
}
