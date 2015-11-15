using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace WoW.Fishing.Utilities
{
    class Windows
    {
        private const int WindowSwapDelay = 50;

        #region Properties

        private static Window _GameWindow;
        private static Window GameWindow
        {
            get
            {
                if (_GameWindow.hWnd == default(IntPtr))
                    _GameWindow = new Window(FindWindow("GxWindowClass", "World Of Warcraft"));

                return _GameWindow;
            }
        }

        private static Rectangle _ScreenArea;
        public static Rectangle ScreenArea
        {
            get
            {
                if (_ScreenArea == default(Rectangle))
                    GetWindowRect(GameWindow.hWnd, ref _ScreenArea);

                return _ScreenArea;
            }
        }

        #endregion

        #region Windows APIs

        // Activate an application window.
        [DllImport("USER32.DLL")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, ref Rectangle rectangle);

        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        #endregion

        #region Structures

        private struct Window
        {
            public IntPtr hWnd;
            public string Title;
            public int ProcessID;

            public bool IsEmpty { get { return hWnd == default(IntPtr); } }

            public Window(IntPtr h)
            {
                hWnd = h;

                Title = string.Empty;
                ProcessID = 0;
            }

            public Window(IntPtr h, string t, int p)
            {
                hWnd = h;
                Title = t;
                ProcessID = p;
            }
        }

        #endregion

        #region Methods

        /* still under construction */
        private static Window _BotWindow;
        private static Window BotWindow
        {
            get
            {
                if (_BotWindow.hWnd == default(IntPtr))
                    _BotWindow = new Window(FindWindow("GxWindowClass", frmMain.Title));

                return _BotWindow;
            }
        }

        public static void ActivateBotWindow()
        {
            if (BotWindow.IsEmpty)
                return;
            else
            {
                bool SyncShow = SetForegroundWindow(BotWindow.hWnd);
                bool ASyncShow = ShowWindowAsync(BotWindow.hWnd, 9);
                System.Threading.Thread.Sleep(WindowSwapDelay);

                if (!(SyncShow || ASyncShow))
                    throw new Exception("Unable to find Bot window.");
            }
        }
        /* end construction */

        public static void ActivateGameWindow()
        {
            if (GameWindow.IsEmpty)
                return;
            else
            {
                bool SyncShow = SetForegroundWindow(GameWindow.hWnd);
                bool ASyncShow = ShowWindowAsync(GameWindow.hWnd, 9);
                System.Threading.Thread.Sleep(WindowSwapDelay);

                if (!(SyncShow || ASyncShow))
                    throw new Exception("Unable to find game window.");
            }
        }

        #endregion
    }

    class WindowsAPI
    {
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool EnumWindows(EnumWindowsProc callback, IntPtr extraData);

        [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, [Out, MarshalAs(UnmanagedType.LPTStr)] StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(HandleRef handle, out int processId);


        public struct Window
        {
            public IntPtr hWnd;
            public string Title;
            public int ProcessID;

            public Window(IntPtr h, string t, int p)
            {
                hWnd = h;
                Title = t;
                ProcessID = p;
            }
        }

        private static List<Window> _Windows;

        private static bool EnumWindowsMethod(IntPtr hWnd, IntPtr lParam)
        {
            StringBuilder sb = new StringBuilder(256);
            GetWindowText(hWnd, sb, 256);

            int ProcID = 0;
            HandleRef hr = new HandleRef(null, hWnd);

            GetWindowThreadProcessId(hr, out ProcID);

            //if (ProcID == 3596)
            _Windows.Add(new Window(hWnd, sb.ToString(), ProcID));

            return true;
        }

        public static void ListWindows()
        {
            System.Diagnostics.Debug.WriteLine("");
            System.Diagnostics.Debug.WriteLine("");
            System.Diagnostics.Debug.WriteLine("");
            System.Diagnostics.Debug.WriteLine("");
            System.Diagnostics.Debug.WriteLine("Enumerating available window handles:");

            _Windows = new List<Window>();

            EnumWindowsProc callback = new EnumWindowsProc(EnumWindowsMethod);
            EnumWindows(callback, new IntPtr());

            System.Diagnostics.Debug.WriteLine("Total handles:" + _Windows.Count);

            foreach (var w in _Windows)
            {
                System.Diagnostics.Debug.WriteLine(w.ProcessID + "  --  " + w.Title + "  --  " + w.hWnd);
            }
        }

    }
}
