using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static Dictionary<int, GameObject> serversidePlayers; //Used on host serverside.
    public static Dictionary<int, PlayerManager> clientsidePlayers; //Used on clientside.

    [SerializeField] private GameObject serversideHostPlayerPrefab = null;
    [SerializeField] private GameObject serversideJoinedClientPlayerPrefab = null;
    [SerializeField] private GameObject clientsideLocalPlayerPrefab = null;
    [SerializeField] private GameObject clientsideJoinedClientPrefab = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists! Destroying object!");
            Destroy(transform.root.gameObject);
        }

        serversidePlayers = new Dictionary<int, GameObject>();
        clientsidePlayers = new Dictionary<int, PlayerManager>();
    }

    /// <summary>
    /// Spawn a locally, technically serverside, controlled non-server-interacting player.
    /// </summary>
    public GameObject SpawnServersideHostPlayer(int id, string username, Vector3 position, Quaternion rotation)
    {
        GameObject newServersideHostPlayer = Instantiate(serversideHostPlayerPrefab, position, rotation);
        //TODO: Set player id, and username.

        serversidePlayers.Add(1, newServersideHostPlayer);

        return newServersideHostPlayer;
    }

    /// <summary>
    /// Spawn a player, serverside, which represents a joined client, that is server-controlled.
    /// </summary>
    public GameObject SpawnServersideJoinedClientPlayer(int id, string username, Vector3 position, Quaternion rotation)
    {
        GameObject newServersidePlayer = Instantiate(serversideJoinedClientPlayerPrefab, position, rotation);
        Player newPlayerData = newServersidePlayer.GetComponentInChildren<Player>();
        newPlayerData.id = id;
        newPlayerData.username = username;

        serversidePlayers.Add(id , newServersidePlayer);

        return newServersidePlayer;
    }

    /// <summary>
    /// Spawn a player, clientside. If the id of the player being spawned matches the local users id, it spawns a prefab that allows them to control them. 
    /// </summary>
    public GameObject SpawnClientSidePlayer(int id, string username, Vector3 position, Quaternion rotation)
    {
        GameObject player;
        if (id == Client.instance.clientID)
        {
            player = Instantiate(clientsideLocalPlayerPrefab, position, rotation); //spawn clientside local inputted player.
        }
        else
        {
            player = Instantiate(clientsideJoinedClientPrefab, position, rotation); //spawn a clientside server controlled player.
        }

        PlayerManager playerManager = player.GetComponentInChildren<PlayerManager>();
        playerManager.id = id;
        playerManager.username = username;

        clientsidePlayers.Add(id, playerManager);
        return player;
    }
}
