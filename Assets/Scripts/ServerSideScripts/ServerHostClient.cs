
public class ServerHostClient : ServerClient
{
    
    public ServerHostClient(int newClientId, string newClientUsername, Player hostPlayer) : base (newClientId, newClientUsername)
    {
        id = newClientId;
        username = newClientUsername;
        player = hostPlayer;
    }

    public override void SendPlayerIntoGame(string newPlayerName)
    {
        //TODO: Send host into game.
        
    }

    public override void Disconnect()
    {
        //TODO: Have host disconnect.
    }

}
