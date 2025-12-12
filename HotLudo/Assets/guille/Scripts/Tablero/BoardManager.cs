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
    }

    public void OnDieRolled(int roll)
    {
        Debug.Log($"BoardManager: OnDieRolled() jugador={GetCurrentPlayerName()} roll={roll}");

        if (inicioManager == null || movPawn == null)
        {
            Debug.LogError("BoardManager: referencias no asignadas");
            return;
        }

        // 1️⃣ Revisar si hay fichas en home
        bool algunaEnHome = false;
        for (int i = 0; i < 4; i++)
        {
            Pawn p = inicioManager.GetPawn(currentPlayerIndex, i)?.GetComponent<Pawn>();
            if (p != null && p.IsAtHome())
            {
                algunaEnHome = true;
                break;
            }
        }

        // 2️⃣ Caso: salió un 5
        if (roll == 5)
        {
            if (algunaEnHome)
            {
                Debug.Log("MovPawn: salió 5 y hay fichas en home -> sacar ficha");
                bool success = movPawn.TryExitHome(currentPlayerIndex);
                if (!success) Debug.LogWarning("No se pudo sacar ficha del home (unexpected)");
                // Nota: TryExitHome inicia una coroutine que mueve la ficha a la entrada
                // Decidimos pasar turno en cuanto se saque (si esa es la regla). Si quieres esperar a que termine, cambia lógica.
                EndTurn();
            }
            else
            {
                Debug.Log("MovPawn: salió 5 pero no hay fichas en home -> mover ficha 5 pasos");
                MoverPrimerPawnFueraDelHome(5);
            }
        }
        else // 3️⃣ Caso: salió otro número distinto de 5
        {
            Debug.Log("MovPawn: número distinto de 5 -> mover ficha disponible");
            MoverPrimerPawnFueraDelHome(roll);
        }
    }

    private void MoverPrimerPawnFueraDelHome(int pasos)
    {
        for (int i = 0; i < 4; i++)
        {
            Pawn p = inicioManager.GetPawn(currentPlayerIndex, i)?.GetComponent<Pawn>();
            if (p != null && !p.IsAtHome())
            {
                Debug.Log($"MovPawn: mover Pawn [{currentPlayerIndex},{i}] {pasos} pasos");

                // Asegurarnos de asignar pathBase y finalPath correctamente antes de mover
                EnsurePawnPathsAssigned(p);

                StartCoroutine(MovePawnAndFinish(p, pasos));
                return;
            }
        }

        Debug.LogWarning("BoardManager: no hay fichas fuera del home para mover");
        EndTurn(); // evitar bloqueo si no hay ficha
    }


    private IEnumerator MovePawnAndFinish(Pawn pawn, int steps)
    {
        // Asegurarnos de que los paths están asignados (si no se hizo antes)
        EnsurePawnPathsAssigned(pawn);

        // Delegamos la corutina de movimiento en MovPawn (espera hasta que termine)
        yield return StartCoroutine(movPawn.MovePawnCoroutine(pawn, steps));

        Debug.Log($"Movimiento completado para {pawn.name}, casillaIndex={pawn.casillaIndex}, inFinal={pawn.inFinal}");
        EndTurn();
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
        Debug.Log($"Turno terminado. Siguiente jugador: {GetCurrentPlayerName()} (index {currentPlayerIndex})");
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
