# Local Speech Transcriber

**Local Speech Transcriber** is a privacy-focused, real-time speech-to-text application for Windows.  
It runs entirely on your local machine, ensuring that your data stays private.  
The application captures audio from your microphone and uses a local Whisper-based server to transcribe it, automatically typing the output into any active window.

---

## Development Status

The project is already functional and structured well enough for third-party contributions.  
However, it is still in an early phase, and code refactoring and testing are ongoing.

### Planned Improvements

- Use [Simul Streaming](https://github.com/ufal/SimulStreaming) instead of [Whisper Streaming](https://github.com/ufal/whisper_streaming)  
- Make the app cross-platform by adding a non-WPF presentation project, using **MAUI Blazor Hybrid**  
- Improve architecture, add tests, and introduce other enhancements  

---

## Usage

1. Clone this project.  
   (This will also clone [Whisper Streaming](https://github.com/ufal/whisper_streaming).)  
2. Ensure you have **Python** and the required packages installed, and verify that you can run Whisper Streaming.  
3. Update [`appsettings.json`](LocalSpeechTranscriber.Presentation.Wpf/appsettings.json)  
   *(or create `appsettings.Local.json`)* with your settings.  
4. Build and run [`LocalSpeechTranscriber.sln`](LocalSpeechTranscriber.sln).  
5. Press the global hotkey (default: `Ctrl+Alt+Shift+D`) or click the **Start Recording** button to begin transcription.  
