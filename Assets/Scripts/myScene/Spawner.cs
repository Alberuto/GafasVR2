using UnityEngine;

public class Spawner : MonoBehaviour {

    public GameObject[] listaBolas;
    public GameController gameController;

    public void SpawnBola() {
        int indiceAleatorio = Random.Range(0,listaBolas.Length);
        GameObject nuevaBola = Instantiate(listaBolas[indiceAleatorio], transform.position, Quaternion.identity);
        Bola scriptBola = nuevaBola.GetComponent<Bola>();
        if (scriptBola != null)
            scriptBola.gameController = gameController;
    }
}