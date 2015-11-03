using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace PodBoxServer
{
    class Program
    {
        public static SimpleHTTPServer httpServer;
        public static void Main(String[] args)
        {



            Console.WriteLine("Starting HTTP Server. ");
            Console.WriteLine("Accepting HTTP requests at http://localhost:8081/");
            httpServer = new SimpleHTTPServer("", 8081);

            PoDServer podserver = new PoDServer();
            Thread podthread = new Thread(podserver.ServerMethod);
            podserver.ProcessPackets(httpServer.packetsServer);
            podthread.Start();
            Console.WriteLine("Press CTRL C to exit HTTPServer.");
            podthread.Join();
            httpServer._serverThread.Join();
        }
    }
    public class PoDServer
    {
        public  PoDSocketServer socketServer;
        // This method will be called when the thread is started. 
        public  void ServerMethod()
        {
            while (!PoDServer._shouldStop)
            {
                Console.WriteLine("Socket thread: working...");
                
                socketServer.SocketServerProcess();
            }
            Console.WriteLine("worker thread: terminating gracefully.");
        }
        public void ProcessPackets(PacketProcessing p)
        {
            socketServer = new PoDSocketServer();
            socketServer.SetPacketProcess(p);
        }
        public static  void RequestStop()
        {
            PoDServer._shouldStop = true;
        }
        // Volatile is used as hint to the compiler that this data 
        // member will be accessed by multiple threads. 
        public static volatile bool _shouldStop;
    }
 

}


