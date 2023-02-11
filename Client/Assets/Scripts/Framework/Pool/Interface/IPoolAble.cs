// author:KIPKIPS
// date:2023.02.03 12:11
// describe:对象池管理对象约束接口
namespace Framework.Pool {
    public interface IPoolAble {
        /// <summary>
        /// 回收方法
        /// </summary>
        void OnRecycled();
        /// <summary>
        /// 是否回收
        /// </summary>
        bool IsRecycled { get; set; }
    }
}