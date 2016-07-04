using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FOToolHelper;
using System.IO;
using FQToolModel;
using System.Threading;
using System.Diagnostics;
using static FOToolHelper.HandleHelper;
using GlobalHotkeys;

namespace FQTool
{
    public partial class frmMain : Form
    {
        private GlobalHotkey ghk;
        private GameClient gaClient = new GameClient();

        public frmMain()
        {
            InitializeComponent();
        }

        protected override void WndProc(ref Message m)
        {
            var hotkeyInfo = HotkeyInfo.GetFromMessage(m);
            if (hotkeyInfo != null)
            {
                HotkeyProc(hotkeyInfo);
            }

            base.WndProc(ref m);
        }

        private void HotkeyProc(HotkeyInfo hotkeyInfo)
        {
            switch (hotkeyInfo.Key)
            {
                case Keys.Home:
                    {
                        Application.Exit();
                    }; break;
                case Keys.Insert:
                    {
                        InsertPriceScript(gaClient);
                    }; break;
            }
        }

        /// <summary>
        /// 判断是否到达最后一页
        /// </summary>
        /// <returns></returns>
        private bool NonNextPage(GameClient gc)
        {
            Bitmap image = new Bitmap(52, 32);
            Graphics imgGraphics = Graphics.FromImage(image);
            //设置截屏区域
            imgGraphics.CopyFromScreen(gc.IG.Next.X, gc.IG.Next.Y, 0, 0, new Size(52, 32));
            //设置屏幕路径
            string pathPM = Environment.CurrentDirectory + "\\Next.jpg";
            //判断是否存在屏幕图片
            if (File.Exists(pathPM))
            {
                File.Delete(pathPM);
            }
            image.Save(pathPM, ImageFormat.Jpeg);

            Bitmap bmpPm = new Bitmap(pathPM);
            Bitmap bmpCz = new Bitmap(Environment.CurrentDirectory + "\\Images\\nextcp.jpg");

            var result = bmpPm.ContainsImg(bmpCz);

            imgGraphics.Dispose();
            bmpPm.Dispose();
            bmpCz.Dispose();

            return result;
        }

        /// <summary>
        /// 设置参照物位置
        /// </summary>
        /// <returns></returns>
        private void SetCzPoint(GameClient gc)
        {
            Bitmap image = new Bitmap(806, 625);
            Graphics imgGraphics = Graphics.FromImage(image);
            ////设置截屏区域
            imgGraphics.CopyFromScreen(gc.GamePosint.X, gc.GamePosint.Y, 0, 0, new Size(806, 625));
            //设置屏幕路径
            string pathPM = Environment.CurrentDirectory + "\\pm.jpg";
            //判断是否存在屏幕图片
            if (File.Exists(pathPM))
            {
                File.Delete(pathPM);
            }

            image.Save(pathPM, ImageFormat.Jpeg);

            Bitmap bmpPm = new Bitmap(pathPM);
            Bitmap bmpCz = new Bitmap(Environment.CurrentDirectory + "\\Images\\cp.jpg");

            gc.IG = new Interrogator() { Refer = bmpPm.ContainsGetPoint(bmpCz) };

            imgGraphics.Dispose();
            bmpPm.Dispose();
            bmpCz.Dispose();
        }

        /// <summary>
        /// 初始化摊位对象
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private void InitIG(GameClient gc)
        {
            Interrogator ig = gc.IG;

            ig.Next = new Point() { X = gc.GamePosint.X + gc.IG.Refer.X + 391, Y = gc.GamePosint.Y + gc.IG.Refer.Y - 45 };
            ig.FirstRow = new Point() { X = gc.GamePosint.X + gc.IG.Refer.X + 245, Y = gc.GamePosint.Y + gc.IG.Refer.Y + 13 };
            ig.TwoRow = new Point() { X = ig.FirstRow.X + 36, Y = ig.FirstRow.Y };
            ig.QueryTextBox = new Point() { X = gc.GamePosint.X + ig.Refer.X + 48, Y = gc.GamePosint.Y + ig.Refer.Y - 18 };
            ig.Arms = new Point() { X = gc.GamePosint.X + ig.Refer.X + 16, Y = gc.GamePosint.Y + ig.Refer.Y + 29 };
            ig.Armor = new Point() { X = ig.Arms.X, Y = ig.Arms.Y + 21 };
            ig.Process = new Point() { X = ig.Arms.X, Y = ig.Armor.Y + 21 };
            ig.Pet = new Point() { X = ig.Arms.X, Y = ig.Process.Y + 21 };
            ig.Props = new Point() { X = ig.Arms.X, Y = ig.Pet.Y + 21 };
            ig.FightingSpirit = new Point() { X = ig.Arms.X, Y = ig.Props.Y + 21 };
            ig.BtnQuery = new Point() { X = gc.GamePosint.X + gc.IG.Refer.X + 145, Y = gc.GamePosint.Y + gc.IG.Refer.Y - 15 };
            //ig.Doller = new Point() { X = 308, Y = 192 };
            ig.NextPage = new Point() { X = gc.GamePosint.X + gc.IG.Refer.X + 425, Y = gc.GamePosint.Y + gc.IG.Refer.Y - 31 };
            //ig.NpImgStr = Convert.ToBase64String(File.ReadAllBytes(Environment.CurrentDirectory + "\\Images\\np2.tif"));
            ig.Buy = new Point() { X = gc.GamePosint.X + ig.Refer.X + 394, Y = gc.GamePosint.Y + ig.Refer.Y + 14 };

            ig.ListPriceMappings = GetPriceMapping();

        }
        /// <summary>
        /// 获取价格图片映射
        /// </summary>
        /// <returns></returns>
        private List<PriceMapping> GetPriceMapping()
        {
            //加载
            DirectoryInfo d = new DirectoryInfo(Environment.CurrentDirectory + "\\Images\\PM");
            var files = d.GetFiles();

            var listPM = new List<PriceMapping>();

            files.ToList().ForEach(p =>
            {
                PriceMapping pm = new PriceMapping();
                pm.Md5Str = Convert.ToBase64String(File.ReadAllBytes(p.FullName));
                pm.Price = Convert.ToInt32(p.Name.Split('.')[0]);

                listPM.Add(pm);
            });

            return listPM;
        }
        /// <summary>
        /// 根据句柄移动窗体
        /// </summary>
        /// <param name="handle"></param>
        public static void MoveFormByHandle(int handle)
        {
            const short SWP_NOSIZE = 1;
            const short SWP_NOZORDER = 0X4;
            const int SWP_SHOWWINDOW = 0x0040;
            HandleHelper.SetWindowPos(new IntPtr(handle), 0, 0, 0, 0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);
        }
        /// <summary>
        /// 获取自由幻想句柄
        /// </summary>
        /// <returns></returns>
        public static int GetFFoHandle()
        {
            Process[] processes = Process.GetProcessesByName("qqffo");

            var p = processes.FirstOrDefault();

            if (p == null)
            {
                return 0;
            }
            else
            {
                return p.MainWindowHandle.ToInt32();
            }
        }

        private void btnInit_Click(object sender, EventArgs e)
        {
            int handle = GetFFoHandle();

            if (handle == 0)
            {
                lblMsg.Text = "请确认已经打开游戏";
                return;
            }
            else
            {
                gaClient.Handle = handle;
                SetClientPosintByHandle(gaClient);
                SetCzPoint(gaClient);

                if (gaClient.IG.Refer.X == 0 && gaClient.IG.Refer.Y == 0)
                {
                    lblMsg.Text += "请打开摆摊查询";
                    return;
                }
                else
                {
                    //初始化摊位对象
                    InitIG(gaClient);
                }


            }

            lblMsg.Text = "初始化完毕,请勿移动游戏窗口";

        }

        /// <summary>
        /// 初始化游戏位置
        /// </summary>
        /// <param name="handle"></param>
        private void SetClientPosintByHandle(GameClient gaClient)
        {
            RECT rec = new RECT();
            HandleHelper.GetWindowRect(new IntPtr(gaClient.Handle), ref rec);

            gaClient.GamePosint = new Point() { X = rec.Left, Y = rec.Top };
        }
        
        private void btnGetPrice_Click(object sender, EventArgs e)
        {
            MouseHelper.MovePoint(gaClient.IG.Buy);
            Thread.Sleep(100);
            MouseHelper.LeftClick();
            //Thread.Sleep(2000);
            //MouseHelper.LeftClick();
            //Thread.Sleep(2000);
            //MouseHelper.MovePoint(new Point(gaClient.IG.Buy.X, gaClient.IG.Buy.Y - 7));
            //Thread.Sleep(100);
            //MouseHelper.LeftClick();
            
        }

        #region 动作类
        /// <summary>
        /// 获取价格
        /// </summary>
        private void GetPrice(GameClient gaClient)
        {
            Bitmap image = new Bitmap(26, 18);
            Graphics imgGraphics = Graphics.FromImage(image);
            //设置截屏区域    
            imgGraphics.CopyFromScreen(gaClient.IG.FirstRow.X, gaClient.IG.FirstRow.Y, 0, 0, new Size(26, 18));
            image = ImageHelper.DealBlackAndWhite(image);

            Bitmap image2 = new Bitmap(26, 18);
            Graphics imgGraphics2 = Graphics.FromImage(image2);
            //设置截屏区域    
            imgGraphics2.CopyFromScreen(gaClient.IG.TwoRow.X, gaClient.IG.TwoRow.Y, 0, 0, new Size(26, 18));
            //保存
            image2 = ImageHelper.DealBlackAndWhite(image2);

            string firstPath = Environment.CurrentDirectory + "\\firstPrice.tif";
            string secondPath = Environment.CurrentDirectory + "\\secondPrice.tif";

            if (File.Exists(firstPath))
            {
                File.Delete(firstPath);
            }

            if (File.Exists(secondPath))
            {
                File.Delete(secondPath);
            }

            image.Save(firstPath, ImageFormat.Tiff);
            image2.Save(secondPath, ImageFormat.Tiff);

            Bitmap firstBmp = new Bitmap(firstPath);
            Bitmap secondBmp = new Bitmap(secondPath);

            string one = firstBmp.ToBaseMd5();
            string two = secondBmp.ToBaseMd5();

            var j = gaClient.IG.ListPriceMappings.FirstOrDefault(p => p.Md5Str == one);
            var y = gaClient.IG.ListPriceMappings.FirstOrDefault(p => p.Md5Str == two);

            if (j != null && y != null)
            {
                lblMsg.Text = string.Format("金：{0} 银：{1}", j.Price, y.Price);

            }
            else
            {
                image.Save("D:\\TestTif2\\" + Guid.NewGuid().ToString("N") + ".tif");
                image2.Save("D:\\TestTif2\\" + Guid.NewGuid().ToString("N") + ".tif");
            }

            firstBmp.Dispose();
            secondBmp.Dispose();
            image.Dispose();
            image2.Dispose();
            imgGraphics.Dispose();
            imgGraphics2.Dispose();
        }
        private void InsertPriceScript(GameClient gaClient)
        {
            while (true)
            {
                MouseHelper.MovePoint(gaClient.IG.QueryTextBox);
                Thread.Sleep(100);
                MouseHelper.LeftClick();
                Thread.Sleep(100);
                MouseHelper.MovePoint(gaClient.IG.Arms);
                Thread.Sleep(100);
                MouseHelper.LeftClick();
                Thread.Sleep(3000);
                MouseHelper.MovePoint(gaClient.IG.BtnQuery);
                Thread.Sleep(1000);
                MouseHelper.LeftClick();
                PageGetPrice(gaClient);

                MouseHelper.MovePoint(gaClient.IG.QueryTextBox);
                Thread.Sleep(100);
                MouseHelper.LeftClick();
                Thread.Sleep(100);
                MouseHelper.MovePoint(gaClient.IG.Armor);
                Thread.Sleep(100);
                MouseHelper.LeftClick();
                Thread.Sleep(3000);
                MouseHelper.MovePoint(gaClient.IG.BtnQuery);
                Thread.Sleep(1000);
                MouseHelper.LeftClick();
                PageGetPrice(gaClient);

                MouseHelper.MovePoint(gaClient.IG.QueryTextBox);
                Thread.Sleep(100);
                MouseHelper.LeftClick();
                Thread.Sleep(100);
                MouseHelper.MovePoint(gaClient.IG.Props);
                Thread.Sleep(100);
                MouseHelper.LeftClick();
                Thread.Sleep(3000);
                MouseHelper.MovePoint(gaClient.IG.BtnQuery);
                Thread.Sleep(1000);
                MouseHelper.LeftClick();
                PageGetPrice(gaClient);
            }
        }
        private void PageGetPrice(GameClient gaClient)
        {
            while (NonNextPage(gaClient))
            {
                Thread.Sleep(300);
                GetPrice(gaClient);
                MouseHelper.MovePoint(gaClient.IG.NextPage);
                Thread.Sleep(100);
                MouseHelper.LeftClick();
                Thread.Sleep(1000);
            }
        }
        #endregion

        private void frmMain_Load(object sender, EventArgs e)
        {
            if (ghk != null)
            {
                ghk.Unregister();
            }

            var key = Keys.Home;
            var mod = Modifiers.NoMod;
            mod |= Modifiers.Alt;

            try
            {
                ghk = new GlobalHotkey(mod, key, this, true);

                ghk = new GlobalHotkey(mod, Keys.Insert, this, true);
            }
            catch (GlobalHotkeyException exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            ghk.Dispose();
        }
    }
}
