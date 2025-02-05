using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Reception
{
    public class IniManager
    {
        private static string filePath;
        private StringBuilder lpReturnedString;
        private int bufferSize;

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string lpString, string lpFileName);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);

        public IniManager(string iniPath)
        {
            filePath = iniPath;
            bufferSize = 512;
            lpReturnedString = new StringBuilder(bufferSize);
        }

        // read ini date depend on section and key
        public string ReadIniFile(string section, string key, string defaultValue)
        {
            var ret_val = new StringBuilder(255);
            GetPrivateProfileString(section, key, defaultValue, ret_val, 255, filePath);
            return ret_val.ToString();
        }

        public string Read(string section, string key)
        {
            return ReadIniFile(section, key, string.Empty);
        }

        // write ini data depend on section and key
        public int WriteIniFile(string section, string key, string val)
        {
            try
            {
                WritePrivateProfileString(section, key, val, filePath);
                return 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }
    }
}
