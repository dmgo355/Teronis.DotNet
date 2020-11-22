﻿using System;
using System.Drawing;
using Teronis.Windows;

namespace Teronis.Drawing.Windows
{
    public static class Win32Utilities
    {
        public static Bitmap CaptureWindow(IntPtr hWnd)
        {
            var rctForm = Rectangle.Empty;
            using (var grfx = Graphics.FromHdc(Win32.GetWindowDC(hWnd))) {
                rctForm = Rectangle.Round(grfx.VisibleClipBounds);
            }

            var pImage = new Bitmap(rctForm.Width, rctForm.Height);
            var graphics = Graphics.FromImage(pImage);
            var hDC = graphics.GetHdc();
            //paint control onto graphics using provided options        
            try {
                Win32.PrintWindow(hWnd, hDC, 0);
            } finally {
                graphics.ReleaseHdc(hDC);
            }
            return pImage;
        }
    }
}
