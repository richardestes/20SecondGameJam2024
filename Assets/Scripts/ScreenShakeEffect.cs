using UnityEngine;
using System.Collections;
using TMPro;

public class ScreenShakeEffect : MonoBehaviour
{
    public TMP_Text textObject; // Assign the TextMeshPro text object in the Inspector
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 5f;

    private Vector3 originalPosition;

    void Start()
    {
        if (textObject == null)
        {
            textObject = GetComponent<TMP_Text>();
        }

        if (textObject != null)
        {
            originalPosition = textObject.transform.localPosition;
        }
    }

    public void StartShake()
    {
        if (textObject != null)
        {
            StartCoroutine(Shake());
        }
    }

    private IEnumerator Shake()
    {
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-shakeMagnitude, shakeMagnitude),
                Random.Range(-shakeMagnitude, shakeMagnitude),
                0f
            );

            textObject.transform.localPosition = originalPosition + randomOffset;

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        textObject.transform.localPosition = originalPosition;
    }
}
