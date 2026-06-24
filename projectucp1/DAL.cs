using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace projectucp1
{
    public class DAL
    {
        public static string GetLocalIPAddress()
        {
            string localIP = string.Empty;
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        localIP = ip.ToString();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting IP: " + ex.Message);
            }
            return localIP;
        }

        public static string GetConnectionString()
        {
            return $"Data Source={GetLocalIPAddress()}\\NANA,27144;Initial Catalog=TOKO_ROTIku;User ID=sa;Password=IstrinyaPascal4Ever;";
        }
    }
}