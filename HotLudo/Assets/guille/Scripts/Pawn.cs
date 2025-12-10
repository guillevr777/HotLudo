using System.Collections;
using UnityEngine;

public class Pawn : MonoBehaviour
{
    [HideInInspector] public int playerIndex;
    [HideInInspector] public int pawnIndex;
    private Vector3 startPos;
    private bool isAtHome = true;

    [HideInInspector] public int casillaIndex = -1; // índice en el path
    [HideInInspector] public Transform[] path;     // ruta completa del jugador

    public void SetToStartPosition(Vector3 pos)
    {
        startPos = pos;
        transform.position = pos;
        isAtHome = true;
        casillaIndex = -1; // sin recorrer nada
    }

    public bool IsAtHome() => isAtHome;

    public void LeaveHome() => isAtHome = false;

    public void ExitHome(Vector3 exitPos, Transform[] playerPath)
    {
        if (!isAtHome) return;
        LeaveHome();
        path = playerPath;  // asignar el path completo del jugador
        casillaIndex = 0;   // la primera casilla del path es la salida
        MoveTo(path[casillaIndex].position);
    }

    public void MoveBy(int pasos)
    {
        if (path == null || casillaIndex < 0) return;

        casillaIndex += pasos;
        if (casillaIndex >= path.Length)
            casillaIndex = path.Length - 1; // no pasar del final

        MoveTo(path[casillaIndex].position);
    }

    public void MoveTo(Vector3 targetPos, float duration = 0.2f)
    {
        StopAllCoroutines();
        StartCoroutine(MoveCoroutine(targetPos, duration));
    }

    private IEnumerator MoveCoroutine(Vector3 target, float dur)
    {
        Vector3 initial = transform.position;
        float t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(initial, target, Mathf.Clamp01(t / dur));
            yield return null;
        }
        transform.position = target;
    }

}
