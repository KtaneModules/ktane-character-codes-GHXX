using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterCodes : MonoBehaviour
{

    public KMAudio Audio;
    public KMBombInfo BombInfo;

    public KMSelectable[] NumberButtons;
    public KMSelectable DisplayButton;
    public TextMesh DisplayTextMesh;
    public Material LCDCoverUpMaterial;
    public MeshRenderer LCDRenderMesh;


    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    private const int letterCount = 5;
    private static readonly MonoRandom rand = new MonoRandom(); // todo replace with unity random number gen.
    private static readonly Dictionary<ushort, string> characterList = new Dictionary<ushort, string>
    {
        {1, "☺"}, {185, "╣"}, {712, "ˈ"}, {982, "ϖ"}, {2, "☻"}, {186, "║"}, {713, "ˉ"}, {983, "ϗ"},
        {3, "♥"}, {187, "╗"}, {714, "ˊ"}, {984, "Ϙ"}, {4, "♦"}, {188, "╝"}, {715, "ˋ"}, {985, "ϙ"},
        {5, "♣"}, {189, "╜"}, {716, "ˌ"}, {986, "Ϛ"}, {6, "♠"}, {190, "╛"}, {717, "ˍ"}, {987, "ϛ"},
        {7, "•"}, {191, "┐"}, {718, "ˎ"}, {990, "Ϟ"}, {8, "◘"}, {192, "└"}, {719, "ˏ"}, {991, "ϟ"},
        {9, "○"}, {193, "┴"}, {720, "ː"}, {992, "Ϡ"}, {10, "◙"}, {194, "┬"}, {721, "ˑ"}, {993, "ϡ"},
        {11, "♂"}, {195, "├"}, {722, "˒"}, {994, "Ϣ"}, {12, "♀"}, {196, "─"}, {723, "˓"}, {995, "ϣ"},
        {13, "♪"}, {197, "┼"}, {724, "˔"}, {996, "Ϥ"}, {14, "♫"}, {198, "╞"}, {725, "˕"}, {998, "Ϧ"},
        {15, "☼"}, {199, "╟"}, {726, "˖"}, {999, "ϧ"}, {16, "►"}, {200, "╚"}, {727, "˗"}, {1000, "Ϩ"},
        {17, "◄"}, {201, "╔"}, {728, "˘"}, {1002, "Ϫ"}, {18, "↕"}, {202, "╩"}, {729, "˙"}, {1004, "Ϭ"},
        {19, "‼"}, {203, "╦"}, {730, "˚"}, {1005, "ϭ"}, {20, "¶"}, {204, "╠"}, {731, "˛"}, {1008, "ϰ"},
        {21, "§"}, {205, "═"}, {732, "˜"}, {1009, "ϱ"}, {22, "▬"}, {206, "╬"}, {733, "˝"}, {1010, "ϲ"},
        {23, "↨"}, {207, "╧"}, {734, "˞"}, {1012, "ϴ"}, {24, "↑"}, {208, "╨"}, {735, "˟"}, {1013, "ϵ"},
        {25, "↓"}, {209, "╤"}, {736, "ˠ"}, {1014, "϶"}, {26, "→"}, {210, "╥"}, {737, "ˡ"}, {1015, "Ϸ"},
        {27, "←"}, {211, "╙"}, {738, "ˢ"}, {1016, "ϸ"}, {28, "∟"}, {212, "╘"}, {739, "ˣ"}, {1017, "Ϲ"},
        {29, "↔"}, {213, "╒"}, {740, "ˤ"}, {1018, "Ϻ"}, {30, "▲"}, {214, "╓"}, {741, "˥"}, {1019, "ϻ"},
        {31, "▼"}, {215, "╫"}, {742, "˦"}, {1020, "ϼ"}, {33, "!"}, {216, "╪"}, {743, "˧"}, {1021, "Ͻ"},
        {34, "“"}, {217, "┘"}, {744, "˨"}, {1022, "Ͼ"}, {35, "#"}, {218, "┌"}, {745, "˩"}, {1023, "Ͽ"},
        {36, "$"}, {219, "█"}, {746, "˪"}, {1026, "Ђ"}, {37, "%"}, {220, "▄"}, {747, "˫"}, {1028, "Є"},
        {38, "&"}, {221, "▬"}, {748, "ˬ"}, {1029, "Ѕ"}, {39, "‘"}, {222, "▐"}, {749, "˭"}, {1030, "І"},
        {40, "("}, {223, "▀"}, {750, "ˮ"}, {1032, "Ј"}, {41, ")"}, {224, "α"}, {751, "˯"}, {1033, "Љ"},
        {42, "*"}, {225, "ß"}, {752, "˰"}, {1034, "Њ"}, {43, "+"}, {226, "Γ"}, {753, "˱"}, {1035, "Ћ"},
        {44, ","}, {227, "π"}, {754, "˲"}, {1039, "Џ"}, {45, "-"}, {228, "Σ"}, {755, "˳"}, {1040, "А"},
        {46, "."}, {229, "σ"}, {756, "˴"}, {1041, "Б"}, {47, "/"}, {230, "µ"}, {757, "˵"}, {1042, "В"},
        {58, ":"}, {231, "τ"}, {758, "˶"}, {1043, "Г"}, {59, ";"}, {232, "Φ"}, {759, "˷"}, {1044, "Д"},
        {60, "<"}, {233, "Θ"}, {761, "˹"}, {1045, "Е"}, {61, "="}, {234, "Ω"}, {762, "˺"}, {1046, "Ж"},
        {62, ">"}, {235, "δ"}, {763, "˻"}, {1047, "З"}, {63, "?"}, {236, "∞"}, {764, "˼"}, {1048, "И"},
        {64, "@"}, {237, "φ"}, {765, "˽"}, {1049, "Й"}, {91, "["}, {238, "ε"}, {766, "˾"}, {1050, "К"},
        {92, "\\"}, {239, "∩"}, {767, "˿"}, {1051, "Л"}, {93, "]"}, {240, "≡"}, {890, "ͺ"}, {1059, "У"},
        {94, "^"}, {241, "±"}, {891, "ͻ"}, {1061, "Х"}, {95, "_"}, {242, "≥"}, {892, "ͼ"}, {1062, "Ц"},
        {96, "`"}, {243, "≤"}, {893, "ͽ"}, {1063, "Ч"}, {123, "{"}, {244, "⌠"}, {901, "΅"}, {1064, "Ш"},
        {124, "|"}, {245, "⌡"}, {916, "Δ"}, {1065, "Щ"}, {125, "}"}, {246, "÷"}, {926, "Ξ"}, {1066, "Ъ"},
        {126, "~"}, {247, "≈"}, {928, "Π"}, {1067, "Ы"}, {127, "⌂"}, {248, "°"}, {936, "Ψ"}, {1068, "Ь"},
        {166, "ª"}, {249, "∙"}, {946, "β"}, {1069, "Э"}, {167, "º"}, {251, "√"}, {947, "γ"}, {1070, "Ю"},
        {168, "¿"}, {252, "ⁿ"}, {948, "δ"}, {1071, "Я"}, {169, "⌐"}, {253, "²"}, {950, "ζ"}, {1072, "а"},
        {170, "¬"}, {254, "■"}, {951, "η"}, {1073, "б"}, {171, "½"}, {153, "™"}, {952, "θ"}, {1083, "л"},
        {172, "¼"}, {697, "ʹ"}, {957, "ν"}, {1119, "џ"}, {173, "¡"}, {698, "ʺ"}, {958, "ξ"}, {1120, "Ѡ"},
        {174, "«"}, {699, "ʻ"}, {961, "ρ"}, {1121, "ѡ"}, {175, "»"}, {700, "ʼ"}, {962, "ς"}, {1122, "Ѣ"},
        {176, "░"}, {701, "ʽ"}, {965, "υ"}, {1124, "Ѥ"}, {177, "▒"}, {702, "ʾ"}, {967, "χ"}, {1126, "Ѧ"},
        {178, "▓"}, {703, "ʿ"}, {968, "ψ"}, {1128, "Ѩ"}, {179, "│"}, {704, "ˀ"}, {969, "ω"}, {1130, "Ѫ"},
        {180, "┤"}, {705, "ˁ"}, {976, "ϐ"}, {1132, "Ѭ"}, {181, "╡"}, {708, "˄"}, {977, "ϑ"}, {1134, "Ѯ"},
        {182, "╢"}, {709, "˅"}, {978, "ϒ"}, {1136, "Ѱ"}, {183, "╖"}, {710, "ˆ"}, {979, "ϓ"}, {1139, "ѳ"},
        {184, "╕"}, {711, "ˇ"}, {981, "ϕ"}, {1149, "ѽ"}
    };

    private byte[] expectedCode;
    private List<byte> enteredCode;
    private string[] chosenLetters = new string[5];
    private bool lcdIsBlackened = false;

    private void ModuleActivated()
    {
        SetTextAndAutoScale(string.Join(" ", this.chosenLetters));

        for (int i = 0; i < this.NumberButtons.Length; i++) // set up button handlers
        {
            var digit = byte.Parse(this.NumberButtons[i].name.Last().ToString());
            var button = this.NumberButtons[i];
            this.NumberButtons[i].OnInteract += () => { NumberButtonPress(digit, button); return false; };
        }
        this.DisplayButton.OnInteract += () => { DisplayButtonPress(); return false; };
    }

    private void DisplayButtonPress()
    {
        if (rand.NextDouble() < 0.8)
        {
            DisableDisplayBlackening();
        }
    }

    private void NumberButtonPress(byte digit, KMSelectable button)
    {
        if (this.moduleSolved)
            return;

        this.enteredCode.Add(digit);
        RecheckCode();

        StartCoroutine(DoButtonPressAndRelease(button));
    }

    private void RecheckCode()
    {
        for (int i = 0; i < Math.Min(this.enteredCode.Count, this.expectedCode.Length); i++)
        {
            if (this.enteredCode[i] != this.expectedCode[i])
            {
                // strike
                Log("Digit #" + this.enteredCode.Count + " was wrong. Expected: " + this.expectedCode[i] + " Entered: " + this.enteredCode[i] + " Resetting input.");
                this.enteredCode.Clear();
                GetComponent<KMBombModule>().HandleStrike();
                return;
            }
        }

        if (this.enteredCode.Count == this.expectedCode.Length)
        {
            this.moduleSolved = true;
            this.DisplayTextMesh.text = ""; // clear display
            GetComponent<KMBombModule>().HandlePass();

            Log("Last digit (#" + this.enteredCode.Count + ") was correct. - Module solved.");
        }
        else
        {
            Log("Digit #" + this.enteredCode.Count + " was correct.");
        }
    }

    IEnumerator DoButtonPressAndRelease(KMSelectable button)
    {
        button.AddInteractionPunch(1);
        button.transform.Translate(0, 0, -0.005f);
        this.Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, button.transform);
        yield return new WaitForSeconds(1);
        button.transform.Translate(0, 0, 0.005f);
        this.Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonRelease, button.transform);
    }

    // Use this for initialization
    public void Start()
    {
        this.moduleId = moduleIdCounter++;
        Log("Initialized with seed: " + rand.Seed);
        this.DisplayTextMesh.text = ""; // clear display

        // set button positions
        for (int i = 1; i < 10; i++)
        {
            this.NumberButtons[i].transform.position = this.NumberButtons[0].transform.position;
            this.NumberButtons[i].transform.localScale = this.NumberButtons[0].transform.localScale;
            this.NumberButtons[i].transform.Translate(0.0245f * (i % 5), 0.028f * (i / 5), 0);
        }
        GetComponent<KMBombModule>().OnActivate += ModuleActivated;

        // pick characters
        var len = characterList.Count;
        var chosenLetterKVs = new List<KeyValuePair<ushort, string>>(letterCount);
        for (int i = 0; i < letterCount; i++)
        {
            chosenLetterKVs.Add(characterList.ElementAt(rand.Next(len)));
        }
        this.chosenLetters = chosenLetterKVs.Select(x => x.Value).ToArray();
        this.expectedCode = chosenLetterKVs.SelectMany(x => GetDigits(x.Key)).ToArray();
        this.enteredCode = new List<byte>(this.expectedCode.Length);

        Log("Letter generation finished. Expected code: " + string.Join(null, this.expectedCode.Select(x => x.ToString()).ToArray()));
    }

    private void DisableDisplayBlackening()
    {
        this.lcdIsBlackened = false;
        this.LCDRenderMesh.enabled = false;
    }

    private void SetRandomDisplayBlackening()
    {
        this.LCDRenderMesh.enabled = true;
        this.LCDCoverUpMaterial.mainTextureOffset = new Vector2((float)(rand.NextDouble() * 2 - 1), (float)(rand.NextDouble() * 2 - 1));
        this.LCDCoverUpMaterial.mainTextureScale = new Vector2((float)(rand.NextDouble() * 2 - 1), (float)(rand.NextDouble() * 2 - 1));
    }

    private List<byte> GetDigits(ushort number)
    {
        var result = new List<byte>();
        var factor = 1;
        while (factor < ushort.MaxValue)
        {
            result.Add((byte)(number % (10 * factor) / factor));
            factor *= 10;
        }
        bool wasNonZeroOnce = false;
        return result.TakeWhile(x => wasNonZeroOnce = x != 0).Reverse().Cast<byte>().ToList();
    }

    float i = 0;
    const int framesPerUpdate = 120;
    public void Update()
    {
        this.i += Time.deltaTime * 60;
        if (this.i > framesPerUpdate)
        {
            this.i %= framesPerUpdate;
            if (!this.lcdIsBlackened && rand.NextDouble() < 0.05f)
                StartCoroutine(BlackenDisplay());
        }
    }

    private IEnumerator BlackenDisplay(bool initial = true)
    {
        if (initial)
        {
            this.lcdIsBlackened = true;
            SetRandomDisplayBlackening();
        }

        if (this.LCDRenderMesh.enabled)
        {
            this.LCDCoverUpMaterial.mainTextureOffset = Vector2.MoveTowards(this.LCDCoverUpMaterial.mainTextureOffset, Vector2.zero, .1f);
            this.LCDCoverUpMaterial.mainTextureScale = Vector2.Scale(this.LCDCoverUpMaterial.mainTextureScale, new Vector2((float)(rand.NextDouble() * 0.05 + 0.95), (float)(rand.NextDouble() * 0.05 + 0.95)));
            yield return new WaitForSeconds(0.05f);
            yield return BlackenDisplay(false);
        }
    }

    private void SetTextAndAutoScale(string text)
    {
        this.DisplayTextMesh.text = text;

        var tm = this.DisplayTextMesh.GetComponent<TextMesh>();
        var rt = this.DisplayTextMesh.GetComponent<RectTransform>();
        var rectSize = Vector2.Scale(rt.rect.size, new Vector2(7, 5));
        int lastFontSize = 8;
        var gs = new GUIStyle()
        {
            font = tm.font,
            fontStyle = tm.fontStyle,
            fontSize = lastFontSize + 1
        };
        var size = gs.CalcSize(new GUIContent(this.DisplayTextMesh.text));
        while (size.x < rectSize.x && size.y < rectSize.y)
        {
            lastFontSize++;
            gs.fontSize = lastFontSize + 1;
            size = gs.CalcSize(new GUIContent(this.DisplayTextMesh.text));
        }
        Log("Scale limit was: " + (size.x >= rectSize.x ? "X" : "Y"));
        tm.fontSize = lastFontSize;
    }


    private void Log(string message)
    {
        Debug.Log("[CharacterCodes #" + this.moduleId + "] " + message);
    }


    //twitch plays
    protected readonly string TwitchHelpMessage = @"!{0} press 1234 [Presses the buttons 1, 2, 3 and 4 in this order] | !{0} tap [Taps the display to try to make it work again.]";

    protected IEnumerator ProcessTwitchCommand(string command)
    {
        var lowered = command.ToLowerInvariant().Split(' ').First();
        var arg = command.ToLowerInvariant().Split(' ').Skip(1).Join(" ").Replace(" ", null);

        switch (lowered)
        {
            case "press":
                yield return null;
                //yield return new[] { this.ButtonDeal };
                int _a;
                var badChar = arg.Select(x => x.ToString()).FirstOrDefault(x => !int.TryParse(x.ToString(), out _a));
                if (badChar != null)
                {
                    throw new FormatException("The button " + badChar + " could not be found.");
                }

                var buttonsToPress = arg.Select(x => this.NumberButtons[(int.Parse(x.ToString()) + 9) % 10]).ToArray();
                foreach (var button in buttonsToPress)
                {
                    yield return new WaitForSeconds(1);
                    yield return new[] { button };
                }
                break;

            case "tap":
                yield return null;
                yield return new[] { this.DisplayButton };
                break;

            default:
                yield break;
        }
    }
}
