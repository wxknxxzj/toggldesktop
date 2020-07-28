﻿using System;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace TogglDesktop
{
    internal static class Win32
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct LASTINPUTINFO
        {
            public static readonly int SizeOf =
                Marshal.SizeOf(typeof(LASTINPUTINFO));

            [MarshalAs(UnmanagedType.U4)]
            public int CbSize;

            [MarshalAs(UnmanagedType.U4)]
            public int DwTime;
        }

        [DllImport("user32.dll")]
        public static extern bool GetLastInputInfo(out LASTINPUTINFO plii);

        [StructLayout(LayoutKind.Sequential)]
        public struct Rectangle
        {
            public readonly int Left;
            public readonly int Top;
            public readonly int Right;
            public readonly int Bottom;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, out Rectangle lpRect);
    }
}
