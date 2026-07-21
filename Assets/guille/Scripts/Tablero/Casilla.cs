using System.Collections.Generic;
using UnityEngine;

public enum CellType
{
    Normal,
    Safe,
    Exit,
    Final
}

public class Casilla : MonoBehaviour
{
    [Header("Tipo de casilla")]
    public CellType cellType = CellType.Normal;

    [Header("Posiciones para fichas")]
    public Transform posA;
    public Transform posB;

    private Pawn pawnA;
    private Pawn pawnB;

    // ──────────────
    // Ocupación
    // ──────────────
    public bool HasFreeSlot()
    {
        return pawnA == null || pawnB == null;
    }

    public int PawnCount()
    {
        int count = 0;
        if (pawnA != null) count++;
        if (pawnB != null) count++;
        return count;
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

        if (pawnB == null)
        {
            pawnB = pawn;
            return posB.position;
        }

        Debug.LogError($"❌ Casilla {name} llena (máx 2 fichas)");
        return posA.position;
    }

    public void RemovePawn(Pawn pawn)
    {
        if (pawnA == pawn) pawnA = null;
        else if (pawnB == pawn) pawnB = null;
    }
}
