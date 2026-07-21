using UnityEngine;

public class EscenaJuegoControl : MonoBehaviour
{
    void Start()
    {
        string json = PlayerPrefs.GetString("JugadoresData", "");
        if (!string.IsNullOrEmpty(json))
        {
            JugadoresData data = JsonUtility.FromJson<JugadoresData>(json);
            Debug.Log("Número de jugadores: " + data.numeroJugadores);
            foreach (string nombre in data.nombresJugadores)
            {
                Debug.Log("Jugador: " + nombre);
            }

            // Aquí puedes usar data.numeroJugadores y data.nombresJugadores en tu juego
        }
    }
}
