using System;
using System.Net.Sockets;

namespace SCTPServer {
    public static partial class ServerClass {
        public static void Welcome(ref Client c) {
			string msgs = "~";
			msgs += string.Format("Online: {0}/{1}\n", clients.Count, maxClientLimit);
			if(clients.Count == 0)
				msgs += "Empty";
			foreach(Client oc in clients)
				msgs += "&" + oc.namecolor + oc.name + "  ";
			if(recentBroadcastBuffer.Count != 0) {
				msgs += "\n";
				msgs += "&80--------< HISTORY >--------\n";
				foreach(string str in recentBroadcastBuffer)
					msgs += "&70 " + str + "\n";
				msgs += "&80--------< HISTORY >--------";
			}
			if(c.muted) {
				if(c.muteTime > DateTime.Now) {
					msgs += string.Format("\n&c0You are muted to {0}.", c.muteTime.ToString());
					c.timerThread.Start();
				} else {
					for(int i = 0; i < IRC.users.Count; i++) {
						if(IRC.users[i].name == c.name) {
							IRC.users[i].Unmute();
							IRC.users[i].WriteFile();
						}
					}
					c.muted = false;
					c.muteTime = DateTime.UnixEpoch;
					msgs += string.Format("\n&e0You are unmuted now.");
				}
			}
			c.Send(msgs);
		}
        public static int ProcIncomingLink(ref Socket client) {
			int recvLength; string message;
			// recvLength = client.Receive(listenerRecvBuffer);
			// message = Encoding.UTF8.GetString(listenerRecvBuffer, 0, recvLength);
			try {
				recvLength = client.Receive(listenerRecvBuffer);
				message = Conv.ToStr(listenerRecvBuffer, recvLength);
				string[] args = message.Split(" ");
				// IOClass.Log(message);
				if(message == "HELLO") {
					Conv.Send(ref client, "!HI");
					IOClass.Log(string.Format("Connection from {0} established.", client.RemoteEndPoint));
					while(true) {
						// Wait until "BYE"
						recvLength = client.Receive(listenerRecvBuffer);
						message = Conv.ToStr(listenerRecvBuffer, recvLength).Trim();
						args = message.Split(" ");
						switch(args[0]) {
							case "INFO": {
								Conv.Send(ref client, 
								IRC.name + "\n" +
								IRC.description + "\n" +
								clients.Count.ToString() + "/" + maxClientLimit.ToString() + "\n"
								);
								break;
							}
							case "REG": {
								if(!allowRegister) {
									client.Send(Conv.ToByte("!CLOSE Register not allowed"));
									IOClass.Log(string.Format("User from {0} failed to register.", client.RemoteEndPoint));
									return 0;
								}
								if(args.Length != 3) {
									client.Send(Conv.ToByte("!CLOSE Username and password required"));
									IOClass.Log(string.Format("User from {0} failed to register.", client.RemoteEndPoint));
									return 0;
								}
								foreach(IRC.User u in IRC.users) {
									if(u.name == args[1]) {
										client.Send(Conv.ToByte("!CLOSE User already exist"));
									IOClass.Log(string.Format("User from {0} failed to register. User already exist.", client.RemoteEndPoint));
									return 0;
									}
								}
								IRC.users.Add(new IRC.User(args[1], args[2], false));
								Client c = new Client(args[1], client, false);
								c.BeginListen();
								Welcome(ref c);
								Broadcast(string.Format("User \"{0}\" joined the server.", c.name));
								clients.Add(c);
								IOClass.Log(string.Format("User from {0} connected.", c.name));
								return 1;
							}
							case "AUTH": {
								if(args.Length == 1) {
									if(clients.Count >= maxClientLimit) {
										client.Send(Conv.ToByte("!CLOSE Server is full"));
										IOClass.Log(string.Format("User from {0} failed to connect. Server is full.", client.RemoteEndPoint));
										return 0;
									}
									if(allowAnonymous) {
										Client c = new Client(client.RemoteEndPoint.ToString(), client, false);
										c.Send("!PASS " + IRC.welcome);
										c.BeginListen();
										Welcome(ref c);

										Broadcast(string.Format("User \"{0}\" joined the server.", c.name));
										clients.Add(c);
										IOClass.Log(string.Format("User {0} connected.", c.name));
										return 1;
									} else {
										client.Send(Conv.ToByte("!CLOSE Anonymous not allowed"));
										IOClass.Log(string.Format("User from {0} failed to connect. Anonymous forbidden.", client.RemoteEndPoint));
										return 0;
									}
								}
								string name = args[1];
								if(clients.Count >= maxClientLimit) {
									client.Send(Conv.ToByte("!CLOSE Server is full"));
									IOClass.Log(string.Format("User \"{0}\" from {1} failed to connect. Server is full.", name, client.RemoteEndPoint));
									return 0;
								}
								foreach(IRC.User u in IRC.users) {
									if(u.name == name) {
										if(u.password == "") {
											Client c = new Client(client, u);
											c.Send("!PASS " + IRC.welcome);
											c.BeginListen();
											Welcome(ref c);
											Broadcast(string.Format("User \"{0}\" joined the server.", c.name));
											clients.Add(c);
											IOClass.Log(string.Format("User \"{0}\" from {1} connected.", c.name, client.RemoteEndPoint));
											return 1;
										} else {
											if(args.Length == 2) {
												client.Send(Conv.ToByte("!CLOSE Password required"));
												IOClass.Log(string.Format("User \"{0}\" from {1} failed to connect. Passworld required.", name, client.RemoteEndPoint));
												return 0;
											} else {
												if(args[2] == u.password) {
													foreach(Client cs in clients) {
														if(cs.name == u.name) {
															client.Send(Conv.ToByte("!CLOSE User already login"));
															IOClass.Log(string.Format("User \"{0}\" from {1} failed to connect. Already login.", name, client.RemoteEndPoint));
															return 0;
														}
													}
													// client.Send(Conv.ToByte("!PASS " + IRC.welcome));
													Client c = new Client(client, u);
													c.Send("!PASS " + IRC.welcome);
													c.BeginListen();
													Welcome(ref c);
													Broadcast(string.Format("User \"{0}\" joined the server.", c.name));
													clients.Add(c);
													IOClass.Log(string.Format("User \"{0}\" from {1} connected.", c.name, client.RemoteEndPoint));
													return 1;
												} else {
													client.Send(Conv.ToByte("!CLOSE Wrong password"));
													IOClass.Log(string.Format("User \"{0}\" from {1} failed to connect. Wrong password.", name, client.RemoteEndPoint));
													return 0;
												}
											}
										}
									}
								}
								client.Send(Conv.ToByte("!CLOSE No user"));
								IOClass.Log(string.Format("User \"{0}\" from {1} failed to connect. No such a user.", args[1], client.RemoteEndPoint));
								return 0;
							}
							case "BYE": {
								Conv.Send(ref client, "!CLOSE BYE");
								client.Disconnect(false);
								return 0;
							}
						}
					}
				}
			} catch(Exception ex) {
				IOClass.Error("\n&C0" + ex.ToString());
				return -1;
			}
			IOClass.Error("&C0Function reached unexpected boundary.");
			return -1;
		}
    }
}