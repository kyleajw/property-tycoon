using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RollButtonHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    bool isHeld = false;
    float timer;

    [SerializeField] Slider slider;
    [SerializeField] PlayerManager playerManager;

    public void OnPointerDown(PointerEventData eventData)
    {
        isHeld = true;
        timer = 0;
        StartCoroutine("TimeButtonHeldDown");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHeld = false;
    }
    /// <summary>
    /// While the roll button is held, increment the amount of time the player has held it down. When the button is released, call OnRollButtonReleased() in the <see cref="PlayerManager"/>
    /// </summary>
    IEnumerator TimeButtonHeldDown()
    {
        while (isHeld)
        {
            timer += Time.deltaTime;
            slider.value = timer;
            yield return new WaitForEndOfFrame();
        }
        playerManager.OnRollButtonReleased(Mathf.Clamp(timer, 0.5f, slider.maxValue));
        timer = 0;
        slider.value = 0;
        yield return null;
    }
}
