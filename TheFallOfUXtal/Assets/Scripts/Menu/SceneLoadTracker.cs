// SceneLoadTracker.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneLoadTracker : MonoBehaviour
{
    // Singleton instance
    public static SceneLoadTracker Instance { get; private set; }

    // How many times each scene has been loaded
    private Dictionary<string, int> _loadCounts = new Dictionary<string, int>();

    void Awake()
    {
        // Ensure there’s only one tracker and it survives scene changes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // Subscribe to the sceneLoaded event
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        // Unsubscribe to prevent leaks
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Called whenever ANY scene finishes loading
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string name = scene.name;
        if (!_loadCounts.ContainsKey(name))
            _loadCounts[name] = 0;
        _loadCounts[name] += 1;
        Debug.Log($"Scene '{name}' has been loaded {_loadCounts[name]} time(s) this session.");
    }

    /// <summary>
    /// Returns how many times the given scene has been loaded since play began.
    /// </summary>
    public int GetLoadCount(string sceneName)
    {
        return _loadCounts.TryGetValue(sceneName, out var count) ? count : 0;
    }

    /// <summary>
    /// Returns how many times the currently active scene has been loaded.
    /// </summary>
    public int GetCurrentSceneLoadCount()
    {
        return GetLoadCount(SceneManager.GetActiveScene().name);
    }
}
