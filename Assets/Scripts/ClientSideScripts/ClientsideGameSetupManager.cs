using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientsideGameSetupManager : MonoBehaviour
{
    public static ClientsideGameSetupManager instance;

    AsyncOperation asyncLoadLevel;

    //This method runs when script is loaded.
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            //Debug.Log("Instance already exists! Destroying object!");
            Destroy(transform.root.gameObject);
        }

        //StartCoroutine(LoadMultiplayer());
    }

    //This method is called before any update method is called.
    private void Start()
    {
        StartCoroutine(LoadMultiplayer());
    }

    IEnumerator LoadMultiplayer()
    {
        //Set IP from setup.
        Client.instance.ClientJoinSetupIP();
        //Attempt to join server.
        Client.instance.ConnectToServer();

        //Change scene to game scene.
        asyncLoadLevel = SceneManager.LoadSceneAsync("GameScene");
        while (!asyncLoadLevel.isDone) //Wait until loading done.
        {
            yield return null;
        }

        //Spawn player.
        ClientSend.RequestPlayerSpawn();
        //GameManager.instance.SpawnClientSidePlayer(Client.instance.clientID, "Temp Client", new Vector3(0, 5, 0), Quaternion.identity);
    }

}
