using System.Collections;
using UnityEngine;

public class MovPawn : MonoBehaviour
{
    [Header("Referencias")]
    public PosicionInicioManager inicioManager;
    public BoardManager boardManager;

    void Awake()
    {
        if (inicioManager == null) Debug.LogWarning("MovPawn: inicioManager no asignado");
        if (boardManager == null) Debug.LogWarning("MovPawn: boardManager no asignado");
    }

    // Intenta sacar la primera ficha en home del jugador.
    public bool TryExitHome(int playerIndex)
    {
        Debug.Log($"MovPawn: TryExitHome para player {playerIndex}");

        int entryIndex = boardManager.entryIndexOnBasePath[playerIndex];
        Casilla entryCasilla = boardManager.pathBase[entryIndex].GetComponent<Casilla>();

        if (entryCasilla == null)
        {
            Debug.LogError($"MovPawn: La Transform de entrada (index {entryIndex}) no tiene script Casilla.");
            return false;
        }

        // Regla de Bloqueo de Salida: Si hay 2 fichas en la casilla de salida, no se puede sacar.
        if (entryCasilla.PawnCount() >= 2)
        {
            Debug.Log($"MovPawn: La casilla de salida ({entryCasilla.name}) ya tiene 2 fichas. Bloqueada la salida.");
            return false;
        }

        for (int i = 0; i < 4; i++)
        {
            GameObject go = inicioManager.GetPawn(playerIndex, i);
            if (go == null) continue;
            Pawn pawn = go.GetComponent<Pawn>();
            if (pawn == null) continue;

            if (pawn.IsAtHome())
            {
                Transform[] basePath = boardManager.pathBase;
                Transform[] finalPath = GetFinalPathForPlayer(playerIndex);

                Debug.Log($"MovPawn: Sacando pawn [{playerIndex},{i}] a pathBase index {entryIndex}");

                StartCoroutine(DoExit(pawn, entryCasilla, entryIndex, basePath, finalPath));
                return true;
            }
        }

        Debug.Log("MovPawn: No se encontró ficha en home para sacar");
        return false;
    }

    // Coroutine de salida
    private IEnumerator DoExit(Pawn pawn, Casilla entryCasilla, int entryIndex, Transform[] basePath, Transform[] finalPath)
    {
        pawn.pathBase = basePath;
        pawn.finalPath = finalPath;

        Vector3 dest = pawn.SetCurrentCasilla(entryCasilla);
        yield return StartCoroutine(pawn.MoveToCoroutine(dest, 0.30f));

        pawn.LeaveHome();
        pawn.casillaIndex = entryIndex;
        pawn.inFinal = false;
        pawn.finalIndex = -1;

        CheckAndEat(pawn, entryCasilla);
    }

    // Mueve un pawn 'steps' pasos paso a paso.
    public IEnumerator MovePawnCoroutine(Pawn pawn, int steps)
    {
        if (pawn == null || pawn.IsAtHome())
        {
            Debug.LogError("MovPawn: MovePawnCoroutine pawn null o en home");
            yield break;
        }

        int finalEntryIndexOnBase = boardManager.finalEntryIndexOnBase[pawn.playerIndex];
        Casilla finalCasilla = null;

        int remaining = steps;
        while (remaining > 0)
        {
            // 1. Calcular el índice de la Casilla de destino para el siguiente paso
            int nextCasillaIndex = pawn.casillaIndex;
            bool nextInFinal = pawn.inFinal;
            int nextFinalIndex = pawn.finalIndex;

            if (!nextInFinal)
            {
                nextCasillaIndex = (nextCasillaIndex + 1) % pawn.pathBase.Length;

                if (nextCasillaIndex == finalEntryIndexOnBase)
                {
                    nextInFinal = true;
                    nextFinalIndex = 0;
                }
            }
            else
            {
                nextFinalIndex++;
                if (nextFinalIndex >= pawn.finalPath.Length)
                {
                    nextFinalIndex = pawn.finalPath.Length - 1;
                }
            }

            // 2. Obtener la Casilla de destino (script)
            Transform targetTransform = nextInFinal
                ? pawn.finalPath[nextFinalIndex]
                : pawn.pathBase[nextCasillaIndex];

            Casilla nextCasilla = targetTransform.GetComponent<Casilla>();

            // Regla de Bloqueo de Puente: Si la casilla de destino es un puente, el movimiento se detiene.
            if (nextCasilla != null && nextCasilla.IsBridge())
            {
                Debug.Log($"MovPawn: La casilla {nextCasilla.name} es un puente. El movimiento se detiene en la casilla anterior.");
                remaining = 0;
                break;
            }

            // Si llegamos a la meta final (dentro del camino final), paramos después de este paso.
            if (nextInFinal && nextFinalIndex == pawn.finalPath.Length - 1)
            {
                remaining = 1;
            }

            // 3. Ejecutar el movimiento
            remaining--;

            pawn.casillaIndex = nextCasillaIndex;
            pawn.inFinal = nextInFinal;
            pawn.finalIndex = nextFinalIndex;

            Vector3 destPosition = pawn.SetCurrentCasilla(nextCasilla);
            yield return StartCoroutine(pawn.MoveToCoroutine(destPosition, 0.18f));

            if (remaining == 0)
            {
                finalCasilla = nextCasilla;
            }
        }

        // La captura solo se ejecuta una vez en la casilla de aterrizaje.
        if (finalCasilla != null)
        {
            CheckAndEat(pawn, finalCasilla);
        }
    }


    private Transform[] GetFinalPathForPlayer(int playerIndex)
    {
        switch (playerIndex)
        {
            case 0: return boardManager.finalPathAzul;
            case 1: return boardManager.finalPathAmarillo;
            case 2: return boardManager.finalPathVerde;
            case 3: return boardManager.finalPathRojo;
            default: return null;
        }
    }

    // --- Dentro de MovPawn.cs (mantén el resto del script igual) ---

    /// <summary>
    /// Comprueba si la ficha que ha movido (pawn) debe comer a alguna otra en la casilla (casillaActual).
    /// </summary>
    // --- Dentro de MovPawn.cs (mantén el resto del script igual) ---

    /// <summary>
    /// Comprueba si la ficha que ha movido (pawn) debe comer a alguna otra en la casilla (casillaActual).
    /// </summary>
    // --- Dentro de MovPawn.cs (mantén el resto del script igual) ---

    /// <summary>
    /// Comprueba si la ficha que ha movido (pawn) debe comer a alguna otra en la casilla (casillaActual).
    /// </summary>
    // --- Dentro de MovPawn.cs (mantén el resto del script igual) ---

    /// <summary>
    /// Comprueba si la ficha que ha movido (pawn) debe comer a alguna otra en la casilla (casillaActual).
    /// </summary>
    private void CheckAndEat(Pawn pawn, Casilla casillaActual)
    {
        if (casillaActual == null) return;

        Debug.Log($"--- Inicio Chequeo de Captura ---");
        Debug.Log($"Casilla: {casillaActual.name} | Tipo: {casillaActual.cellType} | Fichas: {casillaActual.PawnCount()}");


        // =================================================================
        // VERIFICACIÓN CLAVE: Si NO es CellType.Normal, la captura es imposible.
        // =================================================================
        if (casillaActual.cellType != CellType.Normal)
        {
            Debug.Log($"CheckAndEat: CASILLA SEGURA/ESPECIAL ({casillaActual.cellType}). Captura IMPOSIBLE.");
            return;
        }

        // Si llegamos aquí, la casilla es CellType.Normal.

        Pawn rival = casillaActual.GetRivalPawn(pawn.playerIndex);

        if (rival == null)
        {
            Debug.Log($"CheckAndEat: No hay ficha rival para comer.");
            return;
        }

        // Si hay un rival, verificamos si es una captura legal.
        // Una captura es legal si es zona Normal Y la casilla tiene exactamente 2 fichas.
        if (casillaActual.PawnCount() == 2)
        {
            Debug.Log($"¡CAPTURADO! {pawn.name} come a {rival.name} en zona Normal.");

            rival.SetToStartPosition(rival.startPos);

            // boardManager.OnPawnEaten(pawn.playerIndex); 
        }
        else
        {
            Debug.Log($"CheckAndEat: Hay rival ({rival.name}), pero el conteo de fichas ({casillaActual.PawnCount()}) no es 2. No se come.");
        }

        Debug.Log($"--- Fin Chequeo de Captura ---");
    }

    // --- Fin del método CheckAndEat en MovPawn.cs ---

    // --- Fin del método CheckAndEat en MovPawn.cs ---

    // --- Fin del método CheckAndEat en MovPawn.cs ---

    // --- Fin del método CheckAndEat en MovPawn.cs ---
}