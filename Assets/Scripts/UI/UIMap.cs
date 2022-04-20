using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIMap : InputField
{
    protected UIGrid grid;
    protected bool isPointerDown;
    protected int characterIndex;
    protected int buttonIndex;

    protected override void Awake()
    {
        grid = transform.parent.GetComponent<UIGrid>();
        base.Awake();
    }

    public override void OnPointerClick(PointerEventData eventData) {}

    public override void OnPointerDown(PointerEventData eventData)
    {
        PointerEventData pointerData = eventData as PointerEventData;

        RectTransform rectTransform = textComponent.rectTransform;
        Vector2 position = pointerData.position;
        Camera camera = pointerData.pressEventCamera;
        buttonIndex = pointerData.pointerId;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, position, camera, out Vector2 mousePos);
        characterIndex = GetCharacterIndexFromPosition(mousePos);
        characterIndex -= (Map.GetY(characterIndex) + 1);

        isPointerDown = true;
        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;
        base.OnPointerUp(eventData);
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        if (isPointerDown)
        {
            if(buttonIndex == -1)
                Map.SetMapSymbol('~', characterIndex, '.');
            else
                Map.SetMapSymbol('.', characterIndex);
        }
    }
}
