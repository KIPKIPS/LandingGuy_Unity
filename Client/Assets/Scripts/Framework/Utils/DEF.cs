// author:KIPKIPS
// date:2023.02.08 23:15
// describe:枚举定义类

namespace Framework {
    public static class DEF {
        //音频类型
        public enum AudioType {
            BGM = 0,
            EFFECT = 1,
        }
        public static string DataStoragePath = System.Environment.CurrentDirectory+@"\Storage\";

    }
    #region 事件类型枚举
    public enum EventType {
        GAME_LAUNCH,
    }

    #endregion
}
