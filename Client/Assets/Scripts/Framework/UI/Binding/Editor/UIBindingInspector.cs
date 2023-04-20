// author:KIPKIPS
// date:2023.04.14 17:25
// describe:ui绑定器检视面板
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.UI {
    [CustomEditor(typeof(UIBinding))]
    public class UIBindingInspector : Editor {
        private UIBinding _uiBinding;
        private bool _hasScript;
        private SerializedProperty _scriptSerializedProperty;
        private class BindDataWrapper {
            public BinderData bindData;
            public Dictionary<string, int> fieldEnumDict;
            public Dictionary<int, Component> componentEnumDict;
            public Dictionary<int, string> componentDisplayDict;
        }
        private readonly List<BindDataWrapper> _bindDataWrapperList = new();
        private Dictionary<string, int> _lastSelectObjDict = new Dictionary<string, int>();
        private Dictionary<string, int> _lastBindComponentDict = new Dictionary<string, int>();
        private readonly Dictionary<string, Object> _curSelectObjDict = new();
        public override void OnInspectorGUI() {
            if (_scriptSerializedProperty != null && _scriptSerializedProperty.objectReferenceValue != null) {
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
                EditorGUILayout.LabelField($"Bind Fields ({_bindDataWrapperList.Count})", new GUIStyle {
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
                            _curSelectObjDict.Add(bindData.bindKey, bindData.bindComponent);
                        } else {
                            _curSelectObjDict.Add(bindData.bindKey, bindData.bindGo);
                        }
                    }
                    var rebuildFieldDict = false;
                    _curSelectObjDict[bindData.bindKey] = EditorGUI.ObjectField(rect, _curSelectObjDict[bindData.bindKey], typeof(Object));
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
                    } else {
                        bindData.bindGo = null;
                        bindData.bindComponent = null;
                        bindData.bindComponentId = int.MaxValue;
                        bindData.bindFieldId = int.MaxValue;
                        wrapperData.componentDisplayDict.Clear();
                        wrapperData.componentEnumDict.Clear();
                        wrapperData.fieldEnumDict.Clear();
                        _lastSelectObjDict[bindData.bindKey] = int.MaxValue;
                        _lastBindComponentDict[bindData.bindKey] = int.MaxValue;
                        rebuildFieldDict = true;
                    }
                    rect.x += width + 20;
                    if (bindData.bindGo == null) {
                        bindData.bindComponent = null;
                        bindData.bindComponentId = int.MaxValue;
                        bindData.bindFieldId = int.MaxValue;
                        wrapperData.componentDisplayDict.Clear();
                        wrapperData.componentEnumDict.Clear();
                        wrapperData.fieldEnumDict.Clear();
                        _lastSelectObjDict[bindData.bindKey] = int.MaxValue;
                        _lastBindComponentDict[bindData.bindKey] = int.MaxValue;
                        rebuildFieldDict = true;
                    } else {
                        if (!_lastSelectObjDict.ContainsKey(bindData.bindKey) || (_curSelectObjDict[bindData.bindKey] != null && _curSelectObjDict[bindData.bindKey].GetInstanceID() != _lastSelectObjDict[bindData.bindKey])) {
                            var componentDict = new Dictionary<int, Component>();
                            var componentDisplayDict = new Dictionary<int, string>();
                            if (bindData.bindGo != null) {
                                foreach (var component in bindData.bindGo.GetComponents<Component>()) {
                                    var compoName = component.GetType().ToString();
                                    if (UIBinding.IsRegisterComponent(compoName)) {
                                        componentDict.Add(UIBinding.GetRegisterBinderId(compoName), component);
                                        componentDisplayDict.Add(UIBinding.GetRegisterBinderId(compoName), component.GetType().ToString());
                                    }
                                }
                            }
                            wrapperData.componentEnumDict = componentDict;
                            wrapperData.componentDisplayDict = componentDisplayDict;
                        }
                    }
                    var key = _curSelectObjDict[bindData.bindKey] != null ? _curSelectObjDict[bindData.bindKey].GetInstanceID() : int.MaxValue;
                    if (!_lastSelectObjDict.ContainsKey(bindData.bindKey)) {
                        _lastSelectObjDict.Add(bindData.bindKey, key);
                    } else {
                        _lastSelectObjDict[bindData.bindKey] = key;
                    }
                    var componentKeys = wrapperData.componentDisplayDict.Keys.ToArray();
                    var componentEnumNames = GetDisplayEnums(wrapperData.componentDisplayDict.Values.ToArray());
                    bindData.bindComponentId = EditorGUI.IntPopup(rect, bindData.bindComponentId, componentEnumNames, componentKeys);
                    bindData.bindComponent = wrapperData.componentEnumDict.ContainsKey(bindData.bindComponentId) ? wrapperData.componentEnumDict[bindData.bindComponentId] : null;
                    if ((rebuildFieldDict || !_lastBindComponentDict.ContainsKey(bindData.bindKey) || (_lastBindComponentDict.ContainsKey(bindData.bindKey) && bindData.bindComponentId != _lastBindComponentDict[bindData.bindKey])) && bindData.bindComponent != null) {
                        wrapperData.fieldEnumDict = UIBinding.GetComponentBindableField(bindData.bindComponent.GetType().ToString());
                    }
                    if (!_lastBindComponentDict.ContainsKey(bindData.bindKey)) {
                        _lastBindComponentDict.Add(bindData.bindKey, bindData.bindComponentId);
                    } else {
                        _lastBindComponentDict[bindData.bindKey] = bindData.bindComponentId;
                    }
                    rect.x += width + 20;
                    rect.width -= 13;
                    if (wrapperData.fieldEnumDict != null) {
                        var fieldKeys = wrapperData.fieldEnumDict.Values.ToArray();
                        var fieldEnumNames = wrapperData.fieldEnumDict.Keys.ToArray();
                        bindData.bindFieldId = EditorGUI.IntPopup(rect, bindData.bindFieldId, fieldEnumNames, fieldKeys);
                    } else {
                        bindData.bindFieldId = int.MaxValue;
                    }
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
        private string[] GetDisplayEnums(IReadOnlyCollection<string> array) {
            string[] newArr = new string[array.Count];
            int idx = 0;
            foreach (var str in array) {
                newArr[idx] = str.Split(".")[^1];
                idx++;
            }
            return newArr;
        }
        private void OnDisable() {
            _scriptSerializedProperty = null;
        }
        private void OnEnable() {
            UIBinding.Register();
            _bindDataWrapperList.Clear();
            _uiBinding = (UIBinding)target;
            _scriptSerializedProperty = serializedObject.FindProperty("_page");
            var fieldInfos = _scriptSerializedProperty.objectReferenceValue.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            var bindKeyDict = new HashSet<string>();
            foreach (var field in fieldInfos) {
                bindKeyDict.Add(field.Name);
            }
            var list = new List<BinderData>();
            foreach (var binderData in _uiBinding.BinderDataList) {
                if (bindKeyDict.Contains(binderData.bindKey)) {
                    list.Add(binderData);
                }
            }
            _uiBinding.BinderDataList = list;
            _lastBindComponentDict.Clear();
            _lastSelectObjDict.Clear();
            foreach (var binderData in _uiBinding.BinderDataList) {
                Dictionary<string, int> dict = new();
                var componentDict = new Dictionary<int, Component>();
                var componentDisplayDict = new Dictionary<int, string>();
                if (binderData.bindGo == null) {
                    binderData.bindGo = binderData.bindComponent != null ? binderData.bindComponent.gameObject : null;
                }
                if (binderData.bindGo != null) {
                    foreach (var component in binderData.bindGo.GetComponents<Component>()) {
                        var compoName = component.GetType().ToString();
                        if (!UIBinding.IsRegisterComponent(compoName)) continue;
                        componentDict.Add(UIBinding.GetRegisterBinderId(compoName), component);
                        componentDisplayDict.Add(UIBinding.GetRegisterBinderId(compoName), component.GetType().ToString());
                    }
                    if (componentDisplayDict.ContainsKey(binderData.bindComponentId)) {
                        dict = UIBinding.GetComponentBindableField(binderData.bindComponent.GetType().ToString());
                    }
                    _lastSelectObjDict.Add(binderData.bindKey, binderData.bindGo.GetInstanceID());
                    if (binderData.bindComponent != null) {
                        _lastBindComponentDict.Add(binderData.bindKey, binderData.bindComponentId);
                    }
                }
                _bindDataWrapperList.Add(new BindDataWrapper {
                    bindData = binderData,
                    fieldEnumDict = dict,
                    componentEnumDict = componentDict,
                    componentDisplayDict = componentDisplayDict,
                });
            }
        }
        private void Check() {
            UIBinding.Register();
            // _serializedPropertyDict.Clear();
            Dictionary<string, BinderData> cacheBindMap = new();
            foreach (var data in _uiBinding.BinderDataList) {
                cacheBindMap.Add(data.bindKey, new() {
                    bindKey = data.bindKey,
                    bindComponentId = data.bindComponentId,
                    bindFieldId = data.bindFieldId,
                    bindComponent = data.bindComponent,
                    bindGo = data.bindGo,
                    isComponent = data.isComponent,
                });
            }
            _uiBinding.BinderDataList.Clear();
            _bindDataWrapperList.Clear();
            //查字段
            _lastBindComponentDict.Clear();
            _lastSelectObjDict.Clear();
            if (_scriptSerializedProperty != null) {
                // var serGo = new SerializedObject(EditorUtility.InstanceIDToObject(_scriptSerializedProperty.objectReferenceValue.GetInstanceID()));
                var fieldInfos = _scriptSerializedProperty.objectReferenceValue.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                foreach (var field in fieldInfos) {
                    //todo:修改
                    var t = field.FieldType;
                    // Utils.Log(t.DeclaringType);
                    if (!t.Name.StartsWith("Bindable")) continue;
                    // var binder = UIBinding.GetBaseBinderAtBinder(binderAttr);
                    bool isExist = cacheBindMap.ContainsKey(field.Name);
                    var cache = isExist ? cacheBindMap[field.Name] : null;
                    var data = new BinderData {
                        bindKey = field.Name,
                        bindComponentId = isExist ? cache.bindComponentId : int.MaxValue,
                        bindFieldId = isExist ? cache.bindFieldId : int.MaxValue,
                        bindComponent = isExist ? cache.bindComponent : null,
                        bindGo = isExist ? cache.bindGo : null,
                        isComponent = isExist && cache.isComponent,
                    };
                    // Utils.Log();
                    _uiBinding.BinderDataList.Add(data);
                    var componentDict = new Dictionary<int, Component>();
                    var componentDisplayDict = new Dictionary<int, string>();
                    if (cache != null && cache.bindGo != null) {
                        foreach (var component in cache.bindGo.GetComponents<Component>()) {
                            var compoName = component.GetType().ToString();
                            if (UIBinding.IsRegisterComponent(compoName)) {
                                componentDict.Add(UIBinding.GetRegisterBinderId(compoName), component);
                                componentDisplayDict.Add(UIBinding.GetRegisterBinderId(compoName), component.GetType().ToString());
                            }
                        }
                        _lastSelectObjDict.Add(cache.bindKey, cache.bindGo.GetInstanceID());
                        if (cache.bindComponent != null) {
                            _lastBindComponentDict.Add(cache.bindKey, cache.bindComponentId);
                        }
                    }
                    var enumDict = new Dictionary<string, int>();
                    var compoId = isExist ? cache.bindComponentId : 0;
                    if (componentDict.ContainsKey(compoId)) {
                        enumDict = UIBinding.GetComponentBindableField(componentDict[compoId].GetType().FullName);
                    }
                    _bindDataWrapperList.Add(new BindDataWrapper {
                        bindData = data,
                        fieldEnumDict = enumDict,
                        componentEnumDict = componentDict,
                        componentDisplayDict = componentDisplayDict,
                    });
                }
            }
            EditorUtility.SetDirty(_uiBinding);
            serializedObject.ApplyModifiedProperties();
        }
    }
}