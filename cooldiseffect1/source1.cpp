// first file on the repo by G Mort
//anyways, go through the source ig
//keep vibin lol
#include <windows.h>
#include <vector>
#include <cmath>

int WINAPI WinMain(HINSTANCE hInstance, HINSTANCE, LPSTR, int nCmdShow) {
    int w = GetSystemMetrics(SM_CXSCREEN);
    int h = GetSystemMetrics(SM_CYSCREEN);

    HDC hdc = GetDC(NULL);
    HDC hMem = CreateCompatibleDC(hdc);
    HBITMAP bmp = CreateCompatibleBitmap(hdc, w, h);
    SelectObject(hMem, bmp);

    BITMAPINFO bmi = { 0 };
    bmi.bmiHeader.biSize = sizeof(BITMAPINFOHEADER);
    bmi.bmiHeader.biWidth = w;
    bmi.bmiHeader.biHeight = -h;
    bmi.bmiHeader.biPlanes = 1;
    bmi.bmiHeader.biBitCount = 32;
    bmi.bmiHeader.biCompression = BI_RGB;

    std::vector<BYTE> denji(w * h * 4);
    std::vector<BYTE> frostedflakes(w * h * 4);

    float TIMEE = 0.0f;
    int deez = w * 4;

    while (true) {
        BitBlt(hMem, 0, 0, w, h, hdc, 0, 0, SRCCOPY);
        GetDIBits(hMem, bmp, 0, h, denji.data(), &bmi, DIB_RGB_COLORS);

        TIMEE += 0.03f;

        for (int y = 0; y < h; y++) {
            int offsetX = (int)(40.0f * sin(y * 0.04f + TIMEE * 2.0f));
            int offsetY = (int)(20.0f * sin(y * 0.02f + TIMEE * 1.5f));

            for (int x = 0; x < w; x++) {
                int srcX = x + offsetX + (int)(15.0f * sin(y * 0.03f + TIMEE));
                int srcY = y + offsetY + (int)(10.0f * sin(x * 0.02f + TIMEE * 0.7f));

                if (srcX < 0) srcX += w;
                if (srcX >= w) srcX -= w;
                if (srcY < 0) srcY += h;
                if (srcY >= h) srcY -= h;

                int srcIdx = srcY * deez + srcX * 4;
                int dstIdx = y * deez + x * 4;

                frostedflakes[dstIdx] = denji[srcIdx];
                frostedflakes[dstIdx + 1] = denji[srcIdx + 1];
                frostedflakes[dstIdx + 2] = denji[srcIdx + 2];
                frostedflakes[dstIdx + 3] = denji[srcIdx + 3];
            }
        }

        SetDIBits(hMem, bmp, 0, h, frostedflakes.data(), &bmi, DIB_RGB_COLORS);
        BitBlt(hdc, 0, 0, w, h, hMem, 0, 0, SRCCOPY);

        Sleep(10);
    }

    DeleteObject(bmp);
    DeleteDC(hMem);
    ReleaseDC(NULL, hdc);
    return 0;
}
//end of source
