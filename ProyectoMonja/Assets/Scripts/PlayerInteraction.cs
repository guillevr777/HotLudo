using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    public int monedasContador = 0;
    public int monedasEntregadasTotales = 0;

    [Header("Referencias de UI")]
    public TextMeshProUGUI textoBolsillo;

    void Start()
    {
        ActualizarUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            monedasContador++;
            ActualizarUI();
            Destroy(other.gameObject);
        }
    }

    public void ActualizarUI()
    {
        if (textoBolsillo != null)
            textoBolsillo.text = "Monedas: " + monedasContador;
    }
}