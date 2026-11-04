using UnityEngine;

public class MeleeEnemy : Enemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Patrol();
        FindPlayer();

        _maxHealth = 5f;
    }

    void ChasePlayer()
    {

    }

    public override void Attack() //override void es un tipo de funcion que permite sobreescribir una funcion de tipo virtual void
    {
        base.Attack();//base.NombreFuncionOriginal() permite ejecutar la funcion original sin modificarla

        Debug.Log("ataque especial");
    }
}
