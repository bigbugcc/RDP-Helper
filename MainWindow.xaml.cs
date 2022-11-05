using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RDP_Helper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static byte[] s_aditionalEntropy = null;
        public SnackbarMessageQueue messageQueue;
        public MainWindow()
        {
            InitializeComponent();
            messageQueue = iSnackbar.MessageQueue;
        }

        private void Bt_Encrypt_Click(object sender, RoutedEventArgs e)
        {
            string Pass = Pb_InPasswd.Password.ToString();
            if (string.IsNullOrEmpty(Pass)) { MsgBar("Please input content !", 0.75); return; } 
            byte[] secret = Encoding.Unicode.GetBytes(Pass);
            byte[] encryptedSecret = Protect(secret);
            string res = string.Empty;
            foreach (byte b in encryptedSecret)
            {
                res += b.ToString("X2");
            }
            Tb_OutPasswd.Clear();
            Tb_OutPasswd.Text = res;
            MsgBar("Completed !", 1);
        }

        private void Bt_Decode_Click(object sender, RoutedEventArgs e)
        {
            string Pass = Pb_InPasswd.Password.ToString();
            if (string.IsNullOrEmpty(Pass)) { MsgBar("Please input content !", 0.75); return; }
            // Decrypt the data and store in a byte array.
            byte[] originalData = Unprotect(GetBytes(Pass));
            if (string.IsNullOrEmpty(originalData.ToString())) return;
            string str = Encoding.Default.GetString(originalData).Replace("\0", string.Empty);
            Tb_OutPasswd.Clear();
            Tb_OutPasswd.Text = str;
            MsgBar("Completed !", 1);
        }

        private void Bt_Copy_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Tb_OutPasswd.Text)) 
            {
                MsgBar("Nothing needs to be copied !", 1);
                return;
            }
            Clipboard.SetText(Tb_OutPasswd.Text);
            MsgBar("Copied !", 0.75);
        }

        private void Pb_InPasswd_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Pb_InPasswd.Password))
            {
                Tb_OutPasswd.Clear();
            }
        }

        //加密
        public static byte[] Protect(byte[] data)
        {
            try
            {
                // Encrypt the data using DataProtectionScope.CurrentUser. The result can be decrypted
                //  only by the same current user.
                return ProtectedData.Protect(data, s_aditionalEntropy, DataProtectionScope.LocalMachine);
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("Data was not encrypted. An error occurred.");
                
                Console.WriteLine(e.ToString());

                return null;
            }
            
        }

        //解密
        public static byte[] Unprotect(byte[] data)
        {
            try
            {
                //Decrypt the data using DataProtectionScope.CurrentUser.
                return ProtectedData.Unprotect(data, s_aditionalEntropy, DataProtectionScope.LocalMachine);
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("Data was not decrypted. An error occurred.");

                Console.WriteLine(e.ToString());

                return null;
            }
        }

        //16 to byte
        public static byte[] GetBytes(string Pass) 
        {
            byte[] bytes = null;
            try 
            {
                byte[] data = new byte[Pass.Length / 2];
                for (int i = 0; i < Pass.Length; i += 2)
                {
                    string s = Pass[i] + "" + Pass[i + 1];
                    data[i / 2] = Convert.ToByte(s, 16);
                }
                bytes = data;
            } catch(Exception e) {
                Console.WriteLine(e);
            }
            return bytes;
        }

        //消息提示Bar
        public void MsgBar(string Mes, double time)
        {
            try
            {
                messageQueue.Enqueue(Mes, null, null, null, false, true, TimeSpan.FromSeconds(time));
            }
            catch (Exception ex)
            {
                Console.WriteLine("SnackBarMessage:" + ex.Message);
            }
        }

        private static void rdpProfile(string filename, string address, string username, string password, string colordepth)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
            using (StreamWriter streamWriter = new StreamWriter(filename, true))
            {
                streamWriter.WriteLine("screen mode id:i:2");
                streamWriter.WriteLine("desktopwidth:i:0");
                streamWriter.WriteLine("desktopheight:i:0");
                streamWriter.WriteLine("session bpp:i:" + colordepth);
                streamWriter.WriteLine("winposstr:s:0,1,0,0,1234,792");
                streamWriter.WriteLine("compression:i:1");
                streamWriter.WriteLine("keyboardhook:i:2");
                streamWriter.WriteLine("audiocapturemode:i:0");
                streamWriter.WriteLine("videoplaybackmode:i:1");
                streamWriter.WriteLine("connection type:i:6");
                streamWriter.WriteLine("displayconnectionbar:i:1");
                streamWriter.WriteLine("disable wallpaper:i:1");
                streamWriter.WriteLine("allow font smoothing:i:1");
                streamWriter.WriteLine("allow desktop composition:i:1");
                streamWriter.WriteLine("disable full window drag:i:1");
                streamWriter.WriteLine("disable menu anims:i:1");
                streamWriter.WriteLine("disable themes:i:1");
                streamWriter.WriteLine("disable cursor setting:i:0");
                streamWriter.WriteLine("bitmapcachepersistenable:i:0");
                streamWriter.WriteLine("full address:s:" + address);
                streamWriter.WriteLine("audiomode:i:0");
                streamWriter.WriteLine("redirectprinters:i:0");
                streamWriter.WriteLine("redirectcomports:i:0");
                streamWriter.WriteLine("redirectsmartcards:i:0");
                streamWriter.WriteLine("redirectclipboard:i:1");
                streamWriter.WriteLine("redirectposdevices:i:0");
                streamWriter.WriteLine("redirectdirectx:i:1");
                streamWriter.WriteLine("drivestoredirect:s:");
                streamWriter.WriteLine("autoreconnection enabled:i:1");
                streamWriter.WriteLine("authentication level:i:2");
                streamWriter.WriteLine("prompt for credentials:i:0");
                streamWriter.WriteLine("negotiate security layer:i:1");
                streamWriter.WriteLine("remoteapplicationmode:i:0");
                streamWriter.WriteLine("alternate shell:s:");
                streamWriter.WriteLine("shell working directory:s:");
                streamWriter.WriteLine("gatewayhostname:s:");
                streamWriter.WriteLine("gatewayusagemethod:i:4");
                streamWriter.WriteLine("gatewaycredentialssource:i:4");
                streamWriter.WriteLine("gatewayprofileusagemethod:i:0");
                streamWriter.WriteLine("promptcredentialonce:i:1");
                streamWriter.WriteLine("use redirection server name:i:0");
                streamWriter.WriteLine("use multimon:i:0");
                if (!string.IsNullOrEmpty(username))
                {
                    streamWriter.WriteLine("username:s:" + username);
                }
                if (!string.IsNullOrEmpty(password))
                {
                    streamWriter.WriteLine("password 51:b:" + password);
                }
            }
        }

    }
}
