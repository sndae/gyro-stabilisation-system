using System;
using System.Collections.Generic;
using System.Linq;
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

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        
        private const int WH_MOUSE_LL = 14;
        private const int WM_MOUSEMOVE = 0x0200;
        private const int WM_MOUSEWHEEL = 0x020A;
        private const int WM_LBUTTONDOWN = 0x0201;

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static LowLevelKeyboardProc _proc = HookCallback;
        
        private static IntPtr _hookID = IntPtr.Zero;
        private static int press_count = 0;

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
                SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
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
                
                /// TEST ONLY
                press_count++;
                string rrr = press_count.ToString();
                Form1.ActiveForm.Text = rrr;// "fsdfs";
                /// END TEST ONLY
                return (IntPtr)1;
            }
            
            /// This part detect pressing to any key.
            if ((nCode >= 0) && (wParam == (IntPtr)WM_KEYDOWN))
            {
                
                /// TEST ONLY
                press_count++;
                string rrr = press_count.ToString();
                Form1.ActiveForm.Text = rrr;// "fsdfs";
                /// END TEST ONLY
                return (IntPtr)1;

                ///This part detect pressing to only WINDOWS key.
//                int vkCode = Marshal.ReadInt32(lParam);
//                if (((Keys)vkCode == Keys.LWin) || ((Keys)vkCode == Keys.RWin))
//                {
//                    Console.WriteLine("{0} blocked!", (Keys)vkCode);
//                    /// TEST ONLY
//                    press_count++;
//                    string rrr = press_count.ToString();
//                    Form1.ActiveForm.Text = rrr;// "fsdfs";
//                    /// END TEST ONLY
//                    return (IntPtr)1;
//                }
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }


    }
}
