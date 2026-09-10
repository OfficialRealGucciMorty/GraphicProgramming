//elo
//writtn by G Mort
//i rlly like this one ngl
//anyway ill let you go through source

using System;//
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;

namespace shader
{

internal class mainc
{

    [DllImport("user32.dll")]
    static extern IntPtr GetDC(IntPtr hWnd);

    [DllImport("user32.dll")]
    static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

    [DllImport("user32.dll")]
    static extern int GetSystemMetrics(int nIndex);

    [DllImport("gdi32.dll")]
    static extern int SetDIBitsToDevice(
        IntPtr hdc, int x, int y, uint w, uint h,
        int sx, int sy, uint start, uint lines,
        byte[] bits, ref BITMAPINFO bmi, uint usage);


        [StructLayout(LayoutKind.Sequential)]
        struct BITMAPINFOHEADE{
            public uint biSize; public int biWidth;public int biHeight;
            public ushort biPlanes; public ushort biBitCount;
            public uint biCompression;public uint biSizeImage;
            public int biXPelsPerMeter;public int biYPelsPerMeter;
            public uint biClrUsed;public uint biClrImportant;
        }

    [StructLayout(LayoutKind.Sequential)]
    struct BITMAPINFO
    {
        public BITMAPINFOHEADER bmiHeader;
        public uint bmiColors;
    }


[STAThread]
static void Main()
{

    int x = GetSystemMetrics(0);
    int y = GetSystemMetrics(1);

    Bitmap screen =
        new Bitmap(x, y, PixelFormat.Format32bppRgb);

    Graphics gfx = Graphics.FromImage(screen);

    Rectangle rect = new Rectangle(0, 0, x, y);


    BITMAPINFO bmi = new BITMAPINFO();

    bmi.bmiHeader.biSize =
        (uint)Marshal.SizeOf(typeof(BITMAPINFOHEADER));

    bmi.bmiHeader.biWidth = x;
    bmi.bmiHeader.biHeight = -y;

    bmi.bmiHeader.biPlanes = 1;
    bmi.bmiHeader.biBitCount = 32;
    bmi.bmiHeader.biCompression = 0;


        byte[] ptr = new byte[x * y * 4];
    byte[] src =
            new byte[x * y * 4];


    int cx = x / 2;
    int cy = y / 2;

    const int scalez = 1024;
    const double FREQ = 0.008;

    int[] sinR = new int[Math.Max(x, y)];
    int[] cosR = new int[Math.Max(x, y)];

        double t = 0.0;
        int frame = 0;

    while (true)
    {

        IntPtr hdc = GetDC(IntPtr.Zero);


        gfx.CopyFromScreen(
            0, 0, 0, 0,
            new Size(x, y));


        BitmapData data =
            screen.LockBits(
                rect,
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppRgb
            );

        Marshal.Copy(data.Scan0,src,0,src.Length);

        screen.UnlockBits(data);

            t += 0.04;
        frame++;


        int reffect1 =
            (int)(Math.Sin(t * 0.9) * 100);

        int geffect2 =
            (int)(Math.Sin(t * 0.9 + 2.094) * 100);

        int beffect3 =
            (int)(Math.Sin(t * 0.9 + 4.188) * 100);


        int maineffectig =
            (int)(t * 30) & 0x1FF;

        int secondaryeffect =
            (int)(t * 200);


        int maxR =
            (int)Math.Sqrt(cx * cx + cy * cy);


        for (int i = 0;
             i < maxR && i < sinR.Length;
             i++)
        {
            double r = i * FREQ;

            sinR[i] =
                (int)(
                    Math.Sin(r - t * 2.0)
                    * scalez
                );

            cosR[i] =
                (int)(
                    Math.Cos(r * 0.5 + t * 1.4)
                    * scalez
                );
        }


        int ee = 0;


        for (int yy = 0; yy < y; yy++)
        {

            int dy = yy - cy;
            int dy2 = dy * dy;


            for (int xx = 0; xx < x; xx++)
            {

                int dx = xx - cx;

                int dist =
                    (int)Math.Sqrt(dx * dx + dy2);


                if (dist >= sinR.Length)
                    dist = sinR.Length - 1;


                int sr = sinR[dist];
                int cr = cosR[dist];


                int thangle =
                    (sr >> 3) + maineffectig;


                int srcX =
                    xx + (thangle >> 4);

                int srcY =
                    yy + (cr >> 4);


                if (srcX < 0)
                    srcX = 0;
                else if (srcX >= x)
                    srcX = x - 1;


                    if (srcY < 0)
                        srcY = 0;
                    else if (srcY >= y)
                        srcY = y - 1;


                int srcIdx =
                    (srcY * x + srcX) * 4;


                byte b = src[srcIdx];
                byte g = src[srcIdx + 1];
                byte r = src[srcIdx + 2];


                int r4723 =
                    (sr + cr + 2048) >> 4;

                int mix372 =
                    ((dist * 4 + secondaryeffect) & 0xFF);


                int LUM81 =
                    (r * 77 + g * 151 + b * 28) >> 8;


                int nr =
                    r + reffect1 + (sr >> 5);

                int ng =
                    g + geffect2 + (cr >> 5);

                int nb =
                    b + beffect3 + ((sr + cr) >> 6);


                nr =
                    (nr * (256 - mix372)
                    + (LUM81 ^ reffect1) * mix372) >> 8;

                ng =
                    (ng * (256 - mix372)
                    + (((LUM81 + geffect2) & 0xFF))
                    * mix372) >> 8;

                nb =
                    (nb * (256 - mix372)
                    + (((LUM81 - beffect3) & 0xFF))
                    * mix372) >> 8;



                if (nr < 0)
                    nr = 0;
                else if (nr > 255)
                    nr = 255;


                if (ng < 0)
                    ng = 0;
                else if (ng > 255)
                    ng = 255;

                if (nb < 0)
                    nb = 0;
                else if (nb > 255)
                    nb = 255;



                ptr[ee] = (byte)nb;
                ptr[ee + 1] = (byte)ng;
                ptr[ee + 2] = (byte)nr;

                ptr[ee + 3] =
                    src[srcIdx + 3];

                ee += 4;

            }// xx
        }// yy



        SetDIBitsToDevice(
            hdc,0, 0,
            (uint)x, (uint)y,0, 0,0,(uint)y,ptr,ref bmi,0,);


        ReleaseDC(
            IntPtr.Zero,
            hdc
        );


        Thread.Sleep(1);

    }
}

}
}

//end of source, took me abt 15 mins to write
