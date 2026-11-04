using UnityEngine;

public class Caja : MonoBehaviour , IDamageable //Implementamos la interfaz IDamagable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage()
    {
        Debug.Log("DAÑO");
    }
}
