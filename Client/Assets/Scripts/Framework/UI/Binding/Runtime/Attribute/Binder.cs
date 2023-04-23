// author:KIPKIPS
// date:2023.04.16 19:48
// describe:绑定特性
using System;

namespace Framework.UI {
    public class BinderComponent : Attribute {
        public readonly Type binderType;
        public BinderComponent(Type binderType) {
            this.binderType = binderType;
        }
    }

    public class BinderField : Attribute {
        public readonly Type binderType;
        public BinderField(Type binderType) {
            this.binderType = binderType;
        }
    }
}