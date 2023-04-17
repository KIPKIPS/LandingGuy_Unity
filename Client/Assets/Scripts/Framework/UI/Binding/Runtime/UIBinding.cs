// author:KIPKIPS
// date:2023.04.14 16:15
// describe:ui绑定器
using System;
using System.Collections.Generic;
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

        public List<BinderData> BinderDataList => _binderDataList ??= new List<BinderData>();//filed

        //查询用数据
        private static readonly Dictionary<string, BaseBinder> _registerBinderDict = new();//Framework.UI.LImage:Framework.UI.LImageBinder
        private static readonly Dictionary<string, Dictionary<int, string>> _registerBinderEnumDict = new();//Framework.UI.LImageBinder:Enums
        private static readonly HashSet<string> _binderNameMap = new();//Framework.UI.LImageBinder
        private static readonly Dictionary<string, Type> _binderMap = new();//Framework.UI.LImageBinder
        private static readonly Dictionary<string, int> _registerBinderIdDict = new();
        public static void Register() {
            _registerBinderDict.Clear();
            _registerBinderEnumDict.Clear();
            _binderNameMap.Clear();
            _binderMap.Clear();
            _registerBinderIdDict.Clear();
            int allocateId = 0;
            Assembly asm = Assembly.GetAssembly(typeof(BinderParams));
            Type[] types = asm.GetExportedTypes();
            foreach (var t in types) {
                var splits = t.ToString().Split("+");
                var binderName = splits.Length > 0 ? splits[0] : "";
                if (t.IsEnum && _binderNameMap.Contains(binderName) && !_registerBinderEnumDict.ContainsKey(binderName)) {
                    var dict = new Dictionary<int, string>();
                    //todo:sort
                    var array = t.GetEnumNames();
                    for (int i = 0; i < array.Length; i++) {
                        dict.Add(i, array[i]);
                    }
                    _registerBinderEnumDict.Add(binderName, dict);
                }
                var o = Attribute.GetCustomAttributes(t, true);
                foreach (Attribute a in o) {
                    if (a is Binder binderParams && !_registerBinderDict.ContainsKey(binderParams.binderType.ToString())) {
                        var binder = Activator.CreateInstance(t);
                        _binderNameMap.Add(binder.ToString());
                        _registerBinderDict.Add(binderParams.binderType.ToString(), (BaseBinder)binder);
                        _registerBinderIdDict.Add(binderParams.binderType.ToString(),allocateId);
                        allocateId++;
                        _binderMap.Add(binderParams.binderType.ToString(),binderParams.binderType);
                    }
                }
            }
            // Assembly binderAssembly = Assembly.GetAssembly(typeof(Binder));
            // Type[] binderTypes = binderAssembly.GetExportedTypes();
            // foreach (var t in binderTypes) {
            //     
            // }
            // Utils.Log(_registerBinderDict);
            // Utils.Log(_registerBinderEnumDict);
        }
        public static Dictionary<int, string> GetBinderEnum(BaseBinder binder) {
            return GetBinderEnum(binder.ToString());
        }
        public static Dictionary<int, string> GetBinderEnum(string binderName) {
            return _registerBinderEnumDict.ContainsKey(binderName) ? _registerBinderEnumDict[binderName] : null;
        }
        public static BaseBinder GetBaseBinderAtBinder(Binder binder) {
            return GetBaseBinderAtBinder(binder.binderType.ToString());
        }
        public static BaseBinder GetBaseBinderAtBinder(string binderName) {
            return _registerBinderDict.ContainsKey(binderName) ? _registerBinderDict[binderName] : null;
        }
        public static bool IsRegisterComponent(string binderName) {
            return _registerBinderDict.ContainsKey(binderName);
        }
        public static Type GetBinderType(string binderType) {
            return _binderMap.ContainsKey(binderType) ? _binderMap[binderType] : null;
        }
        public static int GetRegisterBinderId(string bindName) {
            return _registerBinderIdDict.ContainsKey(bindName) ? _registerBinderIdDict[bindName] : -1;
        }
    }
}