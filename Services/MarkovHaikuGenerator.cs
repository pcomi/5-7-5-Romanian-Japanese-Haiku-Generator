using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _5_7_5.Services
{
    public class RomanianMarkovHaikuGenerator
    {
        private readonly Dictionary<string, List<string>> _markovChain5Syllables = new Dictionary<string, List<string>>();
        private readonly Dictionary<string, List<string>> _markovChain7Syllables = new Dictionary<string, List<string>>();

        private readonly Dictionary<string, int> _syllableCounts = new Dictionary<string, int>();
        private readonly List<string> _startWords5Syllables = new List<string>();
        private readonly List<string> _startWords7Syllables = new List<string>();
        private readonly Random _random = new Random();
        private readonly RomanianSyllableService _syllableService;

        ///verified 5-syllable verses (first and third lines)
        private readonly string[] _sample5SyllableLines = new[]
        {
            "sub cerul stins",
            "plouă pe-afară",
            "vântul șoptește",
            "nori grei pe cer",
            "stele pe cer",
            "noapte de vară",
            "dincolo-n zări",
            "fluturi în zbor",
            "luna râdea",
            "toamna e-aici",
            "liniște-n jur",
            "soare de-april",
            "ochii copilului",
            "valuri albastre",
            "scoici pe nisip",
            "vântul din nord",
            "pasăre-n zbor",
            "luna pe cer",
            "vis cu ochii",
            "piscul în ceață",
            "raze de soare",
            "după furtună",
            "picuri de rouă",
            "albine-n zbor",
            "flori de cireș",
            "povești de-amor",
            "lacul înghețat",
            "stele căzătoare",
            "pescăruș alb",
            "spumă de valuri",
            "sunet de apă",
            "verde de munte",
            "zăpadă-n prag",
            "iarna-i aici",
            "codrul bătrân",
            "bufnița strigă",
            "miere de tei",
            "floare de cireș",
            "primăvară-n prag",
            "ceață și nori",
            "singur pe drum",
            "vânt rece de-april",
            "dor de duc-acasă",
            "lanul de grâu",
            "seceriș greu",
            "ploaie de vară",
            "viață și dor",
            "salcâmi în floare",
            "dansul albinei",
            "scântei de-argint",
            "stele se-aprind",
            "munte și cer",
            "vulturi în zări",
            "apus de soare",
            "pacea se-așterne",
            "vânt dinspre nord",
            "marea în zori",
            "flori roșii-n lan",
            "noapte cu stele",
            "muguri pe ram",
            "ploile de mai",
            "codrul verde",
            "cântec de mierlă",
            "dealuri cu flori",
            "frunze de-aramă",
            "urme pe zăpadă",
            "brazi încărcați",
            "fumul din horn",
            "fulgi grei de nea",
            "apa-i oglindă",
            "șoapte-n pădure",
            "zorii de zi",
            "pasăre în zbor",
            "vis în culori",
            "flori de salcâm",
            "cerul de seară",
            "câmpul cu maci",
            "spice în vânt",
            "ploaie cu soare",
            "ramuri de tei",
            "râul susură",
            "frunze de plop",
            "clopot de sat",
            "gânduri în zbor",
            "stele pe cer",
            "maci printre grâu",
            "vârful de munte",
            "păsări pe cer",
            "razele lunii",
            "ceața de toamnă",
            "pietre din râu",
            "melcul pe frunză",
            "umbra de plop",
            "nisip și mare",
            "curcubeu rar",
            "soarele-apune",
            "cascade de-azur",
            "fulgi cad lin",
            "barca pe lac",
            "deal și câmpie",
            "parfum de flori",
            "rază prin frunze",
            "maci lângă grâu",
            "cântec de păsări",
            "drumul de țară",
            "stânca din vale",
            "soare de mai",
            "cântec în zori",
            "apa de munte",
            "cerul senin",
            "rugă de seară",
            "brazi și molizi",
            "ecoul din munți",
            "stoluri pe cer",
            "râs de copil",
            "fire de iarbă",
            "zborul de-albină",
            "greier în iarbă",
            "izvor de munte",
            "poarta din sat",
            "piatră de hotar",
            "tei înflorit",
            "gândul poetului",
            "struguri de vie",
            "maci roșii-n lan",
            "bruma de toamnă",
            "drumul spre casă",
            "pași prin zăpadă",
            "miros de pâine",
            "floare de colț",
            "vultur pe cer",
            "cântec de mamă",
            "Carul cu Boi",
            "cerul de-azur",
            "ploaie de stele",
            "norul albit",
            "fum din hornuri",
            "zumzet de-albină",
            "furnica mică",
            "focul din vatră",
            "zorile-n prag",
            "timpul ce trece",
            "căldura din mart",
            "greierul cântă",
            "zâmbetul tău",
            "cerul cu stele",
            "casa din deal",
            "bradul semeț",
            "calea din stele",
            "vorbele curg",
            "munții albastru",
            "frunza de tei",
            "zbor de libelă",
            "visele-mi pier",
            "lumea întreagă",
            "lună pe cer",
            "cerb prin pădure",
            "izvor din stâncă",
            "stropii de ploaie",
            "dangăt din turn",
            "dor de iubire",
            "crengi de cireș",
            "liniște-n sat",
            "câmpul cu flori",
            "roua de-april",
            "macul din lan",
            "arșița verii",
            "lună-n april",
            "cerul cu stele",
            "dorul de casă",
            "sufletul meu",
            "iarba înaltă",
            "mirosul ploii",
            "casa părintească",
            "drumul vieții",
            "berze-n zbor",
            "fluturi albi",
            "cântec de dor",
            "stoluri de berze",
            "poveste veche",
            "taine ascunse",
            "clipele zboară",
            "amintiri vagi",
            "inima bate",
            "cerul senin",
            "castel de nori",
            "liniștea nopții",
            "casa-i bătrână",
            "lanul de orz",
            "strugurii copți",
            "poartă de lemn",
            "flori de câmpie",
            "casă pe deal",
            "miez de vară",
            "clipe de pace",
            "porumbel alb",
            "cireșul vechi",
            "trandafir alb",
            "fânul cosit",
            "umbră de nor",
            "barca pe Olt",
            "caisul alb",
            "pași prin iarbă",
            "cerul curat",
            "vise de-o zi",
            "cântec din țară",
            "vechi amintiri",
            "marea în zori",
            "lanul de grâu",
            "zorii de zi",
            "vorbe de dor",
            "câmpul cu maci",
            "visele-mi pier",
            "undele-n râu",
            "stele pe cer",
            "sunetul ploii",
            "casa din munți",
            "lumina lunii",
            "fire de iarbă",
            "râul zglobiu",
            "zâmbetul blând",
            "flacără vie",
            "aripi de înger",
            "ceasul din turn",
            "liniștea mea",
            "pasul ușor",
            "dorul de tine",
            "murmur de ape",
            "freamăt de tei",
            "verdele crud",
            "zarea departe",
            "pomii din crâng",
            "pârâul sună",
            "picuri de ploaie",
            "umbra din codri",
            "viața mea",
            "inima ta",
            "zorii de zi",
            "munți și păduri",
            "suflet curat",
            "pietre din râu",
            "lună pe cer",
            "pulbere de stele",
            "visul de-o noapte",
            "stropi mici de ploaie",
            "glasul subțire",
            "gânduri în zbor",
            "iarba e udă",
            "miros de pâine",
            "vremea se duce",
            "iarba cosită",
            "focul din vatră",
            "vis de-o noapte",
            "pomii în floare",
            "cale prin codru",
            "marea în zori",
            "noaptea se lasă",
            "liniștea serii",
            "steaua polară",
            "vremea de-atunci",
            "valul se sparge",
            "vârful ascuțit",
            "casa cu flori",
            "barza-n cuib",
            "pasărea-n zbor",
            "drumul pustiu",
            "muguri pe ram",
            "inima mea"
        };

        ///verified 7-syllable verses (second line)
        private readonly string[] _sample7SyllableLines = new[]
        {
            "vântul duce frunze moarte",
            "umbrela se-nvârte-n vânt",
            "grădina-nflorește-acum",
            "stele ce clipesc pe cer",
            "dansând peste flori de câmp",
            "frunzele plutesc în vânt",
            "zăpada scârțâie-n mers",
            "nisipul îmi arde tălpi",
            "brazii foșnesc în tăcere",
            "râul curge lin la vale",
            "siluete de copaci",
            "norii fug pe cerul gol",
            "strălucesc în dimineți",
            "vântul poartă foi uscate",
            "noaptea-nvăluie pădurea",
            "plutesc pe valuri albastre",
            "pietre lucind în lumină",
            "cade fulg cu fulg pe deal",
            "păsări cântă-n cor de zori",
            "cad petale roz din pomi",
            "pășesc prin iarba cu rouă",
            "frunzele se-nvârt în dans",
            "vânt ce-adie lin prin flori",
            "pământul primește apă",
            "parfumul se-ntinde-n zări",
            "luminează tot în jur",
            "stânci uriașe de granit",
            "culori ce se-mbină-n cer",
            "fulgerul străpunge norii",
            "valuri se sparg de țărmuri",
            "păsări ce se-ntorc acasă",
            "ploaia bate-n geam ușor",
            "soarele ne mângâie",
            "copacii dansează-n vânt",
            "florile se-nalță-n soare",
            "luna curge peste lume",
            "copiii se joacă voioși",
            "fluturii zboară prin flori",
            "razele de soare-n geam",
            "frunze se-nvârt în spirală",
            "albine adună nectarul",
            "din muguri ies frunze noi",
            "cerul se-ntunecă tare",
            "umbrele se mișcă lent",
            "mierla cântă-n vârf de tei",
            "roua lucește pe flori",
            "păsările cântă-n cor",
            "iarba unduie în vânt",
            "ceața se ridică lent",
            "merele se coc de-acum",
            "vulturul în cercuri zboară",
            "rândunele pleacă-n stoluri",
            "munții ninși până la vale",
            "stelele clipesc pe cer",
            "râul șerpuiește-n vale",
            "cireșii-s plini de floare",
            "vântul mișcă frunzele",
            "flori de câmp se-apleacă lin",
            "spicele se-ndoaie-n lan",
            "picuri de ploaie pe-acoperiș",
            "tunetul răsună-n depărtare",
            "fumul sus se-nalță drept",
            "buburuze roșii pe frunze",
            "fluturele zboară lin",
            "codrul foșnește în vânt",
            "stele curg pe cer de noapte",
            "luna-și varsă raza ei",
            "norii trec pe cer agale",
            "rouă cade peste flori",
            "roi de-albine trec prin aer",
            "florile-și deschid petalele",
            "vraja serii mă cuprinde",
            "umbrele se sting pe rând",
            "frunze aurii prin aer",
            "norii cresc pe cer albastru",
            "pădurea-mi spune povești",
            "fluturile-n zbor ușor",
            "vântul prin frunziș adie",
            "munții veghează în taină",
            "clopote ce sună-n vale",
            "valuri sparg spuma de mal",
            "macii roșii printre spice",
            "piscul nins lucește-n zare",
            "glasuri de izvor ce cântă",
            "flori de toamnă prin grădini",
            "vinul roșu curge-n cupe",
            "fulgi de nea plutesc ușor",
            "pașii noștri prin zăpadă",
            "visul meu zboară departe",
            "noaptea-și poartă-a ei mantie",
            "porumbeii-și iau zborul",
            "roadele-s coapte-n hambare",
            "melcul trece lin pe-o frunză",
            "focul arde-n vatră leneș",
            "clipe de liniște-adâncă",
            "gândul meu zboară departe",
            "vântul bate prin fereastră",
            "stelele privesc spre noi",
            "stropii cad lin peste flori",
            "mii de stele-n cer lucesc",
            "câmpul verde pân-la zare",
            "pădurea-și schimbă culoarea",
            "apa sare peste pietre",
            "liniștea mă-nconjoară",
            "greierul prin iarbă cântă",
            "cărări duc spre deal și vale",
            "curcubeul peste câmpuri",
            "stropi de ploaie lin pe geam",
            "bufnița se-aude-n noapte",
            "râul curge către mare",
            "umbra mea fuge de mine",
            "glasul mamei se aude",
            "soarele la vale-apune",
            "rândunelele se-ntorc",
            "privighetori cântă lin",
            "dealuri verzi până departe",
            "lebede plutesc pe lac",
            "trandafirii-și deschid boboc",
            "lumina zilei se stinge",
            "liliacul alb parfumat",
            "pescărușii țipă-n zbor",
            "marea-și mișcă valurile",
            "clipe ce trec fără urmă",
            "gândul meu e doar la tine",
            "viața curge ca un râu",
            "amintiri se-ntorc în timp",
            "florile-și scutură roua",
            "cerul noros se-nsenină",
            "cerbul bea apă din râu",
            "codrul vechi ne adăpostește",
            "vulturii plutesc în cercuri",
            "dimineața rece și clară",
            "ciocârliile se-nalță",
            "zarea-mbracă haine noi",
            "satul se trezește la zi",
            "ceața ușor se ridică",
            "valuri de mare se sparg",
            "greieri cântă toată noaptea",
            "pădurea șoptește povești",
            "frunzele-n vânt foșnesc lin",
            "strugurii se coc în vie",
            "ciocănitoarea în trunchi bate",
            "mierla cântă în grădină",
            "fântâna-și așteaptă norocul",
            "norul alb plutește-n zare",
            "porumbul crește în lanuri",
            "pădurea verde și deasă",
            "apele Dunării curg lin",
            "carul cu boi merge-agale",
            "pasărea cântă pe ramuri",
            "sufletul dorește pace",
            "inima-mi bate mai tare",
            "gândurile curg ca apa",
            "noaptea vine cu vise",
            "luna iese dintre nori",
            "apele râului cresc mult",
            "furtuna se-apropie-acum",
            "pescarii pe maluri așteaptă",
            "viața trece neoprit",
            "ploaia udă câmpul verde",
            "glasul tău îmi sună-n gând",
            "ciocârlia zboară sus",
            "focul arde-n vatră liniștit",
            "câinele latră la lună",
            "vraja serii ne cuprinde",
            "norii fug mânați de vânt",
            "greierul cântă prin iarbă",
            "fulgerul brăzdează cerul",
            "undele apei șoptesc",
            "drumeții urcă pe munte",
            "fluturi ce dansează-n vânt",
            "macii roșii printre spice",
            "tremură frunza de plop",
            "cântecul de leagăn lin",
            "merele cad din copaci",
            "foșnet ușor prin pădure",
            "iarba verde se apleacă",
            "ciocârlia sus tot cântă",
            "clopotele bat în sat",
            "cerbul bea apă din lac",
            "cântecul dulce de fluier",
            "zumzet de-albine și flori",
            "fumul se ridică drept",
            "dealul verde-i plin de flori",
            "ceața-nvăluie pădurea",
            "pasărea-și face cuibul ei",
            "vaca paște liniștită",
            "barca pe apă alunecă",
            "caii pasc pe câmp în pace",
            "roata morii se-nvârtește",
            "vântul bate din răsărit",
            "secerătorii prin lan merg",
            "semințe-n pământ rodesc",
            "trandafirul roșu-n soare",
            "mărul înflorește-n roz",
            "cucul cântă prin pădure",
            "luna plină sus pe cer",
            "stele clipesc dintre nori",
            "vântul bate dinspre mare",
            "apa curge lin la vale",
            "mirosul fânului proaspăt",
            "șoapte de iubire-n noapte",
            "umbra ta pe lângă mine",
            "pașii tăi răsună-n noapte",
            "inima-mi bate cu dor",
            "gândul zboară către tine",
            "visele-mi sunt pline de-amor",
            "florile se scaldă-n rouă",
            "greierii cântă-n ascuns",
            "liniștea nopții se-așterne",
            "viața curge zi de zi",
            "frunzele foșnesc în vânt",
            "rândunele zboară-n stoluri",
            "cerbul fuge prin pădure",
            "barca pe valuri se duce",
            "păsările cântă-n cor",
            "vremea se schimbă ușor",
            "merii sunt plini de miresme",
            "vântul îmi șoptește-n gând",
            "cerul este plin de stele",
            "glasul tău sună duios",
            "iarba-i verde ca smaraldul",
            "fluturi de-o zi trec prin aer",
            "frunzele șoptesc povești"
        };

        public RomanianMarkovHaikuGenerator(RomanianSyllableService syllableService)
        {
            _syllableService = syllableService;
            BuildMarkovModel();
        }

        private void BuildMarkovModel()
        {
            ///process 5-syllable lines
            BuildMarkovChainForLines(_sample5SyllableLines, _markovChain5Syllables, _startWords5Syllables);

            ///process 7-syllable lines
            BuildMarkovChainForLines(_sample7SyllableLines, _markovChain7Syllables, _startWords7Syllables);
        }

        private void BuildMarkovChainForLines(string[] lines, Dictionary<string, List<string>> markovChain, List<string> startWords)
        {
            foreach (var line in lines)
            {
                string[] words = line.Split(' ');

                ///start words
                if (words.Length > 0)
                {
                    startWords.Add(words[0]);
                }

                ///syllable counting
                foreach (var word in words)
                {
                    if (!_syllableCounts.ContainsKey(word))
                    {
                        _syllableCounts[word] = _syllableService.CountSyllables(word);
                    }
                }

                ///markov chain
                for (int i = 0; i < words.Length - 2; i++)
                {
                    string key = $"{words[i]} {words[i + 1]}";
                    string value = words[i + 2];

                    if (!markovChain.ContainsKey(key))
                    {
                        markovChain[key] = new List<string>();
                    }

                    markovChain[key].Add(value);
                }

                for (int i = 0; i < words.Length - 1; i++)
                {
                    string key = words[i];
                    string value = words[i + 1];

                    if (!markovChain.ContainsKey(key))
                    {
                        markovChain[key] = new List<string>();
                    }

                    markovChain[key].Add(value);
                }
            }
        }

        public (string, string, string) GenerateHaiku()
        {
            string line1 = GenerateLine(5, _markovChain5Syllables, _startWords5Syllables);
            string line2 = GenerateLine(7, _markovChain7Syllables, _startWords7Syllables);
            string line3 = GenerateLine(5, _markovChain5Syllables, _startWords5Syllables);

            return (line1, line2, line3);
        }

        public string GenerateLine(int targetSyllables, Dictionary<string, List<string>> markovChain, List<string> startWords)
        {
            int maxAttempts = 50;
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                string currentWord = startWords[_random.Next(startWords.Count)];
                int syllableCount = _syllableCounts[currentWord];

                List<string> line = new List<string> { currentWord };
                string previousWord = currentWord;

                while (syllableCount < targetSyllables)
                {
                    string nextWord = "";

                    if (line.Count >= 2)
                    {
                        string twoWordContext = $"{line[line.Count - 2]} {line[line.Count - 1]}";
                        if (markovChain.TryGetValue(twoWordContext, out var possibleNextWords) && possibleNextWords.Count > 0)
                        {
                            var validWords = possibleNextWords
                                .Where(w => _syllableCounts.ContainsKey(w) &&
                                           syllableCount + _syllableCounts[w] <= targetSyllables)
                                .ToList();

                            if (validWords.Count > 0)
                            {
                                nextWord = validWords[_random.Next(validWords.Count)];
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(nextWord) && markovChain.TryGetValue(previousWord, out var possibleWords) && possibleWords.Count > 0)
                    {
                        var validWords = possibleWords
                            .Where(w => _syllableCounts.ContainsKey(w) &&
                                       syllableCount + _syllableCounts[w] <= targetSyllables)
                            .ToList();

                        if (validWords.Count > 0)
                        {
                            nextWord = validWords[_random.Next(validWords.Count)];
                        }
                    }

                    if (string.IsNullOrEmpty(nextWord))
                    {
                        var validWords = _syllableCounts
                            .Where(kv => kv.Value <= targetSyllables - syllableCount)
                            .Select(kv => kv.Key)
                            .ToList();

                        if (validWords.Count > 0)
                        {
                            nextWord = validWords[_random.Next(validWords.Count)];
                        }
                        else
                        {
                            break;
                        }
                    }

                    line.Add(nextWord);
                    syllableCount += _syllableCounts[nextWord];
                    previousWord = nextWord;
                }

                ///syllable check
                if (syllableCount == targetSyllables)
                {
                    return string.Join(" ", line);
                }
            }

            ///in case of fail
            if (targetSyllables == 5 && _sample5SyllableLines.Length > 0)
            {
                return _sample5SyllableLines[_random.Next(_sample5SyllableLines.Length)];
            }
            else if (targetSyllables == 7 && _sample7SyllableLines.Length > 0)
            {
                return _sample7SyllableLines[_random.Next(_sample7SyllableLines.Length)];
            }

            return GenerateSimpleLine(targetSyllables);
        }

        private string GenerateSimpleLine(int targetSyllables)
        {
            var wordsBySyllableCount = _syllableCounts
                .GroupBy(kv => kv.Value)
                .ToDictionary(g => g.Key, g => g.Select(kv => kv.Key).ToList());

            int remainingSyllables = targetSyllables;
            List<string> line = new List<string>();

            while (remainingSyllables > 0)
            {
                var validCounts = wordsBySyllableCount
                    .Where(kv => kv.Key <= remainingSyllables)
                    .Select(kv => kv.Key)
                    .OrderByDescending(k => k)
                    .ToList();

                if (validCounts.Count == 0)
                    break;

                int syllableCount = validCounts[_random.Next(validCounts.Count)];

                string word = wordsBySyllableCount[syllableCount][_random.Next(wordsBySyllableCount[syllableCount].Count)];

                line.Add(word);
                remainingSyllables -= syllableCount;
            }

            return string.Join(" ", line);
        }
    }
}