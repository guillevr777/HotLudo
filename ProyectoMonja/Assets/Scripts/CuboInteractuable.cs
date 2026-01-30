using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class CuboInteractuable : MonoBehaviour
{
    public float distanciaInteraccion = 4f;

    [Header("Referencias de UI")]
    public TextMeshProUGUI textoEntregadas;
    public TextMeshProUGUI textoFlotanteE; // El texto que dice "Pulsa E para entregar"

    private PlayerInteraction playerScript;

    void Start()
    {
        playerScript = Object.FindAnyObjectByType<PlayerInteraction>();

        if (textoFlotanteE != null) textoFlotanteE.gameObject.SetActive(false); // Oculto al empezar
        ActualizarUITotal();
    }

    void Update()
    {
        if (playerScript == null) return;

        float distancia = Vector3.Distance(transform.position, playerScript.transform.position);

        // Si estamos cerca, mostramos el texto de "Pulsa E"
        if (distancia <= distanciaInteraccion)
        {
            if (textoFlotanteE != null) textoFlotanteE.gameObject.SetActive(true);

            // Si además pulsamos E
            if (Keyboard.current.eKey.wasPressedThisFrame && playerScript.monedasContador > 0)
            {
                playerScript.monedasEntregadasTotales += playerScript.monedasContador;
                playerScript.monedasContador = 0;

                playerScript.ActualizarUI();
                ActualizarUITotal();
            }
        }
        else
        {
            // Si nos alejamos, ocultamos el mensaje
            if (textoFlotanteE != null) textoFlotanteE.gameObject.SetActive(false);
        }
    }

    public void ActualizarUITotal()
    {
        if (textoEntregadas != null)
            textoEntregadas.text = "Total Entregado: " + playerScript.monedasEntregadasTotales;
    }
}