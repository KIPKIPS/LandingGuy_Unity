// --[[
//     author:{wkp}
//     time:20:00
// ]]
using Framework.Manager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Framework.UI
{
    [AddComponentMenu("LUI/LDragButton", 38)]
    public class LDragButton : LButton,IBeginDragHandler, IDragHandler, IEndDragHandler{
        public UnityAction<Vector2> onDragBegin;
        public UnityAction<Vector2> onDrag;
        public UnityAction<Vector2> onDragEnd;
        public void OnBeginDrag(PointerEventData eventData) {
            onDragBegin?.Invoke(eventData.position);
        }
        public void OnDrag(PointerEventData eventData) {
            onDrag?.Invoke(eventData.position);
        }
        public void OnEndDrag(PointerEventData eventData) {
            onDragEnd?.Invoke(eventData.position);
        }
    }
}