using UnityEngine;

[System.Serializable]
public class PlayerStartPositions
{
    public string playerName;           // nombre del jugador (opcional)
    public Transform[] positions = new Transform[4]; // 4 posiciones de inicio
}

public class BoardManager : MonoBehaviour
{
    [Header("Prefabs de ficha por jugador")]
    public GameObject[] playerPrefabs = new GameObject[4]; // 0=azul, 1=amarillo, 2=rojo, 3=verde

    [Header("Posiciones de inicio de cada jugador")]
    public PlayerStartPositions[] players = new PlayerStartPositions[4];

    private GameObject[,] instantiatedPawns; // referencia a las fichas instanciadas

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

                // Instanciamos ficha
                GameObject pawn = Instantiate(playerPrefabs[p], spawnPoint.position, Quaternion.identity);
                pawn.name = $"P{p + 1}_Pawn_{i}";

                // Asignamos datos al script Pawn
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

    // Para obtener una ficha concreta si luego quieres moverla
    public GameObject GetPawn(int playerIndex, int pawnIndex)
    {
        if (playerIndex < 0 || playerIndex >= players.Length) return null;
        if (pawnIndex < 0 || pawnIndex >= 4) return null;
        return instantiatedPawns[playerIndex, pawnIndex];
    }
}
