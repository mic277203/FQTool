using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace FOToolHelper
{
    public class MouseHelper
    {
        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, UIntPtr dwExtraInfo);

        /// <summary>
        /// 鼠标左击
        /// </summary>
        public static void LeftClick()
        {
            mouse_event(0x02, 0, 0, 0, UIntPtr.Zero);
            mouse_event(0x04, 0, 0, 0, UIntPtr.Zero);
        }
        /// <summary>
        /// 鼠标移动到指定的位置
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void MovePoint(Point p)
        {
            SetCursorPos(p.X, p.Y);
        }
    }
}
