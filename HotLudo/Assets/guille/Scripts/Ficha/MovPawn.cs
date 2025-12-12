using System.Collections;
using UnityEngine;

/// <summary>
/// MovPawn: coordina movimientos concretos de un Pawn.
/// - TryExitHome(playerIndex): saca la primera ficha en home y la pone en entryIndex (usa coroutine)
/// - MovePawnCoroutine(pawn, steps): mueve 'steps' casillas paso a paso esperando cada animación
/// </summary>
public class MovPawn : MonoBehaviour
{
    [Header("Referencias")]
    public PosicionInicioManager inicioManager; // para localizar pawns (GetPawn)
    public BoardManager boardManager;           // para obtener pathBase y final paths y los índices

    void Awake()
    {
        if (inicioManager == null) Debug.LogWarning("MovPawn: inicioManager no asignado");
        if (boardManager == null) Debug.LogWarning("MovPawn: boardManager no asignado");
    }

    // Intenta sacar la primera ficha en home del jugador. Devuelve true si sacó una ficha (la corutina ya ha empezado).
    public bool TryExitHome(int playerIndex)
    {
        Debug.Log($"MovPawn: TryExitHome para player {playerIndex}");

        for (int i = 0; i < 4; i++)
        {
            GameObject go = inicioManager.GetPawn(playerIndex, i);
            if (go == null) { Debug.Log($"MovPawn: pawn [{playerIndex},{i}] null"); continue; }
            Pawn pawn = go.GetComponent<Pawn>();
            if (pawn == null) { Debug.Log($"MovPawn: pawn [{playerIndex},{i}] sin script Pawn"); continue; }

            if (pawn.IsAtHome())
            {
                // obtener entryIndex y finalEntryIndex desde boardManager
                int entryIndex = boardManager.entryIndexOnBasePath[playerIndex];
                int finalEntryIndex = boardManager.finalEntryIndexOnBase[playerIndex];

                Transform[] basePath = boardManager.pathBase;
                Transform[] finalPath = GetFinalPathForPlayer(playerIndex);

                Debug.Log($"MovPawn: Sacando pawn [{playerIndex},{i}] a pathBase index {entryIndex}, finalEntryIndex={finalEntryIndex}");

                // Llamamos al ExitHomeCoroutine del pawn y lo iniciamos (espera dentro del coroutine)
                StartCoroutine(DoExitAndSetPaths(pawn, entryIndex, basePath, finalPath, finalEntryIndex));
                return true;
            }
        }

        Debug.Log("MovPawn: No se encontró ficha en home para sacar");
        return false;
    }

    // Coroutine que llama al ExitHomeCoroutine y luego asigna el finalEntryIndex en el pawn (se gestiona en el MovePawnCoroutine)
    private IEnumerator DoExitAndSetPaths(Pawn pawn, int entryIndex, Transform[] basePath, Transform[] finalPath, int finalEntryIndexOnBase)
    {
        // Ejecutar la salida (esto mueve la ficha hasta la casilla de entrada)
        yield return StartCoroutine(pawn.ExitHomeCoroutine(entryIndex, basePath, finalPath, finalEntryIndexOnBase));

        // NOTA: pawn ya tiene pathBase y finalPath asignados dentro de ExitHomeCoroutine.
        // Para que movPawn conozca finalEntryIndex, lo pediremos desde boardManager cuando movamos.
        Debug.Log($"MovPawn: Pawn {pawn.name} ha salido del home y está en casillaIndex={pawn.casillaIndex}");
    }

    // Mueve un pawn 'steps' pasos paso a paso. Devuelve cuando haya terminado.
    public IEnumerator MovePawnCoroutine(Pawn pawn, int steps)
    {
        if (pawn == null)
        {
            Debug.LogError("MovPawn: MovePawnCoroutine pawn null");
            yield break;
        }

        Debug.Log($"MovPawn: MovePawnCoroutine iniciada para {pawn.name} pasos={steps}");

        if (pawn.IsAtHome())
        {
            Debug.LogWarning($"MovPawn: {pawn.name} está en home, no se moverá.");
            yield break;
        }

        // Necesitamos el índice en base que marca la entrada a finalPath
        int playerIndex = pawn.playerIndex;
        int finalEntryIndexOnBase = boardManager.finalEntryIndexOnBase[playerIndex];

        int remaining = steps;
        while (remaining > 0)
        {
            // Si no está en final aún:
            if (!pawn.inFinal)
            {
                // calcular siguiente casilla en base
                int next = pawn.casillaIndex + 1;
                if (next >= pawn.pathBase.Length) next = 0; // wrap around
                pawn.casillaIndex = next;

                // Si hemos alcanzado la casilla que marca la entrada al final, y hay finalPath definido,
                // cambiamos el modo a inFinal *después* de mover a la casilla de base (o en función de reglas).
                Debug.Log($"MovPawn: {pawn.name} avanzando en pathBase a index {pawn.casillaIndex}");
                // mover a la casilla
                yield return StartCoroutine(pawn.MoveOneStepCoroutine(0.18f));
                remaining--;

                // Si hemos llegado a la casilla que define la entrada a final, activar inFinal
                if (pawn.casillaIndex == finalEntryIndexOnBase)
                {
                    if (pawn.finalPath != null && pawn.finalPath.Length > 0)
                    {
                        pawn.inFinal = true;
                        pawn.finalIndex = -1; // la próxima iteración incrementará a 0 y moverá a finalPath[0]
                        Debug.Log($"{pawn.name}: ha alcanzado finalEntryOnBase ({finalEntryIndexOnBase}) -> entrará a finalPath en próximos pasos");
                    }
                }
            }
            else
            {
                // estamos ya en finalPath, avanzamos finalIndex
                pawn.finalIndex++;
                if (pawn.finalIndex >= pawn.finalPath.Length)
                {
                    // hemos llegado al final del camino final; bloquear en la última casilla
                    pawn.finalIndex = pawn.finalPath.Length - 1;
                    Debug.Log($"{pawn.name}: ya en la última casilla final, no se mueve más.");
                    yield break;
                }

                Debug.Log($"MovPawn: {pawn.name} avanzando en finalPath a index {pawn.finalIndex}");
                yield return StartCoroutine(pawn.MoveOneStepCoroutine(0.18f));
                remaining--;
            }
        }

        Debug.Log($"MovPawn: MovePawnCoroutine finalizada para {pawn.name}, casillaIndex={pawn.casillaIndex}, inFinal={pawn.inFinal}, finalIndex={pawn.finalIndex}");
        yield break;
    }

    // Devuelve el finalPath según jugador
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
}
