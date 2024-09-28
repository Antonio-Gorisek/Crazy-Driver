using UnityEngine;

/// <summary>
/// This script provides a visual effect by throwing
/// objects upward when they collide with the vehicle.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class ThrowObject : MonoBehaviour
{
    [SerializeField] private float throwForce = 500f;
    [SerializeField] private float rotationForce = 100f;
    private Rigidbody _rigidbody;

    private void Awake() => _rigidbody = GetComponent<Rigidbody>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            ThrowAndRotate();
        }
    }

    /// <summary>
    /// Throws the object upwards and applies random rotation.
    /// </summary>
    public void ThrowAndRotate()
    {
        _rigidbody.useGravity = true;
        Vector3 forceDirection = (transform.up + transform.forward).normalized; // Calculate the throw direction
        _rigidbody.AddForce(forceDirection * throwForce); // Apply the throw force

        // Generate random torque for rotation
        Vector3 randomTorque = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        _rigidbody.AddTorque(randomTorque * rotationForce); // Apply the rotation torque

        Debug.Log($"Object {transform.name} has been hit and thrown into the air.");
        Destroy(gameObject, 2);
    }
}
