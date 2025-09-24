using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ColorManager : MonoBehaviour
{
    public static ColorManager Instance { get; private set; }

    public event EventHandler OnGameOver;

    [Header("UI Elements")]
    public TextMeshProUGUI colorWordText;  
    public Image colorWordBackground;
    public TextMeshProUGUI scoreText;        
    public Slider timeSlider;               
    public float startTime = 60f;            

    [Header("Candidate Buttons")]
    public List<Button> candidateButtons;    

    [Header("Color Data")]
    public string[] colorNames = { "Rojo", "Azul", "Verde", "Amarillo", "Morado", "Naranja", "Marr�n", "Negro", "Blanco" };
    public Color[] colorValues = { Color.red, Color.blue, Color.green, Color.yellow, new Color(0.5f, 0f, 0.5f), new Color(1f, 0.5f, 0f),
                                new Color(0.6f, 0.3f, 0.1f), Color.black, Color.white };

    private float currentTime;
    //private int score = 0;
    private int correctIndex;
    private int lastCorrectIndex = -1; 

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentTime = startTime;
        timeSlider.maxValue = startTime;
        timeSlider.value = startTime;
        UpdateScoreText();
        SetupRound();
    }

    private void Update()
    {
        currentTime -= Time.deltaTime;
        timeSlider.value = currentTime;
        timeSlider.maxValue = startTime;
        if (currentTime <= 0f)
        {
            EndGame();
        }
    }

    private void SetupRound()
    {
        int availableCount = 6;
        if (ColorGamePuntos.Instance.GetScore() >= 10) availableCount = 7;
        if (ColorGamePuntos.Instance.GetScore() >= 20) availableCount = 8;
        if (ColorGamePuntos.Instance.GetScore() >= 30) availableCount = 9;

        // aseguramos que no se repita el mismo color correcto dos veces seguidas
        do
        {
            correctIndex = UnityEngine.Random.Range(0, availableCount);
        } while (correctIndex == lastCorrectIndex);

        lastCorrectIndex = correctIndex;

        string correctColorName = colorNames[correctIndex];

        // Color del texto (distinto al correcto)
        int textColorIndex;
        do
        {
            textColorIndex = UnityEngine.Random.Range(0, availableCount);
        } while (textColorIndex == correctIndex);
        Color displayColor = colorValues[textColorIndex];

        colorWordText.text = correctColorName;
        colorWordText.color = displayColor;

        // Fondo (distinto al texto)
        int backgroundColorIndex;
        do
        {
            backgroundColorIndex = UnityEngine.Random.Range(0, colorValues.Length);
        } while (backgroundColorIndex == textColorIndex);
        colorWordBackground.color = colorValues[backgroundColorIndex];

        // Opciones de los botones
        List<int> candidateIndices = new List<int> { correctIndex, textColorIndex };

        List<int> remainingIndices = new List<int>();
        for (int i = 0; i < availableCount; i++)
        {
            if (i != correctIndex && i != textColorIndex)
                remainingIndices.Add(i);
        }

        // barajamos la lista
        for (int i = 0; i < remainingIndices.Count; i++)
        {
            int r = UnityEngine.Random.Range(i, remainingIndices.Count);
            (remainingIndices[i], remainingIndices[r]) = (remainingIndices[r], remainingIndices[i]);
        }

        if (remainingIndices.Count >= 2)
        {
            candidateIndices.Add(remainingIndices[0]);
            candidateIndices.Add(remainingIndices[1]);
        }
        else if (remainingIndices.Count == 1)
        {
            candidateIndices.Add(remainingIndices[0]);
        }

        // barajar botones
        for (int i = 0; i < candidateIndices.Count; i++)
        {
            int r = UnityEngine.Random.Range(i, candidateIndices.Count);
            (candidateIndices[i], candidateIndices[r]) = (candidateIndices[r], candidateIndices[i]);
        }

        // asignar a botones
        for (int i = 0; i < candidateButtons.Count; i++)
        {
            int assignedIndex = candidateIndices[i];
            candidateButtons[i].image.color = colorValues[assignedIndex];
            candidateButtons[i].onClick.RemoveAllListeners();
            int indexCaptured = assignedIndex;
            candidateButtons[i].onClick.AddListener(() => OnCandidateSelected(indexCaptured));
        }
    }

    public void OnCandidateSelected(int selectedIndex)
    {
        if (selectedIndex == correctIndex)
        {
            ColorGamePuntos.Instance.AddScore();
            UpdateScoreText();

            startTime = Mathf.Max(1f, startTime - 0.1f);
            currentTime = startTime;

            SetupRound();
            UpdateCandidateButtonsSize();
        }
        else
        {
            EndGame();
        }
    }

    private void UpdateCandidateButtonsSize()
    {
        float size1 = 419.92f;
        float size2 = 262.141f;
        if (ColorGamePuntos.Instance.GetScore() >= 50)
        {
            size1 = 220f;
            size2 = 130f;
        }
        else if (ColorGamePuntos.Instance.GetScore() >= 25)
        {
            size1 = 320f;
            size2 = 200f;
        }

        foreach (Button btn in candidateButtons)
        {
            RectTransform rt = btn.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(size1, size2);
        }
    }

    private void UpdateScoreText()
    {
        ColorGamePuntos.Instance.ShowScore();
    }

    private void EndGame()
    {
        ColorGamePuntos.Instance.SafeRecordIfNeeded();

        OnGameOver?.Invoke(this, EventArgs.Empty);  

        // Enviar puntuaci�n a PlayFab
        if (PlayFabLoginManager.Instance != null && PlayFabLoginManager.Instance.IsLoggedIn)
        {
            PlayFabScoreManager.Instance.SubmitScore("ColorScore", ColorGamePuntos.Instance.GetScore());
        }

        //SceneManager.LoadScene("Menu");
    }
}
