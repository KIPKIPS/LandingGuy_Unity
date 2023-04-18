// --[[
//     author:{wkp}
//     time:19:48
// ]]
using System;
using UnityEngine;

namespace Framework.UI {
    public class BinderComponent : Attribute {
        public Type binderType;
        public BinderComponent(Type binderType) {
            this.binderType = binderType;
        }
    }

    public class BinderField : Attribute {
        public Type binderType;
        public BinderField(Type binderType) {
            this.binderType = binderType;
        }
    }
    // public class Bindable : Attribute {
    //     
    // }
}