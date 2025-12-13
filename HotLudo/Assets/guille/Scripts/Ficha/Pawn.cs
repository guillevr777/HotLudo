using System.Collections;
using UnityEngine;

public class Pawn : MonoBehaviour
{
    [HideInInspector] public int playerIndex;
    [HideInInspector] public int pawnIndex;

    [HideInInspector] public Vector3 startPos;
    private bool isAtHome = true;

    [HideInInspector] public Transform[] pathBase;
    [HideInInspector] public Transform[] finalPath;

    [HideInInspector] public int casillaIndex = -1;
    [HideInInspector] public bool inFinal = false;
    [HideInInspector] public int finalIndex = -1;

    [HideInInspector] public Casilla currentCasilla;

    public void SetToStartPosition(Vector3 pos)
    {
        startPos = pos;
        transform.position = pos;
        isAtHome = true;
        casillaIndex = -1;
        inFinal = false;
        finalIndex = -1;
        pathBase = null;
        finalPath = null;

        if (currentCasilla != null)
        {
            currentCasilla.RemovePawn(this);
            currentCasilla = null;
        }
    }

    public bool IsAtHome() => isAtHome;

    public void LeaveHome() => isAtHome = false;

    /// <summary>
    /// Gestiona la transición de casilla y devuelve la posición visual.
    /// </summary>
    public Vector3 SetCurrentCasilla(Casilla newCasilla)
    {
        if (currentCasilla != null)
        {
            currentCasilla.RemovePawn(this);
        }

        currentCasilla = newCasilla;

        if (currentCasilla != null)
        {
            return currentCasilla.GetFreePosition(this);
        }

        return transform.position;
    }

    public IEnumerator MoveToCoroutine(Vector3 target, float duration = 0.18f)
    {
        Vector3 initial = transform.position;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(initial, target, Mathf.Clamp01(t / duration));
            yield return null;
        }
        transform.position = target;
    }

    void OnMouseDown()
    {
        BoardManager bm = FindObjectOfType<BoardManager>();
        if (bm == null) return;

        bm.OnPawnSelected(this);
    }
}