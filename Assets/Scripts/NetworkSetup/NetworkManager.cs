using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;

    public bool serverActive = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(transform.root.gameObject);
        }
    }

    public void StartServer(int maxNumPlayers)
    {
        //QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 30;

        //Start server and add host player.
        Server.Start((maxNumPlayers + 1), 23399); //23399 is the Skype default protocall port. If somebody is trying to use that, while this server is being run, tell them to use Discord.
    }

    public void StopServer()
    {
        if (serverActive)
            Server.Stop();
    }

    private void OnApplicationQuit()
    {
        if(serverActive)
            Server.Stop();
    }
}
