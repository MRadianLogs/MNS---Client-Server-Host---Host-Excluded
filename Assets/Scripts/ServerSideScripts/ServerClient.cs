
public abstract class ServerClient
{
    public int id;
    public string username;

    public Player player;

    public ServerClient(int newClientId, string newClientUsername)
    {
        id = newClientId;
        username = newClientUsername;
    }

    public virtual TCPData GetTCPData() { return null; }

    public virtual UDPData GetUDPData() { return null; }

    public virtual void SendPlayerIntoGame(string newPlayerName) { }

    public virtual void Disconnect() { }


}