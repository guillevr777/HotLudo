using UnityEngine;

public class SyncPlayerRotation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Transform cameraTransform;

    void Update()
    {
        // Obtenemos la rotación actual de la cámara de Cinemachine
        float targetRotationY = cameraTransform.eulerAngles.y;

        // Aplicamos solo la rotación Y al cuerpo del jugador
        transform.rotation = Quaternion.Euler(0, targetRotationY, 0);
    }
}

