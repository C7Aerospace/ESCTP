using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Security.Permissions;

namespace SCTPClient
{
    public partial class Mainform : Form
    {
        byte[] buf = new byte[1048576];
        int recvLen;
        string recvStr;
        Thread listener;
        //public TcpClient c;
        public Socket socket;
        bool connected = false;

        public delegate void ioDelegate(string msg);
        ioDelegate colorIO = new ioDelegate(IOClass.ColorPrint);
        ioDelegate plainIO = new ioDelegate(IOClass.PlainPrint);
        public Mainform()
        {
            InitializeComponent();
        }
        public void Listen()
        {
            int recvLength = 0;
            string recm = "";
            string[] recq;
            try
            {
                while (true)
                {
                    if (!socket.Connected)
                        break;
                    recvLength = socket.Receive(buf);
                    recm = Conv.ToStr(buf, recvLength);
                    // WriteLine(recm);
                    if (recm[0] == '!')
                    {
                        recq = recm.Split(' ');
                        if (recq[0] == "!CLOSE")
                        {
                            MessageBox.Show(string.Format("Disconnected: {0}", recm.Substring(recm.IndexOf(' ') + 1)), "Server", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            socket.Close();
                            connected = false;
                            return;
                        }
                        // recvBox.Invoke(colorIO, recm.Substring(recm.IndexOf(' ') + 1));
                    }
                    else if (recm[0] == '~')
                    {
                        recvBox.Invoke(colorIO, recm.Substring(1));
                    }
                    else if (recm[0] == '@')
                    {
                        recvBox.Invoke(plainIO, recm.Substring(1));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }
        public void Login(IPEndPoint ep, string username, string password, bool register)
        {
            IOClass.ColorPrint(string.Format("&e0Connecting to&c0 {0}&e0 as&c0 \"{1}\"...", ep, username));
            try
            {
                Exception ipv4Ex = new Exception();
                Exception ipv6Ex = new Exception();
                bool ipv4Success = false;
                bool ipv6Success = false;
                try {
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socket.Connect(ep);
                    ipv4Success = true;
                } catch(Exception ex) { ipv4Ex = ex; }
                if (!ipv4Success) {
                    try
                    {
                        socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
                        socket.Connect(ep);
                        ipv6Success = true;
                    } catch (Exception ex) { ipv6Ex = ex; }
                }
                if(!ipv4Success && !ipv6Success)
                {
                    MessageBox.Show(string.Format("Failed to build socket.\nIPv4: {0}\nIPv6: {1}\nEnd Point: {2}", ipv4Ex.ToString(), ipv6Ex.ToString(), ep.ToString()), "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                Conv.Send(ref socket, "HELLO");
                recvLen = socket.Receive(buf);
                recvStr = Conv.ToStr(buf, recvLen);
                if (recvStr != "!HI")
                {
                    MessageBox.Show(string.Format("Unexpected response \"{0}\".", recvStr), "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    socket.Close();
                    return;
                }
                if(register)
                    Conv.Send(ref socket, string.Format("REG {0} {1}", username, password));
                else
                    Conv.Send(ref socket, string.Format("AUTH {0} {1}", username, password));
                recvLen = socket.Receive(buf);
                recvStr = Conv.ToStr(buf, recvLen);
                string cmd = recvStr.Split(' ')[0];
                string arg = recvStr.Substring(recvStr.IndexOf(' ') + 1);
                if (cmd == "!PASS")
                {
                    IOClass.ColorPrint(arg);
                    socket.ReceiveTimeout = 0;
                    listener = new Thread(() => { Listen(); });
                    listener.Start();
                    connected = true;
                } else if (cmd == "!CLOSE")
                {
                    MessageBox.Show(string.Format("Server denied: {0}.", arg), "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    socket.Close();
                    return;
                } else
                {
                    MessageBox.Show(string.Format("Unexpected response {0}.", recvStr), "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    socket.Close();
                    return;
                }
            } catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                socket.Close();
                connected = false;
            }
        }
        private void Send()
        {
            try
            {
                if (!connected)
                {
                    MessageBox.Show("Server is not connected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                string command = sendInput.Text.Trim();
                sendInput.Text = "";
                if (command == "") return;
                if (command[0] == '/')
                {
                    Conv.Send(ref socket, command.Substring(1));
                }
                else if (command[0] == '!')
                {
                    Conv.Send(ref socket, command);
                }
                else
                {
                    Conv.Send(ref socket, "say " + command);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void MainformLoad(object sender, EventArgs e)
        {
            
            IOClass.InitStyle();
            // recvBox.Font = new Font(new FontFamily("微软雅黑"), 10.0f, FontStyle.Regular);
            // sendInput.Font = new Font(new FontFamily("微软雅黑"), 10.0f, FontStyle.Regular);
            // Login(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7655), "Prox", "mh229227", false);
            IOClass.ColorPrint("&a0Type \"#help\" list all local commands.");
            sendInput.Focus();
        }

        private void InputTextChange(object sender, EventArgs e)
        {
            sendInput.Text = sendInput.Text.Replace("\n", "");
            sendInput.Text = sendInput.Text.Replace("\r\n", "");
        }

        private new void Resize(object sender, EventArgs e)
        {
            mainMsgBox.Width = Width - 40;
            mainMsgBox.Height = Height - 113;
            recvBox.Width = Width - 53;
            recvBox.Height = Height - 135;
            sendBox.Width = Width - 40;
            sendBox.Top = Height - 103;
            sendInput.Width = Width - 64;
        }

        private void Rezoom(object sender, EventArgs e)
        {
            recvBox.ZoomFactor = 1.0f;
        }

        private void FormClose(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void OnKeyDown(object sender, KeyPressEventArgs e)
        {
            // IOClass.ColorPrint(((int)e.KeyChar).ToString());
            if (e.KeyChar == 13)
            {
                string message = sendInput.Text;
                sendInput.Text = sendInput.Text.Replace("\n", "");
                message = message.Replace("\n", "").Trim();
                if (message == "") return;
                if (message[0] == '#')
                {
                    LocalCommands.ProcLocalCommands(message.Substring(1));
                    sendInput.Text = "";
                }
                else
                {
                    Send();
                }
            }
        }
    }
}
