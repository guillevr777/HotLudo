using UnityEngine;
using System.Collections;

public class CoinSpawner : MonoBehaviour
{
    public GameObject monedaPrefab;    // El Prefab de tu moneda
    public Transform puntoAparecer;    // Lugar donde aparece
    public float tiempoEspera = 5f;    // Tiempo de respawn

    private GameObject monedaActual;   // Referencia para saber si existe

    void Start()
    {
        // Aparece la primera moneda al empezar
        SpawnMoneda();
    }

    void Update()
    {
        // Si no hay ninguna moneda en escena (monedaActual es null)
        // y no estamos ya esperando para spawnear otra...
        if (monedaActual == null)
        {
            // Iniciamos la espera de 5 segundos
            StartCoroutine(EsperarYSpawnear());
        }
    }

    IEnumerator EsperarYSpawnear()
    {
        // Creamos una moneda temporal "fantasma" para que el Update no 
        // lance esta corrutina mil veces mientras esperamos
        monedaActual = new GameObject("Esperando...");

        yield return new WaitForSeconds(tiempoEspera);

        Destroy(monedaActual); // Borramos el objeto temporal
        SpawnMoneda();
    }

    void SpawnMoneda()
    {
        monedaActual = Instantiate(monedaPrefab, puntoAparecer.position, Quaternion.identity);
        Debug.Log("Moneda spawneada.");
    }
}