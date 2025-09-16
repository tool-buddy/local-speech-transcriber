using System.ComponentModel.DataAnnotations;

namespace ToolBuddy.LocalSpeechTranscriber.Settings
{
    public class WhisperSettings
    {
        public const string SectionName = "Whisper";

        [Required]
        [RegularExpression(
            @"^(tiny\.en|tiny|base\.en|base|small\.en|small|medium\.en|medium|large-v1|large-v2|large-v3|large|large-v3-turbo)$",
            ErrorMessage = "Invalid Whisper model. Valid values are: tiny.en,tiny,base.en,base,small.en,small,medium.en,medium,large-v1,large-v2,large-v3,large,large-v3-turbo"
        )]
        public string Model { get; set; } = "large-v3-turbo";

        [Range(
            1,
            65535
        )]
        public int Port { get; set; }

        [Required]
        public string PythonExecutable { get; set; } = string.Empty;
    }
}