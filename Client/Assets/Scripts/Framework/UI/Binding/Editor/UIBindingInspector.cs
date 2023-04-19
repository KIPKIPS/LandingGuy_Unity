// author:KIPKIPS
// date:2023.04.14 17:25
// describe:ui绑定器检视面板
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
            public Dictionary<string,int> fieldEnumDict;
            public Dictionary<int, Component> componentEnumDict;
            public Dictionary<int, string> componentDisplayDict;
        }
        private readonly List<BindDataWrapper> _bindDataWrapperList = new();
      
        private int _lastSelectObjInstanceID;
        private int _lastBindComponentId = -1;
        private readonly Dictionary<string,Object> _curSelectObjDict = new();
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
                        if (_curSelectObjDict[bindData.bindKey].GetInstanceID() != _lastSelectObjInstanceID) {
                            var componentDict = new Dictionary<int,Component>();
                            var componentDisplayDict = new Dictionary<int,string>();
                            if (bindData.bindGo != null) {
                                foreach (var component in bindData.bindGo.GetComponents<Component>()) {
                                    var compoName = component.GetType().ToString();
                                    if (UIBinding.IsRegisterComponent(compoName)) {
                                        componentDict.Add(UIBinding.GetRegisterBinderId(compoName),component);
                                        componentDisplayDict.Add(UIBinding.GetRegisterBinderId(compoName),component.GetType().ToString());
                                    }
                                }
                            }
                            wrapperData.componentEnumDict = componentDict;
                            wrapperData.componentDisplayDict = componentDisplayDict;
                        }
                    }
                    _lastSelectObjInstanceID = _curSelectObjDict[bindData.bindKey] != null ? _curSelectObjDict[bindData.bindKey].GetInstanceID() : 0;
                    var componentKeys = wrapperData.componentDisplayDict.Keys.ToArray();
                    var componentEnumNames = GetDisplayEnums(wrapperData.componentDisplayDict.Values.ToArray());
       
                    bindData.bindComponentId = EditorGUI.IntPopup(rect, bindData.bindComponentId, componentEnumNames, componentKeys);
                    bindData.bindComponent = wrapperData.componentEnumDict.ContainsKey(bindData.bindComponentId) ? wrapperData.componentEnumDict[bindData.bindComponentId] : null;
                    if (bindData.bindComponentId != _lastBindComponentId && bindData.bindComponent != null) {
                        wrapperData.fieldEnumDict = UIBinding.GetComponentBindableField(bindData.bindComponent.GetType().ToString());
                    }
                    _lastBindComponentId = bindData.bindComponentId;
                    rect.x += width + 20;
                    rect.width -= 13;
                    if (wrapperData.fieldEnumDict != null) {
                        var fieldKeys = wrapperData.fieldEnumDict.Values.ToArray();
                        var fieldEnumNames = wrapperData.fieldEnumDict.Keys.ToArray();
            
                        bindData.bindFieldId = EditorGUI.IntPopup(rect, bindData.bindFieldId, fieldEnumNames, fieldKeys);
                    } else {
                        bindData.bindFieldId = -1;
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
                newArr[idx]= str.Split(".")[^1];
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
            foreach (var binderData in _uiBinding.BinderDataList) {
                Dictionary<string,int> dict = new();
                var componentDict = new Dictionary<int, Component>();
                var componentDisplayDict = new Dictionary<int, string>();
                if (binderData.bindGo == null) {
                    binderData.bindGo = binderData.bindComponent != null ? binderData.bindComponent.gameObject : null;
                }
                if (binderData.bindGo != null) {
                    foreach (var component in binderData.bindGo.GetComponents<Component>()) {
                        var compoName = component.GetType().ToString();
                        if (!UIBinding.IsRegisterComponent(compoName)) continue;
                        componentDict.Add(UIBinding.GetRegisterBinderId(compoName),component);
                        componentDisplayDict.Add(UIBinding.GetRegisterBinderId(compoName),component.GetType().ToString());
                    }
                    if (componentDisplayDict.ContainsKey(binderData.bindComponentId)) {
                        dict = UIBinding.GetComponentBindableField(binderData.bindComponent.GetType().ToString());
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
            // _serializedPropertyDict.Clear();
            Dictionary<string, BinderData> cacheBindMap = _uiBinding.BinderDataList.ToDictionary(bindData => bindData.bindKey);
            _uiBinding.BinderDataList.Clear();
            _bindDataWrapperList.Clear();
            //查字段
            if (_scriptSerializedProperty != null) {
                var serGo = new SerializedObject(EditorUtility.InstanceIDToObject(_scriptSerializedProperty.objectReferenceValue.GetInstanceID()));
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
                        bindComponentId = isExist ? cache.bindComponentId : 0,
                        bindFieldId = isExist ? cache.bindFieldId : 0,
                        bindComponent = isExist ? cache.bindComponent : null,
                        bindGo = isExist ? cache.bindGo : null,
                        isComponent = isExist && cache.isComponent,
                    };
                    // Utils.Log();
                    _uiBinding.BinderDataList.Add(data);
                    var componentDict = new Dictionary<int,Component>();
                    var componentDisplayDict = new Dictionary<int,string>();
                    if (cache != null && cache.bindGo != null) {
                        foreach (var component in cache.bindGo.GetComponents<Component>()) {
                            var compoName = component.GetType().ToString();
                            if (UIBinding.IsRegisterComponent(compoName)) {
                                componentDict.Add(UIBinding.GetRegisterBinderId(compoName),component);
                                componentDisplayDict.Add(UIBinding.GetRegisterBinderId(compoName),component.GetType().ToString());
                            }
                        }
                    }
                    var enumDict = new Dictionary<string, int>();
                    var compoId = isExist ? cache.bindComponentId : 0;
                    if (componentDict.ContainsKey(compoId)) {
                        enumDict = UIBinding.GetComponentBindableField(componentDict[compoId].GetType().Name);
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