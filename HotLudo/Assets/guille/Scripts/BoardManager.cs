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
    [Header("Paths de jugadores")]
    public Transform[] pathAzul;
    public Transform[] pathAmarillo;
    public Transform[] pathRojo;
    public Transform[] pathVerde;


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
        Debug.Log($"--- OnDieRolled llamado ---");
        Debug.Log($"Jugador actual: {players[currentPlayerIndex].playerName} (Index {currentPlayerIndex})");
        Debug.Log($"Número del dado: {roll}");

        // Verificar fichas en home
        bool algunaEnHome = false;
        for (int i = 0; i < 4; i++)
        {
            GameObject pawnGO = instantiatedPawns[currentPlayerIndex, i];
            if (pawnGO == null)
            {
                Debug.Log($"Pawn {i} es null");
                continue;
            }

            Pawn pawnScript = pawnGO.GetComponent<Pawn>();
            if (pawnScript == null)
            {
                Debug.Log($"Pawn {i} no tiene script Pawn");
                continue;
            }

            if (pawnScript.IsAtHome())
            {
                Debug.Log($"Pawn {i} está en home");
                algunaEnHome = true;
            }
            else
            {
                Debug.Log($"Pawn {i} ya salió del home");
            }
        }

        if (algunaEnHome)
        {
            if (roll == 5)
            {
                Debug.Log("Ha salido un 5, intentando sacar ficha del home");
                TrySendPawnOutFromHome(currentPlayerIndex);
                EndTurn();
            }
            else
            {
                Debug.Log("No salió 5, turno sigue esperando");
            }
        }
        else
        {
            // Fichas fuera del home → mover la primera disponible
            Debug.Log("Jugador tiene fichas fuera del home, moviendo según dado");

            for (int i = 0; i < 4; i++)
            {
                GameObject pawnGO = instantiatedPawns[currentPlayerIndex, i];
                if (pawnGO == null) continue;

                Pawn pawnScript = pawnGO.GetComponent<Pawn>();
                if (pawnScript == null) continue;

                if (!pawnScript.IsAtHome())
                {
                    // Asignar path si aún no lo tiene
                    if (pawnScript.path == null || pawnScript.path.Length == 0)
                    {
                        switch (currentPlayerIndex)
                        {
                            case 0: pawnScript.path = pathAzul; break;
                            case 1: pawnScript.path = pathAmarillo; break;
                            case 2: pawnScript.path = pathRojo; break;
                            case 3: pawnScript.path = pathVerde; break;
                        }
                    }

                    pawnScript.MoveBy(roll);
                    Debug.Log($"Moviendo Pawn {i} del jugador {players[currentPlayerIndex].playerName} {roll} pasos");
                    break; // mover solo una ficha por ahora
                }
            }

            EndTurn();
        }


        Debug.Log("--- Fin OnDieRolled ---");
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
            if (pawnGO == null)
            {
                Debug.Log($"Pawn {i} es null en TrySendPawnOutFromHome");
                continue;
            }

            Pawn pawnScript = pawnGO.GetComponent<Pawn>();
            if (pawnScript == null)
            {
                Debug.Log($"Pawn {i} no tiene Pawn script en TrySendPawnOutFromHome");
                continue;
            }

            if (pawnScript.IsAtHome())
            {
                Debug.Log($"Pawn {i} va a salir del home");
                Transform[] path = null;
                switch (playerIndex)
                {
                    case 0: path = pathAzul; break;
                    case 1: path = pathAmarillo; break;
                    case 2: path = pathRojo; break;
                    case 3: path = pathVerde; break;
                }

                pawnScript.ExitHome(exitPos.position, path);
                Debug.Log($"Pawn {i} ha salido del home");
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

    public void EndTurn()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
        Debug.Log($"Turno terminado. Siguiente jugador: {players[currentPlayerIndex].playerName} (Index {currentPlayerIndex})");
    }
}
