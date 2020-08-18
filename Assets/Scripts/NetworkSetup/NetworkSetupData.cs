using UnityEngine;

public class NetworkSetupData : MonoBehaviour
{
    public static NetworkSetupData instance;

    public int maxNumPlayersJoin { get; set; }

    public string serverIP { get; set; }
    public string username { get; set; }


    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(transform.root.gameObject);
        }
    }
}
