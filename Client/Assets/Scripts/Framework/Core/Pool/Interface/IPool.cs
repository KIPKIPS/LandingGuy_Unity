// author:KIPKIPS
// date:2023.02.03 10:52
// describe:对象池接口
namespace Framework.Core.Pool {
    public interface IPool<T> {
        T Allocate();
        bool Recycle(T obj);
    }
}