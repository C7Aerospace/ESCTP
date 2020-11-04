using System;

using static System.Console;
namespace SCTPServer {
	class Program {
		static void Main(string[] args)
		{
			CancelKeyPress += delegate { ServerClass.ExitException("Forced interruption by administrator."); };
			
			Console.Title = "ESCTP Server";
			IOClass.Log("ESCTP Server &e0Indev 0.3.2");
			IOClass.Warn("This ESCTP Server is still in develop.");
			ServerClass.BeginListen();
		}
	}
}
