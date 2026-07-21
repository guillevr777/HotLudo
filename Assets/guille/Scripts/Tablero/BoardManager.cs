using System.Collections;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [Header("Paths (asignar en Inspector)")]
    public Transform[] pathBase;
    public Transform[] finalPathAzul;
    public Transform[] finalPathAmarillo;
    public Transform[] finalPathVerde;
    public Transform[] finalPathRojo;

    [Header("Referencias a otros managers")]
    public PosicionInicioManager inicioManager;
    public MovPawn movPawn;

    [Header("UI")]
    public TurnUIManager turnUI;

    [Header("Indices de entrada en pathBase por jugador (configurar)")]
    public int[] entryIndexOnBasePath = new int[4];
    public int[] finalEntryIndexOnBase = new int[4];

    [Header("Turnos")]
    public int currentPlayerIndex = 0; // 0 = azul, 1 = amarillo, 2 = verde, 3 = rojo

    [HideInInspector] public bool esperandoSeleccionFicha = false;
    [HideInInspector] public int ultimoResultadoDado = 0;

    void Start()
    {
        if (turnUI != null)
            turnUI.UpdateTurnUI(currentPlayerIndex, GetCurrentPlayerName());
    }

    public void OnDieRolled(int roll)
    {
        Debug.Log($"BoardManager: OnDieRolled() jugador={GetCurrentPlayerName()} roll={roll}");

        ultimoResultadoDado = roll;

        // Revisión de fichas
        bool algunaEnHome = false;
        bool algunaFueraDeHome = false;

        for (int i = 0; i < 4; i++)
        {
            Pawn p = inicioManager.GetPawn(currentPlayerIndex, i)?.GetComponent<Pawn>();
            if (p == null) continue;

            if (p.IsAtHome()) algunaEnHome = true;
            else algunaFueraDeHome = true;
        }

        // 🎲 Si sale 5 y hay fichas en home, sacar automáticamente
        if (roll == 5 && algunaEnHome)
        {
            Debug.Log("Sale 5 y hay fichas en home → sacar ficha automáticamente");
            movPawn.TryExitHome(currentPlayerIndex);
            return;
        }

        // 🎲 Si hay fichas fuera de home, activar selección manual
        if (algunaFueraDeHome)
        {
            Debug.Log("Esperando que el jugador seleccione ficha fuera de home");
            esperandoSeleccionFicha = true;
        }
        else
        {
            Debug.Log("No hay fichas para mover → pasa turno");
            EndTurn();
        }
    }

    // Método llamado desde Pawn.cs al hacer clic
    public void OnPawnSelected(Pawn pawn)
    {
        if (!esperandoSeleccionFicha) return;
        if (pawn.playerIndex != currentPlayerIndex) return;
        if (pawn.IsAtHome())
        {
            Debug.Log("Ficha en home, no se puede seleccionar");
            return;
        }

        esperandoSeleccionFicha = false;
        StartCoroutine(MovePawnAndFinish(pawn, ultimoResultadoDado, ultimoResultadoDado == 6));
    }

    private IEnumerator MovePawnAndFinish(Pawn pawn, int steps, bool repetirTurno)
    {
        yield return StartCoroutine(movPawn.MovePawnCoroutine(pawn, steps));

        if (!repetirTurno)
            EndTurn();
        else
            Debug.Log("Turno repetido por sacar 6");
    }

    public void EndTurn()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % 4;

        if (turnUI != null)
            turnUI.UpdateTurnUI(currentPlayerIndex, GetCurrentPlayerName());
    }

    public string GetCurrentPlayerName()
    {
        if (inicioManager != null && inicioManager.players != null && currentPlayerIndex < inicioManager.players.Length)
            return inicioManager.players[currentPlayerIndex].playerName;

        return $"Player{currentPlayerIndex}";
    }

    private void EnsurePawnPathsAssigned(Pawn pawn)
    {
        if (pawn == null) return;

        if (pawn.pathBase == null || pawn.pathBase.Length == 0)
            pawn.pathBase = pathBase;

        if (pawn.finalPath == null || pawn.finalPath.Length == 0)
            pawn.finalPath = GetFinalPathByPlayer(pawn.playerIndex);
    }

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
