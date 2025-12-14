using System.Collections;
using UnityEngine;

/// <summary>
/// Gestiona su posición inicial (Home), movimiento por el pathBase y finalPath,
/// interacción con casillas y selección por clic
/// </summary>
public class Pawn : MonoBehaviour
{
    [HideInInspector] public int playerIndex;
    [HideInInspector] public int pawnIndex;

    // Posición inicial en Home
    [HideInInspector] public Vector3 startPos;
    private bool isAtHome = true;

    // Camino base de la ficha
    [HideInInspector] public Transform[] pathBase;
    // Camino final hacia la meta
    [HideInInspector] public Transform[] finalPath;

    // Índice actual en pathBase
    [HideInInspector] public int casillaIndex = -1;
    // Indica si la ficha está en finalPath
    [HideInInspector] public bool inFinal = false;
    // Índice actual en finalPath
    [HideInInspector] public int finalIndex = -1;

    // Casilla en la que se encuentra la ficha actualmente
    [HideInInspector] public Casilla currentCasilla;

    /// <summary>
    /// Coloca la ficha en su posición inicial (Home) y resetea todos sus estados
    /// </summary>
    /// <param name="pos">Posición inicial en el tablero</param>
    public void SetToStartPosition(Vector3 pos)
    {
        startPos = pos;
        transform.position = pos;
        isAtHome = true;
        casillaIndex = -1;
        inFinal = false;
        finalIndex = -1;
        pathBase = null;
        finalPath = null;

        // Si estaba en una casilla, eliminar referencia de la ficha en la casilla
        if (currentCasilla != null)
        {
            currentCasilla.RemovePawn(this);
            currentCasilla = null;
        }
    }

    /// <summary>
    /// Devuelve true si la ficha sigue en Home
    /// </summary>
    public bool IsAtHome() => isAtHome;

    /// <summary>
    /// Marca la ficha como fuera de Home
    /// </summary>
    public void LeaveHome() => isAtHome = false;

    /// <summary>
    /// Cambia la ficha a una nueva casilla y devuelve la posición visual dentro de la casilla
    /// </summary>
    /// <param name="newCasilla">Casilla a la que se mueve la ficha</param>
    /// <returns>Posición final dentro de la casilla</returns>
    public Vector3 SetCurrentCasilla(Casilla newCasilla)
    {
        // Eliminar ficha de la casilla anterior
        if (currentCasilla != null)
        {
            currentCasilla.RemovePawn(this);
        }

        currentCasilla = newCasilla;

        if (currentCasilla != null)
        {
            return currentCasilla.GetFreePosition(this);
        }

        // Si no hay casilla, quedarse en la posición actual
        return transform.position;
    }

    /// <summary>
    /// Coroutine que mueve la ficha a un objetivo
    /// </summary>
    /// <param name="target">Posición destino</param>
    /// <param name="duration">Duración del movimiento</param>
    public IEnumerator MoveToCoroutine(Vector3 target, float duration = 0.18f)
    {
        Vector3 initial = transform.position;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(initial, target, Mathf.Clamp01(t / duration));
            yield return null;
        }

        // Asegurarse que termina exactamente en destino
        transform.position = target;
    }

    /// <summary>
    /// Detecta el clic del ratón sobre la ficha y lo manda al BoardManager
    /// </summary>
    void OnMouseDown()
    {
        BoardManager bm = FindFirstObjectByType<BoardManager>();
        if (bm == null) return;

        bm.OnPawnSelected(this);
    }
}