using UnityEngine;

[System.Serializable]
public class PlayerStartPositions
{
    public string playerName;
    public Transform[] positions = new Transform[4];
    public Transform exitPosition; // <-- casilla de salida para este jugador
}

public class BoardManager : MonoBehaviour
{
    public GameObject[] playerPrefabs = new GameObject[4];
    public PlayerStartPositions[] players = new PlayerStartPositions[4];

    private GameObject[,] instantiatedPawns;
    public int currentPlayerIndex = 0; // empieza con jugador 0 (Azul)

    void Awake()
    {
        instantiatedPawns = new GameObject[players.Length, 4];
    }

    void Start()
    {
        SetupInitialPawns();
    }

    public void SetupInitialPawns()
    {
        for (int p = 0; p < players.Length; p++)
        {
            if (players[p].positions == null || players[p].positions.Length < 4) continue;

            for (int i = 0; i < 4; i++)
            {
                Transform spawnPoint = players[p].positions[i];
                if (spawnPoint == null) continue;

                GameObject pawn = Instantiate(playerPrefabs[p], spawnPoint.position, Quaternion.identity);
                pawn.name = $"P{p + 1}_Pawn_{i}";

                Pawn pawnScript = pawn.GetComponent<Pawn>();
                if (pawnScript != null)
                {
                    pawnScript.playerIndex = p;
                    pawnScript.pawnIndex = i;
                    pawnScript.SetToStartPosition(spawnPoint.position);
                }

                instantiatedPawns[p, i] = pawn;
            }
        }
    }

    // Llamar desde DieRoller cuando se obtiene un resultado
    public void OnDieRolled(int roll)
    {
        Debug.Log($"BoardManager: roll recibido {roll} para jugador {currentPlayerIndex}");

        // Si el roll es 5 y es el turno del jugador azul (0), sacar una ficha
        if (roll == 5)
        {
            TrySendPawnOutFromHome(currentPlayerIndex);
        }

        // Nota: decidir si cambiar el turno aquí o en otra parte
    }

    private void TrySendPawnOutFromHome(int playerIndex)
    {
        Transform exitPos = players[playerIndex].exitPosition;
        if (exitPos == null)
        {
            Debug.LogWarning($"Player {playerIndex} no tiene exitPosition asignada.");
            return;
        }

        for (int i = 0; i < 4; i++)
        {
            GameObject pawnGO = instantiatedPawns[playerIndex, i];
            if (pawnGO == null) continue;

            Pawn pawnScript = pawnGO.GetComponent<Pawn>();
            if (pawnScript == null) continue;

            if (pawnScript.IsAtHome())
            {
                // Mover usando ExitHome
                pawnScript.ExitHome(exitPos.position);
                Debug.Log($"Jugador {playerIndex} - Pawn {i} sale de home a la casilla de salida.");
                return; // sacamos solo una ficha
            }
        }

        Debug.Log($"Jugador {playerIndex} no tiene fichas en home para sacar.");
    }



    // Si luego quieres obtener una ficha:
    public GameObject GetPawn(int playerIndex, int pawnIndex)
    {
        if (playerIndex < 0 || playerIndex >= players.Length) return null;
        if (pawnIndex < 0 || pawnIndex >= 4) return null;
        return instantiatedPawns[playerIndex, pawnIndex];
    }
}
