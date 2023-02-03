// author:KIPKIPS
// describe:对象工厂接口
using UnityEngine;

namespace Framework.Core.Pool {
    public interface IFactory<T> {
        T Create();
    }
}