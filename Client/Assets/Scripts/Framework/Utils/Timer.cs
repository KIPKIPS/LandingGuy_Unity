// author:KIPKIPS
// date:2023.02.11 02:06
// describe:定时器
using Framework.Manager;
using TimerEntity = Framework.Manager.TimerManager.TimerEntity;
using System;
namespace Framework {
    // ReSharper disable ClassNeverInstantiated.Global
    public class Timer {
        private static TimerEntity _entity;
        // ReSharper disable Unity.PerformanceAnalysis
        public static TimerEntity New(Action<TimerEntity> callback,int millisecond, int times = 1) {
            _entity = TimerManager.Instance.CreateTimer(callback, millisecond, times);
            return _entity;
        }
        public void Destroy() {
            _entity.Destroy();
        }
        public void Start() {
            _entity.Start();
        }
    }
}