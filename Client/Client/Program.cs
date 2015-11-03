using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using PodBoxServer;

namespace Client
{
    class Program
    {
        public static void StartClient()
        {
            // Data buffer for incoming data.
            byte[] bytes = new byte[1024];

            // Connect to a remote device.
            try
            {
                // Establish the remote endpoint for the socket.
                // This example uses port 11000 on the local computer.
                IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 81);
                bool isNotExit = true;
                // Create a TCP/IP  socket.
                bool isConnected = false;
                Socket sender = null;
                string senddata = "";
                Packet packet = new Packet();
                while (isNotExit)
                {
                    // Connect the socket to the remote endpoint. Catch any errors.
                    try
                    {
                        if (!isConnected)
                        {
                            sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                            sender.Connect(remoteEP);
                            Console.WriteLine("Socket connected to {0}", sender.RemoteEndPoint.ToString());
                        }
                        
                        isConnected = true;
                        try
                        {
                        if ( packet.RequestLength >0)
                            packet.AddResponse(System.IO.File.ReadAllText(packet.Request));
                        }
                        catch(Exception ex)
                        {
                            packet.AddResponse("No Response");
                        }
                        // Encode the data string into a byte array.
                        Console.WriteLine("Data sent to Server: " + packet.Response);
                        //Packet SentPacket = new Packet(senddata,PacketType.Response);
                        List<byte> lsb = new List<byte>();
                        packet.GetBytes(ref lsb);
                        byte[] msg = lsb.ToArray();
                        //byte[] msg = Encoding.ASCII.GetBytes("http://localhost:8081/");
                        // Send the data through the socket.
                        int bytesSent = sender.Send(msg);
                        Thread.Sleep(1000);
                        // Receive the response from the remote device.
                        int bytesRec = sender.Receive(bytes);
                        packet = new Packet(bytes);
                        Thread.Sleep(1000);
                        //datarecv = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        Console.WriteLine("Packet req: {0}", packet.Request);
                        if(!isNotExit)
                        {
                        // Release the socket.
                        sender.Shutdown(SocketShutdown.Both);
                        sender.Close();
                        }

                    }
                    catch (ArgumentNullException ane)
                    {
                        Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                        isNotExit = false;
                    }
                    catch (SocketException se)
                    {
                        Console.WriteLine("Socket Server is not running. Please wait...");
                        isNotExit = true;
                        Thread.Sleep(5000);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Unexpected exception : {0}", e.ToString());
                        isNotExit = false ;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public static int Main(String[] args)
        {
          StartClient();

       
            return 0;
        }
    }

 
}
