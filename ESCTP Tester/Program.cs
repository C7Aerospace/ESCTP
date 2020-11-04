using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using static System.Console;
namespace ESCTPTester
{
	class Program{
		public static class Conv {
			static byte[] buf = new byte[4096];
            public static byte[] ToByte(string message) {
			    return Encoding.UTF8.GetBytes(message);
		    }
		    public static string ToStr(byte[] bytes, int length) {
			    return Encoding.UTF8.GetString(bytes, 0, length);
		    }
			public static int HexToDec(char c) {
				if(c >= '0' && c <= '9')
					return c - '0';
				if(c >= 'A' && c <= 'F')
					return c - 'A' + 10;
				if(c >= 'a' && c <= 'f')
					return c - 'a' + 10;
				return -1;
			}
			public static void Send(ref Socket target, string message) {
				buf = Encoding.UTF8.GetBytes(message);
				target.Send(buf);
			}
			public static string Recv(ref Socket target) {
				int recvLength = target.Receive(buf);
				return Encoding.UTF8.GetString(buf, 0, recvLength);
			}
		}
		public static Socket socket;
		static IPAddress ip = IPAddress.Any;
		static EndPoint endp = new IPEndPoint(ip, 7800);
		public static void ResetColor() {
			ForegroundColor = ConsoleColor.Gray;
			BackgroundColor = ConsoleColor.Black;
		}
		public static void ColorPrint(string message) {
			// Console.SetCursorPosition(0, Console.CursorTop);
			ResetColor();
			for(int i = 0; i < message.Length; i++) {
				if(message[i] == '&') {
					int fore, back;
					fore = Conv.HexToDec(message[i + 1]);

					if(fore == -1) {
						if(message[i + 1] == '&') {
							Write('&');
							i += 1;
							continue;
						}
						Write(message[i]);
						continue;
					}
					back = Conv.HexToDec(message[i + 2]);
					if(back == -1) {
						Write(message[i]);
						continue;
					}

					ForegroundColor = (ConsoleColor)fore;
					BackgroundColor = (ConsoleColor)back;
					i += 3;
				}
				Write(message[i]);
			}
			WriteLine();
			ResetColor();
			// WritePrompt();
		}
        static void Listen() {
            socket.ReceiveTimeout = 0;
            byte[] recv = new byte[1024];
            int recvLength = 0;
            string recm = "";
			string[] recq;
			try {
            	while(true) {
					if(!socket.Connected)
						break;
                	recvLength = socket.Receive(recv);
			    	recm = Conv.ToStr(recv, recvLength);
					// WriteLine(recm);
					if(recm[0] == '!') {
						recq = recm.Split(" ");
						ColorPrint(recm.Substring(recm.IndexOf(' ') + 1));
						if(recq[0] == "!CLOSE")
							break;
					} else if(recm[0] == '~') {
						ColorPrint(recm.Substring(1));
					} else if(recm[0] == '@') {
						WriteLine(recm.Substring(1));
					}
            	}
			} catch (Exception) {
				
			}
        }
		static void Main(string[] args)
		{
			Console.Write("Address:");
			string ip = Console.ReadLine();
			Console.Write("Port:");
			int port = int.Parse(Console.ReadLine());
			Console.Write("Register or sign? <Y/N>");
			string regis = Console.ReadLine().ToUpper();
			Console.Write("Username:");
            string name = Console.ReadLine();
			Console.Write("Password:");
			string password = Console.ReadLine();
			socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
			socket.Connect(ip, port);

			Thread listener = new Thread(() => { Listen(); });
            listener.Start();
			Conv.Send(ref socket, "HELLO");
			Thread.Sleep(100);
			if(regis == "N")
				Conv.Send(ref socket, string.Format("AUTH {0} {1}", name, password));
			else if(regis == "Y")
				Conv.Send(ref socket, string.Format("REG {0} {1}", name, password));
            while(socket.Connected) {
				string msg = Console.ReadLine();
				if(msg.Length > 0) {
					if(msg[0] == '#') {
						Conv.Send(ref socket, msg.Substring(1));
					}
					else if(msg[0] == '!') {
						Conv.Send(ref socket, msg);
					}
					else {
						Conv.Send(ref socket, "say " + msg);
						CursorTop = CursorTop -1;
					}
				}
            }
		    // Conv.Send(ref socket, "BYE");
            
		}
	}
}
