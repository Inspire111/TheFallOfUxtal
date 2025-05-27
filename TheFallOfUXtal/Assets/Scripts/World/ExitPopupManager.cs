using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject menu;


    public void GoBackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void Disable()
    {
        menu.SetActive(false);
    }

    public void Enable()
    {
        menu.SetActive(true);
    }
}
