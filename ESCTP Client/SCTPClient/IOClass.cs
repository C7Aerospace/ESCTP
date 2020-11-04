using System;
using System.Drawing;
using System.Drawing.Text;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Configuration;
using System.Drawing.Drawing2D;

namespace SCTPClient
{
	public static class Conv
	{
		static byte[] buf = new byte[4096];
		public static byte[] ToByte(string message)
		{
			return Encoding.UTF8.GetBytes(message);
		}
		public static string ToStr(byte[] bytes, int length)
		{
			return Encoding.UTF8.GetString(bytes, 0, length);
		}
		public static int HexToDec(char c)
		{
			if (c >= '0' && c <= '9')
				return c - '0';
			if (c >= 'A' && c <= 'F')
				return c - 'A' + 10;
			if (c >= 'a' && c <= 'f')
				return c - 'a' + 10;
			return -1;
		}
		public static void Send(ref Socket target, string message)
		{
			buf = Encoding.UTF8.GetBytes(message);
			target.Send(buf);
		}
		public static string Recv(ref Socket target)
		{
			int recvLength = target.Receive(buf);
			return Encoding.UTF8.GetString(buf, 0, recvLength);
		}
	}
	public static class IOClass
	{
		public static Color[] colors = new Color[16];
		public static void InitStyle()
        {
			colors[0] = Color.FromArgb(15, 15, 15);
			colors[1] = Color.FromArgb(15, 15, 191);
			colors[2] = Color.FromArgb(15, 191, 15);
			colors[3] = Color.FromArgb(15, 191, 191);
			colors[4] = Color.FromArgb(191, 15, 15);
			colors[5] = Color.FromArgb(191, 15, 191);
			colors[6] = Color.FromArgb(191, 191, 15);
			colors[7] = Color.FromArgb(191, 191, 191);
			colors[8] = Color.FromArgb(63, 63, 63);
			colors[9] = Color.FromArgb(63, 63, 255);
			colors[10] = Color.FromArgb(63, 255, 63);
			colors[11] = Color.FromArgb(63, 255, 255);
			colors[12] = Color.FromArgb(255, 63, 63);
			colors[13] = Color.FromArgb(255, 63, 255);
			colors[14] = Color.FromArgb(255, 255, 63);
			colors[15] = Color.FromArgb(255, 255, 255);
			string[] lines = File.ReadAllLines("theme");
			foreach(string line in lines)
            {
				string key, val;
				key = line.Split('=')[0];
				val = line.Split('=')[1];

				string[] vals = val.Split(',');
				int r = 0, g = 0, b = 0;
				try
				{
					r = int.Parse(vals[0]);
					g = int.Parse(vals[1]);
					b = int.Parse(vals[2]);
				} catch(Exception) { }
				switch (key)
                {
					case "font":
                        {
							Font font = new Font(new FontFamily(val), 10.0f, FontStyle.Regular);
							Program.mf.mainMsgBox.Font = font;
							Program.mf.recvBox.Font = font;
							Program.mf.sendBox.Font = font;
							Program.mf.sendInput.Font = font;
							break;
                        }
					case "bg":
                        {
							Program.mf.BackColor = Color.FromArgb(r, g, b);
							Program.mf.mainMsgBox.BackColor = Color.FromArgb(r, g, b);
							Program.mf.recvBox.BackColor = Color.FromArgb(r, g, b);
							Program.mf.sendBox.BackColor = Color.FromArgb(r, g, b);
							Program.mf.sendInput.BackColor = Color.FromArgb(r, g, b);
							break;
						}
					case "fg":
						{
							Program.mf.ForeColor = Color.FromArgb(r, g, b);
							Program.mf.mainMsgBox.ForeColor = Color.FromArgb(r, g, b);
							Program.mf.recvBox.ForeColor = Color.FromArgb(r, g, b);
							Program.mf.sendBox.ForeColor = Color.FromArgb(r, g, b);
							Program.mf.sendInput.ForeColor = Color.FromArgb(r, g, b);
							break;
						}
					case "black":
                        {
							colors[0] = Color.FromArgb(r, g, b);
							break;
                        }
					case "blue":
						{
							colors[1] = Color.FromArgb(r, g, b);
							break;
						}
					case "green":
						{
							colors[2] = Color.FromArgb(r, g, b);
							break;
						}
					case "cyan":
						{
							colors[3] = Color.FromArgb(r, g, b);
							break;
						}
					case "red":
						{
							colors[4] = Color.FromArgb(r, g, b);
							break;
						}
					case "purple":
						{
							colors[5] = Color.FromArgb(r, g, b);
							break;
						}
					case "yellow":
						{
							colors[6] = Color.FromArgb(r, g, b);
							break;
						}
					case "gray":
						{
							colors[7] = Color.FromArgb(r, g, b);
							break;
						}
					case "darkgray":
						{
							colors[8] = Color.FromArgb(r, g, b);
							break;
						}
					case "lightblue":
						{
							colors[9] = Color.FromArgb(r, g, b);
							break;
						}
					case "lightgreen":
						{
							colors[10] = Color.FromArgb(r, g, b);
							break;
						}
					case "lightcyan":
						{
							colors[11] = Color.FromArgb(r, g, b);
							break;
						}
					case "lightred":
						{
							colors[12] = Color.FromArgb(r, g, b);
							break;
						}
					case "lightpurple":
						{
							colors[13] = Color.FromArgb(r, g, b);
							break;
						}
					case "lightyellow":
						{
							colors[14] = Color.FromArgb(r, g, b);
							break;
						}
					case "white":
						{
							colors[15] = Color.FromArgb(r, g, b);
							break;
						}
				}
            }
		}
		public static void ResetColor()
		{
			Program.mf.recvBox.SelectionBackColor = colors[0];
			Program.mf.recvBox.SelectionColor = colors[7];
		}
		public static void PlainPrint(string message)
        {
			Program.mf.recvBox.AppendText(message);
		}
		public static void ColorPrint(string message)
		{
			// Console.SetCursorPosition(0, Console.CursorTop);
			message = message.Replace("\r\n", "\n");
			ResetColor();
			for (int i = 0; i < message.Length; i++)
			{
				if (message[i] == '&')
				{
					int fore = -1, back = -1;
					fore = Conv.HexToDec(message[i + 1]);

					if (fore == -1)
					{
						if (message[i + 1] == '&')
						{
							Program.mf.recvBox.AppendText("&");
							i += 1;
							continue;
						}
						Program.mf.recvBox.AppendText(message[i].ToString());
						continue;
					}
					if (i + 2 < message.Length)
						back = Conv.HexToDec(message[i + 2]);
					if (back == -1)
					{
						Program.mf.recvBox.AppendText(message[i].ToString());
						continue;
					}
					Program.mf.recvBox.SelectionColor = colors[fore];
					Program.mf.recvBox.SelectionBackColor = colors[back];
					i += 3;
				}
				if (i < message.Length)
					Program.mf.recvBox.AppendText(message[i].ToString());
			}
			Program.mf.recvBox.AppendText("\n");
			Program.mf.recvBox.Focus();
			Program.mf.recvBox.Select(Program.mf.recvBox.Text.Length, 0);
			Program.mf.recvBox.ScrollToCaret();
			Program.mf.sendInput.Focus();
			// ResetColor();
			// WritePrompt();

		}
		/*
		public static void Log(string message)
		{
			ColorPrint(String.Format("[INFO][{0}] {1}", DateTime.Now, message));
		}
		public static void Warn(string message)
		{
			ColorPrint(String.Format("&E0[WARN]&70[{0}] {1}", DateTime.Now, message));
		}
		public static void Error(string message)
		{
			ColorPrint(String.Format("&C0[ERRO]&70[{0}] {1}", DateTime.Now, message));
		}
		*/
	}
}
