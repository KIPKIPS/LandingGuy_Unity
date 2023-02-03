// author:KIPKIPS
// date:2023.02.03 14:15
// describe:对象池数量约束接口
namespace Framework.Core.Pool {
    public interface ICountObserveAble {
        /// <summary>
        /// 当前数量
        /// </summary>
        int CurCount { get; }
    }
}