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

    private Material LCDCoverUpMaterialCopy;


    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    private const int letterCount = 5;
    private static readonly MonoRandom rand = new MonoRandom(); // todo replace with unity random number gen.
    private static readonly Dictionary<ushort, string> characterList = new Dictionary<ushort, string>
    {
        {3,"♥"},{30,"▲"},{111,"א"},{984,"Ϙ"},{4,"♦"},{31,"▼"},{112,"↟"},{985,"Ø"},
        {5,"♣"},{33,"!"},{113,"≋"},{986,"æ"},{6,"♠"},{34,"“"},{114,"∞"},{987,"ϛ"},
        {7,"•"},{35,"#"},{115,"①"},{990,"ʒ"},{8,"◘"},{36,"$"},{116,"②"},{991,"ϟ"},
        {9,"○"},{37,"%"},{120,"③"},{992,"ə"},{10,"◙"},{38,"&"},{121,"④"},{993,"Ω"},
        {11,"♂"},{39,"Ö"},{122,"⑤"},{994,"Ϣ"},{12,"♀"},{40,"("},{123,"⑥"},{995,"ζ"},
        {13,"♪"},{41,")"},{124,"⑦"},{996,"Ϥ"},{14,"♫"},{42,"*"},{125,"⑧"},{998,"Ϧ"},
        {15,"☼"},{43,"+"},{126,"⑨"},{999,"ϧ"},{16,"►"},{44,","},{130,"⓪"},{1000,"✦"},
        {17,"◄"},{45,"-"},{132,"☣"},{1002,"Ϫ"},{18,"↕"},{46,"|"},{134,"☑"},{1004,"Ϭ"},
        {19,"‼"},{47,"/"},{138,"☒"},{1005,"Π"},{20,"¶"},{58,":"},{139,"צ"},{1008,"ϰ"},
        {21,"§"},{59,";"},{140,"ʬ"},{1009,"ϱ"},{22,"☻"},{60,"<"},{141,"Ч"},{1010,"Χ"},
        {23,"↨"},{61,"="},{142,"И"},{1012,"ϴ"},{24,"↑"},{62,">"},{150,"✐"},{1013,"ϵ"},
        {25,"↓"},{63,"?"},{151,"✏"},{1014,"϶"},{26,"→"},{64,"@"},{152,"✎"},{1015,"Ψ"},
        {27,"←"},{91,"["},{156,"✓"},{1016,"ϸ"},{28,"∟"},{92,"\\"},{157,"❖"},{1017,"Ϲ"},
        {29,"↔"},{93,"]"},{740,"Ȼ"},{1018,"Ϻ"}
    };

    private byte[] expectedCode;
    private List<byte> enteredCode;
    private string[] chosenLetters = new string[5];
    private bool lcdIsBlackened = false;
    private Coroutine _buttonAnimation;

    private void ModuleActivated()
    {
        SetTextAndAutoScale(string.Join(" ", this.chosenLetters));

        for (int i = 0; i < this.NumberButtons.Length; i++) // set up button handlers
        {
            var digit = byte.Parse(this.NumberButtons[i].name.Last().ToString());
            var button = this.NumberButtons[i];
            this.NumberButtons[i].OnInteract += () => { NumberButtonPress(digit, button); return false; };
            this.NumberButtons[i].OnInteractEnded += () => { NumberButtonRelease(digit, button); };
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
        // Animation, sounds, interaction punch.
        button.AddInteractionPunch(0.5f);
        this.Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, button.transform);
        if (_buttonAnimation != null)
            StopCoroutine(_buttonAnimation);
        _buttonAnimation = StartCoroutine(ButtonAnimation(button, true));

        if (this.moduleSolved)
            return;

        this.enteredCode.Add(digit);
        RecheckCode();
    }

    private void NumberButtonRelease(byte digit, KMSelectable button)
    {
        // Animation, sounds.
        this.Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonRelease, button.transform);
        if (_buttonAnimation != null)
            StopCoroutine(_buttonAnimation);
        _buttonAnimation = StartCoroutine(ButtonAnimation(button, false));

        if (this.moduleSolved)
            return;
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

    private IEnumerator ButtonAnimation(KMSelectable button, bool isPress)
    {
        var duration = 0.1f;
        var elapsed = 0f;
        var curPos = button.transform.localPosition;
        var goal = isPress ? 0.011f : 0.0134f;
        while (elapsed < duration)
        {
            button.transform.localPosition = new Vector3(curPos.x, Mathf.Lerp(curPos.y, goal, elapsed / duration), curPos.z);
            yield return null;
            elapsed += Time.deltaTime;
        }
        button.transform.localPosition = new Vector3(curPos.x, goal, curPos.z);
    }

    // Use this for initialization
    public void Start()
    {
        this.moduleId = moduleIdCounter++;
        this.DisplayTextMesh.text = ""; // clear display
        this.LCDCoverUpMaterialCopy = new Material(this.LCDCoverUpMaterial);
        this.LCDRenderMesh.material = this.LCDCoverUpMaterialCopy;

        // set button positions
        for (int i = 1; i < 10; i++)
        {
            var btn1Pos = this.NumberButtons[0].transform.localPosition;
            this.NumberButtons[i].transform.localPosition = new Vector3(btn1Pos.x + 0.0245f * (i % 5), btn1Pos.y, btn1Pos.z - 0.028f * (i / 5));
            this.NumberButtons[i].transform.localScale = this.NumberButtons[0].transform.localScale;
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
        Log("Grouped code: " + string.Join(" ", chosenLetterKVs.Select(x => x.Key.ToString()).ToArray()));
    }

    private void DisableDisplayBlackening()
    {
        this.lcdIsBlackened = false;
        this.LCDRenderMesh.enabled = false;
    }

    private void SetRandomDisplayBlackening()
    {
        this.LCDRenderMesh.enabled = true;
        this.LCDCoverUpMaterialCopy.mainTextureOffset = new Vector2((float)(rand.NextDouble() * 2 - 1), (float)(rand.NextDouble() * 2 - 1));
        this.LCDCoverUpMaterialCopy.mainTextureScale = new Vector2((float)(rand.NextDouble() * 2 - 1), (float)(rand.NextDouble() * 2 - 1));
    }

    private List<byte> GetDigits(ushort number)
    {
        var result = new List<byte>();
        var factor = 1;
        while (factor < ushort.MaxValue && number >= factor)
        {
            result.Add((byte)(number % (10 * factor) / factor));
            factor *= 10;
        }
        return result.Cast<byte>().Reverse().ToList();
    }

    float i = 0;
    const int framesPerUpdate = 120;
    const int framesTillBlackeningCanHappen = 60 * 20 / framesPerUpdate; // every 20s
    private int blackeningFramesLeft = framesTillBlackeningCanHappen;
    public void Update()
    {
        this.i += Time.deltaTime * 60;
        if (this.i > framesPerUpdate)
        {
            this.i %= framesPerUpdate;

            if (this.blackeningFramesLeft-- <= 0 && !this.lcdIsBlackened)
            {
                this.blackeningFramesLeft = framesTillBlackeningCanHappen;

                if (rand.NextDouble() < 0.1f)
                    StartCoroutine(BlackenDisplay());
            }
        }
    }

    private IEnumerator BlackenDisplay()
    {
        this.lcdIsBlackened = true;
        SetRandomDisplayBlackening();

        while (this.LCDRenderMesh.enabled) // TODO maybe move to one material per module
        {
            this.LCDCoverUpMaterialCopy.mainTextureOffset = Vector2.MoveTowards(this.LCDCoverUpMaterialCopy.mainTextureOffset, Vector2.zero, .1f);
            this.LCDCoverUpMaterialCopy.mainTextureScale = Vector2.Scale(this.LCDCoverUpMaterialCopy.mainTextureScale, new Vector2((float)(rand.NextDouble() * 0.05 + 0.95), (float)(rand.NextDouble() * 0.05 + 0.95)));
            yield return new WaitForSeconds(0.05f);
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
                    yield return new WaitForSeconds(0.3f);
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

    private IEnumerator TwitchHandleForcedSolve()
    {
        var code = expectedCode.Select(i => (int)i).ToArray();
        for (int i = enteredCode.Count; i < code.Length; i++)
        {
            NumberButtons[(code[i] + 9) % 10].OnInteract();
            yield return new WaitForSeconds(0.1f);
            NumberButtons[(code[i] + 9) % 10].OnInteractEnded();
            yield return new WaitForSeconds(0.3f);
        }
    }
}
