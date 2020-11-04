using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace SCTPClient
{
    public static class LocalCommands
    {
        public static void ProcLocalCommands(string command)
        {
            string[] args = command.Split(' ');
            string ending = command.Substring(command.IndexOf(' ') + 1);
            switch(args[0])
            {
                case "theme":
                    {
                        IOClass.InitStyle();
                        IOClass.ColorPrint("&a0Theme reloaded.");
                        break;
                    }
                case "dns":
                    {
                        if (args.Length != 2)
                        {
                            IOClass.ColorPrint(string.Format("&c0Usage:\n&c0  dns <hostname>"));
                            IOClass.ColorPrint(string.Format("&e0  e.g. dns localhost"));
                            break;
                        }
                        try
                        {
                            IOClass.ColorPrint(string.Format("&e0Resolving &c0\"{0}\"&e0 ...", args[1]));
                            foreach (IPAddress ip in Dns.GetHostEntry(args[1]).AddressList)
                            {
                                IOClass.ColorPrint("  &f0" + ip.ToString());
                            }
                        } catch(Exception ex)
                        {
                            IOClass.ColorPrint("&c0Local: " + ex.ToString());
                            break;
                        }
                        break;
                    }
                case "exit":
                    {
                        Environment.Exit(0);
                        break;
                    }
                case "disconnect":
                    {
                        Conv.Send(ref Program.mf.socket, "bye");
                        break;
                    }
                case "register":
                    {
                        if (args.Length != 5)
                        {
                            IOClass.ColorPrint(string.Format("&c0Usage:\n&c0  register <host> <username> <password> <password>"));
                            IOClass.ColorPrint(string.Format("&e0  e.g. register 127.0.0.1:7655 user123 pwd1 pwd1"));
                            break;
                        }
                        try
                        {
                            if(args[3] != args[4])
                            {
                                IOClass.ColorPrint(string.Format("&c0Local: Password not confirmed."));
                                break;
                            }
                            string hostname;
                            int port = 7655;
                            hostname = args[1].Split(':')[0];
                            if (args[1].Split(':').Length > 1)
                                if (!int.TryParse(args[1].Split(':')[1], out port))
                                    IOClass.ColorPrint(string.Format("Local: Invalid host."));
                            IPAddress ipa = Dns.GetHostEntry(hostname).AddressList[0];
                            IPEndPoint ipe = new IPEndPoint(ipa, port);
                            Program.mf.Login(ipe, args[2], args[3], true);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            break;
                        }
                        break;
                    }
                case "connect":
                case "login":
                    {
                        if (args.Length != 4)
                        {
                            IOClass.ColorPrint(string.Format("&c0Usage:\n&c0  login <host> <username> <password>"));
                            IOClass.ColorPrint(string.Format("&e0  e.g. login 127.0.0.1:7655 admin pwd123456"));
                            break;
                        }
                        try
                        {
                            string hostname;
                            int port = 7655;
                            hostname = args[1].Split(':')[0];
                            if (args[1].Split(':').Length > 1)
                                if (!int.TryParse(args[1].Split(':')[1], out port))
                                    IOClass.ColorPrint(string.Format("Local: Invalid host."));
                            IPAddress ipa = Dns.GetHostEntry(hostname).AddressList[0];
                            IPEndPoint ipe = new IPEndPoint(ipa, port);
                            Program.mf.Login(ipe, args[2], args[3], false);
                        } catch(Exception ex)
                        {
                            MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            break;
                        }
                        break;
                    }
                case "help":
                    {
                        IOClass.ColorPrint(File.ReadAllText("localhelp"));
                        break;
                    }
                default:
                    {
                        IOClass.ColorPrint(string.Format("&c0Local: No such a command \"{0}\"", args[0]));
                        break;
                    }
            }
        }
    }
}
