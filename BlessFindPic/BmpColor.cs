using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlessFindPic
{
    public class BmpColor
    {
        private static BmpColor instance;

        public Hashtable bmpTable;
        public Hashtable bmpSubTable;
        public BmpColor()
        {
            bmpTable = new Hashtable();
            bmpSubTable = new Hashtable();
        }
        public static BmpColor Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new BmpColor();
                }
                return instance;
            }
        }

        /// <summary>
        /// 在大图里找小图
        /// </summary>
        /// <param name="sBmp">大图</param>
        /// <param name="pBmp">小图</param>
        /// <param name="similar">容错值 取值0--255，数值越高效率越低，不建议超过50</param>
        /// <param name="isOnly">为true时表示只找1个，找到后立刻返回</param>
        /// <returns></returns>
        public static List<Point> findPicOrigin(int left, int top, int width, int height, Bitmap sBmp, Bitmap pBmp, int similar, bool isOnly)
        {
            if (sBmp.PixelFormat != PixelFormat.Format24bppRgb) { throw new Exception("颜色格式只支持24位bmp"); }
            if (pBmp.PixelFormat != PixelFormat.Format24bppRgb) { throw new Exception("颜色格式只支持24位bmp"); }
            int sWidth = sBmp.Width;
            int sHeight = sBmp.Height;
            int pWidth = pBmp.Width;
            int pHeight = pBmp.Height;
            //取出4个角的颜色
            int px1 = pBmp.GetPixel(0, 0).ToArgb(); //左上角
            int px2 = pBmp.GetPixel(pWidth - 1, 0).ToArgb(); //右上角
            int px3 = pBmp.GetPixel(0, pHeight - 1).ToArgb(); //左下角
            int px4 = pBmp.GetPixel(pWidth - 1, pHeight - 1).ToArgb(); //右下角
            Color backColor = pBmp.GetPixel(0, 0); //背景色
            BitmapData sData = sBmp.LockBits(new Rectangle(0, 0, sWidth, sHeight), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData pData = pBmp.LockBits(new Rectangle(0, 0, pWidth, pHeight), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            List<Point> list;
            if (px1 == px2 && px1 == px3 && px1 == px4) //如果4个角的颜色相同
            {
                //透明找图
                list = _FindPic(left, top, width, height, sData, pData, GetPixelData(pData, backColor), similar, isOnly);
            }
            else if (similar > 0)
            {
                //相似找图
                list = _FindPic(left, top, width, height, sData, pData, similar, isOnly);
            }
            else
            {
                //全匹配找图效率最高
                list = _FindPic(left, top, width, height, sData, pData, isOnly);
            }
            sBmp.UnlockBits(sData);
            pBmp.UnlockBits(pData);
            return list;
        }
        /// <summary>
        /// 全匹配找图
        /// </summary>
        /// <param name="sData">大图数据</param>
        /// <param name="pData">小图数据</param>
        /// <returns></returns>
        private static unsafe List<Point> _FindPic(int left, int top, int width, int height, BitmapData sData, BitmapData pData, bool isOnly)
        {
            List<Point> list = new List<Point>();
            int sStride = sData.Stride;
            int pStride = pData.Stride;
            IntPtr sIptr = sData.Scan0;
            IntPtr pIptr = pData.Scan0;
            byte* sPtr;
            byte* pPtr;
            bool isOk = false;
            int breakW = width - pData.Width + 1;
            int breakH = height - pData.Height + 1;
            for (int h = top; h < breakH; h++)
            {
                for (int w = left; w < breakW; w++)
                {
                    pPtr = (byte*)(pIptr);
                    for (int y = 0; y < pData.Height; y++)
                    {
                        for (int x = 0; x < pData.Width; x++)
                        {
                            sPtr = (byte*)((int)sIptr + sStride * (h + y) + (w + x) * 3);
                            pPtr = (byte*)((int)pIptr + pStride * y + x * 3);
                            if (sPtr[0] == pPtr[0] && sPtr[1] == pPtr[1] && sPtr[2] == pPtr[2])
                            {
                                isOk = true;
                            }
                            else
                            {
                                isOk = false; break;
                            }
                        }
                        if (isOk == false) { break; }
                    }
                    if (isOk) { 
                        list.Add(new Point(w, h));
                        if (isOnly) return list;
                    }
                    
                    isOk = false;
                }
            }
            return list;
        }
        /// <summary>
        /// 相似找图
        /// </summary>
        /// <param name="sData">大图数据</param>
        /// <param name="pData">小图数据</param>
        /// <param name="similar">误差值</param>
        /// <returns></returns>
        private static unsafe List<Point> _FindPic(int left, int top, int width, int height, BitmapData sData, BitmapData pData, int similar, bool isOnly)
        {
            List<Point> list = new List<Point>();
            int sStride = sData.Stride;
            int pStride = pData.Stride;
            IntPtr sIptr = sData.Scan0;
            IntPtr pIptr = pData.Scan0;
            byte* sPtr;
            byte* pPtr;
            bool isOk = false;
            int breakW = width - pData.Width + 1;
            int breakH = height - pData.Height + 1;
            for (int h = top; h < breakH; h++)
            {
                for (int w = left; w < breakW; w++)
                {
                    pPtr = (byte*)(pIptr);
                    for (int y = 0; y < pData.Height; y++)
                    {
                        for (int x = 0; x < pData.Width; x++)
                        {
                            sPtr = (byte*)((int)sIptr + sStride * (h + y) + (w + x) * 3);
                            pPtr = (byte*)((int)pIptr + pStride * y + x * 3);
                            if (ScanColor(sPtr[0], sPtr[1], sPtr[2], pPtr[0], pPtr[1], pPtr[2], similar))  //比较颜色
                            {
                                isOk = true;
                            }
                            else
                            {
                                isOk = false; break;
                            }
                        }
                        if (isOk == false) { break; }
                    }
                    if (isOk) { 
                        list.Add(new Point(w, h));
                        if (isOnly) return list;
                    }
                    
                    isOk = false;
                }
            }
            return list;
        }
        /// <summary>
        /// 透明找图
        /// </summary>
        /// <param name="sData">大图数据</param>
        /// <param name="pData">小图数据</param>
        /// <param name="PixelData">小图中需要比较的像素数据</param>
        /// <param name="similar">误差值</param>
        /// <returns></returns>
        private static unsafe List<Point> _FindPic(int left, int top, int width, int height, BitmapData sData, BitmapData pData, int[,] PixelData, int similar, bool isOnly)
        {
            List<Point> list = new List<Point>();
            int len = PixelData.GetLength(0);
            int sStride = sData.Stride;
            int pStride = pData.Stride;
            IntPtr sIptr = sData.Scan0;
            IntPtr pIptr = pData.Scan0;
            byte* sPtr;
            byte* pPtr;
            bool isOk = false;
            int breakW = width - pData.Width + 1;
            int breakH = height - pData.Height + 1;
            for (int h = top; h < breakH; h++)
            {
                for (int w = left; w < breakW; w++)
                {
                    for (int i = 0; i < len; i++)
                    {
                        sPtr = (byte*)((int)sIptr + sStride * (h + PixelData[i, 1]) + (w + PixelData[i, 0]) * 3);
                        pPtr = (byte*)((int)pIptr + pStride * PixelData[i, 1] + PixelData[i, 0] * 3);
                        if (ScanColor(sPtr[0], sPtr[1], sPtr[2], pPtr[0], pPtr[1], pPtr[2], similar))  //比较颜色
                        {
                            isOk = true;
                        }
                        else
                        {
                            isOk = false; break;
                        }
                    }
                    if (isOk) { 
                        list.Add(new Point(w, h));
                        if (isOnly) return list;
                    }
                    
                    isOk = false;
                }
            }
            return list;
        }

        #region FindColor
        /// <summary>
        /// 找色
        /// </summary>
        public static unsafe List<Point> FindColor(int left, int top, int width, int height, Bitmap sBmp, Color clr, int similar)
        {
            if (sBmp.PixelFormat != PixelFormat.Format24bppRgb) { throw new Exception("颜色格式只支持24位bmp"); }
            BitmapData sData = sBmp.LockBits(new Rectangle(0, 0, sBmp.Width, sBmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            IntPtr Iptr = sData.Scan0;
            byte* ptr;
            List<Point> list = new List<Point>();
            for (int y = top; y < height; y++)
            {
                for (int x = left; x < width; x++)
                {
                    ptr = (byte*)((int)Iptr + sData.Stride * (y) + (x) * 3);
                    if (ScanColor(ptr[0], ptr[1], ptr[2], clr.B, clr.G, clr.R, similar))
                    {
                        list.Add(new Point(x, y));
                    }
                }
            }
            sBmp.UnlockBits(sData);
            return list;
        }
        #endregion

        #region isColor
        /// <summary>
        /// 比较两个 Color 
        /// </summary>
        /// <param name="similar">容错值</param>
        /// <returns></returns>
        public static bool isColor(Color clr1, Color clr2, int similar = 0)
        {
            if (ScanColor(clr1.B, clr1.G, clr1.R, clr2.B, clr2.G, clr2.R, similar))
            {
                return true;
            }
            return false;
        }
        #endregion

        #region CopyScreen
        /// <summary>
        /// 屏幕截图
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Bitmap CopyScreen(Rectangle rect)
        {
            Bitmap bitmap = new Bitmap(rect.Width, rect.Height, PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(rect.X, rect.Y, 0, 0, rect.Size);
                g.Dispose();
            }
            System.GC.Collect();
            return bitmap;
        }
        #endregion
        #region 私有方法
        private static unsafe int[,] GetPixelData(BitmapData pData, Color backColor)
        {
            byte B = backColor.B, G = backColor.G, R = backColor.R;
            int Width = pData.Width, Height = pData.Height;
            int pStride = pData.Stride;
            IntPtr pIptr = pData.Scan0;
            byte* pPtr;
            int[,] PixelData = new int[Width * Height, 2];
            int i = 0;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    pPtr = (byte*)((int)pIptr + pStride * y + x * 3);
                    if (B == pPtr[0] & G == pPtr[1] & R == pPtr[2])
                    {

                    }
                    else
                    {
                        PixelData[i, 0] = x;
                        PixelData[i, 1] = y;
                        i++;
                    }
                }
            }
            int[,] PixelData2 = new int[i, 2];
            Array.Copy(PixelData, PixelData2, i * 2);
            return PixelData2;
        }

        //找图BGR比较
        private static unsafe bool ScanColor(byte b1, byte g1, byte r1, byte b2, byte g2, byte r2, int similar)
        {
            if ((Math.Abs(b1 - b2)) > similar) { return false; } //B
            if ((Math.Abs(g1 - g2)) > similar) { return false; } //G
            if ((Math.Abs(r1 - r2)) > similar) { return false; } //R
            return true;
        }
        #endregion




        /// <summary>
        /// 加载图片，把图片放进hashTable里
        /// </summary>
        /// <param name="paths">路径名，用"|"隔开如@"C:\xxx.bmp|C:\ffff.bmp"</param>
        /// <returns>返回-1表示加载失败，返回1表示加载成功</returns>
        public static int loadBmp(String paths)
        {
            String[] subPaths = paths.Split('|');
            for(int i = 0; i < subPaths.Length; i++)
            {
                Bitmap bitmap = new Bitmap(subPaths[i]);
                int nameStartIndex = subPaths[i].LastIndexOf('\\') + 1;
                int nameEndIndex = subPaths[i].LastIndexOf('.');
                if (nameEndIndex == -1)
                {
                    Console.WriteLine("文件名错误：" + subPaths[i]);
                    return -1;
                }
                int len = nameEndIndex - nameStartIndex;
                String key = subPaths[i].Substring(nameStartIndex, len);
                if (Instance.bmpTable.ContainsKey(key))
                {
                    Console.WriteLine("重复加载：" + key);
                    return -1;
                }
                else
                {
                    Instance.bmpTable.Add(key, bitmap);
                }

            }
            return 1;
        }

        /// <summary>
        /// 清空后再加载图片到内存
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static int loadBmpSub(String paths)
        {
            foreach (Bitmap val in Instance.bmpSubTable.Values)
            {
                val.Dispose();
            }
            Instance.bmpSubTable.Clear();

            String[] subPaths = paths.Split('|');
            for (int i = 0; i < subPaths.Length; i++)
            {
                Bitmap bitmap = new Bitmap(subPaths[i]);
                int nameStartIndex = subPaths[i].LastIndexOf('\\') + 1;
                int nameEndIndex = subPaths[i].LastIndexOf('.');
                if (nameEndIndex == -1)
                {
                    Console.WriteLine("文件名错误：" + subPaths[i]);
                    return -1;
                }
                int len = nameEndIndex - nameStartIndex;
                String key = subPaths[i].Substring(nameStartIndex, len);
                if (Instance.bmpSubTable.ContainsKey(key))
                {
                    Console.WriteLine("重复加载：" + key);
                    return -1;
                }
                else
                {
                    Instance.bmpSubTable.Add(key, bitmap);
                }

            }
            return 1;
        }

        /// <summary>
        /// 清空hashTable
        /// </summary>
        public static void freeBmp()
        {
            Instance.bmpTable.Clear();
            foreach(Bitmap val in Instance.bmpTable.Values)
            {
                val.Dispose();
            }
        }


        //------------------------------------------------------
        //命名规则：find + 哪个位置的图片(D为loadBmp加载的,S为loadBmpSub加载的,P为文件路径加载) + 几张图(A为1,N为多) + pic + 返回几个位置(A为1，N为多)
        //------------------------------------------------------
        /// <summary>
        /// 从文件路径加载的图片中按路径名称找1个图1个位置
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="left">左边x坐标</param>
        /// <param name="top">上边y坐标</param>
        /// <param name="right">右边x坐标</param>
        /// <param name="bottom">下边y坐标</param>
        /// <param name="path">所要找的小图路径</param>
        /// <param name="similar">误差值</param>
        /// <returns>返回一个1维数组,0是x坐标，1是y坐标</returns>
        public static int[] findPAPicA(IntPtr hWnd, int left, int top, int right, int bottom, String path, int similar)
        {
            Bitmap pBmp = new Bitmap(path);
            Bitmap sBmp = GetScreen.getWindow(hWnd);
            if (left >= sBmp.Width) left = sBmp.Width;
            if (right >= sBmp.Width) right = sBmp.Width;
            if (top >= sBmp.Height) top = sBmp.Height;
            if (bottom >= sBmp.Height) bottom = sBmp.Height;
            List<Point> p = findPicOrigin(left, top, right, bottom, sBmp, pBmp, similar,true);
            pBmp.Dispose();
            sBmp.Dispose();
            if (p.Count == 0)
            {
                int[] result = {-1, -1};
                return result;
            }
            else
            {
                int[] result = { p[0].X, p[0].Y };
                return result;
            }
           
        }
        /// <summary>
        /// 从loadBmp加载的图片中按图片名称找1个图1个位置
        /// </summary>
        /// <param name="picName">图片名称</param>
        /// <returns></returns>
        public static int[] findDAPicA(IntPtr hWnd, int left, int top, int right, int bottom, String picName, int similar)
        {
            Bitmap pBmp = Instance.bmpTable[picName] as Bitmap;
            Bitmap sBmp = GetScreen.getWindow(hWnd);
            if (left >= sBmp.Width) left = sBmp.Width;
            if (right >= sBmp.Width) right = sBmp.Width;
            if (top >= sBmp.Height) top = sBmp.Height;
            if (bottom >= sBmp.Height) bottom = sBmp.Height;
            List<Point> p = findPicOrigin(left, top, right, bottom, sBmp, pBmp, similar, true);
            sBmp.Dispose();
            if (p.Count == 0)
            {
                int[] result = { -1, -1 };
                return result;
            }
            else
            {
                int[] result = { p[0].X, p[0].Y };
                return result;
            }

        }
        /// <summary>
        /// 从loadBmpSub加载的图片中按图片名称找1个图1个位置
        /// </summary>
        /// <param name="picName">图片名称</param>
        /// <returns></returns>
        public static int[] findSAPicA(IntPtr hWnd, int left, int top, int right, int bottom, String picName, int similar)
        {
            Bitmap pBmp = Instance.bmpSubTable[picName] as Bitmap;
            Bitmap sBmp = GetScreen.getWindow(hWnd);
            if (left >= sBmp.Width) left = sBmp.Width;
            if (right >= sBmp.Width) right = sBmp.Width;
            if (top >= sBmp.Height) top = sBmp.Height;
            if (bottom >= sBmp.Height) bottom = sBmp.Height;
            List<Point> p = findPicOrigin(left, top, right, bottom, sBmp, pBmp, similar, true);
            sBmp.Dispose();
            if (p.Count == 0)
            {
                int[] result = { -1, -1 };
                return result;
            }
            else
            {
                int[] result = { p[0].X, p[0].Y };
                return result;
            }

        }

        /// <summary>
        /// 从文件路径加载的图片中按路径名称找1个图多个位置
        /// </summary>
        /// <param name="path">小图本地路径</param>
        /// <returns>
        /// 二维数组,类似{{x,y},{x,y},{x,y}},第1维存放x坐标和y坐标，第二维存放个数
        /// 通过 数组.GetUpperBound(1) 获取图片个数的下标（第一维参数1，第二维参数2），为-1时表示没有找到
        /// 通过 数组[0,i]获取第i个x坐标,[1,i]获取第i个y坐标
        /// </returns>
        public static int[,] findPAPicN (IntPtr hWnd, int left, int top, int right, int bottom, String path, int similar)
        {
            Bitmap pBmp = new Bitmap(path);
            Bitmap sBmp = GetScreen.getWindow(hWnd);
            if (left >= sBmp.Width) left = sBmp.Width;
            if (right >= sBmp.Width) right = sBmp.Width;
            if (top >= sBmp.Height) top = sBmp.Height;
            if (bottom >= sBmp.Height) bottom = sBmp.Height;
            List<Point> p = findPicOrigin(left, top, right, bottom, sBmp, pBmp, similar, false);
            pBmp.Dispose();
            sBmp.Dispose();
            if (p.Count == 0)
            {
                int[,] result = {};
                return result;

            }
            else
            {
                int[,] result = new int[2,p.Count];
                for (int i = 0; i < p.Count; i++){
                    result[0, i] = p[i].X;
                    result[1, i] = p[i].Y;

                }
                return result;
            }

        }

        /// <summary>
        /// 从loadBmp加载的图片中按图片名称找1个图多个位置
        /// </summary>
        /// <param name="picName">图片名称</param>
        /// <param name="similar"></param>
        /// <returns></returns>
        public static int[,] findDAPicN(IntPtr hWnd, int left, int top, int right, int bottom, String picName, int similar)
        {
            Bitmap pBmp = Instance.bmpTable[picName] as Bitmap;
            Bitmap sBmp = GetScreen.getWindow(hWnd);
            if (left >= sBmp.Width) left = sBmp.Width;
            if (right >= sBmp.Width) right = sBmp.Width;
            if (top >= sBmp.Height) top = sBmp.Height;
            if (bottom >= sBmp.Height) bottom = sBmp.Height;
            List<Point> p = findPicOrigin(left, top, right, bottom, sBmp, pBmp, similar, false);
            sBmp.Dispose();
            if (p.Count == 0)
            {
                int[,] result = {};
                return result;
            }
            else
            {
                int[,] result = new int[2, p.Count];
                for (int i = 0; i < p.Count; i++)
                {
                    result[0, i] = p[i].X;
                    result[1, i] = p[i].Y;
                }
                return result;
            }

        }

        /// <summary>
        /// 从loadBmpSub加载的图片中找多个图中的第1个找到的图的1个位置
        /// </summary>
        /// <returns>1维数组，0为x坐标，1为y坐标，2为第几个图片,找不到时为-1</returns>
        public static int[] findSNPicA(IntPtr hWnd, int left, int top, int right, int bottom, int similar)
        {
            Bitmap sBmp = GetScreen.getWindow(hWnd);
            if (left >= sBmp.Width) left = sBmp.Width;
            if (right >= sBmp.Width) right = sBmp.Width;
            if (top >= sBmp.Height) top = sBmp.Height;
            if (bottom >= sBmp.Height) bottom = sBmp.Height;
            int index = 0;
            foreach (var val in Instance.bmpSubTable.Values)
            {
                List<Point> p = findPicOrigin(left, top, right, bottom, sBmp, val as Bitmap, similar, true);
                if (p.Count != 0)
                {
                    int[] subResult = new int[3];
                    subResult[0] = p[0].X;
                    subResult[1] = p[0].Y;
                    subResult[2] = index;
                    sBmp.Dispose();
                    return subResult;
                }
                index++;
            }
            int[] result = new int[3];
            result[0] = -1;
            result[1] = -1;
            result[2] = -1;
            sBmp.Dispose();
            return result;
        }


        /// <summary>
        /// 从loadBmpSub加载的图片中找多个图多个位置
        /// </summary>
        /// <returns>
        /// 返回一个二维数组{{x,y,index},{x,y,index}}第一维[0]和[1]是x,y坐标,[2]是图片的下标,
        /// 第二维表示找到了几个找不到时为-1；
        /// </returns>
        public static int[,] findSNPicN(IntPtr hWnd, int left, int top, int right, int bottom, int similar)
        {
            Bitmap sBmp = GetScreen.getWindow(hWnd);
            if (left >= sBmp.Width) left = sBmp.Width;
            if (right >= sBmp.Width) right = sBmp.Width;
            if (top >= sBmp.Height) top = sBmp.Height;
            if (bottom >= sBmp.Height) bottom = sBmp.Height;
            List<int[]> listTemp = new List<int[]>();
            int i = 0;
            int index = 0;
            foreach (var val in Instance.bmpSubTable.Values)
            {
                List<Point> points = findPicOrigin(left, top, right, bottom, sBmp, val as Bitmap, similar, false);
                if (points.Count != 0)
                {
                    foreach(var p in points)
                    {
                        int[] arryTemp = new int[3];
                        arryTemp[0] = p.X;
                        arryTemp[1] = p.Y;
                        arryTemp[2] = index;
                        listTemp.Add(arryTemp);
                    }
                    i++;
                }
                index++;
            }
            sBmp.Dispose();
            int[,] result = new int[3, listTemp.Count];
            int t = 0;
            foreach(var val in listTemp)
            {
                
                result[0, t] = val[0];
                result[1, t] = val[1];
                result[2, t] = val[2];
                t++;
            }

            return result;
        }
    }


}
