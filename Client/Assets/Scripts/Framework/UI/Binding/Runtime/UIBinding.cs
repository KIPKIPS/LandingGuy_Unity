// author:KIPKIPS
// date:2023.04.14 16:15
// describe:ui绑定器
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Framework.UI {
    [Serializable]
    public class BinderData {
        [SerializeField]public string bindKey;
        [SerializeField]public int bindComponentId;
        [SerializeField]public int bindFieldId;
        [SerializeField]public Component bindComponent;
        [SerializeField]public GameObject bindGo;
        [SerializeField]public bool isComponent;
    }
    public class UIBinding : MonoBehaviour {
        public BasePage Page { get; set; }
        public string PageType {
            get => _pageType; set=>_pageType=value; }
        [SerializeField] private string _pageType;
        [SerializeField] private string _pagePath;
        [SerializeField,NonReorderable] private List<BinderData> _binderDataList;
        public List<BinderData> BinderDataList {
            get => _binderDataList ??= new List<BinderData>();
            set => _binderDataList = value;
        }
        private Dictionary<string, BinderData> _binderDataDict;
        public Dictionary<string, BinderData> BinderDataDict {
            get {
                if (_binderDataDict != null) return _binderDataDict;
                _binderDataDict = new Dictionary<string, BinderData>();
                foreach (var data in BinderDataList) {
                    _binderDataDict.Add(data.bindKey,data);
                }
                return _binderDataDict;
            }
        }
        
        private static readonly Dictionary<string, BindInfo> _registerBinderDict = new();//Framework.UI.LImage:BindInfo
        private class BindInfo {
            public BaseBinder baseBinder;
            public Dictionary<string,int> bindableFieldDict;
            public int id;
        }
        private static readonly Dictionary<string, Type> _pageDict = new();
        public static void Register() {
            _registerBinderDict.Clear();
            _pageDict.Clear();
            foreach (var t in Assembly.GetAssembly(typeof(BinderComponent)).GetExportedTypes()) {
                if (t.BaseType == typeof(BasePage)) {
                    if (t.FullName != null && !_pageDict.ContainsKey(t.FullName)) {
                        _pageDict.Add(t.FullName,t);
                    }
                }
                var o = Attribute.GetCustomAttributes(t, true);
                foreach (Attribute a in o) {
                    if (a is BinderComponent component) {
                        var binder = Activator.CreateInstance(t);
                        var key = component.binderType.ToString();
                        if (!_registerBinderDict.ContainsKey(key)) {
                            _registerBinderDict.Add(key,new BindInfo {
                                baseBinder = (BaseBinder)binder,
                                id = key.GetHashCode(),
                            });
                        } else {
                            var info = _registerBinderDict[key];
                            info.baseBinder = (BaseBinder)binder;
                            info.id = key.GetHashCode();
                        }
                    }
                    if (a is BinderField field) {
                        var binder = Activator.CreateInstance(t);
                        var key = field.binderType.ToString();
                        var dict = new Dictionary<string,int>();
                        var nameArray = t.GetEnumNames();
                        var enums = t.GetEnumValues();
                        List<int> enumList = new List<int>();
                        foreach (var e in enums) {
                            enumList.Add((int)e);
                        }
                        var enumArray = enumList.ToArray();
                        for (int i = 0; i < nameArray.Length; i++) {
                            dict.Add(nameArray[i],enumArray[i]);
                        }
                        if (!_registerBinderDict.ContainsKey(key)) {
                            _registerBinderDict.Add(key,new() {
                                bindableFieldDict = dict,
                            });
                        } else {
                            _registerBinderDict[key].bindableFieldDict = dict;
                        }
                    }
                }
            }
            // LUtils.Log("RegisterPageDict",_pageDict);
        }
        public static BaseBinder GetBaseBinder(string componentType) {
            return _registerBinderDict.ContainsKey(componentType) ? _registerBinderDict[componentType].baseBinder : null;
        }
        public static Dictionary<string,int> GetComponentBindableField(string componentType) {
            return _registerBinderDict.ContainsKey(componentType) ? _registerBinderDict[componentType].bindableFieldDict : null;
        }
        public static bool IsRegisterComponent(string binderName) {
            return _registerBinderDict.ContainsKey(binderName);
        }
        public static int GetRegisterBinderId(string bindName) {
            return _registerBinderDict.ContainsKey(bindName) ? _registerBinderDict[bindName].id : -1;
        }
        public static Type GetPageType(string pageType) {
            return _pageDict.ContainsKey(pageType) ? _pageDict[pageType] : typeof(BasePage);
        }
    }
}