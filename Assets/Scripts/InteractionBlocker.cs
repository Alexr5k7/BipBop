using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionBlocker : MonoBehaviour
{
    public Button startButton; // Arrastra tu bot�n aqu� desde el Inspector
    private float disableDuration = 2f; // Tiempo que estar� desactivado

    void Start()
    {
        if (startButton != null)
        {
            // Desactiva el bot�n al inicio
            startButton.interactable = false;

            // Reactiva el bot�n despu�s de X segundos
            StartCoroutine(EnableButtonAfterDelay(disableDuration));
        }
    }

    private IEnumerator EnableButtonAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        startButton.interactable = true; // Reactiva el bot�n
    }
}
