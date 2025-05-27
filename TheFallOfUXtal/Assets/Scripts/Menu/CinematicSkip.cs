using UnityEngine;

public class ToggleOnClick : MonoBehaviour
{
    public GameObject sound;

    void Update()
    {
        // Detect left mouse button down
        if (Input.GetMouseButtonDown(0))
        {
            // Toggle this GameObject’s active state
            gameObject.SetActive(false);
            // Activates sound
            sound.SetActive(true);
        }
    }
}
