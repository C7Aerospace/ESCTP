using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;

using static System.Console;
namespace SCTPServer {
	public static partial class ServerClass {
		public static void InitSocket() {
			try {
				IRC.Read();
				if(!File.Exists("serversettings")) {
					FileStream fs = new FileStream("serversettings", FileMode.Create);
					TextWriter tw = new StreamWriter(fs, Encoding.UTF8);
					tw.WriteLine("IRCName=ESCTP Server IRC");
					tw.WriteLine("IRCDescription=This is a test server, and is still under develop.");
					tw.WriteLine("Welcome=&A0Welcome to this server.");
					tw.WriteLine("Address=127.0.0.1");
					tw.WriteLine("Port=7655");
					tw.WriteLine("MaxClients=20");
					tw.WriteLine("ClientTimeout=3000");
					tw.WriteLine("Anonymous=true");
					tw.WriteLine("Register=true");
					tw.WriteLine("RecentMessageCount=16");
					tw.WriteLine("TimerInterval=600");
					tw.WriteLine("AllowUpload=true");
					tw.WriteLine("# There should be no spaces between equal symbol.");
					tw.WriteLine("# Use \"&n\" in the value to start a new line.");
					tw.Close();
					fs.Close();
				}
				if(!Directory.Exists("files"))
					Directory.CreateDirectory("files");

				string[] prop = File.ReadAllLines("serversettings");
				foreach(string p in prop) {
					if(p == "") continue;
					if(p[0] == '#') continue;
					string key, value;
					int breaker = p.IndexOf('=');
					key = p.Substring(0, breaker);
					value = p.Substring(breaker + 1);
					value = value.Replace("&n", "\n");
					switch(key) {
						case "IRCName": { IRC.name = value.Replace("&n", "\n"); break; }
						case "IRCDescription": { IRC.description = value.Replace("&n", "\n"); break; }
						case "Welcome": { IRC.welcome = value.Replace("&n", "\n"); break; }
						case "Address": { ipString = value; break; }
						case "Port": { port = int.Parse(value); break; }
						case "MaxClients": { maxClientLimit = int.Parse(value); break; }
						case "ClientTimeout": { clientTimeout = int.Parse(value); break; }
						case "Anonymous": { allowAnonymous = bool.Parse(value); break; }
						case "Register": { allowRegister = bool.Parse(value); break; }
						case "RecentMessageCount": { recentMessageCount = int.Parse(value); break; }
						case "TimerInterval": { timerInterval = int.Parse(value); break; }
						case "AllowUpload": { allowUpload = bool.Parse(value); break; }
					}
				}
				clients = new List<Client>();
				clearThread = new Thread(() => {
					while(true) {
						DeleteClient();
						Thread.Sleep(2000);
					}
				});
				clearThread.Start();
				listenerSock = new Socket(SocketType.Stream, ProtocolType.Tcp);
				ip = IPAddress.Parse(ipString);
				endp = new IPEndPoint(ip, port);
				listenerSock.Bind(endp);
				listenerSock.ReceiveTimeout = clientTimeout;
				recentBroadcastBuffer = new Queue<string>();
				timeTimer = new System.Timers.Timer();
				if(timerInterval != 0) {
					timeTimer.Interval = timerInterval * 1000;
					timeTimer.Elapsed += new System.Timers.ElapsedEventHandler(BroadcastTime);
					timeTimer.Enabled = true;
					timeTimer.Start();
				}

			} catch(Exception ex) {
				ExitException(ex.ToString());
			}
		}
		public static void BeginListen() {
			InitSocket();
			IOClass.Log(string.Format("Server started listening at {0}", endp));
			while(true) {
				listenerSock.Listen(100);
				Socket curClient = listenerSock.Accept();
				try {
					// Process Incoming Link
					int result = ProcIncomingLink(ref curClient);
					if(result == -1) {
						IOClass.Error(string.Format("Connection from {0} closed by exception.", curClient.RemoteEndPoint));
						curClient.Close();
					}
					if(result == 0) {
						IOClass.Log(string.Format("Connection from {0} closed.", curClient.RemoteEndPoint));
						curClient.Disconnect(false);
						curClient.Close();
					}
				} catch(Exception ex) {
					IOClass.Error("\n&C0" + ex.ToString());
					curClient.Close();
				}
			}
		}
	}
}
