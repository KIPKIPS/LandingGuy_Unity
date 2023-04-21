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
        
        private static Dictionary<string, BindInfo> _registerBinderDict;//Framework.UI.LImage:BindInfo
        private static Dictionary<string, BindInfo> RegisterBinderDict => _registerBinderDict ??= new Dictionary<string, BindInfo>();
        private class BindInfo {
            public BaseBinder baseBinder;
            public Dictionary<string,int> bindableFieldDict;
            public int id;
        }
        public static void Register() {
            RegisterBinderDict.Clear();
            foreach (var t in Assembly.GetAssembly(typeof(BinderComponent)).GetExportedTypes()) {
                var o = Attribute.GetCustomAttributes(t, true);
                foreach (Attribute a in o) {
                    if (a is BinderComponent component) {
                        var binder = Activator.CreateInstance(t);
                        var key = component.binderType.ToString();
                        if (!RegisterBinderDict.ContainsKey(key)) {
                            RegisterBinderDict.Add(key,new BindInfo {
                                baseBinder = (BaseBinder)binder,
                                id = key.GetHashCode(),
                            });
                        } else {
                            var info = RegisterBinderDict[key];
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
                        if (!RegisterBinderDict.ContainsKey(key)) {
                            RegisterBinderDict.Add(key,new() {
                                bindableFieldDict = dict,
                            });
                        } else {
                            RegisterBinderDict[key].bindableFieldDict = dict;
                        }
                    }
                }
            }
        }
        public static BaseBinder GetBaseBinder(string componentType) {
            return RegisterBinderDict.ContainsKey(componentType) ? RegisterBinderDict[componentType].baseBinder : null;
        }
        public static Dictionary<string,int> GetComponentBindableField(string componentType) {
            // Utils.Log(componentType);
            // foreach (var kv in RegisterBinderDict) {
            //     Utils.Log(kv.Key,kv.Value.id,kv.Value.baseBinder);
            //     foreach (var _ in kv.Value.bindableFieldDict) {
            //         Utils.Log(_.Key,_.Value);
            //     }
            // }
            return RegisterBinderDict.ContainsKey(componentType) ? RegisterBinderDict[componentType].bindableFieldDict : null;
        }
        public static bool IsRegisterComponent(string binderName) {
            return RegisterBinderDict.ContainsKey(binderName);
        }
        public static int GetRegisterBinderId(string bindName) {
            return RegisterBinderDict.ContainsKey(bindName) ? RegisterBinderDict[bindName].id : -1;
        }
    }
}