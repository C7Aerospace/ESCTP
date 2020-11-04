using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Timers;
namespace SCTPServer {
	public static partial class ServerClass {
		public static List<Client> clients;
		public static int maxClientLimit;
		public static string ipString = "127.0.0.1";
		public static int port = 7655;
		public static Socket listenerSock;
		public static int clientTimeout;
		public static IPAddress ip;
		public static EndPoint endp;
		public static Thread clearThread;
		public static bool allowAnonymous;
		public static bool allowRegister;
		public static byte[] listenerRecvBuffer = new byte[16384];
		public static int recentMessageCount = 16;
		public static Queue<string> recentBroadcastBuffer;
		public static int timerInterval;
		public static System.Timers.Timer timeTimer;
		public static bool allowUpload;
		public static void BroadcastTime(object sender, System.Timers.ElapsedEventArgs e) {
			IOClass.Log("&80Send current time.");
			Broadcast(string.Format("&80-------- {0} --------", DateTime.Now), false);
		}
		public static void Exit(int num = 0) {
			IOClass.Log("Server shutdown.");
			IRC.Write();
			foreach(Client c in clients) {
				try {
					c.socket.Send(Conv.ToByte("!CLOSE Server closed"));
					c.socket.Close();
				} catch (Exception) {}
			}
			foreach(IRC.User c in IRC.users) {
				c.WriteFile();
			}
			listenerSock.Close();
			Environment.Exit(num);
		}
		public static void Exit(string message, int num = 0) {
			IOClass.Log(message);
			Exit(num);
		}
		public static void ExitException(string message) {
			IOClass.Error(message);
			Exit(0);
		}
		public static void Broadcast(string message, bool add = true) {
			if(add) {
				if(recentBroadcastBuffer.Count >= recentMessageCount)
					recentBroadcastBuffer.Dequeue();
				recentBroadcastBuffer.Enqueue(message);
			}
			foreach(Client c in clients) {
				if(c.socket.Connected) {
					c.Send("~" + message);
				}
			}
		}
		public static void DeleteClient() {
			try {
				for(int i = 0; i < clients.Count; i++) {
					if(clients[i].socket.Connected == false) {
                        IOClass.Warn(string.Format("User \"{0}\" socket cleared.", clients[i].name));
						// Broadcast(string.Format("User \"{0}\" left by exception", clients[i].name));
						clients.Remove(clients[i]);
					}
				}
			} catch(Exception ex) {
				IOClass.Error("\n" + ex.ToString());
			}
		}
	}
}
