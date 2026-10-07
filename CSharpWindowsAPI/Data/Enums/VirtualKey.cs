

namespace Cheng.Windows.DataStructure
{

    /// <summary>
    /// 虚拟键码
    /// </summary>
    public enum VirtualKeyCode
    {

        /// <summary>
        /// 无
        /// </summary>
        None = 0,

        #region code

        #region 鼠标按钮

        /// <summary>鼠标左键</summary>
        LeftMouseButton = 0x01,

        /// <summary>鼠标右键</summary>
        RightMouseButton = 0x02,

        /// <summary>控制中断处理</summary>
        Cancel = 0x03,

        /// <summary>鼠标中键</summary>
        MiddleMouseButton = 0x04,

        /// <summary>X1 鼠标按钮</summary>
        XButton1 = 0x05,

        /// <summary>X2 鼠标按钮</summary>
        XButton2 = 0x06,

        #endregion

        #region 控制键

        /// <summary>Backspace 键</summary>
        Backspace = 0x08,

        /// <summary>Tab 键</summary>
        Tab = 0x09,

        /// <summary>Clear 键</summary>
        Clear = 0x0C,

        /// <summary>Enter 键</summary>
        Enter = 0x0D,

        /// <summary>Shift 键</summary>
        Shift = 0x10,

        /// <summary>Ctrl 键</summary>
        Control = 0x11,

        /// <summary>Alt 键</summary>
        Alt = 0x12,

        /// <summary>Pause 键</summary>
        Pause = 0x13,

        /// <summary>Caps Lock 键</summary>
        CapsLock = 0x14,

        /// <summary>Esc 键</summary>
        Escape = 0x1B,

        /// <summary>空格键</summary>
        Space = 0x20,

        /// <summary>Page Up 键</summary>
        PageUp = 0x21,

        /// <summary>Page Down 键</summary>
        PageDown = 0x22,

        /// <summary>End 键</summary>
        End = 0x23,

        /// <summary>Home 键</summary>
        Home = 0x24,

        /// <summary>方向键 - 左</summary>
        Left = 0x25,

        /// <summary>方向键 - 上</summary>
        Up = 0x26,

        /// <summary>方向键 - 右</summary>
        Right = 0x27,

        /// <summary>方向键 - 下</summary>
        Down = 0x28,

        /// <summary>Select 键</summary>
        Select = 0x29,

        /// <summary>Print 键</summary>
        Print = 0x2A,

        /// <summary>Execute 键</summary>
        Execute = 0x2B,

        /// <summary>Print Screen 键</summary>
        PrintScreen = 0x2C,

        /// <summary>Insert 键</summary>
        Insert = 0x2D,

        /// <summary>Delete 键</summary>
        Delete = 0x2E,

        /// <summary>帮助键</summary>
        Help = 0x2F,

        #endregion

        #region IME 输入法

        /// <summary>IME 假名模式</summary>
        Kana = 0x15,

        /// <summary>IME 朝鲜文模式</summary>
        Hangul = 0x15,

        /// <summary>IME On</summary>
        ImeOn = 0x16,

        /// <summary>IME Junja 模式</summary>
        Junja = 0x17,

        /// <summary>IME 最终模式</summary>
        Final = 0x18,

        /// <summary>IME Hanja 模式</summary>
        Hanja = 0x19,

        /// <summary>IME 汉字模式</summary>
        Kanji = 0x19,

        /// <summary>IME 关闭</summary>
        ImeOff = 0x1A,

        /// <summary>IME 转换</summary>
        Convert = 0x1C,

        /// <summary>IME 非转换</summary>
        NonConvert = 0x1D,

        /// <summary>IME 接受</summary>
        Accept = 0x1E,

        /// <summary>IME 模式更改请求</summary>
        ModeChange = 0x1F,

        #endregion

        #region 数字键（主键盘区）

        /// <summary>0 键</summary>
        D0 = 0x30,

        /// <summary>1 键</summary>
        D1 = 0x31,

        /// <summary>2 键</summary>
        D2 = 0x32,

        /// <summary>3 键</summary>
        D3 = 0x33,

        /// <summary>4 键</summary>
        D4 = 0x34,

        /// <summary>5 键</summary>
        D5 = 0x35,

        /// <summary>6 键</summary>
        D6 = 0x36,

        /// <summary>7 键</summary>
        D7 = 0x37,

        /// <summary>8 键</summary>
        D8 = 0x38,

        /// <summary>9 键</summary>
        D9 = 0x39,

        #endregion

        #region 字母键

        /// <summary>A 键</summary>
        A = 0x41,

        /// <summary>B 键</summary>
        B = 0x42,

        /// <summary>C 键</summary>
        C = 0x43,

        /// <summary>D 键</summary>
        D = 0x44,

        /// <summary>E 键</summary>
        E = 0x45,

        /// <summary>F 键</summary>
        F = 0x46,

        /// <summary>G 键</summary>
        G = 0x47,

        /// <summary>H 键</summary>
        H = 0x48,

        /// <summary>I 键</summary>
        I = 0x49,

        /// <summary>J 键</summary>
        J = 0x4A,

        /// <summary>K 键</summary>
        K = 0x4B,

        /// <summary>L 键</summary>
        L = 0x4C,

        /// <summary>M 键</summary>
        M = 0x4D,

        /// <summary>N 键</summary>
        N = 0x4E,

        /// <summary>O 键</summary>
        O = 0x4F,

        /// <summary>P 键</summary>
        P = 0x50,

        /// <summary>Q 键</summary>
        Q = 0x51,

        /// <summary>R 键</summary>
        R = 0x52,

        /// <summary>S 键</summary>
        S = 0x53,

        /// <summary>T 键</summary>
        T = 0x54,

        /// <summary>U 键</summary>
        U = 0x55,

        /// <summary>V 键</summary>
        V = 0x56,

        /// <summary>W 键</summary>
        W = 0x57,

        /// <summary>X 键</summary>
        X = 0x58,

        /// <summary>Y 键</summary>
        Y = 0x59,

        /// <summary>Z 键</summary>
        Z = 0x5A,

        #endregion

        #region 系统功能键

        /// <summary>左 Windows 徽标键</summary>
        LeftWindows = 0x5B,

        /// <summary>右 Windows 徽标键</summary>
        RightWindows = 0x5C,

        /// <summary>应用程序键（菜单键）</summary>
        Applications = 0x5D,

        /// <summary>睡眠键</summary>
        Sleep = 0x5F,

        #endregion

        #region 数字小键盘

        /// <summary>小键盘 0</summary>
        Num_pad0 = 0x60,

        /// <summary>小键盘 1</summary>
        Num_pad1 = 0x61,

        /// <summary>小键盘 2</summary>
        Num_pad2 = 0x62,

        /// <summary>小键盘 3</summary>
        Num_pad3 = 0x63,

        /// <summary>小键盘 4</summary>
        Num_pad4 = 0x64,

        /// <summary>小键盘 5</summary>
        Num_pad5 = 0x65,

        /// <summary>小键盘 6</summary>
        Num_pad6 = 0x66,

        /// <summary>小键盘 7</summary>
        Num_pad7 = 0x67,

        /// <summary>小键盘 8</summary>
        Num_pad8 = 0x68,

        /// <summary>小键盘 9</summary>
        Num_pad9 = 0x69,

        /// <summary>小键盘 *</summary>
        Num_Multiply = 0x6A,

        /// <summary>小键盘 +</summary>
        Num_Add = 0x6B,

        /// <summary>分隔符键</summary>
        Num_Separator = 0x6C,

        /// <summary>小键盘 -</summary>
        Num_Subtract = 0x6D,

        /// <summary>小键盘 .</summary>
        Num_Decimal = 0x6E,

        /// <summary>小键盘 /</summary>
        Num_Divide = 0x6F,

        #endregion

        #region F系列

        /// <summary>F1 键</summary>
        F1 = 0x70,

        /// <summary>F2 键</summary>
        F2 = 0x71,

        /// <summary>F3 键</summary>
        F3 = 0x72,

        /// <summary>F4 键</summary>
        F4 = 0x73,

        /// <summary>F5 键</summary>
        F5 = 0x74,

        /// <summary>F6 键</summary>
        F6 = 0x75,

        /// <summary>F7 键</summary>
        F7 = 0x76,

        /// <summary>F8 键</summary>
        F8 = 0x77,

        /// <summary>F9 键</summary>
        F9 = 0x78,

        /// <summary>F10 键</summary>
        F10 = 0x79,

        /// <summary>F11 键</summary>
        F11 = 0x7A,

        /// <summary>F12 键</summary>
        F12 = 0x7B,

        /// <summary>F13 键</summary>
        F13 = 0x7C,

        /// <summary>F14 键</summary>
        F14 = 0x7D,

        /// <summary>F15 键</summary>
        F15 = 0x7E,

        /// <summary>F16 键</summary>
        F16 = 0x7F,

        /// <summary>F17 键</summary>
        F17 = 0x80,

        /// <summary>F18 键</summary>
        F18 = 0x81,

        /// <summary>F19 键</summary>
        F19 = 0x82,

        /// <summary>F20 键</summary>
        F20 = 0x83,

        /// <summary>F21 键</summary>
        F21 = 0x84,

        /// <summary>F22 键</summary>
        F22 = 0x85,

        /// <summary>F23 键</summary>
        F23 = 0x86,

        /// <summary>F24 键</summary>
        F24 = 0x87,

        #endregion

        #region 锁定键

        /// <summary>Num Lock 键</summary>
        NumLock = 0x90,

        /// <summary>Scroll Lock 键</summary>
        ScrollLock = 0x91,

        #endregion

        #region 左右修饰键

        /// <summary>左 Shift 键</summary>
        LeftShift = 0xA0,

        /// <summary>右 Shift 键</summary>
        RightShift = 0xA1,

        /// <summary>左 Ctrl 键</summary>
        LeftControl = 0xA2,

        /// <summary>右 Ctrl 键</summary>
        RightControl = 0xA3,

        /// <summary>左 Alt 键</summary>
        LeftAlt = 0xA4,

        /// <summary>右 Alt 键</summary>
        RightAlt = 0xA5,

        #endregion

        #region 浏览器功能键

        /// <summary>浏览器后退键</summary>
        BrowserBack = 0xA6,

        /// <summary>浏览器前进键</summary>
        BrowserForward = 0xA7,

        /// <summary>浏览器刷新键</summary>
        BrowserRefresh = 0xA8,

        /// <summary>浏览器停止键</summary>
        BrowserStop = 0xA9,

        /// <summary>浏览器搜索键</summary>
        BrowserSearch = 0xAA,

        /// <summary>浏览器收藏夹键</summary>
        BrowserFavorites = 0xAB,

        /// <summary>浏览器主页键</summary>
        BrowserHome = 0xAC,

        #endregion

        #region 多媒体键

        /// <summary>音量静音键</summary>
        VolumeMute = 0xAD,

        /// <summary>音量减小键</summary>
        VolumeDown = 0xAE,

        /// <summary>音量增大键</summary>
        VolumeUp = 0xAF,

        /// <summary>下一曲目键</summary>
        MediaNextTrack = 0xB0,

        /// <summary>上一曲目键</summary>
        MediaPrevTrack = 0xB1,

        /// <summary>停止媒体键</summary>
        MediaStop = 0xB2,

        /// <summary>播放/暂停媒体键</summary>
        MediaPlayPause = 0xB3,

        /// <summary>启动邮件键</summary>
        LaunchMail = 0xB4,

        /// <summary>选择媒体键</summary>
        LaunchMediaSelect = 0xB5,

        /// <summary>启动应用程序 1 键</summary>
        LaunchApp1 = 0xB6,

        /// <summary>启动应用程序 2 键</summary>
        LaunchApp2 = 0xB7,

        #endregion

        #region OEM 特定键

        /// <summary>OEM 1 键（US 键盘上为 ;:）</summary>
        Oem1 = 0xBA,

        /// <summary>OEM 加号键（任何国家/地区的 +）</summary>
        OemPlus = 0xBB,

        /// <summary>OEM 逗号键（任何国家/地区的 ,）</summary>
        OemComma = 0xBC,

        /// <summary>OEM 减号键（任何国家/地区的 -）</summary>
        OemMinus = 0xBD,

        /// <summary>OEM 句号键（任何国家/地区的 .）</summary>
        OemPeriod = 0xBE,

        /// <summary>OEM 2 键（US 键盘上为 /?）</summary>
        Oem2 = 0xBF,

        /// <summary>OEM 3 键（US 键盘上为 `~）</summary>
        Oem3 = 0xC0,

        /// <summary>OEM 4 键（US 键盘上为 [{）</summary>
        Oem4 = 0xDB,

        /// <summary>OEM 5 键（US 键盘上为 \|）</summary>
        Oem5 = 0xDC,

        /// <summary>OEM 6 键（US 键盘上为 ]}）</summary>
        Oem6 = 0xDD,

        /// <summary>OEM 7 键（US 键盘上为 '"）</summary>
        Oem7 = 0xDE,

        /// <summary>OEM 8 键（任何国家/地区的 <> 或 \|）</summary>
        Oem8 = 0xDF,

        /// <summary>OEM 102 键（加拿大法语键盘上的 <>）</summary>
        Oem102 = 0xE2,

        #endregion

        #region 其他

        /// <summary>Attn 键</summary>
        Attn = 0xF6,

        /// <summary>CrSel 键</summary>
        CrSel = 0xF7,

        /// <summary>ExSel 键</summary>
        ExSel = 0xF8,

        /// <summary>Erase EOF 键</summary>
        EraseEOF = 0xF9,

        /// <summary>Play 键</summary>
        Play = 0xFA,

        /// <summary>Zoom 键</summary>
        Zoom = 0xFB,

        /// <summary>PA1 键</summary>
        Pa1 = 0xFD,

        /// <summary>Clear 键（OEM）</summary>
        OemClear = 0xFE,

        #endregion

        #endregion

    }

}
