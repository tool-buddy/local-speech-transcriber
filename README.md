# Local Speech Transcriber

**Local Speech Transcriber** is a privacy-focused, real-time speech-to-text application for Windows. It runs entirely on your local machine, ensuring that your data remains private. The application captures audio from your microphone and uses a local Whisper-based server to transcribe it, typing the output into any active window.

## Development Status

This is a WIP .NET equivalent of a Python solution I previously implemented for personal use. That Python solution was implemented in a quick-and-dirty manner, so I am now converting it to C# while making it more robust, perfomant and configurable.

Refactoring and testing are currently ongoing. If you are interested in this project but not in a hurry, please star the repository and check back in about a week for a more polished project.

Planned work includes:

* Using [Simul Streaming](https://github.com/ufal/SimulStreaming) instead of [Whisper Streaming](https://github.com/ufal/whisper_streaming)
* A proper UI
* Improved architecture, adding tests, etc.
* Making the app cross-platform

## Usage

* Clone this project. This will clone the [Whisper Streaming](https://github.com/ufal/whisper_streaming) too.
* Make sure you have Python and the needed packages installed, and that you can run Whisper Streaming.
* Update [appsettings.json](LocalSpeechTranscriber/appsettings.json) (or create a appsettings.Local.json) with your settings.
* Build and run [LocalSpeechTranscriber.sln](LocalSpeechTranscriber.sln)
* Press the global hotkey (default: `Ctrl+Alt+Shift+D`) or click the "Start Recording" button to begin transcription.