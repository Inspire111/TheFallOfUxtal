using UnityEngine;
using UnityEngine.SceneManagement; // Import Scene Management

public class SceneSwitcher : MonoBehaviour
{
    public string sceneToLoad = "World"; // The name of the scene to load

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoad); // Switch to the specified scene
    }
}
