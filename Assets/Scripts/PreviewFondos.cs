using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PreviewFondos : MonoBehaviour
{
    public static PreviewFondos Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject previewPanel; // Panel de preview (desactivado por defecto)
    [SerializeField] private Image previewImage;      // Imagen que muestra la preview del fondo
    [SerializeField] private FondoSelector fondoSelector; // Script para aplicar el fondo en la escena
    [SerializeField] private TextMeshProUGUI priceText; // Texto que muestra el precio en el panel
    [SerializeField] private Button confirmButton;      // Bot�n para comprar/equipar
    [SerializeField] private GameObject botonesPantallas; // Contenedor de los botones del scroll view

    // Variables para el fondo seleccionado en el preview
    private Sprite selectedSprite;
    private int currentPrice;
    private string currentBackgroundID; // Identificador �nico del fondo

    public Sprite CurrentSelectedSprite { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Se llama desde el bot�n de cada fondo en la tienda.
    /// </summary>
    /// <param name="nuevoFondo">El sprite del fondo</param>
    /// <param name="backgroundID">Identificador �nico del fondo</param>
    /// <param name="price">Precio del fondo en monedas</param>
    public void ShowPreview(BackgroundDataSO backgroundDataSO)
    {
        CurrentSelectedSprite = backgroundDataSO.sprite;
        currentBackgroundID = backgroundDataSO.id;
        currentPrice = backgroundDataSO.price;

        previewImage.sprite = backgroundDataSO.sprite;
        priceText.text = "Precio: " + backgroundDataSO.price + " monedas";

        // Si es el fondo predeterminado, lo marcamos como comprado.
        bool purchased = (currentBackgroundID == "DefaultBackground") || (PlayerPrefs.GetInt("Purchased_" + currentBackgroundID, 0) == 1);
        int coins = PlayerPrefs.GetInt("CoinCount", 0);

        if (!purchased)
        {
            if (coins >= currentPrice)
            {
                confirmButton.interactable = true;
                confirmButton.GetComponentInChildren<TextMeshProUGUI>().text = "Comprar";
            }
            else
            {
                confirmButton.interactable = false;
                confirmButton.GetComponentInChildren<TextMeshProUGUI>().text = "Monedas insuficientes";
            }
        }
        else
        {
            string equipped = PlayerPrefs.GetString("SelectedBackground", "");
            if (equipped == currentBackgroundID)
            {
                confirmButton.interactable = false;
                confirmButton.GetComponentInChildren<TextMeshProUGUI>().text = "Equipado";
            }
            else
            {
                confirmButton.interactable = true;
                confirmButton.GetComponentInChildren<TextMeshProUGUI>().text = "Equipar";
            }
        }

        previewPanel.SetActive(true);
        botonesPantallas.SetActive(false);
    }

    /// <summary>
    /// Se llama al presionar el bot�n de confirmar en el panel de preview.
    /// </summary>
    public void ConfirmBackground()
    {
        // Comprueba si el fondo ya se compr�
        bool purchased = PlayerPrefs.GetInt("Purchased_" + currentBackgroundID, 0) == 1;
        if (!purchased)
        {
            // Lee la cantidad de monedas del jugador
            int coins = PlayerPrefs.GetInt("CoinCount", 0);
            if (coins >= currentPrice)
            {
                // Descuenta las monedas del precio del fondo
                coins -= currentPrice;
                PlayerPrefs.SetInt("CoinCount", coins);
                // Marca el fondo como comprado
                PlayerPrefs.SetInt("Purchased_" + currentBackgroundID, 1);
                PlayerPrefs.SetString("SelectedBackground", currentBackgroundID);
                PlayerPrefs.Save();

            }
            else
            {
                // Esto no deber�a suceder, ya que el bot�n deber�a estar deshabilitado si no hay monedas suficientes
                return;
            }
        }
        else
        {
            // Si ya se ha comprado, simplemente equipa el fondo
            PlayerPrefs.SetString("SelectedBackground", currentBackgroundID);
            PlayerPrefs.Save();
        }

        // Aplica el fondo mediante el script FondoSelector
        fondoSelector.CambiarFondo(CurrentSelectedSprite);
        previewPanel.SetActive(false);
        botonesPantallas.SetActive(true);
    }

    /// <summary>
    /// Se llama al presionar el bot�n de cancelar en el panel de preview.
    /// </summary>
    public void CancelPreview()
    {
        previewPanel.SetActive(false);
        botonesPantallas.SetActive(true);
    }
}
