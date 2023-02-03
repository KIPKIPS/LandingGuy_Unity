// author:KIPKIPS
// date:2023.02.03 09:38
// describe:对象工厂接口
using UnityEngine;

namespace Framework.Core.Pool {
    public interface IFactory<T> {
        T Create();
    }
}