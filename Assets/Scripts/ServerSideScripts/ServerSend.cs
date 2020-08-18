
public class ServerSend
{
    public static void Welcome(int clientDest, string msg)
    {
        using (Packet packet = new Packet((int)ServerPackets.welcome))
        {
            packet.Write(msg);
            packet.Write(clientDest);

            SendTCPData(clientDest, packet);
        }
    }

    private static void SendTCPData(int clientDest, Packet packet)
    {
        TCPData destClientTCPData = Server.clients[clientDest].GetTCPData();
        if (destClientTCPData != null)
        {
            packet.WriteLength();
            destClientTCPData.SendData(packet);
        }
    }

    private static void SendTCPDataToAll(Packet packet)
    {
        packet.WriteLength();
        for (int i = 1; i <= Server.maxNumPlayers; i++)
        {
            TCPData destClientTCPData = Server.clients[i].GetTCPData();
            if (destClientTCPData != null)
            {
                destClientTCPData.SendData(packet);
            }
        }
    }

    private static void SendTCPDataToAll(int clientExceptionNum, Packet packet)
    {
        packet.WriteLength();
        for (int i = 1; i <= Server.maxNumPlayers; i++)
        {
            if (i != clientExceptionNum)
            {
                TCPData destClientTCPData = Server.clients[i].GetTCPData();
                if (destClientTCPData != null)
                {
                    destClientTCPData.SendData(packet);
                }
            }
        }
    }

    private static void SendUDPData(int clientDest, Packet packet)
    {
        UDPData destClientUDPData = Server.clients[clientDest].GetUDPData();
        if (destClientUDPData != null)
        {
            packet.WriteLength();
            destClientUDPData.SendData(packet);
        }
    }

    private static void SendUDPDataToAll(Packet packet)
    {
        packet.WriteLength();
        for (int i = 1; i <= Server.maxNumPlayers; i++)
        {
            UDPData destClientUDPData = Server.clients[i].GetUDPData();
            if (destClientUDPData != null)
            {
                destClientUDPData.SendData(packet);
            }
        }
    }

    private static void SendUDPDataToAll(int clientExceptionNum, Packet packet)
    {
        packet.WriteLength();
        for (int i = 1; i <= Server.maxNumPlayers; i++)
        {
            if (i != clientExceptionNum)
            {
                UDPData destClientUDPData = Server.clients[i].GetUDPData();
                if (destClientUDPData != null)
                {
                    destClientUDPData.SendData(packet);
                }
            }
        }
    }

    public static void UDPTest(int clientDestID)
    {
        using (Packet packet = new Packet((int)ServerPackets.udpTest))
        {
            packet.Write("A test packet for UDP.");

            SendUDPData(clientDestID, packet);
        }
    }

    public static void SpawnPlayer(int clientDest, Player playerToSpawn)
    {
        using (Packet packet = new Packet((int)ServerPackets.spawnPlayer))
        {
            packet.Write(playerToSpawn.id);
            packet.Write(playerToSpawn.username);
            packet.Write(playerToSpawn.transform.position);
            packet.Write(playerToSpawn.transform.rotation);

            SendTCPData(clientDest, packet); //Using TCP so we dont risk packet getting lost.
        }
    }

    public static void PlayerPosition(Player player)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerPosition))
        {
            packet.Write(player.id);
            packet.Write(player.transform.position);

            SendUDPDataToAll(packet);
        }
    }

    public static void PlayerRotation(Player player)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerRotation))
        {
            packet.Write(player.id);
            packet.Write(player.transform.rotation);

            SendUDPDataToAll(player.id, packet);
        }
    }
}
