using UnityEngine;
using UnityEngine.InputSystem;

public class Test : MonoBehaviour
{
    private CharacterController _characterController;
    private Animator _animator;

    private Vector2 _moveInput;
    private InputAction _moveAction;
    private InputAction _jumpAction;

    [SerializeField] private float _jumpHeight;
    [SerializeField] private float _movementSpeed;

    [SerializeField] private Vector3 _playerGravity;
    private float _gravity;

    [SerializeField] private Transform _sensor;
    [SerializeField] private float _sensorRadius = 0.5f;
    [SerializeField] private LayerMask _groundLayer;

    [SerializeField] private float _turnSmoothSpeed = 10;
    [SerializeField] private float _smoothTime = 0.2f;

    private Transform _cameraTransform;

    void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();

        _moveAction = InputSystem.actions["Move"];
        _jumpAction = InputSystem.actions["Jump"];

        _gravity = Physics.gravity.y;

        _cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        _moveInput = _moveAction.ReadValue<Vector2>();

        if(_jumpAction.WasPressedThisFrame() && IsGrounded())
        {
            Jump();
        }
        _animator.SetBool("IsJumping", !IsGrounded());

        Gravity();
        Move();
    }

    void Move()
    {
        Vector3 direction = new Vector3 (_moveInput.x, 0, _moveInput.y);

        _animator.SetFloat("Horizontal", _moveInput.x, 0.2f, Time.deltaTime);
        _animator.SetFloat("Vertical", _moveInput.y, 0.2f, Time.deltaTime);

        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _cameraTransform.eulerAngles.y;
        float smoothAngl = Mathf.SmoothDampAngle(transform.eulerAngles.y, _cameraTransform.eulerAngles.y, ref _turnSmoothSpeed, _smoothTime);

        transform.rotation = Quaternion.Euler(0, smoothAngl, 0);

        if(direction != Vector3.zero)
        {
            Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            _characterController.Move(moveDirection * _movementSpeed * Time.deltaTime);
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
        _playerGravity.y = Mathf.Sqrt(_jumpHeight * -2 * _gravity);
    }

    bool IsGrounded()
    {
        return Physics.CheckSphere(_sensor.position, _sensorRadius, _groundLayer);
    }
   
}
