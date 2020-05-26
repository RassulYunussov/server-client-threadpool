using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

// run from other project
namespace client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Socket s = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"),5000);
            await s.ConnectAsync(ep);
            while(true)
            {
                byte [] bytes = Encoding.UTF8.GetBytes("Hello Server!");
                await s.SendAsync(new ArraySegment<byte>(bytes),SocketFlags.None);
            }
        }
    }
}
