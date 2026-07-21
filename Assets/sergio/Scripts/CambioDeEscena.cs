using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Necesario si usas TextMeshPro

[System.Serializable]
public class JugadoresData
{
    public int numeroJugadores;
    public string[] nombresJugadores;
}

public class CambioDeEscena : MonoBehaviour

{

    public MenuNavigatorNew cursor;  // ← arrastra aquí tu cursor en el inspector

    // Para la escena de selección de jugadores
    public TMP_InputField[] inputNombres; // Arrastra aquí los InputFields de nombres
    public int numeroJugadores; // Número de jugadores que seleccionaste previamente

    public void IrAEscenaJuego()
    {
        int numero = cursor.NumeroSeleccionado;

        PlayerPrefs.SetInt("JugadorSeleccionado", numero);

        SceneManager.LoadScene("MenuJugadores");
    }

    public void IrAEscenaJuego2()
    {
        int numero = cursor.NumeroSeleccionado;
        PlayerPrefs.SetInt("JugadorSeleccionado", numero);

        // Guardar los nombres y número de jugadores
        string[] nombres = new string[numeroJugadores];
        for (int i = 0; i < numeroJugadores; i++)
        {
            if (i < inputNombres.Length)
                nombres[i] = inputNombres[i].text;
            else
                nombres[i] = "Jugador" + (i + 1); // Por defecto si falta un InputField
        }

        JugadoresData data = new JugadoresData();
        data.numeroJugadores = numeroJugadores;
        data.nombresJugadores = nombres;

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("JugadoresData", json);
        PlayerPrefs.Save();

        SceneManager.LoadScene("EscenaPrueba");
    }
}
