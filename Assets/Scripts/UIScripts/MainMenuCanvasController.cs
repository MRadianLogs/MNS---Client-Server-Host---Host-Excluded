using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuCanvasController : MonoBehaviour
{
    [SerializeField] private GameObject StartScreenPanel = null;

    [SerializeField] private GameObject JoinGamePanel = null;
    [SerializeField] private Text serverIpInputText = null;

    public void StartGameButtonPressed()
    {
        //Change scene to Single Player Scene.
        SceneManager.LoadScene("SinglePlayerHostScene");
    }

    public void GoToJoinGameMenu()
    {
        StartScreenPanel.SetActive(false);

        JoinGamePanel.SetActive(true);
    }
    public void JoinGameButtonPressed()
    {
        //Attempt to join server with inputted IP.
        NetworkSetupData.instance.serverIP = serverIpInputText.text;
        SceneManager.LoadScene("MultiplayerClientJoinScene");
    }
    public void GoToStartScreenMenu()
    {
        JoinGamePanel.SetActive(false);

        StartScreenPanel.SetActive(true);
    }
}
