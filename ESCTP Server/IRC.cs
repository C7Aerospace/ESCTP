using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SCTPServer {
	public static partial class ServerClass {
		public static class IRC {
			public static string name = "Normal ESCTP Server";
			public static string description = "Test server";
			public static string welcome = "&A0Test welcome";
			public static List<User> users = new List<User>();
			public class User {
				public string name;
				public string password;
				public bool isAdmin;
				public string namecolor = "&70";
				public string description = "";
				public string contact = "";
				public bool muted;
				public DateTime muteTime;
				public User(string name, string pwd, bool isAdmin) {
					this.name = name;
					this.password = pwd;
					this.isAdmin = isAdmin;
					InitFile();
				}
				public void Unmute() {
					muted = false;
					muteTime = DateTime.UnixEpoch;
				}
				public void Mute(int time) {
					muted = true;
					muteTime = DateTime.Now.AddSeconds(time);
				}
				public void InitFile() {
					if(!File.Exists("userinfo/" + name)) {
						if(!Directory.Exists("userinfo/"))
							Directory.CreateDirectory("userinfo/");
						FileStream fs = new FileStream("userinfo/" + name, FileMode.Create);
						TextWriter tw = new StreamWriter(fs, Encoding.UTF8);
						tw.WriteLine("Description=Empty");
						tw.WriteLine("Contact=Empty");
						tw.WriteLine("Color=70");
						tw.WriteLine("Muted=false");
						tw.WriteLine("MuteTime={0}", DateTime.UnixEpoch.ToString());
						tw.Close();
						fs.Close();
					}
					string[] lines = File.ReadAllLines("userinfo/" + name);
					foreach(string str in lines) {
						if(str == "") continue;
						if(str[0] == '#') continue;
						string key, value;
						int breaker = str.IndexOf('=');
						key = str.Substring(0, breaker);
						value = str.Substring(breaker + 1);
						value = value.Replace("&n", "\n");
						switch(key) {
							case "Description": { description = value.Replace("&n", "\n"); break; }
							case "Contact": { contact = value.Replace("&n", "\n"); break; }
							case "Color": { namecolor = value.Replace("&n", "\n"); break; }
							case "Muted": { muted = bool.Parse(value); break; }
							case "MuteTime": { muteTime = DateTime.Parse(value); break; }
						}
					}
				}
				public void WriteFile() {
					string lines = "";
					lines += "Description=" + description.Replace("\n", "&n") + "\n";
					lines += "Contact=" + contact.Replace("\n", "&n") + "\n";
					lines += "Color=" + namecolor + "\n";
					lines += "Muted=" + muted.ToString() + "\n";
					lines += "MuteTime=" + muteTime.ToString() + "\n";
					File.WriteAllText("userinfo/" + name, lines);
				}
			}
			public static void Read() {
				users.Clear();
				IOClass.Log("Loading user list...");
				try {
					string[] lines = File.ReadAllLines("users");
					foreach(string str in lines) {
						if(str == "") continue;
						if(str[0] == '#') continue;
						string key, value;
						key = str.Split("=")[0];
						value = str.Split("=")[1];
						bool isAdmin = str[0] == '~';
						if(isAdmin) key = key.Substring(1);
						users.Add(new User(key, value, isAdmin));
					}
					IOClass.Log("User list loaded.");
				} catch(Exception ex) {
					ExitException(ex.ToString());
				}
			}
			public static void Write() {
				IOClass.Log("Writing user list...");
				try {
					string line = "";
					foreach(User u in users) {
						if(u.isAdmin)
							line += "~";
						line += u.name;
						line += "=";
						line += u.password;
						line += "\n";
					}
					File.WriteAllText("users", line);
					IOClass.Log("Users list writed.");
				} catch(Exception ex) {
					ExitException(ex.ToString());
				}
			}
		}
    }
}
