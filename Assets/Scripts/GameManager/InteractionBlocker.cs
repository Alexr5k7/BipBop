using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractionBlocker : MonoBehaviour
{
    public Button startButton; // Arrastra tu bot�n aqu� desde el Inspector
    private float disableDuration = 0.3f; // Tiempo que estar� desactivado
    [SerializeField] private TextMeshProUGUI recordText; // Asigna este Text desde el inspector
    [SerializeField] private TextMeshProUGUI recordTextGeometric; // Asigna este Text desde el inspector

    void Start()
    {
        if (startButton != null)
        {
            // Desactiva el bot�n al inicio
            startButton.interactable = false;

            // Reactiva el bot�n despu�s de X segundos
            StartCoroutine(EnableButtonAfterDelay(disableDuration));
        }
        // Recupera el r�cord desde PlayerPrefs
        int maxRecord = PlayerPrefs.GetInt("MaxRecord", 0);

        // Actualiza el texto del r�cord en el men�
        recordText.text = $"R�cord M�ximo: {maxRecord}";

        int maxRecordGeometric = PlayerPrefs.GetInt("MaxRecordGeometric", 0);
        recordTextGeometric.text = $"R�cord M�ximo: {maxRecordGeometric}";

    }

    private IEnumerator EnableButtonAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        startButton.interactable = true; // Reactiva el bot�n
    }
}
