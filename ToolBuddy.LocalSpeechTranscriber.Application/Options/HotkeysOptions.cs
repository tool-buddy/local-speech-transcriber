using System.ComponentModel.DataAnnotations;

namespace ToolBuddy.LocalSpeechTranscriber.Application.Options
{
    /// <summary>
    /// Root options bound from configuration for all application hotkeys.
    /// </summary>
    public class HotkeysOptions
    {
        /// <summary>
        /// The configuration section name containing hotkey settings.
        /// </summary>
        public const string SectionName = "Hotkeys";

        /// <summary>
        /// Gets or sets the hotkey configuration to toggle audio recording on and off.
        /// </summary>
        [Required]
        public required HotkeyOption ToggleRecording { get; set; }
    }
}