// author:KIPKIPS
// date:2023.04.17 11:52
// describe:绑定基类
using UnityEngine;
using UnityEngine.Events;

namespace Framework.UI {
    public abstract class BaseBinder {
        public virtual void SetVector2(Object mono, int linkerType, Vector2 value) {
        }
        public virtual void SetVector3(Object mono, int linkerType, Vector3 value) {
        }
        public virtual void SetQuaternion(Object mono, int linkerType, Quaternion value) {
        }
        public virtual void SetBoolean(Object mono, int linkerType, bool value) {
        }
        public virtual void SetInt32(Object mono, int linkerType, int value) {
        }

        public virtual void SetString(Object mono, int linkerType, string value) {
        }
        public virtual void SetSingle(Object mono, int linkerType, float value) {
        }
        public virtual void SetColor(Object mono, int linkerType, Color value) {
        }
        public virtual void SetSprite(Object mono, int linkerType, Sprite value) {
        }
        public virtual void SetChar(Object mono, int linkerType, char value) {
        }
        public virtual void SetRect(Object mono, int linkerType, Rect value) {
        }
        public virtual void SetAction(Object mono, int linkerType, UnityAction action) {
        }
        public virtual void RemoveAction(Object mono, int linkerType, UnityAction action) {
        }
        public virtual void SetActionBoolean(Object mono, int linkerType, UnityAction<bool> action) {
        }
        public virtual void RemoveActionBoolean(Object mono, int linkerType, UnityAction<bool> action) {
        }
        public virtual void SetActionVector2(Object mono, int linkerType, UnityAction<Vector2> action) {
        }
        public virtual void RemoveActionVector2(Object mono, int linkerType, UnityAction<Vector2> action) {
        }
        public virtual void SetActionSingle(Object mono, int linkerType, UnityAction<float> action) {
        }
        public virtual void RemoveActionSingle(Object mono, int linkerType, UnityAction<float> action) {
        }

        public virtual void SetActionInt32(Object mono, int linkerType, UnityAction<int> action) {
        }
        public virtual void RemoveActionInt32(Object mono, int linkerType, UnityAction<int> action) {
        }
        public virtual void SetActionString(Object mono, int linkerType, UnityAction<string> action) {
        }
        public virtual void RemoveActionString(Object mono, int linkerType, UnityAction<string> action) {
        }
        public virtual void SetSystemObject(Object mono, int linkerType, System.Object value) {
        }
        public virtual void RemoveAllAction(Object mono) {
        }
        public virtual void Dispose() {
        }
    }
}