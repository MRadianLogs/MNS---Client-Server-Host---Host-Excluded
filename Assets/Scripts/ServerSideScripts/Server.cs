using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Server
{
    public static int portNum { get; private set; } //The port number the server will use for connections.

    private static TcpListener tcpListener;
    private static UdpClient udpServerClient;

    public static int maxNumPlayers { get; private set; }
    public static Dictionary<int, ServerClient> clients;
    

    public delegate void PacketHandler(int clientOrigin, Packet packet); //A data type that represents/references a method that handles a packet, and keeps track of what client sent the packet.
    public static Dictionary<int, PacketHandler> packetHandlers; //A list of packet handlers, each of which handles a specific packet type, stored with an int identifier.

    /// <summary>
    /// Start the network server with a max number of players that can connect to the server and a port num to use with the server.
    /// </summary>
    /// <param name="newMaxNumPlayers"></param>
    /// <param name="newPortNum"></param>
    public static void Start(int newMaxNumPlayers, int newPortNum)
    {
        maxNumPlayers = newMaxNumPlayers;
        portNum = newPortNum;

        Debug.Log("Starting Server...");
        InitializeServerData(); //Calls method to prepare client list slots and server packet handlers.

        tcpListener = new TcpListener(IPAddress.Any, portNum);

        tcpListener.Start();//Starts listening for incoming connections.
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null); //Waits for new connection in different thread. When new connection made, calls TCPConnectCallback method.

        udpServerClient = new UdpClient(portNum);
        udpServerClient.BeginReceive(UDPReceiveCallback, null);

        Debug.Log($"Server started on port: {portNum}.");
        NetworkManager.instance.serverActive = true;
    }

    /// <summary>
    /// Stop the server by stopping any TCP connections and UDP data packets. The server will no longer accept any connections or data.
    /// </summary>
    public static void Stop()
    {
        tcpListener.Stop();
        udpServerClient.Close();

        Debug.Log("Server stopped.");
    }

    /// <summary>
    /// The method called whenever a new connection has been started. It creates a new TCP client to represent the connecting client. It then begins waiting for new connections. 
    /// If the server has room for more clients, the new client is connected.
    /// </summary>
    /// <param name="result"></param>
    private static void TCPConnectCallback(IAsyncResult result)
    {
        TcpClient newClient = tcpListener.EndAcceptTcpClient(result);
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null); //Waits for more connections.

        Debug.Log($"Incoming connection from {newClient.Client.RemoteEndPoint}...");

        //TODO: Consider making new connections per request, instead of ahead of time. This may save the time of having to loop through all connections.
        for (int i = 2; i <= maxNumPlayers; i++) //Go through list of clients to find open spot.
        {
            TCPData clientTCPData = clients[i].GetTCPData();
            if (clientTCPData != null)
            {
                if (clientTCPData.socket == null) //If that spot is open, 
                {
                    clientTCPData.Connect(newClient); //assign the new client to that client instance/spot.
                    return;
                }
            }
        }

        Debug.Log($"{newClient.Client.RemoteEndPoint} failed to connect: Server full!"); //If this line is reached, it means the server is full and the new client was not connected.
    }

    /// <summary>
    /// This method is called when the server receives data sent using UDP. It identifies who sent the data, determines if the client who sent the data needs to be setup, then 
    /// determines whether or not to handle the data.
    /// </summary>
    /// <param name="result"></param>
    private static void UDPReceiveCallback(IAsyncResult result)
    {
        try
        {
            IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = udpServerClient.EndReceive(result, ref clientEndPoint);
            udpServerClient.BeginReceive(UDPReceiveCallback, null);

            if (data.Length < 4) //If data size is less than 4, it means not enough data was received?
            {
                return;
            }

            using (Packet packet = new Packet(data)) //Create, fill, and handle a packet from the recieved data.
            {
                int clientId = packet.ReadInt(); //Get client id from first part of packet.

                if (clientId == 0) //We dont have a client at 0, so if the clientID is 0, don't bother handling that data.
                {
                    return;
                }

                UDPData clientUDPData = clients[clientId].GetUDPData();
                if (clientUDPData != null)
                {
                    if (clientUDPData.endPoint == null) //If received data from a client that doesnt have UDP setup yet,
                    {
                        clientUDPData.Connect(clientEndPoint); //Setup/connect client UDP class. Allows it to send/receive UDP data.
                        return;
                    }

                    if (clientUDPData.endPoint.ToString() == clientEndPoint.ToString()) //If the id attached to the packet, which represents who sent the packet, has the endpoint(IP address) that matches the endpoint which sent the data, handle the data.
                    {
                        clientUDPData.HandleData(packet);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log($"Error receiving UDP data: {e}");
        }
    }

    /// <summary>
    /// A method used to send data, in the form of a packet, over to a client, specified by their endpoint.
    /// </summary>
    /// <param name="clientEndPoint"></param>
    /// <param name="packet"></param>
    public static void SendUDPData(IPEndPoint clientEndPoint, Packet packet)
    {
        try
        {
            if (clientEndPoint != null)
            {
                udpServerClient.BeginSend(packet.ToArray(), packet.Length(), clientEndPoint, null, null);
            }
        }
        catch (Exception e)
        {
            Debug.Log($"Error sending data to {clientEndPoint} via UDP: {e}");
        }
    }

    /// <summary>
    /// Setup the server's client list with the proper amount of slots, set by MaxNumPlayers, and setup the server's packet handlers.
    /// </summary>
    private static void InitializeServerData()
    {
        clients = new Dictionary<int, ServerClient>();

        // TODO: Consider doing this on a per-connection basis. When new connection recieved, add new client.
        clients.Add(1, new ServerHostClient(1, "Temp", GameManager.serversidePlayers[1].GetComponentInChildren<Player>())); //Add in host client.
        for (int i = 2; i <= maxNumPlayers; i++)
        {
            clients.Add(i, new ServerJoinedClient(i, "Temp"));
        }

        packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived},
                { (int)ClientPackets.udpTestReceived, ServerHandle.UDPTestReceived},
                { (int)ClientPackets.playerMovement, ServerHandle.PlayerMovement},
                { (int)ClientPackets.requestPlayerSpawn, ServerHandle.HandlePlayerSpawnRequest}
            };
        Debug.Log("Initialized packet handlers.");
    }
}