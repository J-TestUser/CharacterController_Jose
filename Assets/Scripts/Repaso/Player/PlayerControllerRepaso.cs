using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerRepaso : MonoBehaviour
{
    //Components
    private CharacterController _characterController;
    private Animator _animator;

    //Inputs
    private Vector2 _moveInput;
    private InputAction _moveAction;
    private InputAction _jumpAction;

    //Atributes
    [SerializeField] private float _movementSpeed = 10;
    [SerializeField] private float _jumpForce = 4;


    //SmoothRotation
    [SerializeField] private float _smoothTime = 0.2f;
    private float _turnSmoothVelocity;

    //gravity
    [SerializeField] private float _gravity;

    //GroundSensor
    [SerializeField] private Transform _sensor;
    [SerializeField] private float _sensorRadius;
    [SerializeField] private LayerMask _groundLayer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();

        _moveAction = InputSystem.actions["Move"];
        _jumpAction = InputSystem.actions["Jump"];
    }

    // Update is called once per frame
    void Update()
    {
        _moveInput = _moveAction.ReadValue<Vector2>();

        Movement();
    }

    void MinMovement() //movimiento minimo, mala nota
    {
        Vector3 direction = new Vector3 (_moveInput.x, 0, _moveInput.y);

        _characterController.Move(direction * _movementSpeed * Time.deltaTime);
    }

    void Movement ()
    {
        Vector3 direction = new Vector3 (_moveInput.x, 0, _moveInput.y);

        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _smoothTime);

        transform.rotation = Quaternion.Euler(0, smoothAngle, 0);

        _characterController.Move(direction * _movementSpeed * Time.deltaTime);
    }
}
