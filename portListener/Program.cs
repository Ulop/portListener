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
        public float[] step_count;
    }


    class Program
    {
        private static List<Socket> clients = new List<Socket>();
        
        private static TcpListener listener;
        private static SensorData sData;

        private static PereodicPortListener pListener;
        

        [DllImport("winmm.dll")]
        public static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);

        private static int ParserFunction(Socket sock)
        {
            byte[] receivedBytes = new byte[300];
            sock.Receive(receivedBytes);

            string recivedString = System.Text.Encoding.UTF8.GetString(receivedBytes);

            recivedString.Replace("\\", "");

            sData = JsonConvert.DeserializeObject<SensorData>(recivedString);
            Console.WriteLine(recivedString);
            int res = (int) sData.light[0];
           // changeVolume(res);
            return 0;
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
            sData.step_count = new float[1];

            SoundPlayer player = new SoundPlayer();
            player.SoundLocation = "ocean.wav";
            player.Play();
            pListener = new PereodicPortListener(5000, 30, ParserFunction);
        }
    }
}
