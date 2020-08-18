using System.Net;

public class UDPData
{
    public IPEndPoint endPoint;

    private int id;

    public UDPData(int newId)
    {
        id = newId;
    }

    public void Connect(IPEndPoint newEndpoint)
    {
        endPoint = newEndpoint;
        //ServerSend.UDPTest(id); //Used for testing UDP.
    }

    public void SendData(Packet packet)
    {
        Server.SendUDPData(endPoint, packet);
    }

    public void HandleData(Packet receivedPacket)
    {
        int packetLength = receivedPacket.ReadInt();
        byte[] packetBytes = receivedPacket.ReadBytes(packetLength);

        ThreadManager.ExecuteOnMainThread(() =>
        {
            using (Packet packet = new Packet(packetBytes))
            {
                int packetId = packet.ReadInt();
                Server.packetHandlers[packetId](id, packet);
            }
        });
    }

    public void Disconnect()
    {
        endPoint = null;
    }
}
