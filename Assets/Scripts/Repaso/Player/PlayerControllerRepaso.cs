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
    [SerializeField] private Vector3 _playerGravity;

    //GroundSensor
    [SerializeField] private Transform _sensor;
    [SerializeField] private float _sensorRadius;
    [SerializeField] private LayerMask _groundLayer;

    //Camera
    private Transform _cameraTransform;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();

        _moveAction = InputSystem.actions["Move"];
        _jumpAction = InputSystem.actions["Jump"];

        _cameraTransform = Camera.main.transform;

        _gravity = Physics.gravity.y;
    }

    // Update is called once per frame
    void Update()
    {
        _moveInput = _moveAction.ReadValue<Vector2>();
        Gravity();
        Movement();

        if(_jumpAction.WasPressedThisFrame() && IsGrounded())
        {
            Jump();
        }

        _animator.SetBool("IsJumping", !IsGrounded());
    }

    void MinMovement() //movimiento minimo, mala nota
    {
        Vector3 direction = new Vector3 (_moveInput.x, 0, _moveInput.y);

        _characterController.Move(direction * _movementSpeed * Time.deltaTime);
    }

    void Movement () //TopDown
    {
        Vector3 direction = new Vector3 (_moveInput.x, 0, _moveInput.y);

        _animator.SetFloat("Horizontal", _moveInput.x, 0.2f, Time.deltaTime);
        _animator.SetFloat("Vertical", _moveInput.y, 0.2f, Time.deltaTime);

        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _cameraTransform.eulerAngles.y;
        float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _cameraTransform.eulerAngles.y, ref _turnSmoothVelocity, _smoothTime);

        transform.rotation = Quaternion.Euler(0, smoothAngle, 0);

        if (direction != Vector3.zero)
        {
            Vector3 _moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            _characterController.Move(_moveDirection * _movementSpeed * Time.deltaTime);
        }
    }

    void Gravity()
    {
        if(!IsGrounded())
        {
            _playerGravity.y += _gravity * Time.deltaTime;                  
        }

        else if (IsGrounded() && _playerGravity.y < 0)
        {
            _playerGravity.y = _gravity;
        }
        _characterController.Move(_playerGravity * Time.deltaTime);  
    }

    void Jump()
    {
        _playerGravity.y = Mathf.Sqrt(_jumpForce * -2 * _gravity);
    }

    bool IsGrounded()
    {
        return Physics.CheckSphere(_sensor.position, _sensorRadius, _groundLayer);
    }
}
