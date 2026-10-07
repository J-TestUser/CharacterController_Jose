using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{

    //General Variables
    [SerializeField] private float _movementSpeed = 10;
    
    //Specific Variables
    private float _turnSmoothVelocity = 10;
    [SerializeField] private float _smoothTime = 1; //Valor entre 0 (rota instantaneo ) y 1 (rota muy lento)

    //Components
    private CharacterController _characterController;

    //Inputs
    private InputAction _moveAction;
    private Vector2 _moveInput;

    //Gravity Controll
    private float _gravity;
    [SerializeField] private Vector3 _playerGravity;

    //GroundSensor
    [SerializeField] private Transform _sensorTransform;
    [SerializeField] private float _sensorRadius;
    [SerializeField] private LayerMask _groundlayer;

    void Awake()
    {
        _characterController = GetComponent <CharacterController>();

        _moveAction = InputSystem.actions["Move"];
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _gravity = Physics.gravity.y;
    }

    // Update is called once per frame
    void Update()
    {
        _moveInput = _moveAction.ReadValue<Vector2>();  

        Gravity();

        Movement(); 
    }

    void Movement()
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

    void Gravity()
    {
        if (!_characterController.isGrounded)
        {
            _playerGravity.y += _gravity * Time.deltaTime;
        }
        
        _characterController.Move(_playerGravity * Time.deltaTime);
    }

    /*bool IsGrounded()
    {

    }*/
}
