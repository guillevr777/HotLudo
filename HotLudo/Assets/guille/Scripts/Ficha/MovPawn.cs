using System.Collections;
using UnityEngine;

public class MovPawn : MonoBehaviour
{
    [Header("Referencias")]
    public PosicionInicioManager inicioManager; // para localizar pawns (GetPawn)
    public BoardManager boardManager;           // para obtener pathBase, finalPaths, entryIndex, etc.

    void Awake()
    {
        if (inicioManager == null) Debug.LogWarning("MovPawn: inicioManager no asignado");
        if (boardManager == null) Debug.LogWarning("MovPawn: boardManager no asignado");
    }

    // Intenta sacar la primera ficha en home del jugador. Devuelve true si sacó una ficha.
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
                // Obtener índices de entrada y final desde BoardManager
                int entryIndex = boardManager.entryIndexOnBasePath[playerIndex];
                int finalEntryIndex = boardManager.finalEntryIndexOnBase[playerIndex];

                // Obtener transforms
                Transform entryTransform = boardManager.pathBase[entryIndex];

                Transform[] finalPath = GetFinalPathForPlayer(playerIndex);

                Debug.Log($"MovPawn: Sacando pawn [{playerIndex},{i}] a pathBase index {entryIndex}");

                // Llamamos al método de Pawn que maneja salida del home
                pawn.ExitHome(entryTransform.position, boardManager.pathBase);

                // Nota: si quieres mover después al finalPath, Pawn.MoveBy lo hará cuando llegue a finalEntryIndex

                return true;
            }
        }

        Debug.Log("MovPawn: No se encontró ficha en home para sacar");
        return false;
    }

    // Coroutine para mover un pawn paso a paso
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

        // Mueve el pawn paso a paso usando Pawn.MoveBy
        pawn.MoveBy(steps);

        // Espera un frame para que se vea movimiento (Pawn.MoveTo ya hace coroutine interna)
        yield return null;

        Debug.Log($"MovPawn: MovePawnCoroutine finalizada para {pawn.name}");
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
            default: return boardManager.finalPathAzul;
        }
    }
}
