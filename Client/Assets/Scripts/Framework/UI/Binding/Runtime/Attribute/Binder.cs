// --[[
//     author:{wkp}
//     time:19:48
// ]]
using System;
using UnityEngine;

namespace Framework.UI {
    public class Binder : Attribute {
        public Type binderType;
        public Binder(Type binderType) {
            this.binderType = binderType;
        }
    }

    public class BinderParams : Attribute {
        public Type binderType;
        public BinderParams(Type binderType) {
            this.binderType = binderType;
        }
    }
}