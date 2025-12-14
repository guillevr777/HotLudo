using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorSelector : MonoBehaviour
{
    [Header("Referencias")]
    public RectTransform cursor;           // Imagen del cursor
    public RectTransform[] opciones;        // Textos 1,2,3,4

    [Header("Configuración")]
    public float moveSpeed = 10f;

    private int indiceActual = 0; // 0 = 1 jugador

    void Update()
    {
        // Flecha derecha
        if (Input.GetKeyDown(KeyCode.RightArrow))
            CambiarSeleccion(1);

        // Flecha izquierda
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            CambiarSeleccion(-1);

        // Enter para continuar (opcional)
        if (Input.GetKeyDown(KeyCode.Return))
            ConfirmarSeleccion();
    }

    void CambiarSeleccion(int dir)
    {
        indiceActual += dir;
        indiceActual = Mathf.Clamp(indiceActual, 0, opciones.Length - 1);
        MoverCursorInstantaneo();
    }

    void MoverCursorInstantaneo()
    {
        cursor.position = opciones[indiceActual].position;
    }

    public void ConfirmarSeleccion()
    {
        int numeroJugadores = indiceActual + 1;

        PlayerPrefs.SetInt("JugadorSeleccionado", numeroJugadores);
        PlayerPrefs.Save();

        SceneManager.LoadScene("MenuJugadores");
    }
}
