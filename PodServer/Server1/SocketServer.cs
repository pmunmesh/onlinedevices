using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using PodBoxServer;

namespace PodBoxServer
{
    public class PoDSocketServer
    {
        // Incoming data from the client.
        public PacketProcessing packetProcessing;
        public void SetPacketProcess(PacketProcessing p)
        {
            packetProcessing = p;
        }
        public string data = null;
        public bool responseReceived = false;
        public string httpResponse = "";
        public Queue<Packet> packets = new Queue<Packet>();
        public void AddRequest(Packet p)
        {
            packets.Enqueue(p);
        }
        public void SocketServerProcess()
        {
            // Data buffer for incoming data.
            byte[] bytes = new Byte[1024];

            // host running the application.
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPEndPoint localEndPoint = null;
            foreach (IPAddress ipa in ipHostInfo.AddressList)
            {
                if (ipa.ToString().Contains("."))
                {
                    IPAddress ipAddress = ipa;
                     localEndPoint = new IPEndPoint(ipAddress, 81);
                }
            }
             
            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            Socket handler = null;
            Console.WriteLine("Socket Server initiated");
            // listen for incoming connections.
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);
                Console.WriteLine("Socket Server accepting connections");
                // Start listening for connections.
                while (true)
                {
                    ;
                    // Program is suspended while waiting for an incoming connection.
                    handler = listener.Accept();
                    data = null;

                    // An incoming connection needs to be processed.
                    while (true)
                    {
                        bytes = new byte[1024];
                        int bytesRec = handler.Receive(bytes);
                        Thread.Sleep(1000);

                        Packet recPacket = new Packet(bytes);
                        //data = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        Console.WriteLine("Socket Server: Request Sent " + recPacket.Request+ "and Response:" + recPacket.Response);

                        lock (SimpleHTTPServer.thisLock)
                        {
                            packetProcessing.AddPacket(recPacket);
                            Console.WriteLine("Response for HTTP Server is: " + recPacket.Response);
                        }


                        Packet packet = new Packet();
                        //BinaryFormatter bf = new BinaryFormatter();
                        //MemoryStream ms = new MemoryStream();
                        //bf.Serialize(ms, packet);
                        byte[] msg;
                        List<byte> dataList = new List<byte>();
                        lock (SimpleHTTPServer.thisLock)
                        {
                            packet = packetProcessing.GetNextPacket(); 
                            if (packet.RequestLength > 0)
                            {
                                //dus = SimpleHTTPServer.requestString;
                               
                                packet.GetBytes(ref dataList);
                            }
                            else
                            {
                                packet = new Packet();
                                packet.GetBytes(ref dataList);
                            }
                        }

                        msg = dataList.ToArray();
                        Console.WriteLine("Server Sending data to Socket client", packet.Request);
                        // Echo the data back to the client.
                        handler.Send(msg);
                        Thread.Sleep(1000);
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            try
            {
                Console.WriteLine("Socket Server closing connection");
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Socket Server closing Error");
                Console.WriteLine(ex.Message);
            }

        }


    }
}
