using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace PodBoxServer
{

    public class Packet
    {
        public static uint IDCounter = 0;
        private string req;
        private string resp;
        private uint reqLen;
        private uint resLen;
        private uint uid;

        public string Request
        {
            get { return req; }
        }
        public string Response
        {
            get { return resp; }
        }
        public uint UID
        {
            get { return uid; }
        }
        public uint RequestLength
        {
            get { return reqLen; }
        }
        public uint ResponseLength
        {
            get { return resLen; }
        }
        public Packet()
        {
            string s = "Dummy Request";
            req = s;
            resp = "";
            reqLen = (uint)s.Length;
            resLen = 0;
            uid = IDCounter++;
        }
        public void RequestPacket(string msg)
        {
            req = msg;
            reqLen = (uint)msg.Length;
            resp = ""; resLen = 0;
            uid = IDCounter++;
        }
        public void AddResponse(string msg)
        {
            resp = msg;
            resLen = (uint)msg.Length;

        }
        public Packet(byte[] data)
        {
            int counter = 2;
            reqLen = BitConverter.ToUInt16(data, 0);
            req = Encoding.ASCII.GetString(data, counter, (int)reqLen);
            counter += (int)reqLen;

            resLen = BitConverter.ToUInt16(data, counter);
            counter += 2;
            resp = Encoding.ASCII.GetString(data, counter, (int)resLen);
            counter += (int)resLen;

            counter += 2;
            uid = BitConverter.ToUInt16(data, counter);

        }


        public void GetBytes(ref List<byte> packetdata)
        {

            byte[] len = BitConverter.GetBytes((uint)reqLen);
            packetdata.Add(len[0]);
            packetdata.Add(len[1]);


            for (int i = 0; i < req.Length; i++)
            {
                byte[] len2 = BitConverter.GetBytes(req[i]);
                packetdata.Add(len2[0]);

            }


            byte[] len1 = BitConverter.GetBytes((uint)resLen);
            packetdata.Add(len1[0]);
            packetdata.Add(len1[1]);


            for (int i = 0; i < resp.Length; i++)
            {
                byte[] len2 = BitConverter.GetBytes(resp[i]);
                packetdata.Add(len2[0]);

            }

            byte[] len4 = BitConverter.GetBytes((uint)uid);
            packetdata.Add(len[0]);
            packetdata.Add(len[1]);


        }

    }


    public class PacketProcessing
    {
        private  List<Packet> packets = new List<Packet>(16);
        private  int nextPacketPos = 0;

        public  void AddPacket(Packet p)
        {
            packets.Add(p);
        }

        public  Packet GetNextPacket()
        {
            if (nextPacketPos > packets.Count - 1)
                return null;
            else
            {
                Packet packet = packets[nextPacketPos];
                nextPacketPos++;
                return packet;
            }
        }
    }
}
