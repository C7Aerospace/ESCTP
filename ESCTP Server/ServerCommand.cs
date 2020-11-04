using System;

namespace SCTPServer {
    public static partial class ServerClass {
        public static void ProcCommand(Client user, string command) {
			try {
				string[] args = command.Split(" ");
				string cmd = args[0].ToLower();
				switch(cmd) {
					case "stop": {
						ServerClass.Exit(string.Format("Server stopped by administrator {0}.", user.name));
						break;
					}
					case "unmute": {
						if(args.Length < 2) {
							user.Send(string.Format("~Execution <{0}> &C0Username required.", user.name));
							break;
						}
						int findOnline = -1;
						for(int i = 0; i < clients.Count; i++) {
							if(clients[i].name == args[1]) {
								findOnline = 1;
								clients[i].muteTime = DateTime.UnixEpoch;
								user.Send(string.Format("~Execution <{0}> Unmuted.", user.name, clients[i].muteTime));
								break;
							}
						}
						if(findOnline == 0) break;
						int findOffline = -1;
						for(int i = 0; i < IRC.users.Count; i++) {
							if(IRC.users[i].name == args[1]) {
								findOffline = 1;
								if(IRC.users[i].isAdmin) {
									user.Send(string.Format("~Execution <{0}> &C0Target is admin.", user.name));
									findOffline = 0;
									break;
								} else {
									IRC.users[i].Unmute();
									IRC.users[i].WriteFile();
									if(findOnline == -1)
										user.Send(string.Format("~Execution <{0}> Offline user unmuted.", user.name, IRC.users[i].muteTime));
									break;
								}
							}
						}
						if(findOffline == 0) break;
						if(findOffline == -1) { user.Send(string.Format("~Execution <{0}> &C0No such a user.", user.name)); break; }
						break;
					}
					case "mute": {
						if(args.Length < 3) {
							user.Send(string.Format("~Execution <{0}> &C0Username and time required.", user.name));
							break;
						}
						int seconds = 0;
						string[] tst = args[2].Split(":");
						try {
							if(tst.Length == 1) {
								seconds += int.Parse(tst[0]);
							} else if(tst.Length == 2) {
								seconds += int.Parse(tst[0]) * 60;
								seconds += int.Parse(tst[1]);
							} else if(tst.Length == 3) {
								seconds += int.Parse(tst[0]) * 60 * 60;
								seconds += int.Parse(tst[1]) * 60;
								seconds += int.Parse(tst[2]);
							} else if(tst.Length == 4) {
								seconds += int.Parse(tst[0]) * 60 * 60 * 24;
								seconds += int.Parse(tst[0]) * 60 * 60;
								seconds += int.Parse(tst[1]) * 60;
								seconds += int.Parse(tst[2]);
							} else {
								throw new Exception();
							}
						} catch(Exception) {
							user.Send(string.Format("~Execution <{0}> &C0Invalid time.", user.name));
							break;
						}
						
						int findOnline = -1;
						for(int i = 0; i < clients.Count; i++) {
							if(clients[i].name == args[1]) {
								findOnline = 1;
								if(clients[i].isAdmin) {
									user.Send(string.Format("~Execution <{0}> &C0Target is admin.", user.name));
									findOnline = 0;
									break;
								} else {
									clients[i].Mute(seconds);
									user.Send(string.Format("~Execution <{0}> Muted to {1}.", user.name, clients[i].muteTime));
									break;
								}
							}
						}
						if(findOnline == 0) break;
						int findOffline = -1;
						for(int i = 0; i < IRC.users.Count; i++) {
							if(IRC.users[i].name == args[1]) {
								findOffline = 1;
								if(IRC.users[i].isAdmin) {
									user.Send(string.Format("~Execution <{0}> &C0Target is admin.", user.name));
									findOffline = 0;
									break;
								} else {
									IRC.users[i].Mute(seconds);
									IRC.users[i].WriteFile();
									if(findOnline == -1)
										user.Send(string.Format("~Execution <{0}> Offline user muted to {1}.", user.name, IRC.users[i].muteTime));
									break;
								}
							}
						}
						if(findOffline == 0) break;
						if(findOffline == -1) { user.Send(string.Format("~Execution <{0}> &C0No such a user.", user.name)); break; }
						break;
					}
					case "kick": {
						if(args.Length < 2) {
							user.Send(string.Format("~Execution <{0}> &C0Username required.", user.name));
							break;
						}
						bool success = false;
						for(int i = 0; i < clients.Count; i++) {
							if(clients[i].name == args[1]) {
								success = true;
								if(clients[i].isAdmin) {
									user.Send(string.Format("~Execution <{0}> &C0Target is admin.", user.name));
									break;
								} else {
									clients[i].Leave(string.Format("~kicked by administrator", user.name));
									break;
								}
							}
						}
						if(!success) user.Send(string.Format("~Execution <{0}> &C0No such a user.", user.name));
						break;
					}
					case "bc": {
						if(args.Length < 2) {
							user.Send(string.Format("~Execution <{0}> &C0Message required.", user.name));
							break;
						}
						Broadcast(command.Substring(command.IndexOf(' ') + 1));
						break;
					}
					case "clearbuffer": {
						recentBroadcastBuffer.Clear();
						user.Send(string.Format("~Execution <{0}> Buffer cleared.", user.name));
						break;
					}
					default: {
						user.Send(string.Format("~Execution <{0}> &C0Unknown command \"{1}\".", user.name, args[0]));
						IOClass.Log(string.Format("Execution <{0}> &C0Unknown command \"{1}\".", user.name, args[0]));
						return;
					}
				}
				IOClass.Log(string.Format("Execution <{0}> \"{1}\"", user.name, command));
			} catch(Exception ex) {
				IOClass.Error("\n&C0" + ex.ToString());
				return;
			}
		}
	}
}