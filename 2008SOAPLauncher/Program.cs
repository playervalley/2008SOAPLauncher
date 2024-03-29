using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace _2008SOAPLauncher
{
    class Program
    {
        private const string Client = "Roblox.exe";
        private const string Server = "RCCService.exe";
        private static readonly string CurrentPath = Environment.CurrentDirectory + "\\data\\clients\\2008E\\Player\\";
        private const int StartPort = 60000;

        static void Error(string reason)
        {
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("ERROR");
            Console.ResetColor();
            Console.Write("]: ");
            Console.WriteLine(reason);
            Thread.Sleep(2000);
            Environment.Exit(0);
        }

        static void Info(string message)
        {
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("INFO");
            Console.ResetColor();
            Console.Write("]: ");
            Console.WriteLine(message);
        }

        static bool PortIsAvailable(int port)
        {
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

            foreach (TcpConnectionInformation tcpi in tcpConnInfoArray)
            {
                if (tcpi.LocalEndPoint.Port == port)
                {
                    return false;
                }
            }
            return true;
        }

        static string DeleteLines(string s, int linesToRemove)
        {
            return s.Split(Environment.NewLine.ToCharArray(), 
                linesToRemove + 1
            ).Skip(linesToRemove).FirstOrDefault();
        }

        static void Main(string[] args)
        {
            Console.Write("Detected function: ");
            if (args.Length > 0)
            {
                if (args[0].StartsWith("http://www.roblox.com/game/join.ashx?UserName="))
                {
                    Console.WriteLine("Join");
                    if (!File.Exists(CurrentPath + Client))
                    {
                        Error("Client does not exist! Aborting...");
                    }

                    Info("Joining server...");
                    Process.Start(CurrentPath + Client, $"-script \"loadfile('{args[0]}')()\"");
                    Thread.Sleep(2000);
                    Environment.Exit(1);
                }
                else
                {
                    Console.WriteLine("Host");
                    if (!File.Exists(CurrentPath + Server))
                    {
                        Error("Server does not exist! Aborting...");
                    }

                    if (!Directory.Exists(Environment.GetEnvironmentVariable("programdata") + "\\Roblox\\content"))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(Environment.GetEnvironmentVariable("programdata"));
                        Console.WriteLine("\\Roblox\\content directory doesn't exist. You might not be able to spawn in your character as a result.");
                        Console.ResetColor();
                        Thread.Sleep(2000);
                    }

                    Info("Checking for open ports...");
                    int Port = 0;
                    for (int i = 0; i <= 100; i++)
                    {
                        int TryingPort = StartPort + i;
                        if (PortIsAvailable(TryingPort))
                        {
                            Console.Write($"Port {TryingPort} is open. ");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("This will be used for SOAP.");
                            Console.ResetColor();
                            Port = TryingPort;
                            break;
                        }
                        else
                        {
                            Console.WriteLine($"Port {TryingPort} is closed.");
                        }
                    }
                    if (Port == 0)
                    {
                        Error("No open ports have been found for SOAP!");
                    }

                    Info("Launching server... Please wait...");
                    Process.Start(CurrentPath + Server, $"-verbose -console {Port}");
                    while (!PortIsAvailable(Port)) { }

                    using (WebClient client = new WebClient())
                    {
                        Info("Fetching GameServer script...");
                        client.Headers.Add("User-Agent", "Roblox/WinHttp");
                        string GameServerScript = DeleteLines(new StreamReader(client.OpenRead(args[0])).ReadToEnd(), 2); // strip the signature out

                        Info("Sending SOAP response...");
                        client.Headers.Remove("User-Agent");
                        client.Headers.Add("Content-Type", "text/xml; charset=utf-8");
                        client.Headers.Add("SOAPAction", "http://roblox.com/OpenJob");
                        client.UploadData(
                            $"http://127.0.0.1:{Port}",
                            Encoding.ASCII.GetBytes($"<?xml version=\"1.0\" encoding=\"UTF-8\"?> <SOAP-ENV:Envelope xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:SOAP-ENC=\"http://schemas.xmlsoap.org/soap/encoding/\" xmlns:ns1=\"http://roblox.com/\" xmlns:ns2=\"http://roblox.com/RCCServiceSoap\" xmlns:ns3=\"http://roblox.com/RCCServiceSoap12\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"> <SOAP-ENV:Body> <ns1:OpenJob> <ns1:job> <ns1:id>Retro</ns1:id> <ns1:expirationInSeconds>1234567890</ns1:expirationInSeconds> <ns1:category>1</ns1:category> <ns1:cores>1</ns1:cores> </ns1:job> <ns1:script> <ns1:name>test</ns1:name> <ns1:script><![CDATA[{GameServerScript}]]></ns1:script> </ns1:script> </ns1:OpenJob> </SOAP-ENV:Body> </SOAP-ENV:Envelope>")
                        );
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Succesfully started the server!");
                    Console.ResetColor();
                    Console.WriteLine("If there are errors present, the GameServer port is closed, or the map is not compatible.");
                    Thread.Sleep(3000);
                    Environment.Exit(1);
                }
            }
            else
            {
                Console.WriteLine("Studio");
                if (!File.Exists(CurrentPath + Client))
                {
                    Error("Client does not exist! Aborting...");
                }

                Info("Launching client... Please wait...");
                Process.Start(CurrentPath + Client);
                Thread.Sleep(2000);
                Environment.Exit(1);
            }
        }
    }
}
