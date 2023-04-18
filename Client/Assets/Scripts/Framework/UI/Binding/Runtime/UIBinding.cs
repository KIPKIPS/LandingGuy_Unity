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
    [RequireComponent(typeof(BasePage))]
    public class UIBinding : MonoBehaviour {
        [SerializeField] private BasePage _page;
        [SerializeField] private List<BinderData> _binderDataList;
        public List<BinderData> BinderDataList => _binderDataList ??= new List<BinderData>();
        
        private static readonly Dictionary<string, BindInfo> _registerBinderDict = new();//Framework.UI.LImage:BindInfo

        private class BindInfo {
            public BaseBinder baseBinder;
            public Dictionary<int, string> bindableFieldDict;
            public int id;
        }
        public static void Register() {
            _registerBinderDict.Clear();
            foreach (var t in Assembly.GetAssembly(typeof(BinderComponent)).GetExportedTypes()) {
                var o = Attribute.GetCustomAttributes(t, true);
                foreach (Attribute a in o) {
                    if (a is BinderComponent component) {
                        var binder = Activator.CreateInstance(t);
                        var key = component.binderType.ToString();
                        if (!_registerBinderDict.ContainsKey(key)) {
                            _registerBinderDict.Add(key,new() {
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
                        var dict = new Dictionary<int, string>();
                        var array = t.GetEnumNames();//todo:sort
                        for (int i = 0; i < array.Length; i++) {
                            dict.Add(i, array[i]);
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
        }
        public static BaseBinder GetBaseBinder(string componentType) {
            return _registerBinderDict.ContainsKey(componentType) ? _registerBinderDict[componentType].baseBinder : null;
        }
        public static Dictionary<int, string> GetComponentBindableField(string componentType) {
            return _registerBinderDict.ContainsKey(componentType) ? _registerBinderDict[componentType].bindableFieldDict : null;
        }
        public static bool IsRegisterComponent(string binderName) {
            return _registerBinderDict.ContainsKey(binderName);
        }
        public static int GetRegisterBinderId(string bindName) {
            return _registerBinderDict.ContainsKey(bindName) ? _registerBinderDict[bindName].id : -1;
        }
    }
}