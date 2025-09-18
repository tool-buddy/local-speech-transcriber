using System.ComponentModel.DataAnnotations;

namespace ToolBuddy.LocalSpeechTranscriber.Application.Configuration.Options
{
    public class HotkeySetting
    {
        [Required]
        public required string Key { get; set; }

        [Required]
        public required string[] Modifiers { get; set; }
    }
}