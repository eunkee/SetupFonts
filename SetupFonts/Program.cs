using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace SetupFonts
{
    internal class Program
    {
        #region 폰트 설치
        //폰트 설치: https://www.it-swarm-ko.tech/ko/c%23/%ed%94%84%eb%a1%9c%ea%b7%b8%eb%9e%98%eb%b0%8d-%eb%b0%a9%ec%8b%9d%ec%9c%bc%eb%a1%9c-%ea%b8%80%ea%bc%b4%ec%9d%84-%ec%84%a4%ec%b9%98%ed%95%98%eb%8a%94-%eb%b0%a9%eb%b2%95-c/1045179572/
        [DllImport("gdi32", EntryPoint = "AddFontResource")]
        public static extern int AddFontResourceA(string lpFileName);
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern int AddFontResource(string lpszFilename);
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern int CreateScalableFontResource(uint fdwHidden, string
            lpszFontRes, string lpszFontFile, string lpszCurrentPath);
        private static void RegisterFont(string contentFontName)
        {
            string sourceDir2 = System.IO.Directory.GetCurrentDirectory() + @"\font";
            // Creates the full path where your font will be installed
            var fontDestination = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Fonts), contentFontName);

            if (!System.IO.File.Exists(fontDestination))
            {
                // Copies font to destination
                System.IO.File.Copy(Path.Combine(sourceDir2, contentFontName), fontDestination);

                PrivateFontCollection fontCol = new PrivateFontCollection();
                fontCol.AddFontFile(fontDestination);
                var actualFontName = fontCol.Families[0].Name;

                //Add font
                AddFontResource(fontDestination);
                //Add registry entry   
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts", actualFontName, contentFontName, RegistryValueKind.String);
            }
        }
        #endregion 폰트 설치

        #region Windows API
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private static readonly int SW_HIDE = 0;
        private static readonly int SW_SHOW = 1;
        #endregion

        static void Main(string[] args)
        {
            ShowWindow(GetConsoleWindow(), SW_HIDE);

            try
            {
                List<string> fontList = new List<string>();
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo("font");
                foreach (System.IO.FileInfo File in di.GetFiles())
                {
                    if (File.Extension.ToLower().CompareTo(".otf") == 0)
                    {
                        fontList.Add(File.Name);
                    }
                }

                foreach (string fontName in fontList)
                {
                    RegisterFont($"{fontName}");
                }
            }
            catch { }

        }
    }
}
