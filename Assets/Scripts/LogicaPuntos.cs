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
    private bool isGameActive = true;
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
        instructionText.text = "";
        scoreText.text = "Puntos: 0"; // Inicializa el texto de la puntuaci�n
    }

    void Start()
    {
        // Configura el slider
        timerUI.fillAmount = 0;
        

        // Inicia el juego con una nueva tarea
        StartNewTask();
    }

    void Update()
    {
        if (!isGameActive)
            return;

        // Reduce el tiempo restante en funci�n del tiempo real transcurrido
        currentTime -= Time.deltaTime;

        // Actualiza el slider con el tiempo restante
        timerUI.fillAmount = currentTime/startTime;

        // Si el tiempo se acaba, termina el juego
        if (currentTime <= 0f)
        {
            OnGameOver?.Invoke(this, EventArgs.Empty);
            EndGame(); // Usa el nuevo m�todo para manejar el fin del juego
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

        // Actualiza el slider con el nuevo tiempo
        timerUI.fillAmount = 0f;
    }

    private void UpdateScoreText()
    {
        // Actualiza el texto en pantalla con la puntuaci�n actual
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

    // Llama a esta funci�n al final del juego
    private void EndGame()
    {
        isGameActive = false;
        instructionText.text = "";
        SaveRecordIfNeeded(); // Guarda el r�cord si es necesario

        // Calcula las monedas ganadas en esta partida (1 moneda por cada 15 puntos)
        int coinsEarned = score / 15; // Divisi�n entera: 15, 30, 45, etc.

        // Recupera el total actual de monedas y suma las nuevas
        int totalCoins = PlayerPrefs.GetInt("CoinCount", 0);
        totalCoins += coinsEarned;
        PlayerPrefs.SetInt("CoinCount", totalCoins);
        PlayerPrefs.Save();

        
        
    }
}
