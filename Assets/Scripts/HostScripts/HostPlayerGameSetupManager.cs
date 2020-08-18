using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostPlayerGameSetupManager : MonoBehaviour
{
    public static HostPlayerGameSetupManager instance;

    AsyncOperation asyncLoadLevel;

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

        StartCoroutine(LoadSinglePlayer());
    }

    IEnumerator LoadSinglePlayer()
    {
        //Change scene to game scene.
        asyncLoadLevel = SceneManager.LoadSceneAsync("GameScene");
        while(!asyncLoadLevel.isDone) //Wait until loading done.
        {
            yield return null;
        }
        //Spawn player after loaded.
        GameManager.instance.SpawnServersideHostPlayer(0, "Host", new Vector3(0, 5, 0), Quaternion.identity);
    }
}
