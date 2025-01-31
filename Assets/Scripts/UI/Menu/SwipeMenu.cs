using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeMenu : MonoBehaviour
{
   
    public RectTransform contentPanel; // Panel que contiene las pantallas
    private Vector2 startPosition; // Posici�n inicial para comparar el swipe
    private Vector2 targetPosition; // Posici�n objetivo del swipe
    private float lerpSpeed = 10f; // Velocidad de interpolaci�n
    private int currentPage = 0; // P�gina actual
    private int totalPages = 2; // N�mero total de pantallas (Ajusta si agregas m�s)

    private void Start()
    {
        // Iniciar en la posici�n correcta del men� principal
        targetPosition = contentPanel.anchoredPosition ;
    }

    private void Update()
    {
        // Interpola suavemente entre la posici�n actual y la deseada
        contentPanel.anchoredPosition = Vector2.Lerp(contentPanel.anchoredPosition, targetPosition, Time.deltaTime * lerpSpeed);
    }

    public void OnBeginDrag()
    {
        // Guarda la posici�n inicial cuando el jugador empieza a deslizar
        startPosition = Input.mousePosition;
    }

    public void OnEndDrag()
    {
        // Obtiene la direcci�n del swipe
        Vector2 endPosition = Input.mousePosition;
        float swipeDelta = endPosition.x - startPosition.x;

        // Determina si se desliz� suficiente para cambiar de pantalla
        if (Mathf.Abs(swipeDelta) > Screen.width * 0.2f)
        {
            if (swipeDelta < 0 && currentPage < totalPages - 1)
            {
                // Ir a la siguiente p�gina
                currentPage++;
            }
            else if (swipeDelta > 0 && currentPage > 0)
            {
                // Ir a la anterior p�gina
                currentPage--;
            }
        }

        // Calcula la nueva posici�n del panel basado en la p�gina actual
        targetPosition = new Vector2(-currentPage * Screen.width, contentPanel.anchoredPosition.y);
    }
}

