using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;




namespace AutoTyping
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            KeyPreview = true;
        }
        
        // 29~96 Keyboard Hooking
        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc callback, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);
        [DllImport("user32.dll")]
        static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, int wParam, IntPtr IParam);

        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);

        private delegate IntPtr LowLevelKeyboardProc(int nCode,IntPtr wParam,IntPtr lParam);
        
        const int WH_KEYBOARD_LL = 13;
        const int WM_KEYDOWN = 0x100;
            
        private LowLevelKeyboardProc _proc = hookProc;
        private static IntPtr hhook = IntPtr.Zero;

        public void SetHook()
        {
            IntPtr hInstace = LoadLibrary("User32");
            hhook = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, hInstace, 0);
        }
        
        public static void UnHook()
        {
            UnhookWindowsHookEx(hhook);
        }

        public static IntPtr hookProc(int code, IntPtr wParam, IntPtr IParam)
        {
            if (code >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(IParam);

                if (vkCode.ToString() == "65")
                {
                    MessageBox.Show("You Pressed a A");
                }
                else if (vkCode.ToString() == "33")
                {
                    string txt = "Page Up이 눌림";
                    for (int j = 0; j < txt.Length; j++)
                    {
                        string data = txt[j].ToString();
                        SendKeys.SendWait(data);
                        Task.Delay(100).Wait();

                    }
                }
                return CallNextHookEx(hhook,code,(int)wParam,IParam);
            }
            else
                return CallNextHookEx(hhook, code, (int)wParam, IParam);
        }

        private void Form1_FormClosing(object sender, CancelEventArgs e)
        {
            UnHook();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetHook();
        }
        //END


        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.PageUp:
                    string txt = MainTextBOx.Text;
                    for (int i = 0; i < txt.Length; i++)
                    {
                        string data = txt[i].ToString();
                        SendKeys.SendWait(data);
                        Task.Delay(100).Wait();

                    }

                    break;
            }
        }
    }   
}
