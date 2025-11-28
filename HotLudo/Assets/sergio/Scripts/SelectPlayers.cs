using UnityEngine;
using UnityEngine.InputSystem; // Necesario para el nuevo Input System

public class MenuNavigatorNew : MonoBehaviour
{
    public RectTransform cursor;        // El icono que se mueve
    public RectTransform[] opciones;    // Opciones del menú (en orden de izquierda a derecha)

    private int index = 0;

    void Update()
    {
        // Navegar a la derecha
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame ||
            Gamepad.current?.dpad.right.wasPressedThisFrame == true)
        {
            index++;
            if (index >= opciones.Length) index = 0;
            MoverCursor();
        }

        // Navegar a la izquierda
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame ||
            Gamepad.current?.dpad.left.wasPressedThisFrame == true)
        {
            index--;
            if (index < 0) index = opciones.Length - 1;
            MoverCursor();
        }
    }

    void MoverCursor()
    {
        cursor.position = opciones[index].position;
    }
}
