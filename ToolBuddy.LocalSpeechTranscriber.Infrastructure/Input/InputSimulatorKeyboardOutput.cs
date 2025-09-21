using GregsStack.InputSimulatorStandard;
using ToolBuddy.LocalSpeechTranscriber.Application.Contracts;

namespace ToolBuddy.LocalSpeechTranscriber.Infrastructure.Input
{
    /// <summary>
    /// Simulates keyboard input using GregsStack.InputSimulatorStandard.InputSimulator to type text into the active control.
    /// </summary>
    public sealed class InputSimulatorKeyboardOutput : IKeyboardOutput
    {
        private readonly InputSimulator _inputSimulator = new ();

        /// <inheritdoc />
        public void TypeText(
            string text) => _inputSimulator.Keyboard.TextEntry(text);
    }
}