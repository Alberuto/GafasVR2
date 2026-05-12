using UnityEngine;

public class Spawner : MonoBehaviour {

    public GameObject[] listaBolas;

    public void SpawnBola() {
        int indiceAleatorio = Random.Range(0,listaBolas.Length);
        GameObject nuevaBola = Instantiate(listaBolas[indiceAleatorio], transform.position, Quaternion.identity);
    }
}