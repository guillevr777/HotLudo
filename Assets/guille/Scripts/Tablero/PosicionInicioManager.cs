using UnityEngine;

[System.Serializable]
public class PlayerStartPositions
{
    // Nombre del jugador (opcional)
    public string playerName;
    // Array de 4 posiciones iniciales (home)
    public Transform[] positions = new Transform[4];
    // Prefab de la ficha del jugador
    public GameObject prefabPawn; 
}

public class PosicionInicioManager : MonoBehaviour
{
    // Array de jugadores con sus posiciones y prefabs
    public PlayerStartPositions[] players = new PlayerStartPositions[4];

    // Guarda las fichas instanciadas para cada jugador
    private GameObject[,] instantiatedPawns;

    void Awake()
    {
        // Inicializamos el array para las fichas instanciadas
        instantiatedPawns = new GameObject[players.Length, 4];

        PosicionarFichasIniciales();
    }

    /// <summary>
    /// Instancia las fichas y las coloca en su posición inicial (home)
    /// </summary>
    void PosicionarFichasIniciales()
    {
        for (int p = 0; p < players.Length; p++)
        {
            // Obtenemos los datos del jugador
            PlayerStartPositions player = players[p];
            if (player.positions == null || player.positions.Length < 4)
            {
                Debug.LogWarning($"PosicionInicioManager: Player {p} no tiene 4 posiciones asignadas");
                continue;
            }

            if (player.prefabPawn == null)
            {
                Debug.LogWarning($"PosicionInicioManager: Player {p} no tiene prefabPawn asignado");
                continue;
            }

            for (int i = 0; i < 4; i++)
            {
                // Obtenemos la posición inicial
                Transform spawnPos = player.positions[i];
                if (spawnPos == null)
                {
                    Debug.LogWarning($"PosicionInicioManager: Player {p}, posición {i} es null");
                    continue;
                }

                // Instanciamos la ficha en la posición
                GameObject pawn = Instantiate(player.prefabPawn, spawnPos.position, Quaternion.identity);
                pawn.name = $"P{p + 1}_Pawn_{i}";

                // Asignamos los datos al script Pawn
                Pawn pawnScript = pawn.GetComponent<Pawn>();
                if (pawnScript != null)
                {
                    pawnScript.playerIndex = p;
                    pawnScript.pawnIndex = i;
                    pawnScript.SetToStartPosition(spawnPos.position);
                }
                else
                {
                    Debug.LogWarning($"PosicionInicioManager: El prefab {player.prefabPawn.name} no tiene script Pawn");
                }

                // Guardamos en array para control futuro
                instantiatedPawns[p, i] = pawn;
            }
        }

        Debug.Log("PosicionInicioManager: Todas las fichas posicionadas");
    }

    /// <summary>
    /// Devuelve la ficha de un jugador y un índice
    /// </summary>
    public GameObject GetPawn(int playerIndex, int pawnIndex)
    {
        if (playerIndex < 0 || playerIndex >= players.Length) return null;
        if (pawnIndex < 0 || pawnIndex >= 4) return null;

        return instantiatedPawns[playerIndex, pawnIndex];
    }
}
