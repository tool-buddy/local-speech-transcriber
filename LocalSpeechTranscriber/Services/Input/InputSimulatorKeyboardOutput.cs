using GregsStack.InputSimulatorStandard;

namespace ToolBuddy.LocalSpeechTranscriber.Services.Input
{
    public sealed class InputSimulatorKeyboardOutput(IInputSimulator inputSimulator) : IKeyboardOutput
    {
        public void TypeText(string text) => inputSimulator.Keyboard.TextEntry(text);
    }
}
