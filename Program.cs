using MyGameServer.Network;

namespace MyGameServer
{
    public class Program
    {
        const int port = 4444;
        
        public void Main(string[] args)
        {
            
            var server = new Server(4444);
            server.Start();
            
            while (true) {
              server.Update();
            }
        }
    }
}
