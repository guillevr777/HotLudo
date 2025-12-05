using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioDeEscena : MonoBehaviour
{
    public MenuNavigatorNew cursor;  // ← arrastra aquí tu cursor en el inspector

    public void IrAEscenaJuego()
    {
        int numero = cursor.NumeroSeleccionado;

        PlayerPrefs.SetInt("JugadorSeleccionado", numero);

        SceneManager.LoadScene("MenuJugadores");
    }
}
