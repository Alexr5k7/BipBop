using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LogicaPuntos : MonoBehaviour
{
    public static LogicaPuntos Instance { get; private set; }

    public TextMeshProUGUI instructionText; // Texto para mostrar la instrucci�n
    public TextMeshProUGUI scoreText; // Texto para mostrar la puntuaci�n
    public Image timerUI; // Slider para mostrar el tiempo restante
    public float startTime = 100f; // Tiempo inicial en segundos

    private float currentTime;
    private bool isGameActive = false; // Se desactiva hasta que pasen los 3 segundos
    private int score = -1; // Puntuaci�n inicial

    public event EventHandler OnGameOver;

    private bool isTaskCompleted = false; // Verifica si la tarea actual ya fue completada
    private string currentTask; // Tarea actual
    private string lastTask;    // �ltima tarea realizada
    private string[] tasks = {
        "�Toca la pantalla!",
        "�Haz zoom hacia dentro!",
        "�Haz zoom hacia fuera!",
        "�Agita el tel�fono!",
        "�Ponlo boca abajo!",
        "�Desliza hacia la derecha!",
        "�Desliza hacia la izquierda!",
        "�Desliza hacia arriba!",
        "�Desliza hacia abajo!",
        "�Gira a la derecha!",
        "�Gira a la izquierda!"
    };

    private void Awake()
    {
        Instance = this;
        instructionText.text = "Prep�rate..."; // Mensaje mientras espera
        scoreText.text = "Puntos: 0"; // Inicializa el texto de la puntuaci�n
        timerUI.fillAmount = 1; // Barra vac�a al inicio
    }

    void Start()
    {
        StartCoroutine(StartGameAfterDelay(3.0f));
    }

    private IEnumerator StartGameAfterDelay(float delay)
    {
        CountDownUI.Instance.Show();

        yield return new WaitForSeconds(delay);

        CountDownUI.Instance.ShowMessage("GO!");

        yield return new WaitForSeconds(0.8f);

        CountDownUI.Instance.Hide();
        isGameActive = true; 
        instructionText.text = ""; 

        StartNewTask();
    }

    void Update()
    {
        if (!isGameActive)
            return;

        currentTime -= Time.deltaTime;

        timerUI.fillAmount = currentTime / startTime;

        if (currentTime <= 0f)
        {
            OnGameOver?.Invoke(this, EventArgs.Empty);
            EndGame(); 
        }
    }

    public void OnTaskAction(string action)
    {
        if (!isGameActive || isTaskCompleted)
            return;

        // Verifica si la acci�n corresponde a la tarea actual
        if (action == currentTask) // Solo acepta acciones que coincidan exactamente
        {
            isTaskCompleted = true; // Marca la tarea como completada
            StartNewTask();
            isTaskCompleted = false; // Resetea la bandera para la pr�xima tarea
        }
    }

    private void StartNewTask()
    {
        score++;
        UpdateScoreText();

        // Filtrar tareas seg�n las configuraciones
        List<string> availableTasks = new List<string>(tasks);

        // Si las tareas de movimiento est�n deshabilitadas
        if (PlayerPrefs.GetInt("MotionTasks", 1) == 0)
        {
            availableTasks.Remove("�Agita el tel�fono!");
            availableTasks.Remove("�Ponlo boca abajo!");
            availableTasks.Remove("�Gira a la derecha!");
            availableTasks.Remove("�Gira a la izquierda!");
        }

        // Seleccionar una nueva tarea diferente
        string newTask;
        do
        {
            newTask = availableTasks[UnityEngine.Random.Range(0, availableTasks.Count)];
        } while (newTask == currentTask);

        currentTask = newTask;
        instructionText.text = currentTask;

        currentTime = startTime;
        startTime = Mathf.Max(2f, startTime - 0.1f);

        timerUI.fillAmount = 0f;
    }

    private void UpdateScoreText()
    {
        scoreText.text = "Puntos: " + score;
    }

    public int GetScore()
    {
        return score;
    }

    public bool IsGameActive()
    {
        return isGameActive;
    }

    private void SaveRecordIfNeeded()
    {
        // Recupera el r�cord actual
        int currentRecord = PlayerPrefs.GetInt("MaxRecord", 0);

        // Si la puntuaci�n actual supera el r�cord, actualiza el valor
        if (score > currentRecord)
        {
            PlayerPrefs.SetInt("MaxRecord", score);
            PlayerPrefs.Save();
        }
    }

    private void EndGame()
    {
        isGameActive = false;
        instructionText.text = "�Juego terminado!";
        SaveRecordIfNeeded(); 

        int coinsEarned = score / 15;

        int totalCoins = PlayerPrefs.GetInt("CoinCount", 0);
        totalCoins += coinsEarned;
        PlayerPrefs.SetInt("CoinCount", totalCoins);
        PlayerPrefs.Save();
    }
}
