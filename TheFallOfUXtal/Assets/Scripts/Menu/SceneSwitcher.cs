using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneSwitcher : MonoBehaviour
{
    [Tooltip("The tutorial GameObject to show before loading the World scene.")]
    public GameObject tuto;

    public GameObject music;

    [Tooltip("Seconds to wait while the tutorial is active.")]
    public float waitTime = 3f;

    /// <summary>
    /// Show the tutorial, wait, then load the World scene.
    /// </summary>
    public void LoadWorld()
    {
        StartCoroutine(ShowTutoAndLoadWorld());
    }

    private IEnumerator ShowTutoAndLoadWorld()
    {
        // desactive la musique du menu
        music.SetActive(false);

        // 1) Activate the tutorial
        if (tuto != null)
            tuto.SetActive(true);

        // 2) Wait for the specified duration
        yield return new WaitForSeconds(waitTime);

        // 3) Deactivate the tutorial
        if (tuto != null)
            tuto.SetActive(false);

        // reactive la musique du menu
        music.SetActive(true);


        // 4) Load the "World" scene
        SceneManager.LoadScene("World");
    }

    /// <summary>
    /// Immediate load of the Menu scene.
    /// </summary>
    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    /// <summary>
    /// Immediate load of the Lobby scene.
    /// </summary>
    public void LoadLobby()
    {
        SceneManager.LoadScene("Lobby");
    }
}
