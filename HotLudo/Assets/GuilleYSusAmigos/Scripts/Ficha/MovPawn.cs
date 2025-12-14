using System.Collections;
using UnityEngine;

/// <summary>
/// Salida desde Home, avance por el path y captura de fichas
/// </summary>
public class MovPawn : MonoBehaviour
{
    [Header("Referencias")]
    public PosicionInicioManager inicioManager;
    public BoardManager boardManager;

    [Header("Audio")]
    public AudioClip eatPawnClip;      
    private AudioSource audioSource;

    /// <summary>
    /// Inicialización de componentes
    /// </summary>
    void Awake()
    {
        if (inicioManager == null) Debug.LogWarning("MovPawn: No se encontró SpriteRenderer");
        if (boardManager == null) Debug.LogWarning("MovPawn: No se encontró BoardManager");

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    /// <summary>
    /// Intenta sacar la primera ficha en Home
    /// </summary>
    /// <param name="playerIndex">Índice del jugador que intenta sacar ficha</param>
    /// <returns>True si se pudo sacar una ficha, false si no</returns>
    public bool TryExitHome(int playerIndex)
    {
        // Índice de entrada en el pathBase para este jugador
        int entryIndex = boardManager.entryIndexOnBasePath[playerIndex];
        Casilla entryCasilla = boardManager.pathBase[entryIndex].GetComponent<Casilla>();

        if (entryCasilla == null)
        {
            Debug.LogError($"MovPawn: La Transform de entrada (index {entryIndex}) no tiene script Casilla.");
            return false;
        }

        // Si la casilla de salida ya tiene 2 fichas, la salida está bloqueada
        if (entryCasilla.PawnCount() >= 2)
        {
            Debug.Log($"MovPawn: La casilla de salida ({entryCasilla.name}) ya tiene 2 fichas. Bloqueada la salida.");
            return false;
        }

        // Buscar la primera ficha en Home del jugador
        for (int i = 0; i < 4; i++)
        {
            GameObject go = inicioManager.GetPawn(playerIndex, i);
            if (go == null) continue;   // Saltar si no existe ficha
            Pawn pawn = go.GetComponent<Pawn>();
            if (pawn == null) continue;

            if (pawn.IsAtHome())
            {
                // Asignar los paths del pawn
                Transform[] basePath = boardManager.pathBase;
                Transform[] finalPath = GetFinalPathForPlayer(playerIndex);

                Debug.Log($"MovPawn: Sacando pawn [{playerIndex},{i}]");

                StartCoroutine(DoExit(pawn, entryCasilla, entryIndex, basePath, finalPath));
                return true;
            }
        }

        Debug.Log("MovPawn: No se encontró ficha en home para sacar");
        return false;
    }

    /// <summary>
    /// Mueve la ficha desde Home hasta la casilla de salida.
    /// </summary>
    private IEnumerator DoExit(Pawn pawn, Casilla entryCasilla, int entryIndex, Transform[] basePath, Transform[] finalPath)
    {
        // Asignamos los paths al pawn
        pawn.pathBase = basePath;
        pawn.finalPath = finalPath;

        // Obtener la posición destino de la casilla
        Vector3 dest = pawn.SetCurrentCasilla(entryCasilla);
        yield return StartCoroutine(pawn.MoveToCoroutine(dest, 0.30f));

        // Actualizar estados del pawn
        pawn.LeaveHome();
        pawn.casillaIndex = entryIndex;
        pawn.inFinal = false;
        pawn.finalIndex = -1;

        // Comprobar si se come alguna ficha rival
        CheckAndEat(pawn, entryCasilla);
    }

    /// <summary>
    /// Mueve una ficha paso a paso por el tablero.
    /// </summary>
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
            // Determinar el próximo paso según si estamos en finalPath o pathBase
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

            // Obtener Transform de destino
            Transform targetTransform = nextInFinal
                ? pawn.finalPath[nextFinalIndex]
                : pawn.pathBase[nextCasillaIndex];

            Casilla nextCasilla = targetTransform.GetComponent<Casilla>();

            // Si la casilla es un puente detener 
            if (nextCasilla != null && nextCasilla.IsBridge())
            {
                remaining = 0;
                break;
            }

            // Si estamos llegando al final del finalPath, solo un paso
            if (nextInFinal && nextFinalIndex == pawn.finalPath.Length - 1)
            {
                remaining = 1;
            }

            remaining--;

            // Actualizamos los estados del pawn
            pawn.casillaIndex = nextCasillaIndex;
            pawn.inFinal = nextInFinal;
            pawn.finalIndex = nextFinalIndex;

            // Mover pawn
            Vector3 destPosition = pawn.SetCurrentCasilla(nextCasilla);
            yield return StartCoroutine(pawn.MoveToCoroutine(destPosition, 0.18f));

            if (remaining == 0)
            {
                finalCasilla = nextCasilla;
            }
        }

        // Comprobar si se come alguna ficha en la casilla final
        if (finalCasilla != null)
        {
            CheckAndEat(pawn, finalCasilla);
        }
    }

    /// <summary>
    /// Devuelve el finalPath del jugador 
    /// </summary>
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

    /// <summary>
    /// Comprueba si la ficha que ha movido peude comer a otra ficha en la casillaActual
    /// </summary>
    private void CheckAndEat(Pawn pawn, Casilla casillaActual)
    {
        if (casillaActual == null) return;

        // Solo se puede comer en casillas normales
        if (casillaActual.cellType != CellType.Normal)
        {
            Debug.Log($"CheckAndEat: Casilla Segura o Especial ({casillaActual.cellType}). Captura IMPOSIBLE.");
            return;
        }

        Pawn rival = casillaActual.GetRivalPawn(pawn.playerIndex);

        if (rival == null)
        {
            Debug.Log($"CheckAndEat: No hay ficha rival para comer");
            return;
        }

        // Solo si hay exactamente 2 fichas en la casilla (el pawn y el rival)
        if (casillaActual.PawnCount() == 2)
        {
            Debug.Log($"CheckAndEat: Captura de {pawn.name} come a {rival.name}");

            // Enviar la ficha rival a Home
            rival.SetToStartPosition(rival.startPos);

            // Reproducir sonido de captura
            if (eatPawnClip != null && audioSource != null)
                audioSource.PlayOneShot(eatPawnClip);
        }
    }
}