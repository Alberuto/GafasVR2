using UnityEngine;

public class Bola : MonoBehaviour {

    public GameController gameController;
    private void OnTriggerEnter(Collider other) {

        Debug.Log("Bola ha entrado en: " + other.gameObject.name + ", tag: " + other.gameObject.tag);

        if (gameController == null) {
            Debug.LogError("gameController es NULL en Bola");
            return;
        }
        // Compara el tag de la BOLA con el tag de la MESA
        if (gameObject.CompareTag(other.gameObject.tag))
                gameController.BolaCorrecta(gameObject);
        else
                gameController.BolaIncorrecta(gameObject);
    }
}