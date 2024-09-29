using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Vehicle : VehicleHealth
{
    public float forwardSpeed = 50f;

    [SerializeField] private float _moveSpeed = 60f;
    [SerializeField] private Transform _pcControls;
    [SerializeField] private Transform _mobileControls;
    [SerializeField] private Transform _mobileSlowdown;
    [SerializeField] private Transform _particleTurnEffect;
    [SerializeField] private UnityEvent _onPlayerFirstMove;
    [SerializeField] private GameObject _bustedAnimation;


    [SerializeField] private List<Button> _slowDownSpeedUpButtons = new List<Button>();

    private Rigidbody _rb;

    private float previousSpeed;
    private float _rightLimit = 8f;
    private float _leftLimit = 18f;
    private bool _vehicleCrashed = false;
    private bool _firstMove = false;
    private bool isAccelerating = true; // Current state
    private bool isChangingSpeed = false; // Status of speed change
    private float policeDanger = 0;

    private void Start()
    {
        isAccelerating = true;
        previousSpeed = forwardSpeed;
        AudioManager.PlayFromResources("Background", 0.1f, 1, true);
        AudioManager.PlayFromResources("CarEngine", 0.1f, 1, true);
        MapGenerator.Instance.StartMapGenerate();
        _rb = GetComponent<Rigidbody>();
        ShowControls();
    }

    private void FixedUpdate()
    {
        // Check if the vehicle is 100% damaged
        if (_vehicleCrashed)
        {
            // Gradually decrease the vehicle's forward and lateral speeds
            if (forwardSpeed > 0) { forwardSpeed -= 1; }
            if (_moveSpeed > 0) { _moveSpeed -= 1; }
        }

        MoveForward();


        float horizontalInput = CheckIsMobile.Check() ? GetScreenInput() : GetKeyboardInput();
        MoveHorizontally(horizontalInput);
        ClampPosition();
        FirstMove(horizontalInput);
    }



    public void VehicleCrashed() => _vehicleCrashed = true;

    private void MoveForward() => _rb.MovePosition(transform.position + transform.forward * forwardSpeed * Time.fixedDeltaTime);

    private void FirstMove(float horizontalInput)
    {
        // Check if it's the first move and the PlayableDirector is disabled, and player has made an input
        if (!_firstMove && !transform.parent.GetComponent<PlayableDirector>().enabled && (horizontalInput != 0 || Input.GetKeyDown(KeyCode.Space)))
        {
            _mobileControls.parent.gameObject.SetActive(false);
            SpawningVehicles.Instance.StartGenerating();
            GameManager.Instance.StartGame();
            _onPlayerFirstMove.Invoke();
            _firstMove = true;
            Debug.Log("The player made the first move, and the game has started.");
        }
    }

    private void ShowControls()
    {
        _pcControls.gameObject.SetActive(!CheckIsMobile.Check());
        _mobileControls.gameObject.SetActive(CheckIsMobile.Check());
        _mobileSlowdown.gameObject.SetActive(CheckIsMobile.Check());
    }

    private float GetScreenInput()
    {
        // Check for touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            float screenWidth = Screen.width;

            // Use Raycast to detect if touch is over a UI element
            if (IsTouchOverUI(touch) && _firstMove == true)
                return 0f; // No movement if the touch is on the UI

            if (touch.phase == TouchPhase.Began)
            {
                if (touch.position.x < screenWidth / 2)
                    return 1f; // Move left
                else if (touch.position.x > screenWidth / 2)
                    return -1f; // Move right
            }
        }

        // Check for mouse input
        if (Input.GetMouseButton(0))
        {
            float mousePositionX = Input.mousePosition.x;
            float screenWidth = Screen.width;

            // Standard check for mouse (PC)
            if (EventSystem.current.IsPointerOverGameObject() && _firstMove)
                return 0f; // No movement if the mouse click is on the UI

            if (mousePositionX < screenWidth / 2)
                return 1f; // Move left
            else if (mousePositionX > screenWidth / 2)
                return -1f; // Move right
        }

        return 0f; // No input
    }

    // Method to check if the touch is over a UI element
    private bool IsTouchOverUI(Touch touch)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = touch.position
        };

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        return raycastResults.Count > 0;
    }

    private float GetKeyboardInput() => -Input.GetAxis("Horizontal");

    private void MoveHorizontally(float horizontalInput)
    {
        if (_vehicleCrashed)
            return;

        if(GameManager.Instance.isGameStarted())
        {
            if (Input.GetKey(KeyCode.Space))
            {
                ToggleAccelerate();
            }

            CheckPoliceDistance();
        }

        _particleTurnEffect.gameObject.SetActive(horizontalInput != 0f);
        Vector3 lateralMovement = new Vector3(0, 0, horizontalInput * (CheckIsMobile.Check() ? _moveSpeed / 2 : _moveSpeed) * Time.fixedDeltaTime);
        _rb.MovePosition(_rb.position + lateralMovement);
    }

    private void CheckPoliceDistance()
    {
        // Update police danger based on whether the vehicle is accelerating or not
        policeDanger += isAccelerating ? -Time.fixedDeltaTime : Time.fixedDeltaTime;
        policeDanger = Mathf.Max(policeDanger, 0); // Ensure policeDanger doesn't go below 0

        // Define minimum and maximum volume limits
        float minVolume = 0.05f;
        float maxVolume = 1f;
        float maxDanger = 5f; // You can change this value if needed

        // Scale the volume proportionally between minVolume and maxVolume
        float volume = Mathf.Lerp(minVolume, maxVolume, Mathf.Clamp01(policeDanger / maxDanger));
        GetComponent<AudioSource>().volume = volume;
        if (policeDanger >= maxDanger)
        {
            OnVehicleBusted();
        }
    }

    public void OnVehicleBusted()
    {
        VehicleCrashed();
        _bustedAnimation.SetActive(true);
        GameManager.Instance.GameOver();
        AudioManager.StopAudioClip("Background");
        _imageEndGame.sprite = Resources.Load<Sprite>("Images/Busted");
    }

    private void ClampPosition()
    {
        float clampedZ = Mathf.Clamp(_rb.position.z, _rightLimit, _leftLimit);
        _rb.position = new Vector3(_rb.position.x, _rb.position.y, clampedZ);
    }

    public void ToggleAccelerate()
    {
        if (isChangingSpeed) return; // If speed is currently changing, do not allow toggle

        if (isAccelerating)
        {
            // If currently accelerating, decelerate
            StartCoroutine(DecelerateCoroutine());
        }
        else
        {
            // If currently decelerating, accelerate back to previous speed
            StartCoroutine(AccelerateCoroutine());
        }

        // Toggle the state
        isAccelerating = !isAccelerating;
    }

    private IEnumerator DecelerateCoroutine()
    {
        isChangingSpeed = true; // Set the speed change status to true
        foreach (Button btn in _slowDownSpeedUpButtons) { btn.interactable = false; }
        previousSpeed = forwardSpeed; // Save the current speed
        float initialSpeed = forwardSpeed;
        float targetSpeed = initialSpeed * 0.4f; // Reduce speed by 60%
        float timeElapsed = 0f;

        while (timeElapsed < 0.5f)
        {
            forwardSpeed = Mathf.Lerp(initialSpeed, targetSpeed, timeElapsed / 0.5f);
            timeElapsed += Time.deltaTime;
            yield return null; // Wait for the next frame
        }
        foreach (Button btn in _slowDownSpeedUpButtons) { btn.interactable = true; }
        forwardSpeed = targetSpeed; // Ensure we set the final value
        isChangingSpeed = false; // Speed change process is complete
    }

    private IEnumerator AccelerateCoroutine()
    {
        isChangingSpeed = true; // Set the speed change status to true
        foreach (Button btn in _slowDownSpeedUpButtons) { btn.interactable = false; }
        float targetSpeed = previousSpeed; // The speed to reach back to
        float initialSpeed = forwardSpeed;
        float timeElapsed = 0f;

        while (timeElapsed < 0.5f)
        {
            forwardSpeed = Mathf.Lerp(initialSpeed, targetSpeed, timeElapsed / 0.5f);
            timeElapsed += Time.deltaTime;
            yield return null; // Wait for the next frame
        }
        foreach (Button btn in _slowDownSpeedUpButtons) { btn.interactable = true; }
        forwardSpeed = targetSpeed; // Ensure we set the final value
        isChangingSpeed = false; // Speed change process is complete
    }
}
