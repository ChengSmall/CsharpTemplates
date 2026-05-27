namespace Cheng.OtherCode.Winapi
{

    /// <summary>
    /// 来自mmsystem.h的Windows多媒体错误代码
    /// </summary>
    public enum MmResult
    {
        /// <summary>
        /// 没有错误
        /// </summary>
        NoError = 0,
        UnspecifiedError = 1,
        BadDeviceId = 2,
        NotEnabled = 3,
        AlreadyAllocated = 4,
        InvalidHandle = 5,
        NoDriver = 6,
        MemoryAllocationError = 7,
        NotSupported = 8,
        BadErrorNumber = 9,
        InvalidFlag = 10,
        InvalidParameter = 11,
        HandleBusy = 12,
        InvalidAlias = 13,
        BadRegistryDatabase = 14,
        RegistryKeyNotFound = 0xF,
        RegistryReadError = 0x10,
        RegistryWriteError = 17,
        RegistryDeleteError = 18,
        RegistryValueNotFound = 19,
        NoDriverCallback = 20,
        MoreData = 21,
        WaveBadFormat = 0x20,
        WaveStillPlaying = 33,
        WaveHeaderUnprepared = 34,
        WaveSync = 35,
        AcmNotPossible = 0x200,
        AcmBusy = 513,
        AcmHeaderUnprepared = 514,
        AcmCancelled = 515,
        MixerInvalidLine = 0x400,
        MixerInvalidControl = 1025,
        MixerInvalidValue = 1026
    }

}
