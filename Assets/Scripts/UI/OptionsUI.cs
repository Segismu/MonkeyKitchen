using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    public static OptionsUI Instance { get; private set; }

    [SerializeField] private Button soundControlButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI soundControlText;
    [SerializeField] private TextMeshProUGUI musicText;

    private void Awake()
    {
        Instance = this;

        soundControlButton.onClick.AddListener(() => { SoundManager.Instance.ChangeVolume(); UpdateVisual(); });
        musicButton.onClick.AddListener(() => { MusicManager.Instance.ChangeVolume(); UpdateVisual(); });
        closeButton.onClick.AddListener(() => { Hide(); });

    }

    private void Start()
    {
        GameManager.Instance.OnGamePaused += GameManager_OnGamePaused;

        UpdateVisual();

        Hide();
    }

    private void GameManager_OnGamePaused(object sender, EventArgs e)
    {
        Hide();
    }

    private void UpdateVisual()
    {
        soundControlText.text = "Sound Effects: " + Mathf.Round(SoundManager.Instance.GetVolume() * 10f);
        musicText.text = "Music: " + Mathf.Round(MusicManager.Instance.GetVolume() * 10f);  
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
