using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class RayCastTest2 : MonoBehaviour
{

    private Vector2 _mouseInput;
    private InputAction _clickAction;
    private InputAction _mousePosition;

    [SerializeField] private float _timer = 5f;
    [SerializeField]private Text _timerText;

    void Awake()
    {
        _clickAction = InputSystem.actions["Attack"];
        _mousePosition = InputSystem.actions["MousePosition"];
    }

    void Update()
    {
        if(_clickAction.WasPressedThisFrame())
        {
            ShootRaycast();
        }
    }

    void ShootRaycast()
    {
        _mouseInput = _mousePosition.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(_mouseInput);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if(hit.collider.CompareTag("Test"))
            {
                StartCoroutine(Timer());
            }
        }
    }

    IEnumerator Timer()
    {
        while (_timer > 0)
        {
            _timer -= Time.deltaTime;
            _timerText.text = Mathf.Max(0, _timer).ToString("F1");
            yield return new WaitForSeconds(Time.deltaTime);   
        }
        
    }
}
