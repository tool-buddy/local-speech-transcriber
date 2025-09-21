using System.ComponentModel.DataAnnotations;

namespace ToolBuddy.LocalSpeechTranscriber.Application.Options
{
    /// <summary>
    /// Represents a single hotkey definition consisting of a key and its potential modifier keys.
    /// </summary>
    public class HotkeyOption
    {
        /// <summary>
        /// Gets or sets the main key of the hotkey (for example, "K" or "R").
        /// </summary>
        [Required]
        public required string Key { get; set; }

        /// <summary>
        /// Gets or sets the collection of modifier keys (for example, "Ctrl", "Alt", "Shift").
        /// </summary>
        [Required]
        public required string[] Modifiers { get; set; }
    }
}