using System.Runtime.Serialization;

namespace ToolBuddy.LocalSpeechTranscriber.Settings
{
    public enum WhisperModel
    {
        [EnumMember(Value = "tiny.en")]
        TinyEn,

        [EnumMember(Value = "tiny")]
        Tiny,

        [EnumMember(Value = "base.en")]
        BaseEn,

        [EnumMember(Value = "base")]
        Base,

        [EnumMember(Value = "small.en")]
        SmallEn,

        [EnumMember(Value = "small")]
        Small,

        [EnumMember(Value = "medium.en")]
        MediumEn,

        [EnumMember(Value = "medium")]
        Medium,

        [EnumMember(Value = "large-v1")]
        LargeV1,

        [EnumMember(Value = "large-v2")]
        LargeV2,

        [EnumMember(Value = "large-v3")]
        LargeV3,

        [EnumMember(Value = "large")]
        Large,

        [EnumMember(Value = "large-v3-turbo")]
        LargeV3Turbo,

        [EnumMember(Value = "turbo")]
        Turbo
    }
}