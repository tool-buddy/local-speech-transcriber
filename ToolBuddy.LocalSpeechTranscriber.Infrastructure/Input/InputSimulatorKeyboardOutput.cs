using GregsStack.InputSimulatorStandard;
using ToolBuddy.LocalSpeechTranscriber.Application.Contracts;

namespace ToolBuddy.LocalSpeechTranscriber.Infrastructure.Input
{
    public sealed class InputSimulatorKeyboardOutput : IKeyboardOutput
    {
        private readonly InputSimulator _inputSimulator = new InputSimulator();

        public void TypeText(
            string text) => _inputSimulator.Keyboard.TextEntry(text);
    }
}