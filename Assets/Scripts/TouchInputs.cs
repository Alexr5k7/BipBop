using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInputs : MonoBehaviour
{
    public static TouchInputs Instance { get; private set; }

    public event EventHandler OnOneTouch;
    public event EventHandler OnZoomIn;
    public event EventHandler OnZoomOut;
    public event EventHandler OnShake;
    public event EventHandler OnLookDown;
    
    
    private float initialPinchDistance; 
    private bool isZooming = false;    


    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        // Detecta un toque o clic del mouse
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                if (touch.phase == TouchPhase.Began)
                {
                    OnOneTouch?.Invoke(this, EventArgs.Empty);
                    LogicaPuntos.Instance.OnTaskAction("�Toca la pantalla!"); // Solo se llama a esta acci�n
                }
            }
        }

        if (Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            // Calcula la distancia actual entre los dos dedos
            float currentPinchDistance = Vector2.Distance(touch1.position, touch2.position);

            if (!isZooming && (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began))
            {
                // Registra la distancia inicial cuando comienza el gesto
                initialPinchDistance = currentPinchDistance;
                isZooming = true;
            }
            else if (isZooming && (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved))
            {
                if (currentPinchDistance > initialPinchDistance * 1.2f) // Zoom In (alejar dedos)
                {
                    OnZoomIn?.Invoke(this, EventArgs.Empty);
                    LogicaPuntos.Instance.OnTaskAction("�Haz zoom hacia dentro!");
                    isZooming = false; // Reinicia la bandera para evitar repeticiones
                }
                else if (currentPinchDistance < initialPinchDistance * 0.8f) // Zoom Out (acercar dedos)
                {
                    OnZoomOut?.Invoke(this, EventArgs.Empty);
                    LogicaPuntos.Instance.OnTaskAction("�Haz zoom hacia fuera!");
                    isZooming = false; // Reinicia la bandera para evitar repeticiones
                }
            }
        }
        else
        {
            isZooming = false; // Reinicia el estado si se sueltan los dedos
        }

        // Detecta una sacudida del dispositivo
        if (Input.acceleration.magnitude > 4.0f)
        {
            OnShake?.Invoke(this, EventArgs.Empty);
            LogicaPuntos.Instance.OnTaskAction("�Agita el tel�fono!"); // Solo se llama a esta acci�n
        }

        // Detecta cuando el dispositivo est� boca abajo
        if (Input.deviceOrientation == DeviceOrientation.FaceDown)
        {
            OnLookDown?.Invoke(this, EventArgs.Empty);
            LogicaPuntos.Instance.OnTaskAction("�Ponlo boca abajo!"); // Solo se llama a esta acci�n
        }
    }
}
