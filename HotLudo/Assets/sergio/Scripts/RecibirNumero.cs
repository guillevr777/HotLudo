using UnityEngine;

public class RecibirNumero : MonoBehaviour
{
    void Start()
    {
        int numero = PlayerPrefs.GetInt("JugadorSeleccionado", 1); // 1 por defecto
        Debug.Log("Número seleccionado: " + numero);
    }
}
