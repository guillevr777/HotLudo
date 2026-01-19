using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{

    float range = 3f;
    public LayerMask interactuableLayer;

    

    // Update is called once per frame
    void Update()
    {
        // 1. Definimos el rayo desde la posición de la cámara hacia adelante
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // Visualización en el editor (solo visible en la escena)
        Debug.DrawRay(ray.origin, ray.direction * range, Color.red);

        // 2. Comprobamos si el rayo choca con algo en la capa seleccionada
        if (Physics.Raycast(ray, out hit, range, interactuableLayer))
        {
            // Aquí el jugador está mirando un objeto interactuable
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                if (gameObject.tag == "Coin") {
                    CollectCoins(hit.collider.gameObject);
                }
            }
        }
    }

    void CollectCoins(GameObject obj)
    {
        Destroy(obj);
    }
}
