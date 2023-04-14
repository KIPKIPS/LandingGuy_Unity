// author:KIPKIPS
// date:2023.04.14 16:15
// describe:ui绑定器
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.UI {
    [RequireComponent(typeof(BasePage))]
    public class UIBinding : MonoBehaviour {
        [SerializeField]
        private BasePage _page;

        private static Dictionary<string, BaseBinder> _registerBinderDict = new();

        public static void Register() {
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetAssembly(typeof(BinderParams));
            Type[] types = asm.GetExportedTypes();
            for (int i = 0; i < types.Length; i++) {
                var o = Attribute.GetCustomAttributes(types[i], true);
                foreach (Attribute a in o) {
                    if (a is Binder binderParams) {
                        _registerBinderDict.Add(binderParams.binderType.ToString(), (BaseBinder)System.Activator.CreateInstance(types[i]));
                    }
                }
            }
        }
    }
}