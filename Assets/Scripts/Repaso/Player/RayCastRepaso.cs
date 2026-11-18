using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class RayCastRepaso : MonoBehaviour
{
    //Inputs
    private Vector2 _mousePosition;
    private InputAction _clickInput; //detect clicks
    private InputAction _positionInput; 

    void Awake()
    {
        _clickInput = InputSystem.actions["Attack"];
        _positionInput = InputSystem.actions["MousePosition2"];
    }

    void Update()
    {
        if(_clickInput.WasPressedThisFrame())
        {
            ShootRayCast();
        }
    }

    void ShootRayCast()
    { 
        _mousePosition = _positionInput.ReadValue<Vector2>();

        Ray ray = Camera.main.ScreenPointToRay(_mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if(hit.collider.name == "Cube")
            {
                SceneManager.LoadScene("Escena 1");
            }

            if(hit.collider.CompareTag("Tag1"))
            {
                SceneManager.LoadScene("Escena 2");
            }

            if(hit.collider.gameObject.layer == 3)
            {
                SceneManager.LoadScene("Escena 3");
            }
        }
    }
}
