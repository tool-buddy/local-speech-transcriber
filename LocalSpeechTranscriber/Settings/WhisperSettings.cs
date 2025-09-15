using System.ComponentModel.DataAnnotations;

namespace ToolBuddy.LocalSpeechTranscriber.Settings
{
    public class WhisperSettings
    {
        public const string SectionName = "Whisper";

        [Required]
        public WhisperModel Model { get; set; } = WhisperModel.LargeV3Turbo;

        [Range(1, 65535)]
        public int Port { get; set; }
    }
}
