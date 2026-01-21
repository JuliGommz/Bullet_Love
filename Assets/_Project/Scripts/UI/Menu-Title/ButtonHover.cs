using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TransparentButtonFeedback : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] float hoverAlpha = 0.2f;
    private Image image;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Color c = image.color;
        c.a = hoverAlpha;
        image.color = c;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Color c = image.color;
        c.a = 0f;
        image.color = c;
    }
}
