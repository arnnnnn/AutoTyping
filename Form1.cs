

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




//Todo - IME Han/Eng State , Check txtbox , han,eng check  // if hangle toEng ->  sendkey



namespace AutoTyping
{
    
    
    public partial class Form1 : Form
    {

        public class Matching_Table
    {
        const int INITIAL_CONS = 19;
        const int MEDIAL_CONS = 21;
        const int FINAL_CONS = 28;


        readonly char[] initialConsonant_kor =  
        {
            'ㄱ', 'ㄲ', 'ㄴ', 'ㄷ',
            'ㄸ', 'ㄹ', 'ㅁ', 'ㅂ',
            'ㅃ', 'ㅅ', 'ㅆ', 'ㅇ',
            'ㅈ', 'ㅉ', 'ㅊ', 'ㅋ',
            'ㅌ', 'ㅍ', 'ㅎ'
        };

        readonly char[] initialConsonant_eng =  
        {
            'r', 'R', 's', 'e',
            'E', 'f', 'a', 'q',
            'Q', 't', 'T', 'd',
            'w', 'W', 'c', 'z',
            'x', 'v', 'g'
        };

        readonly char[] medialConsonant_kor =  
        {
            'ㅏ', 'ㅐ', 'ㅑ', 'ㅒ',
            'ㅓ', 'ㅔ', 'ㅕ', 'ㅖ',
            'ㅗ', 'ㅘ', 'ㅙ', 'ㅚ',
            'ㅛ', 'ㅜ', 'ㅝ', 'ㅞ',
            'ㅟ', 'ㅠ', 'ㅡ', 'ㅢ', 'ㅣ'
        };

        readonly char[] medialConsonant_eng = 
        {
            'k', 'o', 'i', 'O',
            'j', 'p', 'u', 'P',
            'h', ' ', ' ', ' ',
            'y', 'n', ' ', ' ',
            ' ', 'b', 'm', ' ', 'l'
        };

        readonly char[] finalConsonant_kor = 
        {
            ' ', 'ㄱ', 'ㄲ', 'ㄳ',
            'ㄴ', 'ㄵ', 'ㄶ', 'ㄷ',
            'ㄹ', 'ㄺ', 'ㄻ', 'ㄼ',
            'ㄽ', 'ㄾ', 'ㄿ', 'ㅀ',
            'ㅁ', 'ㅂ', 'ㅄ', 'ㅅ',
            'ㅆ', 'ㅇ', 'ㅈ', 'ㅊ',
            'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ'
        };

        readonly char[] finalConsonant_eng = 
        {
            ' ', 'r', 'R', ' ',
            's', ' ', ' ', 'e',
            'f', ' ', ' ', ' ',
            ' ', ' ', ' ', ' ',
            'a', 'q', ' ', 't',
            'T', 'd', 'w', 'c',
            'z', 'x', 'v', 'g'
        };

        readonly char[] pairConsonant_kor = 
        {
            'ㅘ', 'ㅙ', 'ㅚ', 'ㅝ',
            'ㅞ', 'ㅟ', 'ㅢ', 'ㄳ',
            'ㄵ', 'ㄶ', 'ㄺ', 'ㄻ',
            'ㄼ', 'ㄽ', 'ㄾ', 'ㄿ',
            'ㅀ', 'ㅄ'
        };

        //this is string Array, if you using this Array, must recognize that it's a string Array.
        readonly string[] pairConsonant_eng = 
        {
            "hk", "ho", "hl", "nj",
            "np", "nl", "ml", "rt",
            "sw", "sg", "fr", "fa",
            "fq", "ft", "fx", "fv",
            "fg", "qt"
        };
}

        
        public Form1()
        {
            InitializeComponent();


            KeyPreview = true;
        }

        


        #region DllImport Part for Hooking and IME
        //For Keyboard Hooking

        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc callback, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);
        [DllImport("user32.dll")]
        static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, int wParam, IntPtr IParam);

        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);

        //For Check Hangle Mode 
        [DllImport("imm32.dll")]
        private static extern IntPtr ImmGetContext(IntPtr hwnd);
        [DllImport("imm32.dll")]
        private static extern bool ImmGetConversionStatus(IntPtr himc, ref int lpdw, ref int lpdw2);
        #endregion


        #region  Keyboard Hooking Method
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);


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
                    string txt = "gd 안녕";
                    
                    if(Check_hangul(txt, 2))
                        MessageBox.Show("this is hangul");
                    else
                        MessageBox.Show("this is not hangul");
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
        #endregion

        public static bool Check_hangul(string hangul,int pos)
        {
            if (char.GetUnicodeCategory(hangul[pos]) == System.Globalization.UnicodeCategory.OtherLetter)
                return true;
            else
                return false;
        }

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
