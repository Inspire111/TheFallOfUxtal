using UnityEngine;
using UnityEngine.SceneManagement;

public class ActivateOnFirstLoad : MonoBehaviour
{
    [Tooltip("The GameObject to activate on the first load of this scene.")]
    public GameObject target;

    public GameObject music;

    void Start()
    {
        // Ask the tracker how many times this scene has been loaded so far.
        int loadCount = SceneLoadTracker.Instance.GetCurrentSceneLoadCount();

        // Only activate if this is the first time (i.e. loadCount == 1)
        if (loadCount == 1)
        {
            if (target != null)
                target.SetActive(true);
        }
        else
        {
            // Otherwise ensure it stays off
            if (target != null)
                target.SetActive(false);

            music.SetActive(true);
        }
    }
}
