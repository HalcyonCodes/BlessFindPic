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
            //int[] p = BmpColor.findAPicB(hWnd, 0, 0, 1296, 759, "小图1", 5);
            //int[] p = BmpColor.findAPicA(hWnd, 0, 0, 800, 2000, @"C:\Users\Administrator\Desktop\dada\小图1.bmp", 5);
            //Console.WriteLine(p[0]);
            //Console.WriteLine(p[1]);
            //Console.ReadKey();

            //String dd = @"C:\Users\Administrator\Desktop\dada\名字.bmp|C:\Users\Administrator\Desktop\dada\小图1.bmp";
            //BmpColor.loadBmp(dd);
            //Console.WriteLine(BmpColor.Instance.bmpTable.Count);
            //BmpColor.freeBmp();
            //Console.WriteLine(BmpColor.Instance.bmpTable.Count);
            //Console.ReadKey();

            //IntPtr hWnd = GetScreen.findWindow(null, "Bless Unleashed");
            //int[] p = BmpColor.findAPicB(hWnd, 0, 0, 1296, 759, "小图1", 5);

            //int[] p = BmpColor.findAPicA(hWnd, 0, 0, 1200, 700, @"C:\Users\Administrator\Desktop\dada\小图1.bmp", 5);
            //Console.WriteLine(p[0]);
            //Console.WriteLine(p[1]);
            //Console.WriteLine(BmpColor.Instance.bmpTable.Count);
            //Console.ReadKey();

            BmpColor.loadBmp(@"C:\Users\Administrator\Desktop\dada\小图1.bmp");
            IntPtr hWnd = GetScreen.findWindow(null, "Bless Unleashed");
            int[,] aa = BmpColor.findAPicNB(hWnd, 0, 0, 1200, 700, "小图1", 5);
            Console.WriteLine(aa.GetUpperBound(1));
            Console.WriteLine(aa[0, 0]);
            Console.WriteLine(aa[1, 0]);
            Console.ReadKey();


        }
    }
}
