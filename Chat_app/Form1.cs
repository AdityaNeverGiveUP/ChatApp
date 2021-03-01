using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace Chat_app
{
    public partial class Form1 : Form
    {
        Socket sck;
        EndPoint eplocal, epRemote;
          byte[] buffer;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            meip.Text = GetLocalIP();
          //  frdip.Text = GetLocalIP();
        }
        private string GetLocalIP()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();
            }
            return "127.0.0.1";
        }
 
        private void Connect_but_Click(object sender, EventArgs e)
        {
            sck.Close();
            sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            eplocal=new IPEndPoint(IPAddress.Parse(meip.Text),Convert.ToInt32(meport.Text));
            sck.Bind(eplocal);
            epRemote=new IPEndPoint(IPAddress.Parse(frdip.Text),Convert.ToInt32(frdport.Text));
            sck.Connect(epRemote);
            buffer =new byte[1500];
         // sck.BeginReceiveFrom(buffer,0,buffer.Length,SocketFlags.None,ref epRemote,new AsyncCallback(MessageCallBack),buffer);
            sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
        }




        private void MessageCallBack(IAsyncResult aresult)
        {
            try
            {
                byte[] receiveData = new byte[1500];
                receiveData = (byte[])aresult.AsyncState;
                ASCIIEncoding aEncoding = new ASCIIEncoding();
                string receivemessage = aEncoding.GetString(receiveData);
                listBox1.Items.Add("Friend:  " + receivemessage);
                buffer = new byte[1500];
                sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ASCIIEncoding aencoding = new ASCIIEncoding();
            byte[] sendmess = new byte[1500];
            sendmess = aencoding.GetBytes(textBox1.Text);
            if (textBox1.Text != "")        
            {
              
                sck.Send(sendmess);
                listBox1.Items.Add("Me : " + textBox1.Text);
            }
            textBox1.Text = "";
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1.PerformClick();
            }
        }
    }
}
