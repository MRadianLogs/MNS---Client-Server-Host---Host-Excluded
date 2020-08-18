using System;
using System.Net.Sockets;
using UnityEngine;

public class TCPData
{
    public TcpClient socket;

    private readonly int id;

    private NetworkStream stream;
    private byte[] receiveBuffer;
    private Packet receivedDataPacket;

    public TCPData(int newId)
    {
        id = newId;
    }

    public void Connect(TcpClient newSocket)
    {
        socket = newSocket;
        socket.ReceiveBufferSize = Constants.DATA_BUFFER_SIZE;
        socket.SendBufferSize = Constants.DATA_BUFFER_SIZE;

        stream = socket.GetStream();

        receivedDataPacket = new Packet();
        receiveBuffer = new byte[Constants.DATA_BUFFER_SIZE];

        stream.BeginRead(receiveBuffer, 0, Constants.DATA_BUFFER_SIZE, ReceiveCallback, null);

        ServerSend.Welcome(id, "Welcome to the server!");
    }

    public void SendData(Packet packet)
    {
        try
        {
            if (socket != null)
            {
                stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
            }
        }
        catch (Exception e)
        {
            Debug.Log($"Error sending data to player {id} via TCP: {e}");
        }
    }

    private void ReceiveCallback(IAsyncResult result)
    {
        try
        {
            int receivedDataByteLength = stream.EndRead(result);
            //If no data received, this means lost connection, so disconnect.
            if (receivedDataByteLength <= 0)
            {
                //Disconnect.
                Server.clients[id].Disconnect();
                return;
            }

            byte[] receivedData = new byte[receivedDataByteLength];
            Array.Copy(receiveBuffer, receivedData, receivedDataByteLength);

            //Handle received data.
            receivedDataPacket.Reset(HandleData(receivedData));

            //Keep reading more data from network stream.
            stream.BeginRead(receiveBuffer, 0, Constants.DATA_BUFFER_SIZE, ReceiveCallback, null);

        }
        catch (Exception e)
        {
            Debug.Log($"Error recieving TCP data from network stream: {e}");
            //Disconnect.
            Server.clients[id].Disconnect();
        }
    }

    private bool HandleData(byte[] data)
    {
        int packetLength = 0;

        receivedDataPacket.SetBytes(data);

        if (receivedDataPacket.UnreadLength() >= 4)
        {
            packetLength = receivedDataPacket.ReadInt();
            if (packetLength <= 0)
            {
                return true;
            }
        }

        while (packetLength > 0 && packetLength <= receivedDataPacket.UnreadLength())
        {
            byte[] packetBytes = receivedDataPacket.ReadBytes(packetLength);
            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet packet = new Packet(packetBytes))
                {
                    int packetId = packet.ReadInt();
                    Server.packetHandlers[packetId](id, packet);
                }
            });

            packetLength = 0;
            if (receivedDataPacket.UnreadLength() >= 4)
            {
                packetLength = receivedDataPacket.ReadInt();
                if (packetLength <= 0)
                {
                    return true;
                }
            }
        }

        if (packetLength <= 1)
        {
            return true;
        }

        return false;
    }

    public void Disconnect()
    {
        socket.Close();
        stream = null;
        receivedDataPacket = null;
        receiveBuffer = null;
        socket = null;
    }
}
