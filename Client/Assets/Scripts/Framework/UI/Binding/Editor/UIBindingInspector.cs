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
using Object = UnityEngine.Object;

namespace Framework.UI {
    [CustomEditor(typeof(UIBinding))]
    public class UIBindingInspector : Editor {
        private UIBinding _uiBinding;
        private bool _hasScript;
        private SerializedProperty _scriptSerializedProperty;
        class BindDataWrapper {
            public BinderData bindData;
            public Dictionary<int, string> enumDict;
            public Dictionary<int, Component> componentEnum;
            public Dictionary<int, string> componentNameDict;
        }
        private readonly List<BindDataWrapper> _bindDataWrapperList = new();
      
        private int _lastInstanceID;
        private int _lastBindComponentId = -1;
        private Dictionary<string,Object> _curSelectObjDict = new Dictionary<string, Object>();
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
                GUILayout.Space(10);
                EditorGUILayout.LabelField($"Bind Fields ({_bindDataWrapperList.Count})",new GUIStyle {
                    fontStyle = FontStyle.Bold,
                    normal = {
                        textColor = Color.white
                    }
                });
                foreach (var wrapperData in _bindDataWrapperList) {
                    GUILayout.BeginVertical();
                    var bindData = wrapperData.bindData;
                    EditorGUILayout.LabelField(bindData.bindKey);
                    var contextRect = EditorGUILayout.GetControlRect();
                    var width = (contextRect.width - 20 * (3 - 1)) / 3;
                    GUILayout.BeginHorizontal();
                    var rect = new Rect(contextRect.x, contextRect.y, contextRect.width / 3, contextRect.height);
                    if (!_curSelectObjDict.ContainsKey(bindData.bindKey)) {
                        if (bindData.isComponent) {
                            bindData.bindGo = bindData.bindComponent != null ? bindData.bindComponent.gameObject : null;
                            _curSelectObjDict.Add(bindData.bindKey,bindData.bindComponent);
                        } else {
                            _curSelectObjDict.Add(bindData.bindKey,bindData.bindGo);
                        }
                    }
                    _curSelectObjDict[bindData.bindKey] = EditorGUI.ObjectField(rect, _curSelectObjDict[bindData.bindKey],typeof(Object));
                    if (_curSelectObjDict[bindData.bindKey] != null) {
                        if (_curSelectObjDict[bindData.bindKey] is GameObject go) {
                            bindData.isComponent = false;
                            bindData.bindGo = go.gameObject;
                        } else {
                            bindData.isComponent = true;
                            bindData.bindComponent = (Component)_curSelectObjDict[bindData.bindKey];
                            bindData.bindGo = bindData.bindComponent.gameObject;
                            bindData.bindComponentId = UIBinding.GetRegisterBinderId(bindData.bindComponent.GetType().ToString());
                        }
                    }
                    rect.x += width + 20;
                    if (bindData.bindGo == null) {
                        bindData.bindComponent = null;
                        bindData.bindFieldId = -1;
                    } else {
                        if (_curSelectObjDict[bindData.bindKey].GetInstanceID() != _lastInstanceID) {
                            CollectComponent(bindData.bindGo,wrapperData);
                        }
                    }
                    _lastInstanceID = _curSelectObjDict[bindData.bindKey] != null ? _curSelectObjDict[bindData.bindKey].GetInstanceID() : 0;
                    var componentKeys = wrapperData.componentNameDict.Keys.ToArray();
                    var componentEnumNames = wrapperData.componentNameDict.Values.ToArray();
       
                    bindData.bindComponentId = EditorGUI.IntPopup(rect, bindData.bindComponentId, componentEnumNames, componentKeys);
                    bindData.bindComponent = wrapperData.componentEnum.ContainsKey(bindData.bindComponentId) ? wrapperData.componentEnum[bindData.bindComponentId] : null;
                    if (bindData.bindComponentId != _lastBindComponentId) {
                        CollectComponentEnum(bindData.bindComponentId,wrapperData);
                    }
                    _lastBindComponentId = bindData.bindComponentId;
                    rect.x += width + 20;
                    rect.width -= 13;
                    var fieldKeys = wrapperData.enumDict.Keys.ToArray();
                    var fieldEnumNames = wrapperData.enumDict.Values.ToArray();
        
                    bindData.bindFieldId = EditorGUI.IntPopup(rect, bindData.bindFieldId, fieldEnumNames, fieldKeys);
                    GUILayout.EndHorizontal();
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
        void CollectComponentEnum(int componentId,BindDataWrapper wrapperData) {
            if (wrapperData.componentNameDict.ContainsKey(componentId)) {
                wrapperData.enumDict = UIBinding.GetBinderEnum(UIBinding.GetBaseBinderAtBinder(wrapperData.componentNameDict[componentId]));
                
            } else {
                wrapperData.enumDict = new Dictionary<int, string>();
            }
        }
        void CollectComponent(GameObject go,BindDataWrapper wrapperData) {
            var componentDict = new Dictionary<int,Component>();
            var componentNameDict = new Dictionary<int,string>();
            if (go != null) {
                foreach (var component in go.GetComponents<Component>()) {
                    var compoName = component.GetType().ToString();
                    if (UIBinding.IsRegisterComponent(compoName)) {
                        componentDict.Add(UIBinding.GetRegisterBinderId(compoName),component);
                        componentNameDict.Add(UIBinding.GetRegisterBinderId(compoName),component.GetType().ToString());
                    }
                }
            }
            wrapperData.componentEnum = componentDict;
            wrapperData.componentNameDict = componentNameDict;
        }
        void OnEnable() {
            UIBinding.Register();
            _uiBinding = (UIBinding)target;
            _scriptSerializedProperty = serializedObject.FindProperty("_page");
            foreach (var binderData in _uiBinding.BinderDataList) {
                Dictionary<int,string> dict = new();
                var componentDict = new Dictionary<int, Component>();
                var componentNameDict = new Dictionary<int, string>();
                if (binderData.bindGo == null) {
                    binderData.bindGo = binderData.bindComponent != null ? binderData.bindComponent.gameObject : null;
                }
                if (binderData.bindGo != null) {
                    foreach (var component in binderData.bindGo.GetComponents<Component>()) {
                        var compoName = component.GetType().ToString();
                        if (UIBinding.IsRegisterComponent(compoName)) {
                            // Utils.Log(UIBinding.GetRegisterBinderId(compoName),component.GetType().ToString());
                            componentDict.Add(UIBinding.GetRegisterBinderId(compoName),component);
                            componentNameDict.Add(UIBinding.GetRegisterBinderId(compoName),component.GetType().ToString());
                        }
                    }
                    if (componentNameDict.ContainsKey(binderData.bindComponentId)) {
                        dict = UIBinding.GetBinderEnum(UIBinding.GetBaseBinderAtBinder(componentNameDict[binderData.bindComponentId]));
                    }
                }
                _bindDataWrapperList.Add(new BindDataWrapper {
                    bindData = binderData,
                    enumDict = dict,
                    componentEnum = componentDict,
                    componentNameDict = componentNameDict,
                });
            }
        }
        private void Check() {
            // _serializedPropertyDict.Clear();
            Dictionary<string, BinderData> cacheBindMap = new Dictionary<string, BinderData>();
            foreach (var bindData in _uiBinding.BinderDataList) {
                cacheBindMap.Add(bindData.bindKey, bindData);
            }
            _uiBinding.BinderDataList.Clear();
            _bindDataWrapperList.Clear();
            //查字段
            if (_scriptSerializedProperty != null) {
                var serGo = new SerializedObject(EditorUtility.InstanceIDToObject(_scriptSerializedProperty.objectReferenceValue.GetInstanceID()));
                var propertyInfos = _scriptSerializedProperty.objectReferenceValue.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance|BindingFlags.Public);
                foreach (var pi in propertyInfos) {
                    foreach (var attribute in pi.GetCustomAttributes()) {
                        if (attribute is not Binder) continue;
                        Binder binderAttr = attribute as Binder;
                        var binder = UIBinding.GetBaseBinderAtBinder(binderAttr);
                        bool isExist = cacheBindMap.ContainsKey(pi.Name);
                        var cache = isExist ? cacheBindMap[pi.Name] : null;
                        var data = new BinderData {
                            bindKey = pi.Name,
                            bindComponentId = isExist ? cache.bindComponentId : 0,
                            bindFieldId = isExist ? cache.bindFieldId : 0,
                            bindComponent = isExist ? cache.bindComponent : null,
                            bindGo = isExist ? cache.bindGo : null,
                            isComponent = isExist && cache.isComponent,
                        };
                        // Utils.Log(pi.Name, binderAttr.binderType.ToString());
                        _uiBinding.BinderDataList.Add(data);
                        var componentDict = new Dictionary<int,Component>();
                        var componentNameDict = new Dictionary<int,string>();
                        if (cache.bindGo != null) {
                            foreach (var component in cache.bindGo.GetComponents<Component>()) {
                                var compoName = component.GetType().ToString();
                                if (UIBinding.IsRegisterComponent(compoName)) {
                                    // Utils.Log(UIBinding.GetRegisterBinderId(compoName),component.GetType().ToString());
                                    componentDict.Add(UIBinding.GetRegisterBinderId(compoName),component);
                                    componentNameDict.Add(UIBinding.GetRegisterBinderId(compoName),component.GetType().ToString());
                                }
                            }
                        }
                        _bindDataWrapperList.Add(new BindDataWrapper {
                            bindData = data,
                            enumDict = UIBinding.GetBinderEnum(binder.ToString()),
                            componentEnum = componentDict,
                            componentNameDict = componentNameDict,
                        });
                        break;
                    }
                }
            }
            EditorUtility.SetDirty(_uiBinding);
            serializedObject.ApplyModifiedProperties();
        }
    }
}