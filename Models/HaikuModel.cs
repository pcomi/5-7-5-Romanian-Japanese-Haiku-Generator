using System.ComponentModel.DataAnnotations;

namespace _5_7_5.Models
{
    public class HaikuModel
    {
        [Display(Name = "Prima linie (5 silabe)")]
        public string Line1 { get; set; } = string.Empty;

        [Display(Name = "A doua linie (7 silabe)")]
        public string Line2 { get; set; } = string.Empty;

        [Display(Name = "A treia linie (5 silabe)")]
        public string Line3 { get; set; } = string.Empty;

        public int Line1SyllableCount { get; set; }
        public int Line2SyllableCount { get; set; }
        public int Line3SyllableCount { get; set; }

        ///japanese translation
        public string JapaneseLine1 { get; set; } = string.Empty;
        public string JapaneseLine2 { get; set; } = string.Empty;
        public string JapaneseLine3 { get; set; } = string.Empty;

        ///romaji transliteration
        public string RomajiLine1 { get; set; } = string.Empty;
        public string RomajiLine2 { get; set; } = string.Empty;
        public string RomajiLine3 { get; set; } = string.Empty;

        ///audio paths
        public string RomanianAudioPath { get; set; } = string.Empty;
        public string JapaneseAudioPath { get; set; } = string.Empty;

        public bool IsValid =>
            Line1SyllableCount == 5 &&
            Line2SyllableCount == 7 &&
            Line3SyllableCount == 5;

        public bool IsTranslated =>
            !string.IsNullOrEmpty(JapaneseLine1) &&
            !string.IsNullOrEmpty(JapaneseLine2) &&
            !string.IsNullOrEmpty(JapaneseLine3);

        public bool HasAudio =>
            !string.IsNullOrEmpty(RomanianAudioPath) &&
            !string.IsNullOrEmpty(JapaneseAudioPath);

        public string GetFullHaiku()
        {
            return $"{Line1}\n{Line2}\n{Line3}";
        }
    }
}