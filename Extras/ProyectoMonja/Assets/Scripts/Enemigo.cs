using UnityEngine;
using UnityEngine.AI;  
public class Enemigo : MonoBehaviour
{
    public Transform player; // Referencia al jugador
    private NavMeshAgent agent; // Componente NavMeshAgent
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        float distancia = Vector3.Distance(transform.position, player.position);
        Vector3 direccion = (player.position - transform.position).normalized;

        if (distancia < 15f)
        { // Rango de visión
            if (Physics.Raycast(transform.position, direccion, out RaycastHit hit, 15f))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    PerseguirJugador();
                }
            }
        }
    }

    void PerseguirJugador()
    {
        agent.SetDestination(player.position);
    }

    public class EnemyHealth : MonoBehaviour
    {
        public float vida = 50f;

        public void RecibirDaño(float cantidad)
        {
            vida -= cantidad;
            Debug.Log("Vida del enemigo: " + vida);

            if (vida <= 0)
            {
                Morir();
            }
        }

        void Morir()
        {
            Destroy(gameObject); // El enemigo desaparece
        }
    }
}
