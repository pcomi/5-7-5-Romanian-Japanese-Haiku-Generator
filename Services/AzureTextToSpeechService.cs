using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using _5_7_5.Models;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace _5_7_5.Services
{
    public class AzureTextToSpeechService
    {
        private readonly string _subscriptionKey;
        private readonly string _region;
        private readonly string _outputFolder;

        public AzureTextToSpeechService(string subscriptionKey, string region)
        {
            _subscriptionKey = subscriptionKey;
            _region = region;
            _outputFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "audio");

            if (!Directory.Exists(_outputFolder))
            {
                Directory.CreateDirectory(_outputFolder);
            }
        }

        public async Task<string> GenerateRomanianSpeechAsync(string text, string fileName)
        {
            return await GenerateSpeechAsync(text, "ro-RO", "ro-RO-EmilNeural", fileName);
        }

        public async Task<string> GenerateJapaneseSpeechAsync(string text, string fileName)
        {
            return await GenerateSpeechAsync(text, "ja-JP", "ja-JP-NanamiNeural", fileName);
        }

        private async Task<string> GenerateSpeechAsync(string text, string locale, string voiceName, string fileName)
        {
            if (!fileName.EndsWith(".wav"))
            {
                fileName += ".wav";
            }

            string outputPath = Path.Combine(_outputFolder, fileName);

            string webPath = $"/audio/{fileName}";

            var config = SpeechConfig.FromSubscription(_subscriptionKey, _region);
            config.SpeechSynthesisVoiceName = voiceName;

            using var audioOutput = AudioConfig.FromWavFileOutput(outputPath);

            using var synthesizer = new SpeechSynthesizer(config, audioOutput);

            using var result = await synthesizer.SpeakTextAsync(text);

            if (result.Reason == ResultReason.SynthesizingAudioCompleted)
            {
                return webPath;
            }
            else if (result.Reason == ResultReason.Canceled)
            {
                var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                throw new Exception($"Speech synthesis canceled: {cancellation.Reason}. Error details: {cancellation.ErrorDetails}");
            }
            else
            {
                throw new Exception($"Speech synthesis failed with reason: {result.Reason}");
            }
        }

        public async Task<(string RomanianAudioPath, string JapaneseAudioPath)> GenerateHaikuAudioAsync(
            HaikuModel haiku)
        {
            string uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8);

            ///romanian audio
            string romanianFullText = haiku.GetFullHaiku();
            string romanianFilePath = await GenerateRomanianSpeechAsync(
                romanianFullText,
                $"haiku_ro_{uniqueId}.wav");

            ///japanese audio
            string japaneseFullText = $"{haiku.JapaneseLine1}\n{haiku.JapaneseLine2}\n{haiku.JapaneseLine3}";
            string japaneseFilePath = await GenerateJapaneseSpeechAsync(
                japaneseFullText,
                $"haiku_ja_{uniqueId}.wav");

            return (romanianFilePath, japaneseFilePath);
        }
    }
}