using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameStartCountdownUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI countDownText;

    private void Start()
    {
        GameManager.Instance.OnStageChanged += GameManager_OnStageChanged;

        Hide();
    }

    private void Update()
    {
        countDownText.text = Mathf.Ceil(GameManager.Instance.GetCountdownToStartTimer()).ToString();
    }

    private void GameManager_OnStageChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsCountdownToStartActive())
        {
            Show();
        }
        else
        {
            Hide();
        }
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
