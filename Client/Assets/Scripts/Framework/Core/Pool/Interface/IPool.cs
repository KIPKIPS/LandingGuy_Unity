// author:KIPKIPS
// date:2023.02.03 10:52
// describe:对象池接口
namespace Framework.Core {
    public interface IPool<T> {
        /// <summary>
        /// 分配方法
        /// </summary>
        /// <returns></returns>
        T Allocate();
        /// <summary>
        /// 回收方法
        /// </summary>
        /// <param name="obj">回收对象</param>
        /// <returns>是否回收成功</returns>
        bool Recycle(T obj);
    }
}