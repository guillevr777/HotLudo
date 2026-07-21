using UnityEngine;
using UnityEngine.InputSystem;
using static Enemigo; // Necesario para detectar el clic del ratón

public class PlayerShooting : MonoBehaviour
{
    [Header("Configuración de Arma")]
    public float range = 50f;      // Distancia máxima del disparo
    public float damage = 10f;     // Daño que hace cada bala

    void Update()
    {
        // Verificamos si se presionó el clic izquierdo
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Disparar();
        }
    }

    void Disparar()
    {
        // Lanzamos un rayo desde el centro de la pantalla (0.5, 0.5)
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range))
        {
            Debug.Log("Impacto en: " + hit.transform.name);

            // Intentamos obtener el componente de vida del enemigo
            EnemyHealth enemigo = hit.transform.GetComponent<EnemyHealth>();

            if (enemigo != null)
            {
                enemigo.RecibirDaño(damage);
            }
        }
    }
}