using UnityEngine;

public class EventSystemManager : MonoBehaviour
{
    public static EventSystemManager instance;

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
    }
}
