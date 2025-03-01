using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void LoadWorld()
    {
        SceneManager.LoadScene("World");
    }
    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }
    public void LoadLobby()
    {
        SceneManager.LoadScene("Lobby");
    }
}
