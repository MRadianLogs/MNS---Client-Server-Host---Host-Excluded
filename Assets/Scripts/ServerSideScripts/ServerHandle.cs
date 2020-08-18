using UnityEngine;

public class ServerHandle
{
    public static void WelcomeReceived(int clientOrigin, Packet packet)
    {
        int clientId = packet.ReadInt();
        string username = packet.ReadString();

        Debug.Log($"{Server.clients[clientOrigin].GetTCPData().socket.Client.RemoteEndPoint} connected and is now player {clientOrigin}.");
        if (clientOrigin != clientId)
        {
            Debug.Log($"Player \"{username}\" (ID: {clientOrigin}) has assumed the wrong client ID ({clientId})!");
        }

        //Send player into game.
        //Debug.Log($"New client ID is: {clientId}");
        //Debug.Log($"Sending {Server.clients[clientId]} into the game!");
        //Server.clients[clientOrigin].SendIntoGame(username);
    }

    public static void UDPTestReceived(int clientOrigin, Packet packet)
    {
        string msg = packet.ReadString();

        Debug.Log($"Received packet via UDP: {msg}");
    }

    public static void PlayerMovement(int clientOrigin, Packet packet)
    {
        bool[] inputs = new bool[packet.ReadInt()];

        for (int i = 0; i < inputs.Length; i++)
        {
            inputs[i] = packet.ReadBool();
        }
        Quaternion playerRotation = packet.ReadQuaternion();

        Server.clients[clientOrigin].player.movementController.SetInputs(inputs, playerRotation);
    }

    public static void HandlePlayerSpawnRequest(int clientOrigin, Packet packet)
    {
        Server.clients[clientOrigin].SendPlayerIntoGame("Temp client");
    }
}
