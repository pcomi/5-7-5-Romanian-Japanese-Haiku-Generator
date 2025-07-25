using Microsoft.AspNetCore.Mvc;
using _5_7_5.Models;
using _5_7_5.Services;
using System;
using System.Threading.Tasks;

namespace _5_7_5.Controllers
{
    public class HaikuController : Controller
    {
        private readonly RomanianSyllableService _syllableService;
        private readonly RomanianMarkovHaikuGenerator _haikuGenerator;
        private readonly AzureTranslationService _translationService;
        private readonly AzureTextToSpeechService _textToSpeechService;

        public HaikuController(
            RomanianSyllableService syllableService,
            RomanianMarkovHaikuGenerator haikuGenerator,
            AzureTranslationService translationService,
            AzureTextToSpeechService textToSpeechService)
        {
            _syllableService = syllableService;
            _haikuGenerator = haikuGenerator;
            _translationService = translationService;
            _textToSpeechService = textToSpeechService;
        }

        public IActionResult Index()
        {
            return View(new HaikuModel());
        }

        [HttpPost]
        public async Task<IActionResult> ValidateHaiku(HaikuModel model, string action)
        {
            if (action == "generate")
            {
                var (line1, line2, line3) = _haikuGenerator.GenerateHaiku();
                model.Line1 = line1;
                model.Line2 = line2;
                model.Line3 = line3;
            }

            model.Line1SyllableCount = _syllableService.CountSyllables(model.Line1);
            model.Line2SyllableCount = _syllableService.CountSyllables(model.Line2);
            model.Line3SyllableCount = _syllableService.CountSyllables(model.Line3);

            if (action == "translate" && model.IsValid)
            {
                try
                {
                    ///azure translation
                    var (japaneseLine1, japaneseLine2, japaneseLine3,
                         romajiLine1, romajiLine2, romajiLine3) = await _translationService.TranslateHaikuAsync(
                        model.Line1, model.Line2, model.Line3);

                    model.JapaneseLine1 = japaneseLine1;
                    model.JapaneseLine2 = japaneseLine2;
                    model.JapaneseLine3 = japaneseLine3;
                    model.RomajiLine1 = romajiLine1;
                    model.RomajiLine2 = romajiLine2;
                    model.RomajiLine3 = romajiLine3;

                    ///audio for both languages
                    try
                    {
                        var (romanianAudioPath, japaneseAudioPath) = await _textToSpeechService.GenerateHaikuAudioAsync(model);
                        model.RomanianAudioPath = romanianAudioPath;
                        model.JapaneseAudioPath = japaneseAudioPath;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Text-to-speech error: {ex.Message}");
                    }
                }
                catch (Exception ex)
                {
                    ///dummy text (in case of error)
                    model.JapaneseLine1 = "古い池や";
                    model.JapaneseLine2 = "蛙飛び込む";
                    model.JapaneseLine3 = "水の音";
                    model.RomajiLine1 = "furuike ya";
                    model.RomajiLine2 = "kawazu tobikomu";
                    model.RomajiLine3 = "mizu no oto";

                    Console.WriteLine($"Translation error: {ex.Message}");
                }
            }

            ///update
            ModelState.Clear();
            return View("Index", model);
        }
    }
}