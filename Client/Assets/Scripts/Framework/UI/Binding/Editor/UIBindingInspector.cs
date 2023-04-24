﻿// author:KIPKIPS
// date:2023.04.14 17:25
// describe:ui绑定器检视面板
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.UI {
    [CustomEditor(typeof(UIBinding))]
    public class UIBindingInspector : Editor {
        private UIBinding _uiBinding;
        private SerializedProperty _pathSerializedProperty;
        private class BindDataWrapper {
            public BinderData bindData;
            public Dictionary<string, int> fieldEnumDict;
            public Dictionary<int, Object> componentEnumDict;
            public Dictionary<int, string> componentDisplayDict;
            public int bindComponentId = int.MaxValue;
            public bool isMethod;
        }
        private readonly List<BindDataWrapper> _bindDataWrapperList = new();
        private readonly Dictionary<string, string> _lastSelectObjDict = new();
        private readonly Dictionary<string, string> _lastBindComponentDict = new();
        private readonly Dictionary<string, Object> _curSelectObjDict = new();
        private bool IsValid {
            get {
                _pathSerializedProperty ??= serializedObject.FindProperty("_pagePath");
                return !string.IsNullOrEmpty(_pathSerializedProperty.stringValue) && File.Exists(Path.Combine(Application.dataPath, $"Scripts/GamePlay/{_pathSerializedProperty.stringValue}.cs"));
            }
        }
        public override void OnInspectorGUI() {
            EditorGUILayout.PropertyField(_pathSerializedProperty);
            if (!IsValid) {
                EditorGUILayout.HelpBox("Invalid script path!", MessageType.Warning, true);
                return;
            }
            GUILayout.BeginVertical();
            GUILayout.Space(5);
            if (GUILayout.Button("Check")) {
                Check();
            }
            GUILayout.Space(10);
            var fields = new List<BindDataWrapper>();
            var methods = new List<BindDataWrapper>();
            foreach (var wrapper in _bindDataWrapperList) {
                if (!wrapper.isMethod) {
                    fields.Add(wrapper);
                } else {
                    methods.Add(wrapper);
                }
            }
            EditorGUILayout.LabelField($"Bind Fields ({fields.Count})", new GUIStyle {
                fontStyle = FontStyle.Bold,
                normal = {
                    textColor = Color.white
                }
            });
            for (var i = 0; i < fields.Count; i++) {
                DrawRow(fields[i]);
            }
            GUILayout.Space(10);
            EditorGUILayout.LabelField($"Bind Methods ({methods.Count})", new GUIStyle {
                fontStyle = FontStyle.Bold,
                normal = {
                    textColor = Color.white
                }
            });
            for (var i = 0; i < methods.Count; i++) {
                DrawRow(methods[i]);
            }
            GUILayout.EndVertical();
            if (GUI.changed) {
                EditorUtility.SetDirty(_uiBinding);
            }
            // serializedObject.ApplyModifiedProperties();
        }
        void DrawRow(BindDataWrapper wrapperData) {
            GUILayout.BeginVertical();
            var bindData = wrapperData.bindData;
            EditorGUILayout.LabelField(bindData.bindKey);
            var contextRect = EditorGUILayout.GetControlRect();
            var width = (contextRect.width - 20 * (3 - 1)) / 3;
            GUILayout.BeginHorizontal();
            var rect = new Rect(contextRect.x, contextRect.y, contextRect.width / 3, contextRect.height);
            if (!_curSelectObjDict.ContainsKey(bindData.bindKey)) {
                _curSelectObjDict.Add(bindData.bindKey, bindData.bindObj);
            }
            var rebuildFieldDict = false;
            _curSelectObjDict[bindData.bindKey] = EditorGUI.ObjectField(rect, _curSelectObjDict[bindData.bindKey], typeof(Object), true);
            if (_curSelectObjDict[bindData.bindKey] != null) {
                bindData.bindObj = _curSelectObjDict[bindData.bindKey];
            } else {
                bindData.bindObj = null;
                bindData.bindFieldId = int.MaxValue;
                wrapperData.componentDisplayDict.Clear();
                wrapperData.componentEnumDict.Clear();
                wrapperData.fieldEnumDict.Clear();
                _lastSelectObjDict[bindData.bindKey] = "";
                _lastBindComponentDict[bindData.bindKey] = "";
                rebuildFieldDict = true;
            }
            rect.x += width + 20;
            if (bindData.bindObj == null) {
                bindData.bindFieldId = int.MaxValue;
                wrapperData.componentDisplayDict.Clear();
                wrapperData.componentEnumDict.Clear();
                wrapperData.fieldEnumDict.Clear();
                _lastSelectObjDict[bindData.bindKey] = "";
                _lastBindComponentDict[bindData.bindKey] = "";
                rebuildFieldDict = true;
            } else {
                if (!_lastSelectObjDict.ContainsKey(bindData.bindKey) || (_curSelectObjDict[bindData.bindKey] != null && _curSelectObjDict[bindData.bindKey].GetInstanceID().ToString() != _lastSelectObjDict[bindData.bindKey])) {
                    var componentDict = new Dictionary<int, Object>();
                    var componentDisplayDict = new Dictionary<int, string>();
                    if (bindData.bindObj != null) {
                        Transform trs = null;
                        if (bindData.bindObj is Component obj) {
                            trs = obj.transform;
                        }else if (bindData.bindObj is GameObject gameObject) {
                            trs = gameObject.transform;
                        }
                        if (trs != null) {
                            foreach (var component in trs.GetComponents<Component>()) {
                                var compoName = GetType(component);
                                // LUtil.Log(compoName);
                                if (UIBinding.IsRegisterComponent(compoName)) {
                                    componentDict.Add(UIBinding.GetRegisterBinderId(compoName), component);
                                    componentDisplayDict.Add(UIBinding.GetRegisterBinderId(compoName), GetType(component));
                                }
                            }
                            componentDict.Add("UnityEngine.GameObject".GetHashCode(),trs.gameObject);
                            componentDisplayDict.Add("UnityEngine.GameObject".GetHashCode(), "GameObject");
                        }
                    }
                    wrapperData.componentEnumDict = componentDict;
                    wrapperData.componentDisplayDict = componentDisplayDict;
                }
            }
            var key = _curSelectObjDict[bindData.bindKey] != null ? _curSelectObjDict[bindData.bindKey].GetInstanceID().ToString() : "";
            if (!_lastSelectObjDict.ContainsKey(bindData.bindKey)) {
                _lastSelectObjDict.Add(bindData.bindKey, key);
            } else {
                _lastSelectObjDict[bindData.bindKey] = key;
            }
            var componentKeys = wrapperData.componentDisplayDict.Keys.ToArray();
            var componentEnumNames = GetDisplayEnums(wrapperData.componentDisplayDict.Values.ToArray());
            wrapperData.bindComponentId = EditorGUI.IntPopup(rect, wrapperData.bindComponentId, componentEnumNames, componentKeys);
            bindData.bindObj = wrapperData.componentEnumDict.ContainsKey(wrapperData.bindComponentId) ? wrapperData.componentEnumDict[wrapperData.bindComponentId] : null;
            if ((rebuildFieldDict || !_lastBindComponentDict.ContainsKey(bindData.bindKey) || (_lastBindComponentDict.ContainsKey(bindData.bindKey) && bindData.bindObj!= null && bindData.bindObj.GetInstanceID().ToString() != _lastBindComponentDict[bindData.bindKey])) && bindData.bindObj != null) {
                wrapperData.fieldEnumDict = UIBinding.GetComponentBindableField(GetType(bindData.bindObj));
            }
            if (bindData.bindObj != null) {
                if (!_lastBindComponentDict.ContainsKey(bindData.bindKey)) {
                    _lastBindComponentDict.Add(bindData.bindKey, GetType(bindData.bindObj));
                } else {
                    _lastBindComponentDict[bindData.bindKey] = GetType(bindData.bindObj);
                }
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
        private string[] GetDisplayEnums(IReadOnlyCollection<string> array) {
            string[] newArr = new string[array.Count];
            int idx = 0;
            foreach (var str in array) {
                newArr[idx] = str.Split(".")[^1];
                idx++;
            }
            return newArr;
        }
        private void OnEnable() {
            UIBinding.Register();
            _bindDataWrapperList.Clear();
            _uiBinding = (UIBinding)target;
            _pathSerializedProperty = serializedObject.FindProperty("_pagePath");
            if (string.IsNullOrEmpty(_pathSerializedProperty.stringValue)) return;
            // Utils.Log(_pathSerializedProperty.stringValue);
            var path = Path.Combine(Application.dataPath, $"Scripts/GamePlay/{_pathSerializedProperty.stringValue}.cs");
            var bindKeyDict = new HashSet<string>();
            var methodMap = new HashSet<string>();
            if (File.Exists(path)) {
                string content = File.ReadAllText(path, Encoding.UTF8);
                var fields = Regex.Matches(content, @"DOBind\s*<\s*.+\s*>\s*\(\s*"".+""\s*\)(\s^\S)*;+");
                // Utils.Log(matches.Count);
                foreach (var match in fields) {
                    if (Regex.IsMatch(@match.ToString(), @""".+""")) {
                        var m = Regex.Match(@match.ToString(), @""".+""");
                        bindKeyDict.Add(m.Value.Replace(@"""", ""));
                    }
                }
                var methods = Regex.Matches(content, @"DOBind\s*(<UnityAction>)?\s*\(\s*"".+""\s*,\S+\)");
                foreach (var match in methods) {
                    if (Regex.IsMatch(@match.ToString(), @""".+""")) {
                        var m = Regex.Match(@match.ToString(), @""".+""");
                        var key = m.Value.Replace(@"""", "");
                        bindKeyDict.Add(key);
                        methodMap.Add(key);
                    }
                }
                var matchNamespace = Regex.Match(content, "namespace.+{").Value.Replace("namespace", "").Replace("{", "").Replace(" ", "");
                var matchClassName = Regex.Match(content, @"class.+:.*BasePage").Value.Replace("class", "").Replace(":", "").Replace("BasePage", "").Replace(" ", "");
                var pageType = $"{matchNamespace}.{matchClassName}";
                _uiBinding.PageType = pageType;
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
                var componentDict = new Dictionary<int, Object>();
                var componentDisplayDict = new Dictionary<int, string>();
                if (binderData.bindObj != null) {
                    Transform trs = null;
                    if (binderData.bindObj is Component obj) {
                        trs = obj.transform;
                    }else if (binderData.bindObj is GameObject gameObject) {
                        trs = gameObject.transform;
                    }
                    if (trs != null) {
                        foreach (var component in trs.GetComponents<Component>()) {
                            var compoName = GetType(component);
                            if (!UIBinding.IsRegisterComponent(compoName)) continue;
                            componentDict.Add(UIBinding.GetRegisterBinderId(compoName), component);
                            componentDisplayDict.Add(UIBinding.GetRegisterBinderId(compoName), GetType(component));
                        }
                        componentDict.Add("UnityEngine.GameObject".GetHashCode(),trs.gameObject);
                        componentDisplayDict.Add("UnityEngine.GameObject".GetHashCode(), "GameObject");
                        dict = UIBinding.GetComponentBindableField(GetType(binderData.bindObj));
                        _lastSelectObjDict.Add(binderData.bindKey, binderData.bindObj.GetInstanceID().ToString());
                        if (binderData.bindObj != null) {
                            _lastBindComponentDict.Add(binderData.bindKey, GetType(binderData.bindObj));
                        }
                    }
                }
                var compoType = GetType(binderData.bindObj);
                _bindDataWrapperList.Add(new BindDataWrapper {
                    bindData = binderData,
                    fieldEnumDict = dict,
                    componentEnumDict = componentDict,
                    componentDisplayDict = componentDisplayDict,
                    isMethod = methodMap.Contains(binderData.bindKey),
                    bindComponentId = UIBinding.GetRegisterBinderId(compoType),
                });
                // }
            }
            // EditorUtility.SetDirty(_uiBinding);
            // serializedObject.ApplyModifiedProperties();
        }
        private void Check() {
            UIBinding.Register();
            Dictionary<string, BinderData> cacheBindMap = new();
            foreach (var data in _uiBinding.BinderDataList) {
                cacheBindMap.Add(data.bindKey, new BinderData {
                    bindKey = data.bindKey,
                    bindFieldId = data.bindFieldId,
                    bindObj = data.bindObj,
                });
            }
            _uiBinding.BinderDataList.Clear();
            _bindDataWrapperList.Clear();
            //查字段
            _lastBindComponentDict.Clear();
            _lastSelectObjDict.Clear();
            if (string.IsNullOrEmpty(_pathSerializedProperty.stringValue)) return;
            var path = Path.Combine(Application.dataPath, $"Scripts/GamePlay/{_pathSerializedProperty.stringValue}.cs");
            if (File.Exists(path)) {
                string content = File.ReadAllText(path, Encoding.UTF8);
                var fields = Regex.Matches(content, @"DOBind\s*<\s*.+\s*>\s*\(\s*"".+""\s*\)(\s^\S)*;+");
                // Utils.Log(matches.Count);
                var checkDict = new HashSet<string>();
                var list = new List<string>();
                foreach (var match in fields) {
                    if (Regex.IsMatch(@match.ToString(), @""".+""")) {
                        var m = Regex.Match(@match.ToString(), @""".+""");
                        var key = m.Value.Replace(@"""", "");
                        list.Add(key);
                    }
                }
                var methods = Regex.Matches(content, @"DOBind\s*(<UnityAction>)?\s*\(\s*"".+""\s*,\S+\)");
                foreach (var match in methods) {
                    if (Regex.IsMatch(@match.ToString(), @""".+""")) {
                        var m = Regex.Match(@match.ToString(), @""".+""");
                        var key = m.Value.Replace(@"""", "");
                        list.Add(key);
                        checkDict.Add(key);
                    }
                }
                // LUtils.Log(list);
                foreach (var key in list) {
                    bool isExist = cacheBindMap.ContainsKey(key);
                    var cache = isExist ? cacheBindMap[key] : null;
                    var data = new BinderData {
                        bindKey = key,
                        // bindComponentId = isExist ? cache.bindComponentId : int.MaxValue,
                        bindFieldId = isExist ? cache.bindFieldId : int.MaxValue,
                        // bindComponent = isExist ? cache.bindComponent : null,
                        // bindGo = isExist ? cache.bindGo : null,
                        // isComponent = isExist && cache.isComponent,
                        bindObj = isExist ? cache.bindObj : null,
                    };
                    // Utils.Log();
                    _uiBinding.BinderDataList.Add(data);
                    var componentDict = new Dictionary<int, Object>();
                    var componentDisplayDict = new Dictionary<int, string>();
                    if (cache != null && cache.bindObj != null) {
                        Transform trs = null;
                        if (cache.bindObj is Component obj) {
                            trs = obj.transform;
                        }else if (cache.bindObj is GameObject gameObject) {
                            trs = gameObject.transform;
                        }
                        if (trs != null) {
                            foreach (var component in trs.GetComponents<Component>()) {
                                var compoName = GetType(component);
                                if (UIBinding.IsRegisterComponent(compoName)) {
                                    componentDict.Add(UIBinding.GetRegisterBinderId(compoName), component);
                                    componentDisplayDict.Add(UIBinding.GetRegisterBinderId(compoName), GetType(component));
                                }
                            }
                            componentDict.Add("UnityEngine.GameObject".GetHashCode(),trs.gameObject);
                            componentDisplayDict.Add("UnityEngine.GameObject".GetHashCode(), "GameObject");
                            _lastSelectObjDict.Add(cache.bindKey, cache.bindObj.GetInstanceID().ToString());
                            if (cache.bindObj != null) {
                                _lastBindComponentDict.Add(cache.bindKey, GetType(cache.bindObj));
                            }
                        }
                    }
                    var enumDict = new Dictionary<string, int>();
                    var compoType = isExist ? GetType(cache.bindObj) : "";
                    if (!string.IsNullOrEmpty(compoType)) {
                        enumDict = UIBinding.GetComponentBindableField(compoType);
                    }
                    _bindDataWrapperList.Add(new BindDataWrapper {
                        bindData = data,
                        fieldEnumDict = enumDict,
                        componentEnumDict = componentDict,
                        componentDisplayDict = componentDisplayDict,
                        isMethod = checkDict.Contains(key),
                        bindComponentId = UIBinding.GetRegisterBinderId(compoType),
                    });
                }
            }
            EditorUtility.SetDirty(_uiBinding);
            // serializedObject.ApplyModifiedProperties();
        }

        private static string GetType(Object obj) {
            return obj is Component ? obj.GetType().ToString() : "UnityEngine.GameObject";
        }
    }
}