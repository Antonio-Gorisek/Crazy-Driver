using UnityEngine;

/// <summary>
/// Generic Singleton base class for MonoBehaviour-derived classes.
/// Ensures that there is only one instance of the class and provides global access to it.
/// 
/// View Script Documentation V
/// https://docs.google.com/document/d/17evrh3IO_rsN1MzmVnnU8DtzKGmvkFuRdrEiMxqhon4/edit#heading=h.4c3mifmwou4d
/// </summary>
/// <typeparam name="T">The type of the Singleton class which must derive from MonoBehaviour.</typeparam>
public class Singleton<T> : MonoBehaviour where T : Component
{
    // The single instance of the Singleton class
    private static T instance;

    /// <summary>
    /// Provides global access to the Singleton instance.
    /// Creates a new instance if one does not already exist.
    /// </summary>
    public static T Instance
    {
        get
        {
            // Check if the instance is not set
            if (instance == null)
            {
                // Try to find an existing instance in the scene
                instance = FindObjectOfType<T>();

                // If no instance is found, create a new GameObject and attach the component
                if (instance == null)
                {
                    GameObject gameObject = new GameObject("SingletonController");
                    instance = gameObject.AddComponent<T>();
                }
            }
            return instance;
        }
    }

    /// <summary>
    /// Ensures that there is only one instance of this Singleton class.
    /// Destroys any duplicate instances found during initialization.
    /// </summary>
    private void Awake()
    {
        // Check if the instance is not set
        if (instance == null)
        {
            // Set the current instance as the Singleton instance
            instance = this as T;
        }
        else
        {
            // Destroy any additional instances
            Destroy(gameObject);
        }
    }
}
