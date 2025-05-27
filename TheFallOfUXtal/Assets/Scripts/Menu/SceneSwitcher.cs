using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneSwitcher : MonoBehaviour
{
    [Tooltip("The tutorial GameObject to show before loading the World scene.")]
    public GameObject tuto;

    [Tooltip("Seconds to wait while the tutorial is active (can be skipped with left click).")]
    public float waitTime = 3f;

    public GameObject music;

    /// <summary>
    /// Show the tutorial (or skip on click), then load the World scene.
    /// </summary>
    public void LoadWorld()
    {
        StartCoroutine(ShowTutoAndLoadWorld());
    }

    private IEnumerator ShowTutoAndLoadWorld()
    {
        // get rid of music
        music.SetActive(false);

        // 1) Activate the tutorial
        if (tuto != null)
            tuto.SetActive(true);

        // 2) Wait for either the elapsed time or a left-click
        float elapsed = 0f;
        while (elapsed < waitTime)
        {
            // If left mouse button clicked, break out early
            if (Input.GetMouseButtonDown(0))
                break;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 3) Deactivate the tutorial
        if (tuto != null)
            tuto.SetActive(false);

        // Re-activate music
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
