using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryResultsUI : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Color failColor;
    [SerializeField] private Color successColor;

    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
        DeliveryManager.Instance.OnRecipeFail += DeliveryManager_OnRecipeFail;

        gameObject.SetActive(false);
    }

    private void DeliveryManager_OnRecipeFail(object sender, System.EventArgs e)
    {
        gameObject.SetActive(true);
        backgroundImage.color = failColor;
        messageText.text = "DELIVERY\nFAILED";

        StartCoroutine(WaitForPopUP());

    }

    private void DeliveryManager_OnRecipeSuccess(object sender, System.EventArgs e)
    {
        gameObject.SetActive(true);
        backgroundImage.color = successColor;
        messageText.text = "DELIVERY\nSUCCESS";

        StartCoroutine(WaitForPopUP());

    }

    IEnumerator WaitForPopUP()
    {
        yield return new WaitForSeconds(2);
        gameObject.SetActive(false);
    }
}
