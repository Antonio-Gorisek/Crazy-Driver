using UnityEngine;
using UnityEngine.UI;

public class VehicleHealth : MonoBehaviour
{
    [SerializeField] protected Image _imageEndGame;
    [SerializeField] private Slider _healthSlider;
    private float _currentHealth = 100;
    private bool vehicleDestroyed;

    public float CurrentHealth
    {
        get => _currentHealth;
        private set
        {
            _currentHealth = value;

            _healthSlider.value = _currentHealth;

            OnHealthChange();
        }
    }

    private void Awake() => _currentHealth = _healthSlider.maxValue;

    private void OnHealthChange()
    {
        if (vehicleDestroyed) 
            return;

        CameraShaker.Instance.ShakeOnce(4f, 4f, .1f, 1f);
        if (_currentHealth <= 0)
        {
            BreakGlassEffect.Instance.BreakForce(160);
            OnVehicleDestroyed();
        }

        Debug.Log($"Damage taken, current health {_currentHealth}");
    }

    public void TakeDamage(int damage)
    {
        if (damage < 0) return;
        CurrentHealth -= damage;
    }

    private void OnVehicleDestroyed()
    {
        if (vehicleDestroyed)
            return;

        Debug.Log("Vehicle is destroyed!");
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        GetComponent<Vehicle>().VehicleCrashed();
        BreakGlassEffect.Instance.onVehicleDestroyed.Invoke();
        AudioManager.PlayFromResources("smoke", 1, 1);
        AudioManager.PlayFromResources("gameover", 0.5f, 1);
        AudioManager.StopAudioClip("Background");
        AudioManager.StopAudioClip("CarEngine");
        GameManager.Instance.GameOver();
        vehicleDestroyed = true;
        _imageEndGame.sprite = Resources.Load<Sprite>("Images/Totalled");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Vehicle")
            || collision.collider.gameObject.layer == LayerMask.NameToLayer("Barrier"))
        {
            if (vehicleDestroyed)
                return;
            CurrentHealth -= collision.gameObject.GetComponent<Damage>().value;
            AudioManager.PlayFromResources("crash", 0.3f, Random.Range(0.8f, 1.2f));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Vehicle")
            || other.gameObject.layer == LayerMask.NameToLayer("Barrier"))
        {
            if (vehicleDestroyed)
                return;
            CurrentHealth -= other.gameObject.GetComponent<Damage>().value;
            AudioManager.PlayFromResources("punch", 0.3f, Random.Range(0.6f, 0.9f));
        }
    }
}