using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour {

    [Header("Timer")]
    public float startSeconds = 60f;
    float timeLeft;
    public TMP_Text timeText; // asignar TextMeshProUGUI

    [Header("Locks")]
    public LockManager lockManager; // referencia al manager que expone AllUnlocked() o booleans
                                    // Alternativa: puedes asignar directamente los SocketDoorBridge y preguntarlos
    [Header("UI Panels")]
    public GameObject victoryPanel;
    public GameObject defeatPanel;

    [Header("Options")]
    public bool startOnAwake = true;

    bool running = false;
    bool finished = false;

    void Awake() {
        if (startOnAwake) StartGame();
    }
    public void StartGame() {
        timeLeft = startSeconds;
        running = true;
        finished = false;
        victoryPanel?.SetActive(false);
        defeatPanel?.SetActive(false);
        UpdateTimeUI();
        // opcional: resetear locks si tu LockManager tiene método Reset()
        if (lockManager != null) lockManager.ResetLocksIfAvailable();
    }
    void Update() {
        if (!running || finished) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft < 0f) timeLeft = 0f;
        UpdateTimeUI();

        // Comprobar victoria
        if (lockManager != null && lockManager.AllUnlocked()) {
            Win();
            return;
        }
        // Tiempo agotado -> derrota si no ganó
        if (timeLeft <= 0f) {
            if (lockManager == null || !lockManager.AllUnlocked())
                Lose();
            else
                Win();
        }
    }
    void UpdateTimeUI() {
        if (timeText != null) {
            var sec = Mathf.CeilToInt(timeLeft);
            timeText.text = sec.ToString();
        }
    }
    void Win() {
        finished = true;
        running = false;
        victoryPanel?.SetActive(true);
        // opcional: pausar juego Time.timeScale = 0f;
        Debug.Log("[GameManager] Win");
    }
    void Lose()  {
        finished = true;
        running = false;
        defeatPanel?.SetActive(true);
        Debug.Log("[GameManager] Lose");
    }
    // Expuesto para botones de UI
    public void Restart() {
        // opcional: recargar escena o reiniciar estado
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}