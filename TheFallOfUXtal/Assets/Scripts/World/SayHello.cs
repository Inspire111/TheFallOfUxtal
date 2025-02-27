using UnityEngine;

public class SayHello : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Hello");
        }
    }
}