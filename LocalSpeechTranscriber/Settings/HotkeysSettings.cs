using System.ComponentModel.DataAnnotations;

namespace ToolBuddy.LocalSpeechTranscriber.Settings
{
    public class HotkeysSettings
    {
        public const string SectionName = "Hotkeys";

        [Required]
        public required HotkeySetting ToggleRecording { get; set; }
    }
}