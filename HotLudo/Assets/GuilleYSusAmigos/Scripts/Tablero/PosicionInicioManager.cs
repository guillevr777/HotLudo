using UnityEngine;

/// <summary>
/// Contiene información de inicio de cada jugador
/// </summary>
[System.Serializable]
public class PlayerStartPositions
{
    public string playerName;             
    public Transform[] positions = new Transform[4]; 
    public GameObject prefabPawn;        
}

/// <summary>
/// Gestiona la colocación inicial de las fichas en sus posiciones home
/// </summary>
public class PosicionInicioManager : MonoBehaviour
{
    [Header("Configuración de jugadores")]
    public PlayerStartPositions[] players = new PlayerStartPositions[4]; 

    private GameObject[,] instantiatedPawns;

    [HideInInspector] public int numeroJugadores = 2; 

    void Awake()
    {
        // Leer número de jugadores desde PlayerPrefs
        numeroJugadores = PlayerPrefs.GetInt("JugadorSeleccionado", 2);
        Debug.Log("PosicionInicioManager: Número de jugadores = " + numeroJugadores);

        // Inicializar array según número de jugadores
        instantiatedPawns = new GameObject[numeroJugadores, 4];

        PosicionarFichasIniciales();
    }

    /// <summary>
    /// Instancia las fichas y las coloca en su posición inicial (home)
    /// </summary>
    void PosicionarFichasIniciales()
    {
        for (int p = 0; p < numeroJugadores; p++)
        {
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
                Transform spawnPos = player.positions[i];
                if (spawnPos == null)
                {
                    Debug.LogWarning($"PosicionInicioManager: Player {p}, posición {i} es null");
                    continue;
                }

                GameObject pawn = Instantiate(player.prefabPawn, spawnPos.position, Quaternion.identity);
                pawn.name = $"P{p + 1}_Pawn_{i}";

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
        if (playerIndex < 0 || playerIndex >= numeroJugadores) return null;
        if (pawnIndex < 0 || pawnIndex >= 4) return null;

        return instantiatedPawns[playerIndex, pawnIndex];
    }

    /// <summary>
    /// Devuelve la posición HOME de un jugador y un índice
    /// </summary>
    public Vector3 GetHomePosition(int playerIndex, int pawnIndex)
    {
        if (playerIndex < 0 || playerIndex >= numeroJugadores)
        {
            Debug.LogError("GetHomePosition: playerIndex fuera de rango");
            return Vector3.zero;
        }

        if (pawnIndex < 0 || pawnIndex >= players[playerIndex].positions.Length)
        {
            Debug.LogError("GetHomePosition: pawnIndex fuera de rango");
            return players[playerIndex].positions[0].position;
        }

        return players[playerIndex].positions[pawnIndex].position;
    }
}