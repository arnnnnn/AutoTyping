﻿

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
            public static bool stat;
            public static int conversion = 0;
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

            public static  char[] initialConsonant_kor =
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
        
        class handle:Form1
        {
            private IntPtr hwnd;
            public void set(IntPtr hwndApp)
            {
                hwnd = ImmGetContext(hwndApp);
            }
            public void set()
            {
                hwnd = ImmGetContext(this.Handle);
            }
            public IntPtr get()
            {
                return hwnd;
            }
            ~handle() { }

        }
        


        #region DllImport Part for Hooking and IME
        //For Keyboard Hooking

        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc callback, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);
        [DllImport("user32.dll")]
        static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, int wParam, IntPtr IParam);

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte vk, byte scan, int flags, int extrainfo);

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
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();
        [DllImport("imm32.dll")]
        private static extern bool ImmGetConversionStatus(IntPtr hWnd, int dwConversion, int flags);
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
                    handle handle = new handle();
                    IntPtr hwndApp = GetForegroundWindow();
                    handle.set(hwndApp);
                    IntPtr hwnd = handle.get();
                    try
                    {
                        if (txt==null || txt.Length == 0)
                            MessageBox.Show("I can't find any Text for run this program.\nInput some text.");
                        
                        else
                        {
                            for (int i = 0; i < txt.Length; i++)
                            {
                                if (txt[i] == '\n' || txt[i]=='\r')
                                {
                                    SendKeys.SendWait("\n");// "\n" or "\r" has two index so must be i+=1
                                    i += 1;
                                }
                                else if (txt[i] == ' ')
                                {
                                    SendKeys.SendWait(" ");
                                }
                                else if (IsHangul(txt, i))
                                {
                                    string dvd = DivideHangul(txt, i);
                                    string output=ToAlphabet(dvd);
                                    for (int k = 0; k < output.Length; k++)
                                    {
                                        ChangeIME(hwnd,true);
                                        SendKeys.SendWait(output[k].ToString());
                                        Thread.Sleep(25);
                                    }
                                }
                                else
                                {
                                    ChangeIME(hwnd,false);
                                    SendKeys.SendWait(txt[i].ToString());
                                    Thread.Sleep(30);
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
        }
        #endregion

        #region IMEmode change between korean and english

        

        public static void ChangeIME(IntPtr hwnd ,bool target)
        {
            try
            {
                ImmGetConversionStatus(hwnd,GlobalVar.conversion, 0);
                if (GlobalVar.conversion == 1)
                    GlobalVar.stat = true;
                else
                    GlobalVar.stat = false;

                if (target!= GlobalVar.stat)
                {
                    keybd_event((byte)21, 0, 0, 0);
                }
            }
            catch(Exception e)
            {
                MessageBox.Show("Error occured ! \r Error is =>> \r" + e.ToString());
                MessageBox.Show("If you see this MessageBox Let me know. \r contact : dbs8543@gmail.com");
            }
            return;
        }



        #endregion


        public static string ToAlphabet(string divided)
        {
            string Changed="";
            try
            {
                for (int rep = 0; rep < divided.Length; rep++)
                {
                    if (Matching_Table.initialConsonant_kor.Contains(divided[rep]))
                    {
                        Changed += Matching_Table.initialConsonant_eng[Array.IndexOf(Matching_Table.initialConsonant_kor, divided[rep])];
                    }
                    else if (Matching_Table.medialConsonant_kor.Contains(divided[rep]))
                    {
                        Changed += Matching_Table.medialConsonant_eng[Array.IndexOf(Matching_Table.medialConsonant_kor, divided[rep])];
                    }
                    else if (Matching_Table.finalConsonant_kor.Contains(divided[rep]))
                    {
                        Changed += Matching_Table.finalConsonant_eng[Array.IndexOf(Matching_Table.finalConsonant_kor, divided[rep])];
                    }
                    else if (Matching_Table.pairConsonant_kor.Contains(divided[rep]))
                    {
                        Changed += Matching_Table.pairConsonant_eng[Array.IndexOf(Matching_Table.pairConsonant_kor, divided[rep])];
                    }
                }
            }
            catch(Exception e)
            {
                MessageBox.Show("Error occured ! \r Error is =>> \r" + e.ToString());
                MessageBox.Show("If you see this MessageBox Let me know. \r contact : dbs8543@gmail.com");
            }
            return Changed;
        }


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
            // have to test many case for catch some error.
            string divided = null;
            int interval=0;

            try
            {
                if (Matching_Table.initialConsonant_kor.Contains(hangul[pos]) || Matching_Table.medialConsonant_kor.Contains(hangul[pos])
                || Matching_Table.finalConsonant_kor.Contains(hangul[pos]))
                    return hangul[pos].ToString();
                else
                {
                    interval = hangul[pos] - Matching_Table.HANGUL_UNICODE_START_INDEX;
                    divided += Matching_Table.initialConsonant_kor[interval / Matching_Table.MF];
                    divided += Matching_Table.medialConsonant_kor[interval % Matching_Table.MF / Matching_Table.FINAL_CONS];
                    divided += (Matching_Table.finalConsonant_kor[interval % Matching_Table.FINAL_CONS] == ' ') ? '\0' : Matching_Table.finalConsonant_kor[interval % Matching_Table.FINAL_CONS];
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error occured ! \r Error is =>> \r" + e.ToString());
                MessageBox.Show("If you see this MessageBox Let me know. \r contact : dbs8543@gmail.com");
            }

            return divided;
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
        }

        #endregion


        private void MainTextBox_TextChanged(object sender, EventArgs e)
        {
            GlobalVar.Text = MainTextBox.Text;
        }
    }   
}
