// author:KIPKIPS
// date:2023.04.14 16:15
// describe:ui绑定器
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Framework.UI {
    [RequireComponent(typeof(BasePage))]
    public class UIBinding : MonoBehaviour {
        [SerializeField]private BasePage _page;
        [SerializeField]private List<string> _binderNameList; 
        [SerializeField]private List<string> _binderList ;
        public List<string> BinderNameList =>_binderNameList??=new List<string>();
        public List<string> BinderList => _binderList ??= new List<string>();
        private static readonly Dictionary<string, BaseBinder> _registerBinderDict = new();
        private static readonly Dictionary<string, Dictionary<int,string>> _registerBinderEnumDict = new();
        private static readonly HashSet<string> _binderNameMap = new();
        public static void Register() {
            // _registerBinderDict.Clear();
            // _registerBinderEnumDict.Clear();
            Assembly asm = Assembly.GetAssembly(typeof(BinderParams));
            Type[] types = asm.GetExportedTypes();
            foreach (var t in types) {
                var o = Attribute.GetCustomAttributes(t, true);
                foreach (Attribute a in o) {
                    if (a is Binder binderParams && !_registerBinderDict.ContainsKey(binderParams.binderType.ToString())) {
                        var binder = Activator.CreateInstance(t);
                        _binderNameMap.Add(binder.ToString());
                        _registerBinderDict.Add(binderParams.binderType.ToString(), (BaseBinder)binder);
                    }
                }
            }
            Assembly binderAssembly = Assembly.GetAssembly(typeof(Binder));
            Type[] binderTypes = binderAssembly.GetExportedTypes();
            foreach (var t in binderTypes) {
                var splits = t.ToString().Split("+");
                var binderName = splits.Length > 0 ? splits[0] : "";
                if (t.IsEnum &&_binderNameMap.Contains(binderName)&& !_registerBinderEnumDict.ContainsKey(binderName)) {
                    var dict = new Dictionary<int, string>();
                    var array = t.GetEnumNames();
                    for (int i = 0; i < array.Length; i++) {
                        dict.Add(i,array[i]);
                    }
                    _registerBinderEnumDict.Add(binderName,dict);
                }
            }
            Utils.Log(_registerBinderDict);
            Utils.Log(_registerBinderEnumDict);
        }
        public static Dictionary<int,string> GetBinderEnum(string binderName) {
            return _registerBinderEnumDict.ContainsKey(binderName) ? _registerBinderEnumDict[binderName] : null;
        }

        public static BaseBinder GetBaseBinderAtBinder(Binder binder) {
            var key = binder.binderType.ToString();
            return _registerBinderDict.ContainsKey(key) ? _registerBinderDict[key] : null;
        }
        public static BaseBinder GetBaseBinderAtBinder(string binderName) {
            return _registerBinderDict.ContainsKey(binderName) ? _registerBinderDict[binderName] : null;
        }
    }
}