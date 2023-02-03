// author:KIPKIPS
// date:2023.02.03 12:11
// describe:对象池管理对象约束接口
namespace Framework.Core.Pool {
    public interface IPoolAble {
        void OnRecycled();
        bool IsRecycled { get; set; }
    }
}