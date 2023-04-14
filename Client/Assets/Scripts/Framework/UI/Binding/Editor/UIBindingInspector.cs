// author:KIPKIPS
// date:2023.04.14 17:25
// describe:ui绑定器检视面板
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEditor;
using UnityEngine;

namespace Framework.UI {
    [CustomEditor(typeof(UIBinding))]
    public class UIBindingInspector : Editor {
        private UIBinding _uiBinding;
        // private SerializedProperty _serializedBinderKeyListProperty;
        // private SerializedProperty _serializedBinderTypeListProperty;
        private bool _hasScript;
        private SerializedProperty _scriptSerializedProperty;
        private readonly Dictionary<string, Dictionary<int,string>> _baseBinderDict = new();
        int enumInt;
        public override void OnInspectorGUI() {
            if (_scriptSerializedProperty != null) {
                _hasScript = true;
                EditorGUILayout.PropertyField(_scriptSerializedProperty);
            } else {
                _scriptSerializedProperty = serializedObject.FindProperty("_page");
            }
            if (_hasScript) {
                GUILayout.BeginVertical();
                GUILayout.Space(5);
                if (GUILayout.Button("Check")) {
                    Check();
                }
                GUILayout.Space(5);
                // List<SerializedProperty> serializedProperties = new List<SerializedProperty>(_serializedPropertyDict.Values);
                foreach (var kvp in _baseBinderDict) {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(kvp.Key);
                    if (kvp.Value!=null) {
                        var ints = kvp.Value.Keys.ToArray();
                        var s = kvp.Value.Values.ToArray();
                        if (ints.Length > 0 && s.Length>0) {
                            enumInt=EditorGUILayout.IntPopup(enumInt,s,ints);
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }
            if (GUI.changed) {
                EditorUtility.SetDirty(_uiBinding);
            }
            serializedObject.ApplyModifiedProperties();
        }
        private void OnDisable() {
            _scriptSerializedProperty = null;
        }
        void OnEnable() {
            UIBinding.Register();
            _uiBinding = (UIBinding)target;
            _scriptSerializedProperty = serializedObject.FindProperty("_page");
            
            // _serializedPropertyDict.Clear();
            // Utils.Log(_uiBinding.BinderNameList.Count,_uiBinding.BinderList.Count);
            for (int i = 0; i < _uiBinding.BinderNameList.Count; i++) {
                _baseBinderDict.Add(_uiBinding.BinderNameList[i],UIBinding.GetBinderEnum(UIBinding.GetBaseBinderAtBinder(_uiBinding.BinderList[i]).ToString()));
            }
            // List<SerializedProperty> values = new List<SerializedProperty>();
            // while (valueEnumerator.MoveNext()) {
            //     // values.Add(valueEnumerator.Current.);
            // }
            // for (int i = 0; i < keys.Count; i++) {
            //     _serializedPropertyDict.Add(keys[i],values[i]);
            // }
        }
        private void Check() {
            // _serializedPropertyDict.Clear();
            _uiBinding.BinderNameList.Clear();
            _uiBinding.BinderList.Clear();
            _baseBinderDict.Clear();
            //查字段
            if (_scriptSerializedProperty != null) {
                var serGo = new SerializedObject(EditorUtility.InstanceIDToObject(_scriptSerializedProperty.objectReferenceValue.GetInstanceID()));
                var propertyInfos = _scriptSerializedProperty.objectReferenceValue.GetType().GetFields();
                foreach (var pi in propertyInfos) {
                    foreach (var attribute in pi.GetCustomAttributes()) {
                        if (attribute is not Binder) continue;
                        Binder binderAttr = attribute as Binder;
                        var binder = UIBinding.GetBaseBinderAtBinder(binderAttr);
                        _uiBinding.BinderNameList.Add(pi.Name);
                        // Utils.Log(binderAttr.binderType.ToString());
                        _uiBinding.BinderList.Add(binderAttr.binderType.ToString());
                        // var prop = serGo.FindProperty(pi.Name);
                        // _serializedPropertyDict.Add(pi.Name, prop);
                        _baseBinderDict.Add(pi.Name,UIBinding.GetBinderEnum(binder.ToString()));
                        break;
                    }
                }
            }
            EditorUtility.SetDirty(_uiBinding);
            serializedObject.ApplyModifiedProperties();
        }
    }
}