using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{

    //General Variables
    [SerializeField] private float _movementSpeed = 10;
    [SerializeField] private float _jumpHeight = 2;
    [SerializeField] private float _pushForce = 5;
    [SerializeField] private float _throwForce = 20;
    
    //Specific Variables
    private float _turnSmoothVelocity = 10;
    [SerializeField] private float _smoothTime = 1; //Valor entre 0 (rota instantaneo ) y 1 (rota muy lento)

    //Components
    private CharacterController _characterController;
    private Animator _animator;

    //Inputs
    private InputAction _moveAction;
    private Vector2 _moveInput;
    private InputAction _jumpAction;
    private InputAction _aimAction;
    private InputAction _grabAction;
    private InputAction _attackAction;

    //Gravity Controll
    private float _gravity;
    [SerializeField] private Vector3 _playerGravity;

    //GroundSensor
    [SerializeField] private Transform _sensorTransform;
    [SerializeField] private float _sensorRadius;
    [SerializeField] private LayerMask _groundlayer;

    //Camera
    private Transform _cameraTransform;

    //Grab
    private GameObject _grabbedObject;
    [SerializeField] private Transform _handTransform;
    [SerializeField] private float _handRadius = 1;

    void Awake()
    {
        _characterController = GetComponent <CharacterController>();
        _animator = GetComponentInChildren <Animator>();

        _moveAction = InputSystem.actions["Move"];
        _jumpAction = InputSystem.actions["Jump"];
        _aimAction = InputSystem.actions["Aim"];
        _grabAction = InputSystem.actions["Interact"];
        _attackAction = InputSystem.actions["Attack"];

        _cameraTransform = Camera.main.transform;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _gravity = Physics.gravity.y;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        _moveInput = _moveAction.ReadValue<Vector2>();  

        Gravity();

        if(_jumpAction.WasPressedThisFrame() && IsGrounded())
        {
            Jump();
        }

        _animator.SetBool("IsJumping", !IsGrounded());

        if(_aimAction.IsPressed())
        {
            AimMovement();
        }
        else
        {
            ThirdPersonMovement();  
        }    
        if (_grabAction.WasPressedThisFrame())
        {
            GrabItems();
        }
        if (_attackAction.WasPressedThisFrame())
        {
            ThrowObject();
        }
    }

    void TopDownMovement()
    {
        //Creamos un Vector 3 (moveDirection) y le determinamos que la dirección se verá afectada por la X del moveInput y la Z de este (como se encuentra en la tercera posición (0,0,0), utilizamos la y del input, pero funcionará como eje z), como no queremos poder subir o bajar, no afectará a la y
        Vector3 moveDirection = new Vector3 (_moveInput.x, 0 , _moveInput.y); 

        if(moveDirection != Vector3.zero)
        {
            //creamos un float.función matemática para convertir la dirección en radiales (Atan2) y después a angulos (Rad2Deg) para poder controlar la rotación
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg; 

            //Creamos un float. función matemática para suavizar el giro
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _smoothTime);

            transform.rotation = Quaternion.Euler(0, smoothAngle, 0);

            //Función nativa para movernos, utiliza el Vector 3 de dirección, junto con la velocidad de movimineto y se multipkica para que se ejecute a la misma velocidad independientemente de los FPS (Time.deltaTime)
            _characterController.Move(moveDirection * _movementSpeed * Time.deltaTime); 
        }
    }

    void ThirdPersonMovement()
    {
        Vector3 direction = new Vector3 (_moveInput.x, 0 , _moveInput.y); 

        _animator.SetFloat("Vertical", direction.magnitude, 0.2f, Time.deltaTime);
        _animator.SetFloat("Horizontal", 0);

        if(direction != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _cameraTransform.eulerAngles.y; 
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _smoothTime);

            transform.rotation = Quaternion.Euler(0, smoothAngle, 0);

            Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

            _characterController.Move(moveDirection * _movementSpeed * Time.deltaTime); 
        }
    }

    void AimMovement()
    {
        Vector3 direction = new Vector3 (_moveInput.x, 0 , _moveInput.y);

        _animator.SetFloat("Horizontal", _moveInput.x, 0.2f, Time.deltaTime);
        _animator.SetFloat("Vertical", _moveInput.y, 0.2f, Time.deltaTime);

        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _cameraTransform.eulerAngles.y; 
        float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _cameraTransform.eulerAngles.y, ref _turnSmoothVelocity, _smoothTime);
        transform.rotation = Quaternion.Euler(0, smoothAngle, 0); 

        if(direction != Vector3.zero)
        {
            Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

            _characterController.Move(moveDirection * _movementSpeed * Time.deltaTime); 
        }    
    }
    void Jump()
    {
        _playerGravity.y = Mathf.Sqrt(_jumpHeight * -2 * _gravity);
    }

    void Gravity()
    {
        if (!IsGrounded())
        {
            _playerGravity.y += _gravity * Time.deltaTime;
        }

        else if(IsGrounded() && _playerGravity.y < 0 )
        {
            _playerGravity.y = _gravity;
        }
        
        _characterController.Move(_playerGravity * Time.deltaTime);
    }

    bool IsGrounded()
    {
        return Physics.CheckSphere(_sensorTransform.position, _sensorRadius, _groundlayer);

    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.gameObject.CompareTag("Pushable"))
        {
            Rigidbody _rBody = hit.gameObject.GetComponent<Rigidbody>();

            Vector3 pushDirection = hit.moveDirection;
            pushDirection.y = 0;

           // _rBody.AddForce(pushDirection * _pushForce, ForceMode.Force); //Funciona bien con objetos redondos (pelotas)
           _rBody.linearVelocity = pushDirection * _pushForce / _rBody.mass;
        }
    }

    void GrabItems()
    {
        if(_grabbedObject == null)
        {
            Collider[] objectsToGrab = Physics.OverlapSphere(_handTransform.position, _handRadius);
            
            foreach(Collider item in objectsToGrab)
            {
                if(item.CompareTag("Pushable"))
                {
                    _grabbedObject = item.gameObject;
                    _grabbedObject.transform.SetParent(_handTransform);
                    _grabbedObject.transform.position = _handTransform.position;
                    _grabbedObject.transform.rotation = _handTransform.rotation;
                    Rigidbody _objectRigidBody = _grabbedObject.GetComponent<Rigidbody>();
                    _objectRigidBody.isKinematic = true;
                    //_objectRigidBody.constraints = RigidbodyConstraints.None;

                    return;
                }
            }
        }
        else
        {   
            Rigidbody _objectRigidBody = _grabbedObject.GetComponent<Rigidbody>();
            _objectRigidBody.isKinematic = false;
            _grabbedObject.transform.SetParent(null);
            _grabbedObject = null;
            

        }
    }

    void ThrowObject()
    {
        if(_grabbedObject != null)
        {
            Rigidbody _objectRigidBody = _grabbedObject.GetComponent<Rigidbody>();
            _objectRigidBody.isKinematic = false;
            _grabbedObject.transform.SetParent(null);
            _objectRigidBody.AddForce(_cameraTransform.forward * _throwForce, ForceMode.Impulse);            
            _grabbedObject = null;

        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_sensorTransform.position, _sensorRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_handTransform.position, _handRadius);
    }
}
