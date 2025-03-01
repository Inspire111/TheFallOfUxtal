using UnityEngine;

public class temp : MonoBehaviour
{
    void Awake()
    {
        if (gameObject.scene.rootCount == 1) // Objects in DDOL have rootCount = 1
        {
            Debug.LogError(gameObject.name + " is being moved to DontDestroyOnLoad!", this);
        }
    }

}
