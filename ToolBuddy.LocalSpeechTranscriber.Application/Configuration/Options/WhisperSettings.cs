using System.ComponentModel.DataAnnotations;

namespace ToolBuddy.LocalSpeechTranscriber.Application.Configuration.Options
{
    public class WhisperSettings
    {
        public const string SectionName = "Whisper";

        [RegularExpression(
            @"^(tiny\.en|tiny|base\.en|base|small\.en|small|medium\.en|medium|large-v1|large-v2|large-v3|large|large-v3-turbo)$",
            ErrorMessage =
                "Invalid Whisper model. Valid values are: tiny.en,tiny,base.en,base,small.en,small,medium.en,medium,large-v1,large-v2,large-v3,large,large-v3-turbo"
        )]
        public string Model { get; set; } = "large-v3-turbo";

        [Required]
        public WhisperImplementation Implementation { get; set; } = WhisperImplementation.WhisperStreaming;

        [Range(
            1,
            65535
        )]
        public int Port { get; set; } = 9090;
    }
}