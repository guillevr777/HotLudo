using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controla la selección del número de jugadores en el menú
/// </summary>
public class CursorSelector : MonoBehaviour
{
    [Header("Referencias")]
    public RectTransform cursor;           
    public RectTransform[] opciones;        

    [Header("Configuración")]
    public float moveSpeed = 10f;

    private int indiceActual = 0; 

    void Update()
    {
        // Flecha derecha
        if (Input.GetKeyDown(KeyCode.RightArrow))
            CambiarSeleccion(1);

        // Flecha izquierda
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            CambiarSeleccion(-1);

        // Enter para continuar
        if (Input.GetKeyDown(KeyCode.Return))
            ConfirmarSeleccion();
    }

    /// <summary>
    /// Cambia la selección del cursor según la dirección
    /// </summary>
    /// <param name="dir">1 = derecha, -1 = izquierda</param>
    void CambiarSeleccion(int dir)
    {
        indiceActual += dir;

        // Limitar el índice para no salirse del arreglo
        indiceActual = Mathf.Clamp(indiceActual, 0, opciones.Length - 1);

        // Actualizar posición del cursor
        MoverCursorInstantaneo();
    }

    /// <summary>
    /// Mueve el cursor a la posición de la opción seleccionada
    /// </summary>
    void MoverCursorInstantaneo()
    {
        cursor.position = opciones[indiceActual].position;
    }

    /// <summary>
    /// Confirma la selección y guarda el número de jugadores en PlayerPrefs
    /// </summary>
    public void ConfirmarSeleccion()
    {
        int numeroJugadores = indiceActual + 1;

        PlayerPrefs.SetInt("JugadorSeleccionado", numeroJugadores);
        PlayerPrefs.Save();

        // Cargar la escena del menú de jugadores
        SceneManager.LoadScene("MenuJugadores");
    }
}