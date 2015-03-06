using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace portListener
{
    class PereodicPortListener
    {
        private static int portNumber = 5000;
        private static int listenPeriod = 30; //miliseconds
        private static List<Socket> clients = new List<Socket>();
        private static Thread listening_thread;
        private static TcpListener listener;
        private static Func<Socket, int> parseFunc = null;

        public PereodicPortListener(int pNumber, int lPer, Func<Socket, int> pFunc)
        {
            portNumber = pNumber;
            listenPeriod = lPer;
            parseFunc = pFunc;

            listening_thread = new Thread(new ThreadStart(ListeningThread));
            listening_thread.Start();
        }

        void ListeningThread()
        {
            listener = new TcpListener(IPAddress.Any, portNumber);

            try
            {
                listener.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine("couldn't bind to port " + portNumber + " -> " + e.Message);
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
                        if (parseFunc != null) parseFunc(sock);

                Thread.Sleep(30);
            }
        }
    }
}
