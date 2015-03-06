using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Xml.Linq;

namespace portListener
{
    class SensorData
    {
        public float[] gyroscope;
        public float[] light;
        public float[] rotation;
        public float[] accelerometer;
        public float[] gravity;
    }


    class Program
    {
        private static List<Socket> clients = new List<Socket>();
        private static Thread listening_thread;
        private static TcpListener listener;
        private static SensorData sData;
        

        [DllImport("winmm.dll")]
        public static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);

        private static void ListeningThread() // let's listen in another thread instead!!
        {
            int port = 5000; // change as required

            listener = new TcpListener(IPAddress.Any, port);

            try
            {
                listener.Start();
            }
            catch (Exception e) {
                Console.WriteLine("couldn't bind to port " + port + " -> " + e.Message); 
                return; 
            }

            while (true)
            {
                if (listener.Pending())
                    clients.Add(listener.AcceptSocket()); // won't block because pending was true

                foreach (Socket sock in clients)
                    if (sock.Poll(0, SelectMode.SelectError))
                        clients.Remove(sock);
                    else if (sock.Poll(0, SelectMode.SelectRead))
                        ParserFunction(sock);

                Thread.Sleep(30);
            }
        }

        private static void ParserFunction(Socket sock)
        {
            byte[] receivedBytes = new byte[255];
            sock.Receive(receivedBytes);

            string recivedString = System.Text.Encoding.UTF8.GetString(receivedBytes);

            recivedString.Replace("\\", "");

            sData = JsonConvert.DeserializeObject<SensorData>(recivedString);
            Console.WriteLine(sData.light[0]);
            int res = (int) sData.light[0];
            changeVolume(res);
        }

        private static void changeVolume(int newVolume)
        {
            int NewVolume = ((ushort.MaxValue / 25) * newVolume);
            // Set the same volume for both the left and the right channels
            uint NewVolumeAllChannels = (((uint)NewVolume & 0x0000ffff) | ((uint)NewVolume << 16));


            Console.WriteLine(waveOutSetVolume(IntPtr.Zero, NewVolumeAllChannels));
        }



        static void Main(string[] args)
        {
            sData = new SensorData();
            sData.gyroscope = new float[3];
            sData.light = new float[2];
            sData.rotation = new float[4];
            sData.accelerometer = new float[3];
            sData.gravity = new float[3];

            SoundPlayer player = new SoundPlayer();
            player.SoundLocation = "ocean.wav";
            player.Play();
            listening_thread = new Thread(new ThreadStart(ListeningThread));
            listening_thread.Start();
        }
    }
}
