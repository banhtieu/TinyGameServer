using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace MyGameServer.Network {
  
  public class Client {
    
    public static int GameClientId = 0;
    
    public static int PacketLength = 38;
    
    public int clientId;
    
    public TcpClient client;
    
    public Stream stream;
    
    public bool disconnected;
    
    public bool available {
      get {
        return client.Available > PacketLength;
      }
    }
    
    public Client(TcpClient client) {
      this.client = client;
      
      disconnected = false;
      clientId = ++GameClientId;
      
      stream = client.GetStream();
      
    }
  }
  
  public class Server {
    
    /// list of clients
    private List<Client> clients;
    
    private int port;
    
    private TcpListener listener;
    
    private MemoryStream stream;
    
    
    // create a server
    public Server(int port) {
      this.port = port;
      stream = new MemoryStream();
    }
    
    // the server
    public void Start() {
      
      clients = new List<Client>();
      listener = new TcpListener(IPAddress.Parse("0.0.0.0"), port);
      listener.Start();
    }
    
    
    // update the socket manager
    public void Update() {
      if (listener.Pending()) {
        var tcpClient = listener.AcceptTcpClient();
        
        var client = new Client(tcpClient);
        clients.Add(client);
        
        Console.WriteLine("Client Connected");
      }
      
      // read message from clients
      ReadIncomingMessages();
      
      RelayMessages();
      
      DeleteDisconnectedClients();
    }
    
    // delete all disconnected clients
    public void DeleteDisconnectedClients() {
      for (var i = 0; i < clients.Count; i++) {
        if (clients[i].disconnected) {
          clients.RemoveAt(i);
          Console.WriteLine("Delete at item " + i);
          i--;
        }
      }
    }
    
    // relay messages
    public void RelayMessages(){
      RelayMessage(stream.ToArray());
      
      stream = new MemoryStream();
    }
    
    public void RelayMessage(byte[] message) {
      
      foreach (var client in clients) {
        try {
          if (!client.disconnected) {
            client.stream.Write(message, 0, message.Length);
            client.stream.Flush();
          }
        } catch (Exception ex) {
          Console.WriteLine(ex.Message);
          client.disconnected = true;
        }
      }
    }
    
    // read incomming message
    public void ReadIncomingMessages() {
      foreach (var client in clients) {
        try {
          while (client.available) {
            var data = new byte[Client.PacketLength];
            client.stream.Read(data, 0, data.Length);
            // relay message
            stream.Write(data, 0, data.Length);
          }
        } catch (Exception ex) {
          Console.WriteLine(ex.Message);
          client.disconnected = true;
        }
      }
    }
    
  }
}