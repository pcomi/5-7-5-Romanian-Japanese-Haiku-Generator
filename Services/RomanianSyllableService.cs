namespace _5_7_5.Services
{
    public class RomanianSyllableService
    {
        ///vowels
        private readonly char[] _vowels = new[] { 'a', 'e', 'i', 'o', 'u', 'ă', 'â', 'î' };

        ///consonants
        private readonly char[] _consonants = new[] {
            'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm',
            'n', 'p', 'q', 'r', 's', 'ș', 't', 'ț', 'v', 'w', 'x', 'z'
        };

        ///diphthongs
        private readonly string[] _diphthongs = new[] {
            "ea", "eo", "ia", "ie", "io", "iu",
            "oa", "ua", "uă", "ui", "uo",
            "ăi", "ău", "âi", "âu", "îi", "îu",
            "ai", "au", "ei", "eu", "oi", "ou",
            "ee", "ii", "uu"
        };

        ///triphthongs
        private readonly string[] _triphthongs = new[] {
            "eai", "eau", "iai", "iau", "iei", "ioa", "iou",
            "oai", "uai", "uau", "uăi"
        };

        ///clusters
        private readonly string[] _consonantClusters = new[] {
            "bl", "br", "cl", "cr", "dr", "fl", "fr", "gl", "gr",
            "pl", "pr", "tr", "vl", "vr", "zl", "zr", "st", "șt",
            "sc", "sf", "sp", "sb", "sm", "sn", "zm", "zn"
        };

        public int CountSyllables(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 0;

            text = ProcessHyphenation(text);

            string[] words = text.ToLower().Split(new[] { ' ', '.', ',', ';', ':', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);

            int totalSyllables = 0;

            foreach (var word in words)
            {
                totalSyllables += CountSyllablesInWord(word.Trim().ToLower());
            }

            return totalSyllables;
        }

        private string ProcessHyphenation(string text)
        {
            //hyphens
            text = text.Replace("-n ", " ");
            text = text.Replace("-i ", " ");
            text = text.Replace("-l ", " ");

            while (text.Contains("-"))
            {
                int idx = text.IndexOf("-");
                if (idx > 0 && idx < text.Length - 1 &&
                    char.IsLetter(text[idx - 1]) && char.IsLetter(text[idx + 1]))
                {
                    text = text.Remove(idx, 1);
                }
                else
                {
                    text = text.Replace("-", " ");
                }
            }

            return text;
        }

        private int CountSyllablesInWord(string word)
        {
            if (string.IsNullOrEmpty(word))
                return 0;

            string processedWord = PreprocessWord(word);

            int syllableCount = CountSyllablesInProcessedWord(processedWord);

            syllableCount = ApplyRomanianPhonologicalRules(word, syllableCount);

            return syllableCount;
        }

        private string PreprocessWord(string word)
        {
            string processedWord = word;

            foreach (var triphthong in _triphthongs)
            {
                int index = processedWord.IndexOf(triphthong, StringComparison.OrdinalIgnoreCase);
                while (index >= 0)
                {
                    processedWord = processedWord.Substring(0, index) + "T" +
                                   processedWord.Substring(index + triphthong.Length);
                    index = processedWord.IndexOf(triphthong, index + 1, StringComparison.OrdinalIgnoreCase);
                }
            }

            foreach (var diphthong in _diphthongs)
            {
                int index = processedWord.IndexOf(diphthong, StringComparison.OrdinalIgnoreCase);
                while (index >= 0)
                {
                    processedWord = processedWord.Substring(0, index) + "D" +
                                   processedWord.Substring(index + diphthong.Length);
                    index = processedWord.IndexOf(diphthong, index + 1, StringComparison.OrdinalIgnoreCase);
                }
            }

            return processedWord;
        }

        private int CountSyllablesInProcessedWord(string processedWord)
        {
            int syllableCount = 0;
            bool inVowelGroup = false;

            ///syllable counting
            foreach (char c in processedWord)
            {
                bool isVowelOrMarker = _vowels.Contains(c) || c == 'T' || c == 'D';

                if (isVowelOrMarker && !inVowelGroup)
                {
                    syllableCount++;
                    inVowelGroup = true;
                }
                else if (!isVowelOrMarker)
                {
                    inVowelGroup = false;
                }
            }

            return Math.Max(syllableCount, 1);///at least one syllable
        }

        private int ApplyRomanianPhonologicalRules(string word, int syllableCount)
        {
            if (word.Length >= 2 && word.EndsWith("i") && IsConsonant(word[word.Length - 2]))
            {
                int lastVowelPos = FindLastVowelBeforePosition(word, word.Length - 2);
                if (lastVowelPos >= 0)
                {
                    string possibleDiphthong = word.Substring(lastVowelPos, 1) + "i";
                    if (!_diphthongs.Contains(possibleDiphthong.ToLower()))
                    {
                        syllableCount--;
                    }
                }
            }

            if (word.Contains("ie") && word.Length > 2)
            {
                int iePos = word.IndexOf("ie");
                if (iePos > 0 && IsConsonant(word[iePos - 1]))
                {
                }
            }

            if (word.Length >= 3 && word.EndsWith("e"))
            {
                string lastTwoChars = word.Substring(word.Length - 3, 2);
                if (_consonantClusters.Contains(lastTwoChars.ToLower()))
                {
                }
            }

            return Math.Max(syllableCount, 1);
        }

        private bool IsConsonant(char c)
        {
            return _consonants.Contains(char.ToLower(c));
        }

        private bool IsVowel(char c)
        {
            return _vowels.Contains(char.ToLower(c));
        }

        private int FindLastVowelBeforePosition(string word, int position)
        {
            for (int i = position - 1; i >= 0; i--)
            {
                if (IsVowel(word[i]))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}