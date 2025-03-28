namespace Cheng.OtherCode.Winapi.Wave
{
    public enum WaveFormatEncoding : ushort
    {

        Unknown = 0,
        Pcm = 1,
        Adpcm = 2,
        IeeeFloat = 3,
        Vselp = 4,
        IbmCvsd = 5,
        ALaw = 6,
        MuLaw = 7,
        Dts = 8,
        Drm = 9,
        WmaVoice9 = 10,
        OkiAdpcm = 0x10,
        DviAdpcm = 17,
        ImaAdpcm = 17,
        MediaspaceAdpcm = 18,
        SierraAdpcm = 19,
        G723Adpcm = 20,
        DigiStd = 21,
        DigiFix = 22,
        DialogicOkiAdpcm = 23,
        MediaVisionAdpcm = 24,
        CUCodec = 25,
        YamahaAdpcm = 0x20,
        SonarC = 33,
        DspGroupTrueSpeech = 34,
        EchoSpeechCorporation1 = 35,
        AudioFileAf36 = 36,
        Aptx = 37,

        AudioFileAf10 = 38,
        Prosody1612 = 39,
        Lrc = 40,
        DolbyAc2 = 48,
        Gsm610 = 49,
        MsnAudio = 50,
        AntexAdpcme = 51,
        ControlResVqlpc = 52,
        DigiReal = 53,
        DigiAdpcm = 54,
        ControlResCr10 = 55,
        WAVE_FORMAT_NMS_VBXADPCM = 56,
        WAVE_FORMAT_CS_IMAADPCM = 57,
        WAVE_FORMAT_ECHOSC3 = 58,
        WAVE_FORMAT_ROCKWELL_ADPCM = 59,
        WAVE_FORMAT_ROCKWELL_DIGITALK = 60,
        WAVE_FORMAT_XEBEC = 61,
        WAVE_FORMAT_G721_ADPCM = 0x40,
        WAVE_FORMAT_G728_CELP = 65,
        WAVE_FORMAT_MSG723 = 66,
        //
        // 摘要:
        //     WAVE_FORMAT_MPEG, Microsoft Corporation
        Mpeg = 80,
        WAVE_FORMAT_RT24 = 82,
        WAVE_FORMAT_PAC = 83,
        //
        // 摘要:
        //     WAVE_FORMAT_MPEGLAYER3, ISO/MPEG Layer3 Format Tag
        MpegLayer3 = 85,
        WAVE_FORMAT_LUCENT_G723 = 89,
        WAVE_FORMAT_CIRRUS = 96,
        WAVE_FORMAT_ESPCM = 97,
        WAVE_FORMAT_VOXWARE = 98,
        WAVE_FORMAT_CANOPUS_ATRAC = 99,
        WAVE_FORMAT_G726_ADPCM = 100,
        WAVE_FORMAT_G722_ADPCM = 101,
        WAVE_FORMAT_DSAT_DISPLAY = 103,
        WAVE_FORMAT_VOXWARE_BYTE_ALIGNED = 105,
        WAVE_FORMAT_VOXWARE_AC8 = 112,
        WAVE_FORMAT_VOXWARE_AC10 = 113,
        WAVE_FORMAT_VOXWARE_AC16 = 114,
        WAVE_FORMAT_VOXWARE_AC20 = 115,
        WAVE_FORMAT_VOXWARE_RT24 = 116,
        WAVE_FORMAT_VOXWARE_RT29 = 117,
        WAVE_FORMAT_VOXWARE_RT29HW = 118,
        WAVE_FORMAT_VOXWARE_VR12 = 119,
        WAVE_FORMAT_VOXWARE_VR18 = 120,
        WAVE_FORMAT_VOXWARE_TQ40 = 121,
        WAVE_FORMAT_SOFTSOUND = 0x80,
        WAVE_FORMAT_VOXWARE_TQ60 = 129,
        WAVE_FORMAT_MSRT24 = 130,
        WAVE_FORMAT_G729A = 131,
        WAVE_FORMAT_MVI_MVI2 = 132,
        WAVE_FORMAT_DF_G726 = 133,
        WAVE_FORMAT_DF_GSM610 = 134,
        WAVE_FORMAT_ISIAUDIO = 136,
        WAVE_FORMAT_ONLIVE = 137,
        WAVE_FORMAT_SBC24 = 145,
        WAVE_FORMAT_DOLBY_AC3_SPDIF = 146,
        WAVE_FORMAT_MEDIASONIC_G723 = 147,
        WAVE_FORMAT_PROSODY_8KBPS = 148,
        WAVE_FORMAT_ZYXEL_ADPCM = 151,
        WAVE_FORMAT_PHILIPS_LPCBB = 152,
        WAVE_FORMAT_PACKED = 153,
        WAVE_FORMAT_MALDEN_PHONYTALK = 160,
        //
        // 摘要:
        //     WAVE_FORMAT_GSM
        Gsm = 161,
        //
        // 摘要:
        //     WAVE_FORMAT_G729
        G729 = 162,
        //
        // 摘要:
        //     WAVE_FORMAT_G723
        G723 = 163,
        //
        // 摘要:
        //     WAVE_FORMAT_ACELP
        Acelp = 164,
        //
        // 摘要:
        //     WAVE_FORMAT_RAW_AAC1
        RawAac = 0xFF,
        WAVE_FORMAT_RHETOREX_ADPCM = 0x100,
        WAVE_FORMAT_IRAT = 257,
        WAVE_FORMAT_VIVO_G723 = 273,
        WAVE_FORMAT_VIVO_SIREN = 274,
        WAVE_FORMAT_DIGITAL_G723 = 291,
        WAVE_FORMAT_SANYO_LD_ADPCM = 293,
        WAVE_FORMAT_SIPROLAB_ACEPLNET = 304,
        WAVE_FORMAT_SIPROLAB_ACELP4800 = 305,
        WAVE_FORMAT_SIPROLAB_ACELP8V3 = 306,
        WAVE_FORMAT_SIPROLAB_G729 = 307,
        WAVE_FORMAT_SIPROLAB_G729A = 308,
        WAVE_FORMAT_SIPROLAB_KELVIN = 309,
        WAVE_FORMAT_G726ADPCM = 320,
        WAVE_FORMAT_QUALCOMM_PUREVOICE = 336,
        WAVE_FORMAT_QUALCOMM_HALFRATE = 337,
        WAVE_FORMAT_TUBGSM = 341,
        WAVE_FORMAT_MSAUDIO1 = 352,
        //
        // 摘要:
        //     Windows Media Audio, WAVE_FORMAT_WMAUDIO2, Microsoft Corporation
        WindowsMediaAudio = 353,
        //
        // 摘要:
        //     Windows Media Audio Professional WAVE_FORMAT_WMAUDIO3, Microsoft Corporation
        WindowsMediaAudioProfessional = 354,
        //
        // 摘要:
        //     Windows Media Audio Lossless, WAVE_FORMAT_WMAUDIO_LOSSLESS
        WindowsMediaAudioLosseless = 355,
        //
        // 摘要:
        //     Windows Media Audio Professional over SPDIF WAVE_FORMAT_WMASPDIF (0x0164)
        WindowsMediaAudioSpdif = 356,
        WAVE_FORMAT_UNISYS_NAP_ADPCM = 368,
        WAVE_FORMAT_UNISYS_NAP_ULAW = 369,
        WAVE_FORMAT_UNISYS_NAP_ALAW = 370,
        WAVE_FORMAT_UNISYS_NAP_16K = 371,
        WAVE_FORMAT_CREATIVE_ADPCM = 0x200,
        WAVE_FORMAT_CREATIVE_FASTSPEECH8 = 514,
        WAVE_FORMAT_CREATIVE_FASTSPEECH10 = 515,
        WAVE_FORMAT_UHER_ADPCM = 528,
        WAVE_FORMAT_QUARTERDECK = 544,
        WAVE_FORMAT_ILINK_VC = 560,
        WAVE_FORMAT_RAW_SPORT = 576,
        WAVE_FORMAT_ESST_AC3 = 577,
        WAVE_FORMAT_IPI_HSX = 592,
        WAVE_FORMAT_IPI_RPELP = 593,
        WAVE_FORMAT_CS2 = 608,
        WAVE_FORMAT_SONY_SCX = 624,
        WAVE_FORMAT_FM_TOWNS_SND = 768,
        WAVE_FORMAT_BTV_DIGITAL = 0x400,
        WAVE_FORMAT_QDESIGN_MUSIC = 1104,
        WAVE_FORMAT_VME_VMPCM = 1664,
        WAVE_FORMAT_TPC = 1665,
        WAVE_FORMAT_OLIGSM = 0x1000,
        WAVE_FORMAT_OLIADPCM = 4097,
        WAVE_FORMAT_OLICELP = 4098,
        WAVE_FORMAT_OLISBC = 4099,
        WAVE_FORMAT_OLIOPR = 4100,
        WAVE_FORMAT_LH_CODEC = 4352,
        WAVE_FORMAT_NORRIS = 5120,
        WAVE_FORMAT_SOUNDSPACE_MUSICOMPRESS = 5376,
        //
        // 摘要:
        //     Advanced Audio Coding (AAC) audio in Audio Data Transport Stream (ADTS) format.
        //     The format block is a WAVEFORMATEX structure with wFormatTag equal to WAVE_FORMAT_MPEG_ADTS_AAC.
        //
        // 言论：
        //     The WAVEFORMATEX structure specifies the core AAC-LC sample rate and number of
        //     channels, prior to applying spectral band replication (SBR) or parametric stereo
        //     (PS) tools, if present. No additional data is required after the WAVEFORMATEX
        //     structure.
        MPEG_ADTS_AAC = 5632,
        //
        // 言论：
        //     Source wmCodec.h
        MPEG_RAW_AAC = 5633,
        //
        // 摘要:
        //     MPEG-4 audio transport stream with a synchronization layer (LOAS) and a multiplex
        //     layer (LATM). The format block is a WAVEFORMATEX structure with wFormatTag equal
        //     to WAVE_FORMAT_MPEG_LOAS.
        //
        // 言论：
        //     The WAVEFORMATEX structure specifies the core AAC-LC sample rate and number of
        //     channels, prior to applying spectral SBR or PS tools, if present. No additional
        //     data is required after the WAVEFORMATEX structure.
        MPEG_LOAS = 5634,
        //
        // 摘要:
        //     NOKIA_MPEG_ADTS_AAC
        //
        // 言论：
        //     Source wmCodec.h
        NOKIA_MPEG_ADTS_AAC = 5640,
        //
        // 摘要:
        //     NOKIA_MPEG_RAW_AAC
        //
        // 言论：
        //     Source wmCodec.h
        NOKIA_MPEG_RAW_AAC = 5641,
        //
        // 摘要:
        //     VODAFONE_MPEG_ADTS_AAC
        //
        // 言论：
        //     Source wmCodec.h
        VODAFONE_MPEG_ADTS_AAC = 5642,
        //
        // 摘要:
        //     VODAFONE_MPEG_RAW_AAC
        //
        // 言论：
        //     Source wmCodec.h
        VODAFONE_MPEG_RAW_AAC = 5643,
        //
        // 摘要:
        //     High-Efficiency Advanced Audio Coding (HE-AAC) stream. The format block is an
        //     HEAACWAVEFORMAT structure.
        MPEG_HEAAC = 5648,
        //
        // 摘要:
        //     WAVE_FORMAT_DVM
        WAVE_FORMAT_DVM = 0x2000,
        //
        // 摘要:
        //     WAVE_FORMAT_VORBIS1 "Og" Original stream compatible
        Vorbis1 = 26447,
        //
        // 摘要:
        //     WAVE_FORMAT_VORBIS2 "Pg" Have independent header
        Vorbis2 = 26448,
        //
        // 摘要:
        //     WAVE_FORMAT_VORBIS3 "Qg" Have no codebook header
        Vorbis3 = 26449,
        //
        // 摘要:
        //     WAVE_FORMAT_VORBIS1P "og" Original stream compatible
        Vorbis1P = 26479,
        //
        // 摘要:
        //     WAVE_FORMAT_VORBIS2P "pg" Have independent headere
        Vorbis2P = 26480,
        //
        // 摘要:
        //     WAVE_FORMAT_VORBIS3P "qg" Have no codebook header
        Vorbis3P = 26481,
        //
        // 摘要:
        //     WAVE_FORMAT_EXTENSIBLE
        Extensible = 65534,
        WAVE_FORMAT_DEVELOPMENT = ushort.MaxValue

    }
}
