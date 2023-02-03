// author:KIPKIPS
// describe:对象的创建器

namespace Framework.Core.Pool {
    public class BaseFactory<T> : IFactory<T> where T : new() {
        public T Create() {
            return new T();
        }
    }
}