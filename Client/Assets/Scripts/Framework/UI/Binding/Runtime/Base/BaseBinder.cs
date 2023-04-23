// author:KIPKIPS
// date:2023.04.17 11:52
// describe:绑定基类
using UnityEngine;
using UnityEngine.Events;

namespace Framework.UI {
    public abstract class BaseBinder {
        public virtual void SetVector2(UnityEngine.Object mono, int linkerType, Vector2 value) {
        }
        public virtual void SetVector3(UnityEngine.Object mono, int linkerType, Vector3 value) {
        }
        public virtual void SetQuaternion(UnityEngine.Object mono, int linkerType, Quaternion value) {
        }
        public virtual void SetBoolean(UnityEngine.Object mono, int linkerType, bool value) {
        }
        public virtual void SetInt32(UnityEngine.Object mono, int linkerType, int value) {
        }

        public virtual void SetString(Object mono, int linkerType, string value) {
        }
        public virtual void SetSingle(UnityEngine.Object mono, int linkerType, float value) {
        }
        public virtual void SetColor(UnityEngine.Object mono, int linkerType, Color value) {
        }
        public virtual void SetSprite(UnityEngine.Object mono, int linkerType, Sprite value) {
        }
        public virtual void SetChar(UnityEngine.Object mono, int linkerType, char value) {
        }
        public virtual void SetRect(UnityEngine.Object mono, int linkerType, Rect value) {
        }
        public virtual void SetAction(UnityEngine.Object mono, int linkerType, UnityAction action) {
        }
        public virtual void RemoveAction(UnityEngine.Object mono, int linkerType, UnityAction action) {
        }
        public virtual void SetActionBoolean(UnityEngine.Object mono, int linkerType, UnityAction<bool> action) {
        }
        public virtual void RemoveActionBoolean(UnityEngine.Object mono, int linkerType, UnityAction<bool> action) {
        }
        public virtual void SetActionVector2(UnityEngine.Object mono, int linkerType, UnityAction<Vector2> action) {
        }
        public virtual void RemoveActionVector2(UnityEngine.Object mono, int linkerType, UnityAction<Vector2> action) {
        }
        public virtual void SetActionSingle(UnityEngine.Object mono, int linkerType, UnityAction<float> action) {
        }
        public virtual void RemoveActionSingle(UnityEngine.Object mono, int linkerType, UnityAction<float> action) {
        }

        public virtual void SetActionInt32(UnityEngine.Object mono, int linkerType, UnityAction<int> action) {
        }
        public virtual void RemoveActionInt32(UnityEngine.Object mono, int linkerType, UnityAction<int> action) {
        }
        public virtual void SetActionString(UnityEngine.Object mono, int linkerType, UnityAction<string> action) {
        }
        public virtual void RemoveActionString(UnityEngine.Object mono, int linkerType, UnityAction<string> action) {
        }
        public virtual void SetSystemObject(UnityEngine.Object mono, int linkerType, System.Object value) {
        }
        public virtual void RemoveAllAction(UnityEngine.Object mono) {
        }
        public virtual void Dispose() {
        }
    }
}