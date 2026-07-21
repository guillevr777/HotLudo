using UnityEngine;
using UnityEngine.InputSystem;

public class MenuNavigatorNew : MonoBehaviour
{
    public RectTransform cursor;
    public RectTransform[] opciones;

    private int index = 0;

    void Update()
    {
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame ||
            Gamepad.current?.dpad.right.wasPressedThisFrame == true)
        {
            index++;
            if (index >= opciones.Length) index = 0;
            MoverCursor();
        }

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

    // Aquí obtenemos 1, 2, 3 o 4 directamente
    public int NumeroSeleccionado => index + 1;
}
