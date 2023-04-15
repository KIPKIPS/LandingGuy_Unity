// author:KIPKIPS
// date:2023.04.14 17:25
// describe:ui绑定器检视面板
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using NUnit.Framework;
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
        class BindDataWrapper {
            public BinderData bindData;
            public Dictionary<int, string> enumDict;
        }
        private readonly List<BindDataWrapper> _bindDataWrapperList = new();
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
                foreach (var wrapperData in _bindDataWrapperList) {
                    GUILayout.BeginVertical();
                    var bindData = wrapperData.bindData;
                    EditorGUILayout.LabelField(bindData.bindKey);
                    if (!string.IsNullOrEmpty(bindData.bindComponentType)) {
                        var contextRect = EditorGUILayout.GetControlRect();
                        var width = (contextRect.width- 20 ) / 3 ;
                        GUILayout.BeginHorizontal();
                        // EditorGUILayout.LabelField(bindData.bindComponentType);
                        var rect = new Rect(contextRect.x, contextRect.y, contextRect.width / 3, contextRect.height);
                        bindData.bindGo = (GameObject)EditorGUI.ObjectField(rect, bindData.bindGo, typeof(GameObject));
                        rect.x += width + 10;
                        if (bindData.bindGo == null) {
                            bindData.bindComponent = null;
                            bindData.bindEnum = -1;
                        }
                        bindData.bindComponent = (Component)EditorGUI.ObjectField(rect, bindData.bindComponent, UIBinding.GetBinderType(bindData.bindComponentType));
                        rect.x += width + 10;
                        var keys = wrapperData.enumDict.Keys.ToArray();
                        var enumNames = wrapperData.enumDict.Values.ToArray();
                        if (keys.Length > 0 && enumNames.Length>0) {
                            bindData.bindEnum = EditorGUI.IntPopup(rect,bindData.bindEnum,enumNames,keys);
                        }
                        GUILayout.EndHorizontal();
                    } else {
                        EditorGUILayout.LabelField("mount component");
                    }
                    GUILayout.EndVertical();
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
            
            foreach (var binderData in _uiBinding.BinderDataList) {
                var dict = UIBinding.GetBinderEnum(UIBinding.GetBaseBinderAtBinder(binderData.bindComponentType));
                _bindDataWrapperList.Add(new BindDataWrapper {
                    bindData = binderData,
                    enumDict = dict,
                });
            }
        }
        private void Check() {
            // _serializedPropertyDict.Clear();
            Dictionary<string, BinderData> cacheBindMap = new Dictionary<string, BinderData>();
            foreach (var bindData in _uiBinding.BinderDataList) {
                cacheBindMap.Add(bindData.bindKey,bindData);
            }
            _uiBinding.BinderDataList.Clear();
            _bindDataWrapperList.Clear();
            //查字段
            if (_scriptSerializedProperty != null) {
                var serGo = new SerializedObject(EditorUtility.InstanceIDToObject(_scriptSerializedProperty.objectReferenceValue.GetInstanceID()));
                var propertyInfos = _scriptSerializedProperty.objectReferenceValue.GetType().GetFields();
                foreach (var pi in propertyInfos) {
                    foreach (var attribute in pi.GetCustomAttributes()) {
                        if (attribute is not Binder) continue;
                        Binder binderAttr = attribute as Binder;
                        var binder = UIBinding.GetBaseBinderAtBinder(binderAttr);
                        bool isExist = cacheBindMap.ContainsKey(pi.Name);
                        var cache = isExist ? cacheBindMap[pi.Name] : null;
                        var data = new BinderData {
                            bindKey = pi.Name,
                            bindComponentType = binderAttr.binderType.ToString(),
                            bindEnum = isExist ? cache.bindEnum : 0,
                            bindComponent = isExist ? cache.bindComponent : null,
                            bindGo = isExist ? cache.bindGo : null,
                        };
                        Utils.Log(pi.Name,binderAttr.binderType.ToString());
                        _uiBinding.BinderDataList.Add(data);
                        _bindDataWrapperList.Add(new BindDataWrapper {
                            bindData = data,
                            enumDict = UIBinding.GetBinderEnum(binder.ToString())
                        });
                        // var prop = serGo.FindProperty(pi.Name);
                        // _serializedPropertyDict.Add(pi.Name, prop);
                        // _baseBinderDict.Add(pi.Name,UIBinding.GetBinderEnum(binder.ToString()));
                        break;
                    }
                }
            }
            EditorUtility.SetDirty(_uiBinding);
            serializedObject.ApplyModifiedProperties();
        }
    }
}