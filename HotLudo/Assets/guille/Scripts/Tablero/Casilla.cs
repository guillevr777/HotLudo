using UnityEngine;

public enum CellType
{
    Normal,
    Safe, // Casillas seguras (no se puede comer)
    Exit, // Casilla de salida (donde aparece la ficha al sacar)
    Final // Casilla dentro del camino final
}

public class Casilla : MonoBehaviour
{
    [Header("Tipo de casilla")]
    public CellType cellType = CellType.Normal;

    [Header("Posiciones para fichas")]
    public Transform posA;
    public Transform posB;

    public Pawn pawnA;
    public Pawn pawnB;

    // ──────────────
    // Ocupación
    // ──────────────
    public bool HasFreeSlot()
    {
        return pawnA == null || (posB != null && pawnB == null);
    }

    public int PawnCount()
    {
        int count = 0;
        if (pawnA != null) count++;
        if (pawnB != null) count++;
        return count;
    }

    /// <summary>
    /// Verifica si la casilla es un "puente" (dos fichas del mismo color).
    /// </summary>
    public bool IsBridge()
    {
        // Debe tener exactamente dos fichas
        if (pawnA != null && pawnB != null)
        {
            // Comprueba si ambas fichas son del mismo color (índice de jugador)
            if (pawnA.playerIndex == pawnB.playerIndex)
            {
                return true;
            }
        }
        return false;
    }

    // ──────────────
    // Gestión de fichas
    // ──────────────
    public Vector3 GetFreePosition(Pawn pawn)
    {
        if (pawnA == null)
        {
            pawnA = pawn;
            return posA.position;
        }

        if (posB != null && pawnB == null)
        {
            pawnB = pawn;
            return posB.position;
        }

        // Si la casilla está llena (por ejemplo, si tiene un rival), se devuelve posA, 
        // pero la lógica de juego se encarga de comer/bloquear.
        return posA.position;
    }

    /// <summary>
    /// Quita la referencia a la ficha de esta casilla y consolida si es necesario.
    /// </summary>
    public void RemovePawn(Pawn pawn)
    {
        if (pawnA == pawn)
        {
            pawnA = null;

            // CONSOLIDACIÓN: Si hay una ficha en B, la movemos a A (si posB existe)
            if (posB != null && pawnB != null)
            {
                pawnA = pawnB;
                pawnB = null;
                pawnA.transform.position = posA.position;
            }
        }
        else if (pawnB == pawn)
        {
            pawnB = null;
        }
    }

    public Pawn GetRivalPawn(int playerIndex)
    {
        // Se asegura que el pawnA exista Y que su índice de jugador sea diferente
        if (pawnA != null && pawnA.playerIndex != playerIndex) return pawnA;

        // Se asegura que el pawnB exista Y que su índice de jugador sea diferente
        if (pawnB != null && pawnB.playerIndex != playerIndex) return pawnB;

        return null;
    }
}