using UnityEngine;

public class Bola : MonoBehaviour {

    public GameController gameController;
    private void OnTriggerEnter(Collider other) {
       
            // Compara el tag de la BOLA con el tag de la MESA
            if (gameObject.CompareTag(other.gameObject.tag))
                gameController.BolaCorrecta(gameObject);
            else
                gameController.BolaIncorrecta(gameObject);
    }
}