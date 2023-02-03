// author:KIPKIPS
// describe:对象池接口
namespace Framework.Core.Pool {
    public interface IPool<T> {
        T Allocate();
        bool Recycle(T obj);
    }
}