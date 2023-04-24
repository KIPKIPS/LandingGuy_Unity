// author:KIPKIPS
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
        private const string QuotationMark = @""".+""";
        private const string Quotation = @"""";
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
                (!wrapper.isMethod ? fields : methods).Add(wrapper);
            }
            EditorGUILayout.LabelField($"Bind Fields ({fields.Count})", new GUIStyle {
                fontStyle = FontStyle.Bold,
                normal = {
                    textColor = Color.white
                }
            });
            foreach (var t in fields) {
                DrawRow(t);
            }
            GUILayout.Space(10);
            EditorGUILayout.LabelField($"Bind Methods ({methods.Count})", new GUIStyle {
                fontStyle = FontStyle.Bold,
                normal = {
                    textColor = Color.white
                }
            });
            foreach (var t in methods) {
                DrawRow(t);
            }
            GUILayout.EndVertical();
            if (GUI.changed) {
                EditorUtility.SetDirty(_uiBinding);
            }
        }
        private void DrawRow(BindDataWrapper wrapperData) {
            GUILayout.BeginVertical(EditorStyles.textField);
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
                        var trs = bindData.bindObj switch {
                            Component obj => obj.transform,
                            GameObject gameObject => gameObject.transform,
                            _ => null
                        };
                        if (trs != null) {
                            foreach (var component in trs.GetComponents<Component>()) {
                                var compoName = UIBinding.GetType(component);
                                if (!UIBinding.IsRegisterComponent(compoName)) continue;
                                componentDict.Add(UIBinding.GetRegisterBinderId(compoName), component);
                                componentDisplayDict.Add(UIBinding.GetRegisterBinderId(compoName), UIBinding.GetType(component));
                            }
                            componentDict.Add("UnityEngine.GameObject".GetHashCode(), trs.gameObject);
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
            if ((rebuildFieldDict || !_lastBindComponentDict.ContainsKey(bindData.bindKey) || (_lastBindComponentDict.ContainsKey(bindData.bindKey) && bindData.bindObj != null && bindData.bindObj.GetInstanceID().ToString() != _lastBindComponentDict[bindData.bindKey])) && bindData.bindObj != null) {
                wrapperData.fieldEnumDict = UIBinding.GetComponentBindableField(UIBinding.GetType(bindData.bindObj));
            }
            if (bindData.bindObj != null) {
                if (!_lastBindComponentDict.ContainsKey(bindData.bindKey)) {
                    _lastBindComponentDict.Add(bindData.bindKey, UIBinding.GetType(bindData.bindObj));
                } else {
                    _lastBindComponentDict[bindData.bindKey] = UIBinding.GetType(bindData.bindObj);
                }
            }
            rect.x += width + 20;
            rect.width -= 13;
            bindData.bindFieldId = wrapperData.fieldEnumDict != null ? EditorGUI.IntPopup(rect, bindData.bindFieldId, wrapperData.fieldEnumDict.Keys.ToArray(), wrapperData.fieldEnumDict.Values.ToArray()) : int.MaxValue;
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
        private static string[] GetDisplayEnums(IReadOnlyCollection<string> array) {
            var newArr = new string[array.Count];
            var idx = 0;
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
            var path = Path.Combine(Application.dataPath, $"Scripts/GamePlay/{_pathSerializedProperty.stringValue}.cs");
            var bindKeyDict = new HashSet<string>();
            var methodMap = new HashSet<string>();
            if (File.Exists(path)) {
                var content = File.ReadAllText(path, Encoding.UTF8);
                var fields = Regex.Matches(content, @"DOBind\s*<\s*.+\s*>\s*\(\s*"".+""\s*\)(\s^\S)*;+");
                foreach (var match in fields) {
                    if (!Regex.IsMatch(@match.ToString(), QuotationMark)) continue;
                    var m = Regex.Match(@match.ToString(), QuotationMark);
                    bindKeyDict.Add(m.Value.Replace(Quotation, ""));
                }
                var methods = Regex.Matches(content, @"DOBind\s*(<UnityAction>)?\s*\(\s*"".+""\s*,\S+\)");
                foreach (var match in methods) {
                    if (!Regex.IsMatch(@match.ToString(), QuotationMark)) continue;
                    var m = Regex.Match(@match.ToString(), QuotationMark);
                    var key = m.Value.Replace(Quotation, "");
                    bindKeyDict.Add(key);
                    methodMap.Add(key);
                }
                var matchNamespace = Regex.Match(content, "namespace.+{").Value.Replace("namespace", "").Replace("{", "").Replace(" ", "");
                var matchClassName = Regex.Match(content, @"class.+:.*BasePage").Value.Replace("class", "").Replace(":", "").Replace("BasePage", "").Replace(" ", "");
                var pageType = $"{matchNamespace}.{matchClassName}";
                _uiBinding.PageType = pageType;
            }
            var list = _uiBinding.BinderDataList.Where(binderData => bindKeyDict.Contains(binderData.bindKey)).ToList();
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
                    } else if (binderData.bindObj is GameObject gameObject) {
                        trs = gameObject.transform;
                    }
                    if (trs != null) {
                        foreach (var component in trs.GetComponents<Component>()) {
                            var compoName = UIBinding.GetType(component);
                            if (!UIBinding.IsRegisterComponent(compoName)) continue;
                            componentDict.Add(UIBinding.GetRegisterBinderId(compoName), component);
                            componentDisplayDict.Add(UIBinding.GetRegisterBinderId(compoName), UIBinding.GetType(component));
                        }
                        componentDict.Add("UnityEngine.GameObject".GetHashCode(), trs.gameObject);
                        componentDisplayDict.Add("UnityEngine.GameObject".GetHashCode(), "GameObject");
                        dict = UIBinding.GetComponentBindableField(UIBinding.GetType(binderData.bindObj));
                        _lastSelectObjDict.Add(binderData.bindKey, binderData.bindObj.GetInstanceID().ToString());
                        if (binderData.bindObj != null) {
                            _lastBindComponentDict.Add(binderData.bindKey, UIBinding.GetType(binderData.bindObj));
                        }
                    }
                }
                var compoType = UIBinding.GetType(binderData.bindObj);
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
            var cacheBindMap = _uiBinding.BinderDataList.ToDictionary(data => data.bindKey, data => new BinderData {
                bindKey = data.bindKey, bindFieldId = data.bindFieldId, bindObj = data.bindObj,
            });
            _uiBinding.BinderDataList.Clear();
            _bindDataWrapperList.Clear();
            //查字段
            _lastBindComponentDict.Clear();
            _lastSelectObjDict.Clear();
            if (string.IsNullOrEmpty(_pathSerializedProperty.stringValue)) return;
            var path = Path.Combine(Application.dataPath, $"Scripts/GamePlay/{_pathSerializedProperty.stringValue}.cs");
            if (File.Exists(path)) {
                var content = File.ReadAllText(path, Encoding.UTF8);
                var fields = Regex.Matches(content, @"DOBind\s*<\s*.+\s*>\s*\(\s*"".+""\s*\)(\s^\S)*;+");
                // Utils.Log(matches.Count);
                var checkDict = new HashSet<string>();
                var list = new List<string>();
                foreach (var match in fields) {
                    if (!Regex.IsMatch(@match.ToString(), QuotationMark)) continue;
                    var key = Regex.Match(@match.ToString(), QuotationMark).Value.Replace(Quotation, "");
                    list.Add(key);
                }
                var methods = Regex.Matches(content, @"DOBind\s*(<UnityAction>)?\s*\(\s*"".+""\s*,\S+\)");
                foreach (var match in methods) {
                    if (!Regex.IsMatch(@match.ToString(), QuotationMark)) continue;
                    var key = Regex.Match(@match.ToString(), QuotationMark).Value.Replace(Quotation, "");
                    list.Add(key);
                    checkDict.Add(key);
                }
                foreach (var key in list) {
                    var isExist = cacheBindMap.ContainsKey(key);
                    var cache = isExist ? cacheBindMap[key] : null;
                    var data = new BinderData {
                        bindKey = key,
                        bindFieldId = isExist ? cache.bindFieldId : int.MaxValue,
                        bindObj = isExist ? cache.bindObj : null,
                    };
                    _uiBinding.BinderDataList.Add(data);
                    var componentDict = new Dictionary<int, Object>();
                    var componentDisplayDict = new Dictionary<int, string>();
                    if (cache != null && cache.bindObj != null) {
                        Transform trs = null;
                        if (cache.bindObj is Component obj) {
                            trs = obj.transform;
                        } else if (cache.bindObj is GameObject gameObject) {
                            trs = gameObject.transform;
                        }
                        if (trs != null) {
                            foreach (var component in trs.GetComponents<Component>()) {
                                var compoName = UIBinding.GetType(component);
                                if (!UIBinding.IsRegisterComponent(compoName)) continue;
                                componentDict.Add(UIBinding.GetRegisterBinderId(compoName), component);
                                componentDisplayDict.Add(UIBinding.GetRegisterBinderId(compoName), UIBinding.GetType(component));
                            }
                            componentDict.Add("UnityEngine.GameObject".GetHashCode(), trs.gameObject);
                            componentDisplayDict.Add("UnityEngine.GameObject".GetHashCode(), "GameObject");
                            _lastSelectObjDict.Add(cache.bindKey, cache.bindObj.GetInstanceID().ToString());
                            if (cache.bindObj != null) {
                                _lastBindComponentDict.Add(cache.bindKey, UIBinding.GetType(cache.bindObj));
                            }
                        }
                    }
                    var enumDict = new Dictionary<string, int>();
                    var compoType = isExist ? UIBinding.GetType(cache.bindObj) : "";
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
        }
    }
}