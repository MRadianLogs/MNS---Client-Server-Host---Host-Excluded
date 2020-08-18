using UnityEngine;

public class LoadPersistAnchor : MonoBehaviour
{
    private void Awake()
    {
        DontDestoryRootParent();
    }

    private void DontDestoryRootParent()
    {
        DontDestroyOnLoad(transform.root.gameObject);
    }
}
