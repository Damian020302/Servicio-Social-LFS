using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CauldronManager : MonoBehaviour
{
    [Header("Wrist Flexion/Extension Counter")]
    public float flexionAngle = 0.0f;
    public float extensionAngle = 0.0f;
    private Quaternion neutralRotation;
    [Header("Constant Warning")]
    public TextMeshProUGUI warning;
    public float warningBlinkSpeed = 5.0f;
    public float warningScaleMultiplier = 1.2f;
    private Coroutine warningCoroutine;
    private Vector3 warningOriginalScale;
    void Start()
    {
        if(warning != null)
        {
            warningOriginalScale = warning.transform.localScale;
            if(warningOriginalScale == Vector3.zero)
            {
                warningOriginalScale = Vector3.one;
            }
        }
        StartReminder();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 forwardNeutral = neutralRotation * Vector3.forward;
        Vector3 rightNeutral = neutralRotation * Vector3.right;
        float flexExtAngle = Vector3.SignedAngle(forwardNeutral, transform.forward, rightNeutral);
        flexionAngle = 0.0f;
        extensionAngle = 0.0f;
        if (flexExtAngle > 0)
        {
            flexionAngle = flexExtAngle;
        }
        else if (flexExtAngle < 0)
        {
            extensionAngle = Mathf.Abs(flexExtAngle);
        }
        Debug.Log($"Flexion: {flexionAngle:F1}° | Extension: {extensionAngle:F1}°");
    }

    IEnumerator WarningAnimationRoutine(string message)
    {
        if (warning == null) yield break;
        warning.text = message;
        warning.gameObject.SetActive(true);
        Color originalColor = warning.color;
        float tiempo = 0.0f;
        while (true)
        {
            tiempo += Time.deltaTime * warningBlinkSpeed;
            float alpha = (Mathf.Sin(tiempo) + 1.0f) / 2.0f; // Oscila entre 0 y 1
            Color nuevoColor = originalColor;
            nuevoColor.a = Mathf.Lerp(0.5f, 1.0f, alpha); // Cambia la transparencia entre 50% y 100%
            warning.color = nuevoColor;
            float scaleMultiplier = Mathf.Lerp(1.0f, warningScaleMultiplier, alpha); // Cambia el tamaño entre 100% y el multiplicador
            warning.transform.localScale = warningOriginalScale * scaleMultiplier;
            yield return null;
        }
    }

    public void StartReminder()
    {
        if (warningCoroutine != null) StopCoroutine(warningCoroutine);
        warningCoroutine = StartCoroutine(WarningAnimationRoutine("Flexiona y extiende tu muñeca para lanzar el ingrediente"));
    }

    public void StopReminder()
    {
        if (warningCoroutine != null)
        {
            StopCoroutine(warningCoroutine);
            warningCoroutine = null;
        }
        if (warning != null)
        {
            warning.gameObject.SetActive(false);
            Color c = warning.color;
            c.a = 1.0f; // Reset alpha to fully visible
            warning.color = c;
        }
    }
}
