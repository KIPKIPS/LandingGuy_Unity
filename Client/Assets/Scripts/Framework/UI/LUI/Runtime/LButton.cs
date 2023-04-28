// author:KIPKIPS
// date:2023.04.21 22:58
// describe:扩展Button
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Framework.UI {
    [AddComponentMenu("LUI/LButton", 30)]
    public class LButton : Button {
        // protected bool isEnter;
        public UnityAction<Vector2> onPointerEnter;
        public UnityAction<Vector2> onPointerExit;
        public override void OnPointerEnter(PointerEventData eventData) {
            base.OnPointerEnter(eventData);
            // isEnter = true;
            onPointerEnter?.Invoke(eventData.position);
        }
        public override void OnPointerExit(PointerEventData eventData) {
            base.OnPointerExit(eventData);
            // isEnter = false;
            onPointerExit?.Invoke(eventData.position);
        }
    }
}