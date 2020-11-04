using System;
using System.Threading;
using System.Net.Sockets;
using System.IO;
using System.Text;

namespace SCTPServer {
	public static partial class ServerClass {
		public class Client {
			byte[] buf = new byte[1048576];
			public string name;
			public Socket socket;
			public Thread listener;
			public bool isAdmin;
			public string namecolor = "70";
			public string description = "Empty";
			public string contact = "Empty";
			public bool muted = false;
			public DateTime muteTime;
			public Thread timerThread;
			public bool timerThreadSign;
			public Client(string name, Socket socket, bool isAdmin) {
				this.name = name;
				this.socket = socket;
				this.isAdmin = isAdmin;
				this.muteTime = DateTime.UnixEpoch;
				timerThreadSign = true;
				timerThread = new Thread(() => {
					while(muteTime > DateTime.Now) { 
						if(timerThreadSign == false) return;
						Thread.Sleep(1000);
				 	}
					Unmute();
				});
			}
			public Client(Socket socket, IRC.User user) {
				this.socket = socket;
				isAdmin = user.isAdmin;
				name = user.name;
				namecolor = user.namecolor;
				description = user.description;
				contact = user.contact;
				this.muted = user.muted;
				muteTime = user.muteTime;
				timerThreadSign = true;
				timerThread = new Thread(() => {
					while(muteTime > DateTime.Now) { 
						if(timerThreadSign == false) return;
						Thread.Sleep(1000);
				 	}
					Unmute();
				});
			}
			public void Unmute() {
				muteTime = DateTime.UnixEpoch;
				muted = false;
				Send(string.Format("~&E0You are unmuted now."));
			}
			public void Mute(int time) {
				if(timerThread.IsAlive)
					timerThread.Interrupt();
				timerThreadSign = true;
				timerThread = new Thread(() => {
					while(muteTime > DateTime.Now) { 
						if(timerThreadSign == false) return;
						Thread.Sleep(1000);
				 	}
					Unmute();
				});
				muted = true;
				muteTime = DateTime.Now.AddSeconds(time);
				Send(string.Format("~&C0You are muted by administrator to {0}.", muteTime));
				timerThread.Start();
			}
			public void Send(string message) {
				try {
					Conv.Send(ref socket, message);
				} catch(Exception ex) {
					IOClass.Error(String.Format("&C0Exception occured in {0}({1})\n" + ex.ToString(), name, socket.RemoteEndPoint));
				}
			}
			public void Leave(string message) {
				try {
					Send(string.Format("!CLOSE {0}", message));
					listener.Interrupt();
					timerThreadSign = false;
					
					socket.Disconnect(false);
					// clients.Remove(this);
					IOClass.Log(string.Format("User \"{0}\" from {1} disconnected.", name, socket.RemoteEndPoint));
					Broadcast(string.Format("User \"{0}\" left the server.", name));
					clients.Remove(this);

					socket.Close();
				} catch(Exception ex) {
					IOClass.Error(String.Format("&C0Exception occured in {0}:\n" + ex.ToString(), name));
				}
			}
			public int ProcRecv(string message) {
				string ending = "";
				string[] args;
				args = message.Split(" ");
				ending = message.Substring(message.IndexOf(' ') + 1);
				switch(args[0].ToUpper()) {
					case "DELETE": {
						if(args.Length < 2) {
							Send("~&C0Error: File name and BASE64 data required.");
							return -1;
						}
						if(!File.Exists("files/" + name + "/" + args[1])) {
							Send("~&C0Error: File not exist.");
							return -1;
						}
						File.Delete("files/" + name + "/" + args[1]);
						Send(string.Format("~&A0File from \"{0}\" deleted.", "files/" + name + "/" + args[1]));
						return 0;
					}
					case "UPLOAD": {
						if(!allowUpload) {
							Send("~&C0Error: File upload is not allowed.");
							return -1;
						}
						if(args.Length < 3) {
							Send("~&C0Error: File name and BASE64 data required.");
							return -1;
						}
						if(!Directory.Exists("files/" + name))
							Directory.CreateDirectory("files/" + name);
						if(File.Exists("files/" + name + "/" + args[1])) {
							Send("~&C0Error: File already exists.");
							return -1;
						}
						bool success;
						int length;
						success = Convert.TryFromBase64String(args[2], buf, out length);
						if(!success) {
							Send("~&C0Error: Failed to convert BASE64 to binary.");
							return -1;
						} else {
							byte[] write = new byte[length];
							for(int i = 0; i < length; i++)
								write[i] = buf[i];
							File.WriteAllBytes("files/" + name + "/" + args[1], write);
							Send(string.Format("~&A0File uploaded to \"{0}\".", "files/" + name + "/" + args[1]));
							return 0;
						}
					}
					case "TIME": {
						Send(string.Format("~&E0Server Time:&F0 {0}", DateTime.Now));
						return 0;
					}
					case "RFILE": {
						if(args.Length < 2) {
							Send("~&C0Error: File name required.");
							return -1;
						}
						if(!File.Exists("files/" + ending)) {
							Send("~&C0Error: No such a file.");
							return -1;
						}
						Send("@" + File.ReadAllText("files/" + ending));
						return 0;
					}
					case "FILE": {
						if(args.Length < 2) {
							Send("~&C0Error: File name required.");
							return -1;
						}
						if(!File.Exists("files/" + ending)) {
							Send("~&C0Error: No such a file.");
							return -1;
						}
						Send("~" + File.ReadAllText("files/" + ending));
						return 0;
					}
					case "CHANGEPWD": {
						if(args.Length < 4) {
							Send("~&C0Error: Wrong arguments.");
							return -1;
						}
						for(int i = 0; i < IRC.users.Count; i++) {
							if(IRC.users[i].name == name) {
								if(IRC.users[i].password != args[1]) {
									Send("~&C0Error: Wrong password.");
									return -1;
								}
								if(args[2] != args[3]) {
									Send("~&C0Error: Password not confirmed.");
									return -1;
								}
								IRC.users[i].password = args[2];
								IRC.users[i].WriteFile();
								Send("~&A0Changed your password successfully.");
								return 0;
							}
						}
						Send("~&C0Error: Can not find your account.");
						return -1;
					}
					case "INFO": {
						string msg = "Server Info:\n"; 
						msg += IRC.name + "\n";
						msg += IRC.description;
						Send("~" + msg);
						return 0;
					}
					case "HELP": {
						Send("~" + File.ReadAllText("helplist"));
						return 0;
					}
					case "SAY": {
						if(muted) {
							Send("~&C0Error: Muted.");
							return -1;
						}
						if(args.Length < 2) {
							Send("~&C0Error: Message required.");
							return -1;
						}
						Broadcast(string.Format("<&{2}{0}&70> {1}", name, ending, namecolor));
						IOClass.Log(string.Format("<&{2}{0}&70> {1}", name, ending, namecolor));
						return -255;
					}
					case "ME": {
						if(muted) {
							Send("~&C0Error: Muted.");
							return -1;
						}
						if(args.Length < 2) {
							Send("~&C0Error: Message required.");
							return -1;
						}
						Broadcast(string.Format("&D0* &{2}{0}&70 {1}", name, ending, namecolor));
						IOClass.Log(string.Format("&D0* &{2}{0}&70 {1}", name, ending, namecolor));
						return -255;
					}
					case "BYE": {
						Leave("User disconnected");
						return 0;
					}
					case "LIST": {
						string list = string.Format("~Online: {0}/{1}\n", clients.Count, maxClientLimit);
						if(clients.Count == 0) list += "Empty";
						foreach(Client c in clients)
							list += "&" + c.namecolor + c.name + "  ";
						Send(list);
						return 0;
					}
					case "USER": {
						string tname;
						if(args.Length <= 1) {
							Send("~&C0Error: Username required.");
							return -1;
						}
						tname = args[1];
						for(int i = 0; i < clients.Count; i++) {
							if(clients[i].name == tname) {
								string lines = "~";
								lines += String.Format("&{0}{1}&70 ", clients[i].namecolor, clients[i].name);
								if(clients[i].isAdmin)
									lines += "&F0[Admin] ";
								if(clients[i].muted) {
									lines += string.Format("&C0[Muted to {0}]", clients[i].muteTime);
								}
								lines += "\n";
								lines += String.Format("&70Description:\n{0}\n", clients[i].description);
								lines += String.Format("&70Contact:\n{0}", clients[i].contact);
								Send(lines);
								return 0;
							}
						}
						Send("~&C0Error: No such a user.");
						return -1;
					}
					case "TELL": {
						string tname;
						string msg;
						if(args.Length <= 2) {
							Send("~&C0Error: Username and message required.");
							return -1;
						}
						tname = args[1];
						msg = ending.Substring(ending.IndexOf(' ') + 1);
						if(msg == "") {
							Send("~&C0Error: Message required.");
							return -1;
						}
						for(int i = 0; i < clients.Count; i++) {
							if(clients[i].name == tname) {;
								Send(string.Format("~&80You whispered to \"{0}\": {1}", tname, msg));
								clients[i].Send(string.Format("~&80\"{0}\" whispered to you: {1}", name,  msg));
								IOClass.Log(string.Format("&80<{0}> -> <{1}> {2}", name, tname, msg));
								return 0;
							}
						}
						Send("~&C0Error: No such a user.");
						return -1;
					}
					case "COLOR": {
						if(ending.Length != 2 || Conv.HexToDec(ending[0]) == -1 || Conv.HexToDec(ending[0]) == 1) {
							Send("~&C0Error: Invalid color string.");
							break;
						} else {
							namecolor = ending;
							for(int i = 0; i < IRC.users.Count; i++) {
								if(IRC.users[i].name == name) {
									IRC.users[i].namecolor = ending;
									IRC.users[i].WriteFile();
									Send("~&A0Changed your name color successfully.");
									return 0;
								}
							}
							Send("~&C0Error: Failed to change your name color.");
							return -1;
						}
					}
					case "DESCRIPT": {
						description = ending.Replace("&n", "\n");
						for(int i = 0; i < IRC.users.Count; i++) {
							if(IRC.users[i].name == name) {
								IRC.users[i].description = description;
								IRC.users[i].WriteFile();
								Send("~&A0Changed your description successfully.");
								return 0;
							}
						}
						Send("~&C0Error: Failed to change your description.");
						return -1;
					}
					case "CONTACT": {
						contact = ending.Replace("&n", "\n");
						for(int i = 0; i < IRC.users.Count; i++) {
							if(IRC.users[i].name == name) {
								IRC.users[i].contact = contact;
								IRC.users[i].WriteFile();
								Send("~&A0Changed your contact successfully.");
								return 0;
							}
						}
						Send("~&C0Error: Failed to change your contact.");
						return -1;
					}
					default: {
						Send(string.Format("~&C0Error: No such a command \"{0}\".", args[0]));
						return -1;
					}
				}
				return -1;
			}
			public void Listen() {
				int recvLength;
				string message;
				while(true) {
					try {
						if(socket.Connected == false) {
							return;
						}
						recvLength = socket.Receive(buf);
						message = Conv.ToStr(buf, recvLength);
						if(message == "") continue;
						if(message[0] == '!') {
							if(!isAdmin) {
								Send("~&C0Premission denied. You are not admin.");
								IOClass.Log(string.Format("Denied execution <{0}> \"{1}\"", name, message));
								continue;
							}
							ProcCommand(this, message.Substring(1));
							continue;
						}
						int rec = ProcRecv(message);
						if(rec == 0) {
							IOClass.Log(string.Format("User \"{0}\" execution: {1}", name, message));
						} else if (rec == -1) {
							IOClass.Log(string.Format("User \"{0}\" failed execution: {1}", name, message));
						}
					} catch(Exception ex) {
						try {
							Send("~&C0Exception occured in remote server.");
						} catch(Exception) {}
						if(ex.HResult == -2147467259) continue;
						IOClass.Error(String.Format("&C0Exception occured in \"{0}\"\n" + ex.ToString(), name));
					}
				}
			}
			public void BeginListen() {
				socket.ReceiveTimeout = 0;
				listener = new Thread(() => { Listen(); });
				listener.Start();
				clients.Remove(this);
			}
		}
    }
}
