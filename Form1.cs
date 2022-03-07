

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
        public static class GlobalVar
        {
            public static string Text;
            public static IntPtr hwnd;
            public static bool IMEmode=false;
        }
        public static class Matching_Table
        {

            /// <summary>
            /// number of three type consonants in Hangul ( Initial,Medial, Final ) 
            /// </summary>
            public const int INITIAL_CONS = 19;
            public const int MEDIAL_CONS = 21;
            public const int FINAL_CONS = 28;
            public const int MF = MEDIAL_CONS * FINAL_CONS;
            /// <summary>
            ///  HANGUL_UNICODE_START_INDEX -> Hangul Unicode Start Index
            ///  HANGUL_UNICODE_END_INDEX -> Hangul Unicode End Index
            /// </summary>
            public const int HANGUL_UNICODE_START_INDEX = 0xAC00;
            public const int HANGUL_UNICODE_END_INDEX = 0xD7A3;
            /// <summary>
            /// divide Start INDEX  
            /// INITIAL -> initial consonant Start Index
            /// MEDIAL ->  medial consonant Start Index
            /// FINAL -> final consonant Start Index
            /// </summary>
            public const int INITIAL_START_INDEX = 0x1100;
            public const int MEDIAL_START_INDEX = 0x1161;
            public const int FINAL_START_INDEX = 0x11a7;

            public static readonly char[] initialConsonant_kor =
                    {
                    'ㄱ', 'ㄲ', 'ㄴ', 'ㄷ',
                    'ㄸ', 'ㄹ', 'ㅁ', 'ㅂ',
                    'ㅃ', 'ㅅ', 'ㅆ', 'ㅇ',
                    'ㅈ', 'ㅉ', 'ㅊ', 'ㅋ',
                    'ㅌ', 'ㅍ', 'ㅎ'
                };
            public static readonly char[] initialConsonant_eng =
                    {
                    'r', 'R', 's', 'e',
                    'E', 'f', 'a', 'q',
                    'Q', 't', 'T', 'd',
                    'w', 'W', 'c', 'z',
                    'x', 'v', 'g'
                };
            public static readonly char[] medialConsonant_kor =
                    {
                    'ㅏ', 'ㅐ', 'ㅑ', 'ㅒ',
                    'ㅓ', 'ㅔ', 'ㅕ', 'ㅖ',
                    'ㅗ', 'ㅘ', 'ㅙ', 'ㅚ',
                    'ㅛ', 'ㅜ', 'ㅝ', 'ㅞ',
                    'ㅟ', 'ㅠ', 'ㅡ', 'ㅢ', 'ㅣ'
                };
            public static readonly char[] medialConsonant_eng =
                    {
                    'k', 'o', 'i', 'O',
                    'j', 'p', 'u', 'P',
                    'h', ' ', ' ', ' ',
                    'y', 'n', ' ', ' ',
                    ' ', 'b', 'm', ' ', 'l'
                };
            public static readonly char[] finalConsonant_kor =
                    {
                    ' ', 'ㄱ', 'ㄲ', 'ㄳ',
                    'ㄴ', 'ㄵ', 'ㄶ', 'ㄷ',
                    'ㄹ', 'ㄺ', 'ㄻ', 'ㄼ',
                    'ㄽ', 'ㄾ', 'ㄿ', 'ㅀ',
                    'ㅁ', 'ㅂ', 'ㅄ', 'ㅅ',
                    'ㅆ', 'ㅇ', 'ㅈ', 'ㅊ',
                    'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ'
                };
            public static readonly char[] finalConsonant_eng =
                    {
                    ' ', 'r', 'R', ' ',
                    's', ' ', ' ', 'e',
                    'f', ' ', ' ', ' ',
                    ' ', ' ', ' ', ' ',
                    'a', 'q', ' ', 't',
                    'T', 'd', 'w', 'c',
                    'z', 'x', 'v', 'g'
                };
            public static readonly char[] pairConsonant_kor =
                    {
                    'ㅘ', 'ㅙ', 'ㅚ', 'ㅝ',
                    'ㅞ', 'ㅟ', 'ㅢ', 'ㄳ',
                    'ㄵ', 'ㄶ', 'ㄺ', 'ㄻ',
                    'ㄼ', 'ㄽ', 'ㄾ', 'ㄿ',
                    'ㅀ', 'ㅄ'
                };
            //this is string Array, if you using this Array, must recognize that it's a string Array.
            public static readonly string[] pairConsonant_eng =
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

        //For exchange between korean and english
        [DllImport("imm32.dll")]
        private static extern IntPtr ImmGetContext(IntPtr hWnd);
        [DllImport("imm32.dll")]
        private static extern Boolean ImmSetConversionStatus(IntPtr hIMC, Int32 fdwConversion,Int32 fdwSentence);
        [DllImport("imm32.dll")]
        public static extern IntPtr ImmGetDefaultIMEWnd(IntPtr hWnd);
        [DllImport("imm32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

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

                if (vkCode.ToString() == "45")
                {
                    string txt = GlobalVar.Text;
                    try
                    {
                        if (txt==null || txt.Length == 0)
                            MessageBox.Show("There \ris nothing");
                        
                        else
                        {
                            for(int i = 0; i < txt.Length; i++)
                            {
                                if (txt[i] == '\n')
                                {
                                    //SendKeys.SendWait(txt[i].ToString());
                                    continue;
                                }
                                    
                                if (IsHangul(txt, i))
                                {
                                    string dvd= DivideHangul(txt, i);
                                    for(int k=0;k<dvd.Length; k++)
                                    {
                                        SendKeys.SendWait(dvd[k].ToString());
                                        Thread.Sleep(1000);
                                    }
                                    
                                    //MessageBox.Show(txt[i].ToString() + " is hangul");
                                }

                                else
                                {
                                    //MessageBox.Show(txt[i].ToString() + " is not hangul");
                                    SendKeys.SendWait(txt[i].ToString());
                                }
                            }
                        }
                    }
                    catch(Exception e)
                    {
                        MessageBox.Show("Error occured ! \r Error is =>> \r" + e.ToString());
                        MessageBox.Show("If you see this MessageBox Let me know. \r contact : dbs8543@gmail.com");
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
            IntPtr hwnd=ImmGetContext(this.Handle);
            GlobalVar.hwnd = hwnd;
            
        }
        #endregion

        #region IMEmode change between korean and english

        public static void ChangeIME(int target)
        {
            try
            {
                ImmSetConversionStatus(GlobalVar.hwnd, target, 0);
                /*if (GlobalVar.IMEmode)
                {
                    GlobalVar.IMEmode = false;
                    
                    ImmSetConversionStatus(GlobalVar.hwnd, 0, 0);
                    MessageBox.Show("Done. Change IME MODE kor to eng");
                }
                else
                {
                    GlobalVar.IMEmode = true;
                    ImmSetConversionStatus(GlobalVar.hwnd, 1, 0);
                    MessageBox.Show("Done. Change IME MODE eng to kor");
                }*/
            }
            catch(Exception e)
            {
                MessageBox.Show("Error occured ! \r Error is =>> \r" + e.ToString());
                MessageBox.Show("If you see this MessageBox Let me know. \r contact : dbs8543@gmail.com");
            }

        }



        #endregion


        #region this region for check,divide hangul method...  will be add change hangul to english method.
        /// <summary>
        /// Get Some string and INDEX as string and int 
        /// </summary>
        /// <param name="hangul">Get string for check hangul</param>
        /// <param name="pos">Get Index number as int.</param>
        /// <returns></returns>
        public static bool IsHangul(string hangul,int pos)
        {
            bool res=false;
            if (Regex.IsMatch(hangul[pos].ToString(), @"[ㄱ-ㅎㅏ-ㅣ가-힣]"))
                res= true;
            else
                res = false;
            return res;
        }

        /// <summary>
        /// DivideHangul Method. 
        /// </summary>
        /// <param name="hangul">This param is get sentence as string</param>
        /// <param name="pos">This param is for index number as int</param>
        /// <returns></returns>
        public static string DivideHangul(string hangul,int pos)
        {
            // test at 2022.03.05 ... it was perfect.
            string divided = null;
            int interval=0;

            if (Matching_Table.initialConsonant_kor.Contains(hangul[pos]) || Matching_Table.medialConsonant_kor.Contains(hangul[pos])
                || Matching_Table.finalConsonant_kor.Contains(hangul[pos]) )
                return hangul[pos].ToString();
            else
            {
                interval = hangul[pos] - Matching_Table.HANGUL_UNICODE_START_INDEX;
                divided += Matching_Table.initialConsonant_kor[interval / Matching_Table.MF];
                divided += Matching_Table.medialConsonant_kor[interval % Matching_Table.MF / Matching_Table.FINAL_CONS];
                divided += (Matching_Table.finalConsonant_kor[interval % Matching_Table.FINAL_CONS] == ' ') ? '\0' : Matching_Table.finalConsonant_kor[interval % Matching_Table.FINAL_CONS];
            }
            
            /* init method -> it's imperfect
            int index = hangul[pos] - Matching_Table.HANGUL_UNICODE_START_INDEX;

            
            int initial = Matching_Table.INITIAL_START_INDEX + index/(Matching_Table.MEDIAL_CONS*Matching_Table.FINAL_CONS);
            int medial = Matching_Table.MEDIAL_START_INDEX + (index % (Matching_Table.MEDIAL_CONS * Matching_Table.FINAL_CONS)) / Matching_Table.FINAL_CONS;
            int final = -Matching_Table.FINAL_START_INDEX + index % Matching_Table.FINAL_CONS;

            if (final == 4519)
            {
                divided += (char)initial;
                divided+=(char)medial;
            }
            else
            {
                divided += (char)initial;
                divided += (char)medial;
                divided += (char)final;
            }
            */

            return divided;
        }

        #endregion

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.PageDown:
                    string txt = MainTextBox.Text;
                    for (int i = 0; i < txt.Length; i++)
                    {
                        string data = txt[i].ToString();
                        SendKeys.SendWait(data);
                        Task.Delay(100).Wait();

                    }

                    break;
            }
        }

        private void MainTextBox_TextChanged(object sender, EventArgs e)
        {
            GlobalVar.Text = MainTextBox.Text;
        }
    }   
}
