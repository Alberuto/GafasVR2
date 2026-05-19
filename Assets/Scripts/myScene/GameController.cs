using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    public int puntos = 0;
    public int vidas = 5;
    public int objetivoPuntos = 10;

    public TextMeshProUGUI textoPuntos;
    public TextMeshProUGUI textoVidas;
    public GameObject panelVictoria;
    public GameObject panelDerrota;

    void Start() {
        Debug.Log(
       "GameController: " + gameObject.name +
       "\n panelDerrota = " + panelDerrota +
       "\n panelVictoria = " + panelVictoria +
       "\n textoPuntos = " + textoPuntos +
       "\n textoVidas = " + textoVidas
        );

        if (panelDerrota == null)
            Debug.LogError("panelDerrota es NULL en " + gameObject.name);
        if (panelVictoria == null)
            Debug.LogError("panelVictoria es NULL en " + gameObject.name);
        panelVictoria.SetActive(false);
        panelDerrota.SetActive(false);
        ActualizarUI();
    }

    public void BolaCorrecta(GameObject bola) {
        puntos += 1;
        Destroy(bola);
        ComprobarEstado();
        ActualizarUI();
    }
    public void BolaIncorrecta(GameObject bola) {
        vidas -= 1;
        puntos -= 1;
        Destroy(bola);
        ComprobarEstado();
        ActualizarUI();
    }
    void ComprobarEstado() {
        if (puntos >= objetivoPuntos) {
            panelVictoria.SetActive(true);
            Time.timeScale = 0f;
        }
        else if (vidas <= 0) {
            panelDerrota.SetActive(true);
            Time.timeScale = 0f;
        }
    }
    void ActualizarUI() {
        if (textoPuntos != null)
            textoPuntos.text = "Puntos: " + puntos.ToString();
        if (textoVidas != null)
            textoVidas.text = "Vidas: " + vidas.ToString();
    }
}