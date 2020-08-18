using UnityEngine;

public class ServerJoinedClient : ServerClient
{
    public TCPData tcp;
    public UDPData udp;

    public ServerJoinedClient(int newClientId, string newClientUsername) : base (newClientId, newClientUsername)
    {
        id = newClientId;
        username = newClientUsername;

        tcp = new TCPData(id);
        udp = new UDPData(id);
    }

    public override TCPData GetTCPData()
    {
        return tcp;
    }

    public override UDPData GetUDPData()
    {
        return udp;
    }

    /// <summary>
    /// This method spawns a new player into the game on the serverside, before sending a packet to spawn the player on the clientside. 
    /// </summary>
    /// <param name="newPlayerName"></param>
    public override void SendPlayerIntoGame(string newPlayerName)
    {
        //Spawn player on serverside.
        player = GameManager.instance.SpawnServersideJoinedClientPlayer(id, newPlayerName, new Vector3(0, 5, 0), Quaternion.identity).GetComponentInChildren<Player>();

        //TODO: Double check this logic.

        //Spawn all other connected clients players on the clientside(before spawning new player in. Essentially preparing the world).
        foreach (ServerClient client in Server.clients.Values)
        {
            if (client.player != null)
            {
                if (client.id != id)
                {
                    ServerSend.SpawnPlayer(id, client.player);
                }
            }
        }

        //Spawn new player for all other connected clients on the clientside.
        foreach (ServerClient client in Server.clients.Values)
        {
            if (client.player != null) //If that client has their player already spawned in,
            {

                ServerSend.SpawnPlayer(client.id, player);

            }
        }
    }

    public override void Disconnect()
    {
        Debug.Log($"{tcp.socket.Client.RemoteEndPoint} has disconnected.");

        ThreadManager.ExecuteOnMainThread(() =>
        {
            if (player != null)
            {
                UnityEngine.Object.Destroy(player.gameObject);
                player = null;
            }
        });

        tcp.Disconnect();
        udp.Disconnect();
    }

}
