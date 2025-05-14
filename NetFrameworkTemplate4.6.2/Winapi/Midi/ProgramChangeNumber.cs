
namespace Cheng.Windows.MIDI
{

    /// <summary>
    /// MIDI音色（乐器）种类编号
    /// </summary>
    public enum ProgramChangeNumber : byte
    {

        #region 钢琴

        /// <summary>
        /// 钢琴 - 大钢琴
        /// </summary>
        Piano_Grand = 0,

        /// <summary>
        /// 钢琴 - 明亮的钢琴
        /// </summary>
        Piano_Bright,

        /// <summary>
        /// 钢琴 - 电子琴
        /// </summary>
        Piano_Electric,

        /// <summary>
        /// 钢琴 - 酒吧钢琴
        /// </summary>
        Piano_Bar,

        /// <summary>
        /// 钢琴 - 柔和电子琴
        /// </summary>
        Piano_SoftElectric,

        /// <summary>
        /// 钢琴 - 有合唱效果的电子琴
        /// </summary>
        Paino_ChoirElectric,

        /// <summary>
        /// 钢琴 - 羽管键琴（拨弦古钢琴）
        /// </summary>
        Piano_Harpsichord,

        /// <summary>
        /// 钢琴 - 科拉维科特琴（击弦古钢琴）
        /// </summary>
        Piano_Kolavikot = 7,

        #endregion

        #region 色彩打击乐器

        /// <summary>
        /// 色彩打击乐器 - 钢片琴
        /// </summary>
        CPI_SteelSheet = 8,

        /// <summary>
        /// 色彩打击乐器 - 钟琴
        /// </summary>
        CPI_Bell,

        /// <summary>
        /// 色彩打击乐器 - 八音盒
        /// </summary>
        CPI_EightToneBox,

        /// <summary>
        /// 色彩打击乐器 - 颤音琴
        /// </summary>
        CPI_Vibraphone,

        /// <summary>
        /// 色彩打击乐器 - 马林巴
        /// </summary>
        CPI_Malimba,

        /// <summary>
        /// 色彩打击乐器 - 木琴
        /// </summary>
        CPI_xylophone,

        /// <summary>
        /// 色彩打击乐器 - 管钟
        /// </summary>
        CPI_TubularBells = 14,

        /// <summary>
        /// 色彩打击乐器 - 大扬琴
        /// </summary>
        Organ_LargeYangqin = 15,

        #endregion

        #region 风琴

        /// <summary>
        /// 风琴 - 拉杆式管风琴
        /// </summary>
        Organ_HammondStylePipe = 16,

        /// <summary>
        /// 风琴 - 敲击式管风琴
        /// </summary>
        Organ_Percussion,

        /// <summary>
        /// 风琴 - 摇滚管风琴
        /// </summary>
        Organ_Rock,

        /// <summary>
        /// 风琴 - 教堂管风琴
        /// </summary>
        Organ_Church,

        /// <summary>
        /// 风琴 - 簧管风琴（竖笛）
        /// </summary>
        Organ_Clarinet,

        /// <summary>
        /// 风琴 - 手风琴
        /// </summary>
        Organ_Accordion,

        /// <summary>
        /// 风琴 - 口琴
        /// </summary>
        Organ_Harmonica,

        /// <summary>
        /// 风琴 - 探戈手风琴
        /// </summary>
        Organ_TangoAccordion = 23,

        #endregion

        #region 吉他

        /// <summary>
        /// 吉他 - 尼龙弦吉他
        /// </summary>
        Guitar_Nylon = 24,

        /// <summary>
        /// 吉他 - 钢弦吉他
        /// </summary>
        Guitar_Steel,

        /// <summary>
        /// 吉他 - 爵士电吉他
        /// </summary>
        Guitar_JazzElectric,

        /// <summary>
        /// 吉他 - 清音电吉他
        /// </summary>
        Guitar_QingyinElectric,

        /// <summary>
        /// 吉他 - 闷音电吉他
        /// </summary>
        Guitar_SilentElectric,

        /// <summary>
        /// 吉他 - 加驱动效果的电吉他
        /// </summary>
        Guitar_DrivingElectric,

        /// <summary>
        /// 吉他 - 加失真效果的电吉他
        /// </summary>
        Guitar_DistortionElectric,

        /// <summary>
        /// 吉他 - 吉他和声
        /// </summary>
        Guitar_Harmony = 31,

        #endregion

        #region 贝司

        /// <summary>
        /// 贝斯 - 声学贝司
        /// </summary>
        Bass_Acoustic = 32,

        /// <summary>
        /// 贝斯 - 指弹电贝司
        /// </summary>
        Bass_FingerElectric,

        /// <summary>
        /// 贝斯 - 拨片电贝司
        /// </summary>
        Bass_ElectricBassPick,

        /// <summary>
        /// 贝斯 - 无品贝司
        /// </summary>
        Bass_Fretless,

        /// <summary>
        /// 贝斯 - 掌击贝司 1
        /// </summary>
        Bass_Palm,

        /// <summary>
        /// 贝斯 - 掌击贝司 2
        /// </summary>
        Bass_Palm_2,

        /// <summary>
        /// 贝斯 - 合成贝司 1
        /// </summary>
        Bass_Synth,

        /// <summary>
        /// 贝斯 - 合成贝司 2
        /// </summary>
        Bass_Synth_2 = 39,

        #endregion

        #region 弦乐

        /// <summary>
        /// 弦乐 - 小提琴
        /// </summary>
        Stringed_Violin = 40,

        /// <summary>
        /// 弦乐 - 中提琴
        /// </summary>
        Stringed_Viola,

        /// <summary>
        /// 弦乐 - 大提琴
        /// </summary>
        Stringed_ViolonCello,

        /// <summary>
        /// 弦乐 - 低音大提琴
        /// </summary>
        Stringed_DoubleBass,

        /// <summary>
        /// 弦乐 - 弦乐群颤音
        /// </summary>
        Stringed_GroupVibrato,

        /// <summary>
        /// 弦乐 - 弦乐群拨奏
        /// </summary>
        Stringed_EnsemblePlucking,

        /// <summary>
        /// 弦乐 - 竖琴
        /// </summary>
        Stringed_Harp = 46,

        /// <summary>
        /// 打击乐器 - 定音鼓
        /// </summary>
        CPI_Timpani = 47,

        #endregion

        #region 合奏与合唱

        /// <summary>
        /// 合奏与合唱 - 弦乐合奏音色 1
        /// </summary>
        Ensemble_StringTimbre = 48,

        /// <summary>
        /// 合奏与合唱 - 弦乐合奏音色 2
        /// </summary>
        Ensemble_StringTimbre_2,

        /// <summary>
        /// 合奏与合唱 - 合成弦乐合奏 1
        /// </summary>
        Ensemble_SyntheticString,

        /// <summary>
        /// 合奏与合唱 - 合成弦乐合奏 2
        /// </summary>
        Ensemble_SyntheticString_2,

        /// <summary>
        /// 合奏与合唱 - 人声合唱“啊”
        /// </summary>
        Ensemble_Ahh,

        /// <summary>
        /// 合奏与合唱 - 人声“嘟”
        /// </summary>
        Ensemble_Du,

        /// <summary>
        /// 合奏与合唱 - 合成人声
        /// </summary>
        Ensemble_SynthVoice,

        /// <summary>
        /// 合奏与合唱 - 管弦乐敲击齐奏
        /// </summary>
        Ensemble_OrchestraHit = 55,

        #endregion

        #region 铜管

        /// <summary>
        /// 铜管 - 小号
        /// </summary>
        Brass_Trumpet = 56,

        /// <summary>
        /// 铜管 - 长号
        /// </summary>
        Brass_Trombone,

        /// <summary>
        /// 铜管 - 大号
        /// </summary>
        Brass_LargeSize,

        /// <summary>
        /// 铜管 - 加弱音器小号
        /// </summary>
        Brass_MuteTrumpet,

        /// <summary>
        /// 铜管 - 圆号
        /// </summary>
        Brass_Horn,

        /// <summary>
        /// 铜管 - 铜管合奏
        /// </summary>
        Brass_Ensemble,

        /// <summary>
        /// 铜管 - 合成铜管 1
        /// </summary>
        Brass_Synthetic,

        /// <summary>
        /// 铜管 - 合成铜管 2
        /// </summary>
        Brass_Synthetic_2 = 63,

        #endregion

        #region 簧管

        /// <summary>
        /// 簧管 - 高音萨克斯
        /// </summary>
        ReedPipe_HighSaxophone = 64,

        /// <summary>
        /// 簧管 - 次中音萨克斯
        /// </summary>
        ReedPipe_TenorSaxophone,

        /// <summary>
        /// 簧管 - 中音萨克斯
        /// </summary>
        ReedPipe_AltoSaxophone,

        /// <summary>
        /// 簧管 - 低音萨克斯
        /// </summary>
        ReedPipe_BassSaxophone,

        /// <summary>
        /// 簧管 - 双簧管
        /// </summary>
        ReedPipe_Oboe,

        /// <summary>
        /// 簧管 - 英国管
        /// </summary>
        ReedPipe_EnglishHorn,

        /// <summary>
        /// 簧管 - 巴松管（大管）
        /// </summary>
        ReedPipe_Bassoon,

        /// <summary>
        /// 簧管 - 单簧管
        /// </summary>
        ReedPipe_Clarinet = 71,

        #endregion

        #region 笛

        /// <summary>
        /// 笛 - 短笛
        /// </summary>
        Flute_Piccolo = 72,

        /// <summary>
        /// 笛 - 长笛
        /// </summary>
        Flute_flute,

        /// <summary>
        /// 笛 - 竖笛
        /// </summary>
        Flute_Clarinet,

        /// <summary>
        /// 笛 - 排箫
        /// </summary>
        Flute_Panpipe,

        /// <summary>
        /// 笛 - 吹瓶声
        /// </summary>
        Flute_BlowBottle,

        /// <summary>
        /// 笛 - 日本尺八
        /// </summary>
        Flute_ChiBa,

        /// <summary>
        /// 笛 - 口哨
        /// </summary>
        Flute_Whistle,

        /// <summary>
        /// 笛子 - 奥卡雷那笛
        /// </summary>
        Flute_Okarina = 79,

        #endregion

        #region 合成主音

        /// <summary>
        /// 合成主音 - 方波
        /// </summary>
        SynthLead_SquareWave = 80,

        /// <summary>
        /// 合成主音 - 锯齿波
        /// </summary>
        SynthLead_SawtoothWave,

        /// <summary>
        /// 合成主音 - 卡里欧佩（Carope）
        /// </summary>
        SynthLead_Carope,

        /// <summary>
        /// 合成主音 - 奇夫（Giff）
        /// </summary>
        SynthLead_Giff,

        /// <summary>
        /// 合成主音 - 查朗（Chalan）
        /// </summary>
        SynthLead_Chalan,

        /// <summary>
        /// 合成主音 - 人声
        /// </summary>
        SynthLead_Vocal,

        /// <summary>
        /// 合成主音 - 平行五度
        /// </summary>
        SynthLead_ParallelPentatonic,

        /// <summary>
        /// 合成主音 -  贝司加主音
        /// </summary>
        SynthLead_BassLead = 87,

        #endregion

        #region 合成音色

        /// <summary>
        /// 合成音色 - 新世纪
        /// </summary>
        SynthVoice_NewCentury = 88,

        /// <summary>
        /// 合成音色 - 温暖
        /// </summary>
        SynthVoice_Warm,

        /// <summary>
        /// 合成音色 - 多合成器
        /// </summary>
        SynthVoice_Multiple,

        /// <summary>
        /// 合成音色 - 合唱
        /// </summary>
        SynthVoice_Choral,

        /// <summary>
        /// 合成音色 - 弓形
        /// </summary>
        SynthVoice_BowShaped,

        /// <summary>
        /// 合成音色 - 金属
        /// </summary>
        SynthVoice_MetalSound,

        /// <summary>
        /// 合成音色 - 光环
        /// </summary>
        SynthVoice_Halo,

        /// <summary>
        /// 合成音色 - 风吹
        /// </summary>
        SynthVoice_WindBlown = 95,


        #endregion

        #region 合成效果

        /// <summary>
        /// 合成效果 - 雨声
        /// </summary>
        SynthEffects_Rain = 96,

        /// <summary>
        /// 合成效果 - 音轨
        /// </summary>
        SynthEffects_Track,

        /// <summary>
        /// 合成效果 - 水晶
        /// </summary>
        SynthEffects_Crystal,

        /// <summary>
        /// 合成效果 - 大气
        /// </summary>
        SynthEffects_Air,

        /// <summary>
        /// 合成效果 - 明亮
        /// </summary>
        SynthEffects_Bright,

        /// <summary>
        /// 合成效果 - 鬼怪
        /// </summary>
        SynthEffects_Ghosts,

        /// <summary>
        /// 合成效果 - 回声
        /// </summary>
        SynthEffects_Echo,

        /// <summary>
        /// 合成效果 - 科幻
        /// </summary>
        SynthEffects_ScienceFiction = 103,

        #endregion

        #region 民间乐器

        /// <summary>
        /// 民间乐器 - 西塔尔（印度）
        /// </summary>
        Folk_Sitar = 104,

        /// <summary>
        /// 民间乐器 - 班卓琴（美洲） 
        /// </summary>
        Folk_Banjo,

        /// <summary>
        /// 民间乐器 - 三昧线（日本）
        /// </summary>
        Folk_Shamisen,

        /// <summary>
        /// 民间乐器 - 十三弦筝（日本）
        /// </summary>
        Folk_Koto,

        /// <summary>
        /// 民间乐器 - 卡林巴
        /// </summary>
        Folk_Caringbah,

        /// <summary>
        /// 民间乐器 - 风笛
        /// </summary>
        Folk_Whistle,

        /// <summary>
        /// 民间乐器 - 民族提琴
        /// </summary>
        Folk_EthnicViolin,

        /// <summary>
        /// 民间乐器 - 山奈
        /// </summary>
        Folk_Shanna = 111,

        #endregion

        #region 打击乐器

        /// <summary>
        /// 打击乐器 - 叮当铃
        /// </summary>
        Percussion_DingdangBell = 112,

        /// <summary>
        /// 打击乐器 - 摇摆舞铃
        /// </summary>
        Percussion_SwingDanceBell,

        /// <summary>
        /// 打击乐器 - 钢鼓
        /// </summary>
        Percussion_SteelDrum,

        /// <summary>
        /// 打击乐器 - 木鱼
        /// </summary>
        Percussion_WoodenFish,

        /// <summary>
        /// 打击乐器 - 太鼓
        /// </summary>
        Percussion_Taiko,

        /// <summary>
        /// 打击乐器 - 通通鼓
        /// </summary>
        Percussion_TomTom,

        /// <summary>
        /// 打击乐器 - 合成鼓
        /// </summary>
        Percussion_SyntheticDrum,

        /// <summary>
        /// 打击乐器 - 反向铜钹 ReverseCopperCymbal
        /// </summary>
        Percussion_ReverseCopperCymbal = 119,

        #endregion

        #region 声音效果

        /// <summary>
        /// 音效 - 吉他换把杂音
        /// </summary>
        Effects_ChangeGuitarHandleNoise = 120,

        /// <summary>
        /// 音效 - 呼吸
        /// </summary>
        Effects_Breath,

        /// <summary>
        /// 音效 - 海浪
        /// </summary>
        Effects_SeaWave,

        /// <summary>
        /// 音效 - 鸟鸣
        /// </summary>
        Effects_Birdsong,

        /// <summary>
        /// 音效 - 电话铃
        /// </summary>
        Effects_PhoneRing,

        /// <summary>
        /// 音效 - 直升机声
        /// </summary>
        Effects_Helicopter,

        /// <summary>
        /// 音效 - 鼓掌声
        /// </summary>
        Effects_Applaud,

        /// <summary>
        /// 音效 - 枪声
        /// </summary>
        Effects_Gunshot = 127,

        #endregion

    }


}
