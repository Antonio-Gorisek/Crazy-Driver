using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MoveForward : MonoBehaviour
{
    [SerializeField] private float _speed = 40f;
    [SerializeField] private bool startWhenPlayerIsNear = false;
    private Transform _player;
    private bool _crashedDisableMove;

    private Rigidbody rb;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
        InvokeRepeating(nameof(TryToDestroyObject), 2, 2);
    }

    private void Update() {
        if (_crashedDisableMove || GameManager.Instance.isGameOver())
            return;

        if (startWhenPlayerIsNear == true && IsPlayerNear() == false)
            return;

        // Set the linear velocity of the Rigidbody to move forward at a constant speed
        rb.linearVelocity = transform.forward * _speed;
    }

    private bool IsPlayerNear()
    {
        return Vector3.Distance(transform.position, _player.position) <= 120;
    }

    private void TryToDestroyObject()
    {
        if(Vector3.Distance(transform.position, _player.position) >= 1000)
        {
            Destroy(gameObject);
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            _crashedDisableMove = true;
        }
    }
}
