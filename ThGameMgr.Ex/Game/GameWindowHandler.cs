using System.Runtime.InteropServices;

namespace ThGameMgr.Ex.Game
{
    internal partial class GameWindowHandler
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [LibraryImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool GetWindowRect(IntPtr hWnd, out RECT rect);

        [LibraryImport("user32.dll", SetLastError = true)]
        private static partial int MoveWindow(IntPtr hwnd, int x, int y,
            int nWidth, int nHeight, int bRepaint);

        [LibraryImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, int uFlags);

        [LibraryImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool SetForegroundWindow(IntPtr hWnd);

        [LibraryImport("user32.dll", SetLastError =true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool IsWindow(IntPtr hWnd);

        public static IntPtr GetGameWindow(Process? gameProcess)
        {
            if (gameProcess != null)
            {
                return IntPtr.Zero;
            }
            else
            {
                return IntPtr.Zero;
            }
        }

        public static GameWindowSizes GetWindowSizes(IntPtr gameWindow)
        {
            //ウィンドウサイズの取得
            _ = GetWindowRect(gameWindow, out RECT rect);
            int width = rect.right - rect.left;
            int height = rect.bottom - rect.top;

            return new GameWindowSizes
            {
                Width = width,
                Height = height
            };
        }

        public static GameWindowPosition GetWindowPosition(IntPtr gameWindow)
        {
            _ = GetWindowRect(gameWindow, out RECT rect);

            return new GameWindowPosition
            {
                X = rect.left,
                Y = rect.top
            };
        }

        public static void ResizeWindow(IntPtr gameWindow, int width, int height)
        {
            GameWindowPosition gameWindowPosition = GetWindowPosition(gameWindow);

            _ = MoveWindow(gameWindow, gameWindowPosition.X, gameWindowPosition.Y, width, height, 1);

            SetForegroundWindow(gameWindow);
        }

        public static void FixTopMost(IntPtr gameWindow)
        {
            const int SWP_NOSIZE = 0x0001;
            const int SWP_NOMOVE = 0x0002;

            const int HWND_TOPMOST = -1;
            //ウィンドウを最前面に固定
            _ = SetWindowPos(gameWindow, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
        }

        public static void ReleaseTopMost(IntPtr gameWindow)
        {
            const int SWP_NOSIZE = 0x0001;
            const int SWP_NOMOVE = 0x0002;

            const int HWND_TOPMOST = -2;
            //ウィンドウの最前面固定を解除
            _ = SetWindowPos(gameWindow, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
        }
    }
}
