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

    IEnumerator TimeButtonHeldDown()
    {
        while (isHeld)
        {
            timer += Time.deltaTime;
            slider.value = timer;
            yield return new WaitForEndOfFrame();
        }
        playerManager.OnRollButtonReleased(Mathf.Clamp(timer, 0, slider.maxValue));
        timer = 0;
        slider.value = 0;
        yield return null;
    }
}
