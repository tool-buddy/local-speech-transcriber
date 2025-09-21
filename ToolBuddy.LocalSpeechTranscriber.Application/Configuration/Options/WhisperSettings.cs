using System.ComponentModel.DataAnnotations;

namespace ToolBuddy.LocalSpeechTranscriber.Application.Configuration.Options
{
    /// <summary>
    /// Options that configure the Whisper-based speech-to-text engine.
    /// </summary>
    public class WhisperSettings
    {
        /// <summary>
        /// The configuration section name containing Whisper settings.
        /// </summary>
        public const string SectionName = "Whisper";

        /// <summary>
        /// Gets or sets the Whisper model name to use for transcription.
        /// </summary>
        /// <remarks>
        /// Valid values: tiny.en, tiny, base.en, base, small.en, small, medium.en, medium, large-v1, large-v2, large-v3, large, large-v3-turbo.
        /// </remarks>
        [RegularExpression(
            @"^(tiny\.en|tiny|base\.en|base|small\.en|small|medium\.en|medium|large-v1|large-v2|large-v3|large|large-v3-turbo)$",
            ErrorMessage =
                "Invalid Whisper model. Valid values are: tiny.en,tiny,base.en,base,small.en,small,medium.en,medium,large-v1,large-v2,large-v3,large,large-v3-turbo"
        )]
        public string Model { get; set; } = "large-v3-turbo";

        /// <summary>
        /// Gets or sets the Whisper implementation to run.
        /// </summary>
        [Required]
        public WhisperImplementation Implementation { get; set; } = WhisperImplementation.WhisperStreaming;

        /// <summary>
        /// Gets or sets the TCP port used by the Whisper server.
        /// </summary>
        /// <remarks>Valid range is 1–65535.</remarks>
        [Range(
            1,
            65535
        )]
        public int Port { get; set; } = 9090;
    }
}