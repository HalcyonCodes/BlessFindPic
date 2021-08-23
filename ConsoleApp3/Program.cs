using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlessFindPic;
using System.Drawing;
using System.Collections;


namespace ConsoleApp3
{
    class Program
    {
        static void Main(string[] args)
        {
            //IntPtr hWnd = GetScreen.findWindow(null,"Bless Unleashed");
            //Bitmap b = GetScreen.getWindow(hWnd);
            //GetScreen.saveBitMapByOffset(b, 400, 400, 600, 700, "名字", @"C:\Users\Administrator\Desktop\dada\");
            //String dd = @"C:\Users\Administrator\Desktop\dada\GSD.bmp";

            //IntPtr hWnd = GetScreen.findWindow(null, "Bless Unleashed");
            //int[] p = BmpColor.findAPicAB(hWnd, 0, 0, 1296, 759, "小图1", 5);
            //int[] p = BmpColor.findAPicAA(hWnd, 0, 0, 800, 2000, @"C:\Users\Administrator\Desktop\dada\锁头1.bmp", 5);
            //Console.WriteLine(p[0]);
            //Console.WriteLine(p[1]);
            //Console.ReadKey();

            //String dd = @"C:\Users\Administrator\Desktop\dada\锁头1.bmp";

            // BmpColor.loadBmp(dd);
            //Console.WriteLine(BmpColor.Instance.bmpTable.Count);
            //BmpColor.freeBmp();
            //Console.WriteLine(BmpColor.Instance.bmpTable.Count);
            //Console.ReadKey();

            // IntPtr hWnd = GetScreen.findWindow(null, "Bless Unleashed");
            //int[] p = BmpColor.findDAPicA(hWnd, 0, 0, 1296, 759, "锁头1", 15);

            //int[] p = BmpColor.findAPicA(hWnd, 0, 0, 1200, 700, @"C:\Users\Administrator\Desktop\dada\小图1.bmp", 5);
            // Console.WriteLine(p[0]);
            //Console.WriteLine(p[1]);
            //Console.WriteLine(BmpColor.Instance.bmpTable.Count);
            //Console.ReadKey();

            /*
            BmpColor.loadBmp(@"C:\Users\Administrator\Desktop\dada\锁头1.bmp");
            IntPtr hWnd = GetScreen.findWindow(null, "Bless Unleashed");
            int[,] aa = BmpColor.findAPicNB(hWnd, 0, 0, 1400, 900, "锁头1", 15);
            Console.WriteLine(aa.GetUpperBound(1));
            for(int i = 0; i<aa.GetUpperBound(1) + 1; i++)
            {
                Console.WriteLine(aa[0, i]);
                Console.WriteLine(aa[1, i]);
            }
            Console.ReadKey();
            */

            /*
            String dd = @"C:\Users\Administrator\Desktop\dada\小图1.bmp|C:\Users\Administrator\Desktop\dada\名字.bmp|C:\Users\Administrator\Desktop\dada\锁头1.bmp";
            int h = BmpColor.loadBmpList(dd);
            IntPtr hWnd = GetScreen.findWindow(null, "Bless Unleashed");
            int[] aa = BmpColor.findLNPicA(hWnd, 0, 0, 1400, 900, 15);
            Console.WriteLine(aa[0]);
            Console.WriteLine(aa[1]);
            Console.WriteLine(aa[2]);
            Console.ReadKey();
            */

            /*String dd = @"C:\Users\Administrator\Desktop\dada\名字.bmp|C:\Users\Administrator\Desktop\dada\小图1.bmp|C:\Users\Administrator\Desktop\dada\锁头1.bmp";
            IntPtr hWnd = GetScreen.findWindow(null, "Bless Unleashed");
            int h = BmpColor.loadBmpSub(dd);
            int[,] aa = BmpColor.findNPicN(hWnd, 0, 0, 1400, 900, 15);
            for (int i = 0; i < aa.GetUpperBound(1) + 1; i++)
            {
                Console.WriteLine(aa[0, i]);
                Console.WriteLine(aa[1, i]);
                Console.WriteLine(aa[2, i]);
            }*/
            //Console.ReadKey();
            int[] aa = GetScreen.getScreenBound();
            /*IntPtr hWnd = GetScreen.findWindow(null, "Bless Unleashed");
            
            int[] aa = GetScreen.getWindowBasePoint(hWnd);*/
            Console.WriteLine(aa[0]);
            Console.WriteLine(aa[1]);
            Console.ReadKey();
        }
    }
}
