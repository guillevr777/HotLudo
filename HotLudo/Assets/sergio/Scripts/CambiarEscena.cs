using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioDeEscena : MonoBehaviour
{
    public void IrAEscenaJuego()
    {
        SceneManager.LoadScene("MenuJugadores");
    }
}
