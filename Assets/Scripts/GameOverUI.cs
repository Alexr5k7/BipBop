using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Button retryButton;
    [SerializeField] private TextMeshProUGUI retryText;
    [SerializeField] private TextMeshProUGUI scoreText; // Texto para mostrar la puntuaci�n final

    private void Awake()
    {
        retryButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("SampleScene");
            Hide();
        });
    }

    private void Start()
    {
        LogicaPuntos.Instance.OnGameOver += LogicaPuntos_OnGameOver;
        Hide();
    }

    private void LogicaPuntos_OnGameOver(object sender, System.EventArgs e)
    {
        // Muestra la puntuaci�n final
        scoreText.text = $"Score: {LogicaPuntos.Instance.GetScore()}";
        Show();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
}
