using System.Collections;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [Header("Paths (asignar en Inspector)")]
    public Transform[] pathBase;           // Casillas base del tablero (ej. 70)
    public Transform[] finalPathAzul;      // Casillas finales del azul
    public Transform[] finalPathAmarillo;  // Casillas finales del amarillo
    public Transform[] finalPathVerde;     // Casillas finales del verde
    public Transform[] finalPathRojo;      // Casillas finales del rojo

    [Header("Referencias a otros managers")]
    public PosicionInicioManager inicioManager; // Para acceder a las fichas
    public MovPawn movPawn;                     // Script encargado de mover fichas
    
    [Header("UI")]
    public TurnUIManager turnUI;

    [Header("Indices de entrada en pathBase por jugador (configurar)")]
    public int[] entryIndexOnBasePath = new int[4];   // índice de salida del home
    public int[] finalEntryIndexOnBase = new int[4];  // índice donde empieza la finalPath

    [Header("Turnos")]
    public int currentPlayerIndex = 0; // 0 = azul, 1 = amarillo, 2 = verde, 3 = rojo

    void Awake()
    {
        Debug.Log("BoardManager: Awake - listo");
        if (inicioManager == null) Debug.LogWarning("BoardManager: inicioManager NO asignado.");
        if (movPawn == null) Debug.LogWarning("BoardManager: movPawn NO asignado.");
    }

    void Start()
    {
        Debug.Log("BoardManager: Start - jugador inicial -> " + GetCurrentPlayerName());
        if (turnUI != null)
            turnUI.UpdateTurnUI(currentPlayerIndex, GetCurrentPlayerName());
    }


    public void OnDieRolled(int roll)
    {
        Debug.Log($"BoardManager: OnDieRolled() jugador={GetCurrentPlayerName()} roll={roll}");

        if (inicioManager == null || movPawn == null)
        {
            Debug.LogError("BoardManager: referencias no asignadas");
            return;
        }

        bool algunaEnHome = false;
        bool algunaFueraDeHome = false;

        // Revisar estado de fichas
        for (int i = 0; i < 4; i++)
        {
            Pawn p = inicioManager.GetPawn(currentPlayerIndex, i)?.GetComponent<Pawn>();
            if (p == null) continue;

            if (p.IsAtHome()) algunaEnHome = true;
            else algunaFueraDeHome = true;
        }

        // 🎲 1️⃣ SALE 5
        if (roll == 5)
        {
            if (algunaEnHome)
            {
                Debug.Log("Sale 5 y hay fichas en home → sacar ficha (NO pasa turno)");
                movPawn.TryExitHome(currentPlayerIndex);

                return; // NO pasa turno
            }
            else if (algunaFueraDeHome)
            {
                Debug.Log("Sale 5 y no hay fichas en home → mover 5");
                MoverPrimerPawnFueraDelHome(5, false);
                return;
            }
        }

        // 🎲 2️⃣ SALE 6
        if (roll == 6)
        {
            if (algunaFueraDeHome)
            {
                Debug.Log("Sale 6 y hay fichas fuera → mover 6 y REPITE turno");
                MoverPrimerPawnFueraDelHome(6, true);
                return;
            }
            else
            {
                Debug.Log("Sale 6 pero todas en home → pasa turno");
                EndTurn();
                return;
            }
        }

        // 🎲 3️⃣ OTRO NÚMERO
        if (algunaFueraDeHome)
        {
            Debug.Log($"Sale {roll} → mover ficha y pasar turno");
            MoverPrimerPawnFueraDelHome(roll, false);
        }
        else
        {
            Debug.Log("No hay fichas para mover → pasa turno");
            EndTurn();
        }
    }


    private void MoverPrimerPawnFueraDelHome(int pasos, bool repetirTurno)
    {
        for (int i = 0; i < 4; i++)
        {
            Pawn p = inicioManager.GetPawn(currentPlayerIndex, i)?.GetComponent<Pawn>();
            if (p != null && !p.IsAtHome())
            {
                EnsurePawnPathsAssigned(p);
                StartCoroutine(MovePawnAndFinish(p, pasos, repetirTurno));
                return;
            }
        }

        EndTurn();
    }




    private IEnumerator MovePawnAndFinish(Pawn pawn, int steps, bool repetirTurno)
    {
        yield return StartCoroutine(movPawn.MovePawnCoroutine(pawn, steps));

        if (!repetirTurno)
            EndTurn();
        else
            Debug.Log("Turno repetido por sacar 6");
    }



    // Asegura que pawn.pathBase y pawn.finalPath están asignados con los arrays correctos
    private void EnsurePawnPathsAssigned(Pawn pawn)
    {
        if (pawn == null) return;

        // pathBase es el recorrido común (siempre)
        if (pawn.pathBase == null || pawn.pathBase.Length == 0)
        {
            pawn.pathBase = pathBase;
            Debug.Log($"BoardManager: asignado pathBase a {pawn.name}");
        }

        // finalPath según jugador
        if (pawn.finalPath == null || pawn.finalPath.Length == 0)
        {
            pawn.finalPath = GetFinalPathByPlayer(pawn.playerIndex);
            Debug.Log($"BoardManager: asignado finalPath a {pawn.name}");
        }
    }

    public void EndTurn()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % 4;

        Debug.Log($"Turno terminado. Siguiente jugador: {GetCurrentPlayerName()}");

        if (turnUI != null)
            turnUI.UpdateTurnUI(currentPlayerIndex, GetCurrentPlayerName());
    }


    public string GetCurrentPlayerName()
    {
        if (inicioManager != null && inicioManager.players != null && currentPlayerIndex < inicioManager.players.Length)
            return inicioManager.players[currentPlayerIndex].playerName;
        return $"Player{currentPlayerIndex}";
    }

    // Devuelve el finalPath según jugador (azul NO usa finalPathAzul para su base recorrido;
    // finalPathAzul sólo se usa cuando la ficha entra a la columna final)
    private Transform[] GetFinalPathByPlayer(int playerIndex)
    {
        switch (playerIndex)
        {
            case 0: return finalPathAzul;
            case 1: return finalPathAmarillo;
            case 2: return finalPathVerde;
            case 3: return finalPathRojo;
            default: return null;
        }
    }
}
