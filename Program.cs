using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace server
{
    class Program
    {
        static List<Socket> clients = new List<Socket>();
        static Dictionary<int, int> counters = new Dictionary<int,int>();
        static int maxThreadId = 0;
        static async Task HandleClient(Socket client)
        {
            clients.Add(client);
            ProcessIncomingMessages(clients.Count, client);
        }
        static async Task ProcessIncomingMessages(int id, Socket client)
        {
            byte [] buffer = new byte[1024];
            while (true)
            {
                var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer),SocketFlags.None);
                var message = Encoding.UTF8.GetString(buffer,0,result);
                if(!counters.ContainsKey(id)) 
                    counters.Add(id,0);
                counters[id]++;
                if(maxThreadId<Thread.CurrentThread.ManagedThreadId)
                    maxThreadId = Thread.CurrentThread.ManagedThreadId;
            }
        }
        static async Task Main(string[] args)
        {
            Socket s = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5000);
            System.Timers.Timer t = new System.Timers.Timer(1000);
            t.Elapsed += (sender, o) => {
                foreach(var kvp in counters)
                {
                    System.Console.WriteLine($"Client: {kvp.Key} Messages: {kvp.Value}");
                    System.Console.WriteLine($"MaxThreadId: {maxThreadId}");
                }
            };
            t.Start();
            s.Bind(ep);
            s.Listen(1000);
            while(true)
            {
                var client = await s.AcceptAsync();
                HandleClient(client);
            }
        }
    }
}
