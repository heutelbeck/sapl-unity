using System;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour {

    private static float flashTimer;
    private static float flashDuration = 1f;

    public static void UpdateTimerDisplay(TextMeshProUGUI textField, float time, bool flash = false) {
        // Sekunden setzen (nicht unter 0)
        float timer = Mathf.Max(0, time);
        // Aus Sekunden TimeSpan ermitteln
        TimeSpan timeSpan = System.TimeSpan.FromSeconds(timer);
        // Textfeld aktualisieren
        textField.text = timeSpan.ToString(@"hh\:mm\:ss");
        // Ggf. bei 0 Sekunden Flash-Methode aufrufen
        if (timer == 0 && flash == true) {
            Flash(textField);
        } else {
            textField.enabled = true;
        }
    }

    public static void Flash(TextMeshProUGUI textField) {
        // Falls Timer <= 0: Flash-Timer mit Flash-Dauer setzen
        if (flashTimer <= 0) {
            flashTimer = flashDuration;
        // Falls Flash-Timer gesetzt und noch nicht mehr als halbe Flash-Dauer abgelaufen
        } else if (flashTimer >= flashDuration / 2) {
            // Flash-Timer verringern
            flashTimer -= Time.deltaTime;
            // Textfeld ausblenden
            textField.enabled = false;
        // Falls Flash-Timer gesetzt und mindestens halbe Flash-Dauer abgelaufen
        } else {
            // Flash-Timer verringern
            flashTimer -= Time.deltaTime;
            // Textfeld einblenden
            textField.enabled = true;
        }
    }

}