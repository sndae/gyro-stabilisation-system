using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SemaphoreAppKeyb
{
    static class Program
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        /// <summary>
        /// that functions used for getting active window handle
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowTextLength(IntPtr hwnd); 

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hwnd, StringBuilder lpString, int nMaxCount);

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        
        private const int WH_MOUSE_LL = 14;
        private const int WM_MOUSEMOVE = 0x0200;
        private const int WM_MOUSEWHEEL = 0x020A;
        private const int WM_LBUTTONDOWN = 0x0201;

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        /// <summary>
        /// This variable is public to be accesed from Form1.Timer task. Just counter.
        /// </summary>
        public static int press_count = 0;
        public static int Length = 0;
        /// <summary>
        /// This variable will be used for save form name.
        /// </summary>
        public static string ActiveWindowName;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            _hookID = SetHook(_proc);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            UnhookWindowsHookEx(_hookID);
        }

        /// <summary>
        /// SetHook function called for setting hook.
        /// </summary>
        /// <param name="proc"></param>
        /// <returns></returns>
        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                ///Set hook for mouse wheel.
                SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
                ///Set hook for any key.
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        /// <summary>
        /// HookCallback function called by delegate and used for setted hook.
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            
            /// This part detect mouse wheel.
            if ((nCode >= 0) && (wParam == (IntPtr)WM_MOUSEWHEEL))
            {
                press_count++;
                return (IntPtr)1;
            }
            /// This part detect pressing to any key.
            if ((nCode >= 0) && (wParam == (IntPtr)WM_KEYDOWN))
            {
                press_count++;
                return (IntPtr)1;
            }

            ///Here we are looking for activ window to get handler to it.
            IntPtr hWnd = GetForegroundWindow();
            ///Here we are getting length of wind. name to create right variable for reading wind. name.
            Length = GetWindowTextLength(hWnd);
            ///creating variable for reading window name.
            StringBuilder WinName = new StringBuilder(Length + 1);
            ///reading window name.
            Length = GetWindowText(hWnd,WinName, WinName.Capacity);
            ///send window name to string variable.
            ///We can not creat global variable, we do not know size of window name.
            ActiveWindowName = WinName.ToString();
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }
    }
}

