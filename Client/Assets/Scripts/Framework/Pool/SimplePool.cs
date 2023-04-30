﻿// author:KIPKIPS
// date:2023.02.03 11:39
// describe:普通对象池
using System;

namespace Framework.Pool {
    /// <summary>
    /// 简易对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SimplePool<T> : BasePool<T> where T : IPoolAble, new() {
        /// <summary>
        /// 构造器
        /// </summary>
        public SimplePool() => Factory = new BaseFactory<T>();
        
        /// <summary>
        /// 分配函数
        /// </summary>
        /// <returns>分配的对象</returns>
        public override T Allocate() {
            var result = base.Allocate();
            result.IsRecycled = false;
            return result;
        }

        /// <summary>
        /// 回收函数
        /// </summary>
        /// <param name="obj">回收的对象</param>
        /// <returns>是否回收成功</returns>
        public override bool Recycle(T obj) {
            if (obj == null || obj.IsRecycled) {
                return false;
            }
            obj.IsRecycled = true;
            obj.OnRecycled();
            CacheStack.Push(obj);
            return true;
        }

        public void Clear() => CacheStack.Clear();
    }
}