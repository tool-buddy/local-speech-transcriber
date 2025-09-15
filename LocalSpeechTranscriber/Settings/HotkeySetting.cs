using System.ComponentModel.DataAnnotations;
using System.Windows.Input;

namespace ToolBuddy.LocalSpeechTranscriber.Settings
{
    public class HotkeySetting
    {
        [Required]
        public required Key Key { get; set; }
        [Required]
        public required ModifierKeys[] Modifiers { get; set; }
    }
}