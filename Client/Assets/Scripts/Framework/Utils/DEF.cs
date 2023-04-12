// author:KIPKIPS
// date:2023.02.08 23:15
// describe:枚举定义类

namespace Framework {
    // ReSharper disable InconsistentNaming
    public static class DEF {
        #region 系统默认值
        public const int SYSTEM_STANDARD_DPI = 96; //系统默认dpi
        public const int TRUE = 1;
        public const int FASLE = 0;
        #endregion
        
        #region AUDIO
        //音频类型
        public enum AudioType {
            BGM = 0,
            EFFECT = 1,
        }
        public static readonly string DataStoragePath = System.Environment.CurrentDirectory+@"\Storage\";
        #endregion
    }

    public enum AudioType {
        BGM = 0,
        EFFECT = 1,
    }
}
