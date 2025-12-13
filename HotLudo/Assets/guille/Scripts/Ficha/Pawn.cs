using System.Collections;
using UnityEngine;

/// <summary>
/// Script del prefab Ficha. Contiene estado (home / path / final), índice actual y
/// corutina pública para mover suavemente a una posición.
/// </summary>
public class Pawn : MonoBehaviour
{
    [HideInInspector] public int playerIndex;
    [HideInInspector] public int pawnIndex;

    private Vector3 startPos;
    private bool isAtHome = true;

    // Path actual (base) y final para este pawn
    [HideInInspector] public Transform[] pathBase;
    [HideInInspector] public Transform[] finalPath;

    // Índices
    [HideInInspector] public int casillaIndex = -1; // índice en pathBase (si inFinal==false)
    [HideInInspector] public bool inFinal = false;
    [HideInInspector] public int finalIndex = -1;   // índice dentro de finalPath si inFinal==true

    // Guardar la posición inicial del home
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
    }

    public bool IsAtHome() => isAtHome;

    public void LeaveHome() => isAtHome = false;

    /// <summary>
    /// Saque del home: asigna rutas y pone casillaIndex al entryIndex (donde debe aparecer).
    /// No espera animaciones: mueve a la casilla de entrada usando MoveToCoroutine.
    /// </summary>
    /// <param name="entryIndex">índice en pathBase donde aparecer</param>
    /// <param name="basePath">array completo pathBase</param>
    /// <param name="finalPathArr">final path del jugador (puede ser null si no hay)</param>
    /// <param name="finalEntryIndexOnBase">índice en base que marca la entrada a finalPath</param>
    public IEnumerator ExitHomeCoroutine(int entryIndex, Transform[] basePath, Transform[] finalPathArr, int finalEntryIndexOnBase)
    {
        if (!isAtHome)
        {
            Debug.LogWarning($"{name}: ExitHomeCoroutine llamado pero ya no está en home.");
            yield break;
        }

        if (basePath == null || basePath.Length == 0)
        {
            Debug.LogError($"{name}: ExitHomeCoroutine -> basePath nulo o vacío.");
            yield break;
        }

        // asignar rutas
        pathBase = basePath;
        finalPath = finalPathArr;

        // sanity check entryIndex
        if (entryIndex < 0 || entryIndex >= pathBase.Length)
        {
            Debug.LogError($"{name}: entryIndex {entryIndex} fuera de rango para pathBase (len {pathBase.Length}). Usando 0.");
            entryIndex = 0;
        }

        casillaIndex = entryIndex;
        inFinal = false;
        finalIndex = -1;

        // dejar home
        LeaveHome();

        Vector3 dest = pathBase[casillaIndex].position;
        Debug.Log($"{name}: ExitHome -> casillaIndex = {casillaIndex}, destino = {dest}");

        // animar movimiento hasta la casilla de entrada
        // Velocidad de mov de la ficha
        yield return MoveToCoroutine(dest, 0.30f);
    }

    /// <summary>
    /// Movimiento suave a una posición. Devuelve al terminar.
    /// Público para que MovPawn pueda esperarlo.
    /// </summary>
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
        transform.position = target;
    }

    /// <summary>
    /// Mover *un solo paso* (a la siguiente casilla ya calculada fuera) — usa MoveToCoroutine internamente.
    /// Si inFinal == false, casillaIndex debe apuntar a la próxima casilla en pathBase.
    /// Si inFinal == true, finalIndex debe apuntar a la próxima casilla en finalPath.
    /// </summary>
    public IEnumerator MoveOneStepCoroutine(float duration = 0.18f)
    {
        if (inFinal)
        {
            if (finalPath == null || finalPath.Length == 0)
            {
                Debug.LogError($"{name}: MoveOneStepCoroutine en final pero finalPath vacío.");
                yield break;
            }
            if (finalIndex < 0 || finalIndex >= finalPath.Length)
            {
                Debug.LogError($"{name}: finalIndex inválido ({finalIndex}) en finalPath.");
                yield break;
            }
            yield return MoveToCoroutine(finalPath[finalIndex].position, duration);
        }
        else
        {
            if (pathBase == null || pathBase.Length == 0)
            {
                Debug.LogError($"{name}: MoveOneStepCoroutine en base pero pathBase vacío.");
                yield break;
            }
            if (casillaIndex < 0 || casillaIndex >= pathBase.Length)
            {
                Debug.LogError($"{name}: casillaIndex inválido ({casillaIndex}) en pathBase.");
                yield break;
            }
            yield return MoveToCoroutine(pathBase[casillaIndex].position, duration);
        }
    }

    void OnMouseDown()
    {
        BoardManager bm = FindObjectOfType<BoardManager>();
        if (bm == null) return;

        bm.OnPawnSelected(this);
    }
}
