using MyGameServer.Network;

namespace MyGameServer
{
    public class Program
    {
        const int port = 4444;
        
        public void Main(string[] args)
        {
            
            var server = new Server(port);
            server.Start();
            
            while (true) {
              server.Update();
            }
        }
    }
}
