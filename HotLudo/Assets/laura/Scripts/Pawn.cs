using UnityEngine;

public class Pawn : MonoBehaviour
{
    [HideInInspector] public int playerIndex;
    [HideInInspector] public int pawnIndex;
    private Vector3 startPos;

    public void SetToStartPosition(Vector3 pos)
    {
        startPos = pos;
        transform.position = pos;
    }

    // Método simple para mover ficha a otra posición
    public void MoveTo(Vector3 targetPos, float duration = 0.2f)
    {
        StopAllCoroutines();
        StartCoroutine(MoveCoroutine(targetPos, duration));
    }

    private System.Collections.IEnumerator MoveCoroutine(Vector3 target, float dur)
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
