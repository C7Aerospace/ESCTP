using System;
using System.Net.Sockets;
using System.Text;

using static System.Console;
namespace SCTPServer {
	public static class Conv {
		static byte[] buf = new byte[4096];
		public static string Formalize(string str) {
			return str.Replace("&", "&&");
		}
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
	public static class IOClass {
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
						if(i < message.Length) Write(message[i]);
						continue;
					}
					back = Conv.HexToDec(message[i + 2]);
					if(back == -1) {
						if(i < message.Length) Write(message[i]);
						continue;
					}

					ForegroundColor = (ConsoleColor)fore;
					BackgroundColor = (ConsoleColor)back;
					i += 3;
				}
				if(i < message.Length)
					if(i < message.Length) Write(message[i]);
			}
			WriteLine();
			ResetColor();
			// WritePrompt();
		}
		public static void Log(string message) {
			ColorPrint(String.Format("[INFO][{0}] {1}", DateTime.Now, message));
		}
		public static void Warn(string message) {
			ColorPrint(String.Format("&E0[WARN]&70[{0}] {1}", DateTime.Now, message));
		}
		public static void Error(string message) {
			ColorPrint(String.Format("&C0[ERRO]&70[{0}] {1}", DateTime.Now, message));
		}
	}
}
