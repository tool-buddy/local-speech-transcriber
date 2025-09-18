using System.ComponentModel.DataAnnotations;

namespace ToolBuddy.LocalSpeechTranscriber.Application.Configuration.Options
{
    public class HotkeysSettings
    {
        public const string SectionName = "Hotkeys";

        [Required]
        public required HotkeySetting ToggleRecording { get; set; }
    }
}