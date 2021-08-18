using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;

namespace BlessFindPic
{
    public class GetScreen
    {
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateDC(
           string lpszDriver,        // driver name驱动名
           string lpszDevice,        // device name设备名
           string lpszOutput,        // not used; should be NULL
           IntPtr lpInitData  // optional printer data
           );
        [DllImport("gdi32.dll")]
        public static extern int BitBlt(
         IntPtr hdcDest, // handle to destination DC目标设备的句柄
         int nXDest,  // x-coord of destination upper-left corner目标对象的左上角的X坐标
         int nYDest,  // y-coord of destination upper-left corner目标对象的左上角的Y坐标
         int nWidth,  // width of destination rectangle目标对象的矩形宽度
         int nHeight, // height of destination rectangle目标对象的矩形长度
         IntPtr hdcSrc,  // handle to source DC源设备的句柄
         int nXSrc,   // x-coordinate of source upper-left corner源对象的左上角的X坐标
         int nYSrc,   // y-coordinate of source upper-left corner源对象的左上角的Y坐标
         UInt32 dwRop  // raster operation code光栅的操作值
         );

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(
         IntPtr hdc // handle to DC
         );

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(
         IntPtr hdc,        // handle to DC
         int nWidth,     // width of bitmap, in pixels
         int nHeight     // height of bitmap, in pixels
         );

        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(
         IntPtr hdc,          // handle to DC
         IntPtr hgdiobj   // handle to object
         );

        [DllImport("gdi32.dll")]
        public static extern int DeleteDC(
         IntPtr hdc          // handle to DC
         );

        [DllImport("user32.dll")]
        public static extern bool PrintWindow(
         IntPtr hwnd,               // Window to copy,Handle to the window that will be copied.
         IntPtr hdcBlt,             // HDC to print into,Handle to the device context.
         UInt32 nFlags              // Optional flags,Specifies the drawing options. It can be one of the following values.
         );

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(
         IntPtr hwnd
         );

        [DllImport("user32.dll")]
        private extern static IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        public struct RECT
        {
            public int Left;                             //最左坐标
            public int Top;                             //最上坐标
            public int Right;                           //最右坐标
            public int Bottom;                        //最下坐标
        }

        /// <summary>
        /// 获取窗口截图，图像周围有边框，和按键精灵截图工具的图像边框大小一致
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <returns>Bitmap</returns>
        public static Bitmap getWindow(IntPtr hWnd)
        {
            IntPtr hscrdc = GetWindowDC(hWnd);
            //Control control = Control.FromHandle(hWnd);
            RECT rc = new RECT();
            GetWindowRect(hWnd, ref rc);
            IntPtr hbitmap = CreateCompatibleBitmap(hscrdc, (rc.Right - rc.Left), (rc.Bottom - rc.Top));
            IntPtr hmemdc = CreateCompatibleDC(hscrdc);
            SelectObject(hmemdc, hbitmap);
            PrintWindow(hWnd, hmemdc, 0);
            Bitmap bmp = Bitmap.FromHbitmap(hbitmap);
            DeleteDC(hscrdc);
            DeleteDC(hmemdc);
            return bmp;
        }

        /// <summary>
        /// 保存截图
        /// </summary>
        /// <param name="b">Bitmap</param>
        /// <param name="fileName">名称</param>
        /// <param name="path">保存路径</param>
        public static void saveBitMap(Bitmap b, String fileName, String path)
        {
            b.Save(path + fileName + ".bmp", ImageFormat.Bmp);
        }

        
        /// <summary>
        /// 找到窗口
        /// </summary>
        /// <param name="className">类名</param>
        /// <param name="gameName">窗口名</param>
        /// <returns>句柄</returns>
        public static IntPtr findWindow(String className,String gameName)
        {
            IntPtr hWnd = FindWindow(className, gameName);
            if (hWnd != IntPtr.Zero)
            { }
            else
            {
                Console.Write("className：" + className +  "|"  + "gameName：" + gameName +  "|" + "没找到窗口");
            }
            return hWnd;
        }

        /// <summary>
        /// 输入坐标保存截图中的一小块截图，用于开发用
        /// </summary>
        /// <param name="b">Bitmap</param>
        /// <param name="left">左上角坐标</param>
        /// <param name="top">顶部坐标</param>
        /// <param name="right">右边坐标</param>
        /// <param name="bottom">底部坐标</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="path">路径名称</param>
        public static void saveBitMapByOffset(Bitmap b, int left, int top, int right, int bottom,String fileName,String path)
        {
            Image fromImage = b;
            //创建新图位图
            int width = right - left;
            int height = bottom - top;
            Bitmap bitmap = new Bitmap(width, height);
            //创建作图区域
            Graphics graphic = Graphics.FromImage(bitmap);
            
            //截取原图相应区域写入作图区

            graphic.DrawImage(fromImage, 0, 0, new Rectangle(left, top, width, height), GraphicsUnit.Pixel);
            //从作图区生成新图
            Image saveImage = Image.FromHbitmap(bitmap.GetHbitmap());
            //保存图片
            saveImage.Save(path + fileName + ".bmp", ImageFormat.Bmp);
            //释放资源   
            saveImage.Dispose();
            graphic.Dispose();
            bitmap.Dispose();
        }

    }


}
