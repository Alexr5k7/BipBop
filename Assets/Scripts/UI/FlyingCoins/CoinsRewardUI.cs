using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinsRewardUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform coinSpawnCenter;
    [SerializeField] private RectTransform targetIcon;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private CanvasGroup panelCanvasGroup;
    [SerializeField] private TextMeshProUGUI totalCoinsText;

    [Header("Animation Settings")]
    [SerializeField] private float scatterRadius = 200f;
    [SerializeField] private float spawnDelay = 0.05f;
    [SerializeField] private float fadeDuration = 0.5f;

    private int coinsEarned;
    private int coinsArrived;
    private int totalBefore;

    private Vector3 originalIconScale;
    private int pendingPops = 0;
    private bool isPopping = false;

    private void Start()
    {
        panelCanvasGroup.alpha = 0;
        originalIconScale = targetIcon.localScale;
    }

    public void ShowReward(int coins)
    {
        gameObject.SetActive(true);
        coinsEarned = coins;
        coinsArrived = 0;
        totalBefore = CurrencyManager.Instance.GetCoins();

        panelCanvasGroup.alpha = 0;
        totalCoinsText.text = totalBefore.ToString();

        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        // Generar exactamente las monedas ganadas
        for (int i = 0; i < coinsEarned; i++)
        {
            GameObject coin = Instantiate(coinPrefab, coinSpawnCenter.parent);
            FlyingCoin fc = coin.GetComponent<FlyingCoin>();
            fc.Initialize(coinSpawnCenter.position, targetIcon.position, scatterRadius, OnCoinArrived);
            yield return new WaitForSeconds(spawnDelay);
        }

        // Peque�a pausa antes del panel
        yield return new WaitForSeconds(0.8f);

        // Fade in panel
        yield return StartCoroutine(FadeCanvas(panelCanvasGroup, 0, 1, fadeDuration));

        // Esperar hasta que todas lleguen
        while (coinsArrived < coinsEarned)
            yield return null;

        // Peque�a pausa antes del fade out
        yield return new WaitForSeconds(0.8f);

        // Fade out panel
        yield return StartCoroutine(FadeCanvas(panelCanvasGroup, 1, 0, fadeDuration));

        gameObject.SetActive(false);
    }

    private IEnumerator FadeCanvas(CanvasGroup cg, float from, float to, float duration)
    {
        float t = 0f;
        cg.alpha = from;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            cg.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }
        cg.alpha = to;
    }

    private void OnCoinArrived()
    {
        coinsArrived++;
        totalCoinsText.text = (totalBefore + coinsArrived).ToString();

        // Contar pops pendientes y lanzar animaci�n si no est� corriendo
        pendingPops++;
        if (!isPopping)
            StartCoroutine(PopIcon());

        // Cuando llegan todas, actualizar en el CurrencyManager
        if (coinsArrived == coinsEarned)
        {
            CurrencyManager.Instance.AddCoins(coinsEarned);
        }
    }

    // Pop r�pido y leve del icono del panel
    private IEnumerator PopIcon()
    {
        isPopping = true;

        while (pendingPops > 0)
        {
            pendingPops--;

            float popTime = 0.05f;
            Vector3 targetScale = originalIconScale * 1.2f; // agranda 20%

            // Agrandar
            float t = 0f;
            while (t < popTime)
            {
                t += Time.deltaTime;
                targetIcon.localScale = Vector3.Lerp(originalIconScale, targetScale, t / popTime);
                yield return null;
            }

            // Volver a tama�o original
            t = 0f;
            while (t < popTime)
            {
                t += Time.deltaTime;
                targetIcon.localScale = Vector3.Lerp(targetScale, originalIconScale, t / popTime);
                yield return null;
            }

            targetIcon.localScale = originalIconScale; // asegurar tama�o final exacto
        }

        isPopping = false;
    }
}
