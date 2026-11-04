using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public  float _maxHealth;
    public float _attackDamage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FindPlayer()
    {

    }

    public void Patrol()
    {

    }

    public virtual void Attack() //virtual void es un tipo de funcion que se puede sobrescribir por herencia (polimorfismo)
    {
        Debug.Log("Ataque generico");
    }

    public void TakeDamage()
    {
        Debug.Log("Recibiendo daño");
    }

    
}
