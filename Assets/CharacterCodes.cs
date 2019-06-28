using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterCodes : MonoBehaviour
{

    public KMAudio Audio;
    public KMBombInfo BombInfo;

    public KMSelectable[] Buttons;
    public KMSelectable DisplayButton;
    public TextMesh DisplayTextMesh;


    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    private List<KMSelectable> NumberButtons = new List<KMSelectable>();
    private static readonly MonoRandom rand = new MonoRandom();
    private const string charRanges = "1-31, 33-47, 58-64, 91-96, 123-127, 166-249, 251-255, 697-705, 708-759, 761-767 890-893, 901, 916, 926, 928, 936, 946-948, 950-952, 957-958, 961-962, 965, 967-969, 976-979, 981-987, 990-1000, 1002, 1004-1005, 1008-1010, 1012-1023, 1026, 1028-1030, 1032-1035, 1039-1051, 1059, 1061-1073, 1083, 1119-1122, 1124, 1126, 1128, 1130, 1132, 1134, 1136, 1139, 1149";
    private static List<char> characters = charRanges
        .Replace(" ", null)
        .Split(',')
        .Select(x => x.Contains('-') ? x.Split('-').Select(y => int.Parse(y)).ToArray() : new int[] { int.Parse(x) })
        .SelectMany(x => x.Length == 1 ? new[] { (char)x[0] } : Enumerable.Range(x[0], x[1] - x[0]).Select(y => (char)y).ToArray())
        .ToList();

    private void ModuleActivated()
    {
        this.moduleId = moduleIdCounter++;

        //RenewDeal();

        //this.ButtonDeal.OnInteract += () => { ButtonDealPress(); return false; };
        //this.ButtonRenew.OnInteract += () => { ButtonRenewPress(); return false; };

    }

    IEnumerator DoButtonPressAndRelease(KMSelectable button)
    {
        button.AddInteractionPunch(1);
        button.transform.Translate(0, 0, -0.01f);
        this.Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, button.transform);
        yield return new WaitForSeconds(1);
        button.transform.Translate(0, 0, 0.01f);
        this.Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonRelease, button.transform);
    }

    //private void ButtonRenewPress()
    //{
    //    if (this.moduleSolved || this.textIsStillUpdating)
    //        return;

    //    if (this.isGoodDeal)
    //        GetComponent<KMBombModule>().HandleStrike();

    //    StartCoroutine(DoButtonPressAndRelease(this.ButtonRenew));
    //    RenewDeal();
    //}

    //private void ButtonDealPress()
    //{
    //    if (this.moduleSolved || this.textIsStillUpdating)
    //        return;

    //    StartCoroutine(DoButtonPressAndRelease(this.ButtonDeal));

    //    Log("Deal Pressed. Checking result.");
    //    if (!this.isGoodDeal)
    //    {
    //        GetComponent<KMBombModule>().HandleStrike();
    //        RenewDeal();
    //    }
    //    else
    //    {
    //        this.moduleSolved = true;
    //        ClearDisplay(true); // clear display fast

    //        GetComponent<KMBombModule>().HandlePass();
    //    }
    //}


    // Use this for initialization
    public void Start()
    {
        Log("Initialized with seed: " + rand.Seed);
        this.DisplayTextMesh.text = ""; // clear display

        // generate buttons
        this.NumberButtons.Add(this.BaseButton);
        //var colliderSize = this.BaseButton.GetComponentInChildren<BoxCollider>().size;
        for (int i = 1; i < 10; i++)
        {
            instance.transform.position = this.BaseButton.transform.position;
            instance.transform.localScale = this.BaseButton.transform.localScale;
            instance.transform.Translate(0.0245f * (i % 5), 0.028f * (i / 5),0);
            var highlight = instance.transform.GetComponentInChildren<KMHighlightable>().gameObject;
            var boxCollider = highlight.AddComponent<BoxCollider>();
            //boxCollider.size = this.BaseButton.GetComponentInChildren<BoxCollider>().size;
            var testSelectableArea = highlight.AddComponent<TestSelectableArea>();
            testSelectableArea.Selectable = instance.GetComponent<TestSelectable>();
            this.NumberButtons.Add(instance);
            this.BaseButton.transform.parent.GetComponent<KMSelectable>().Children[i + 1] = this.NumberButtons[i]; // setup selectables
        }

        GetComponent<KMBombModule>().OnActivate += ModuleActivated;
    }

    float i = 0;
    const int framesPerUpdate = 5;
    bool textIsStillUpdating = false;
    public void Update()
    {
        this.i += Time.deltaTime * 60;
        if (this.i > framesPerUpdate)
        {
            this.i %= framesPerUpdate;

        }
    }


    private void Log(string message)
    {
        Debug.Log("[CharacterCodes #" + this.moduleId + "] " + message);
    }


    ////twitch plays
    //protected readonly string TwitchHelpMessage = @"!{0} deal [Presses the DEAL!-button] | !{0} nodeal [Fetches a new deal]";

    //protected IEnumerator ProcessTwitchCommand(string command)
    //{
    //    var lowered = command.ToLowerInvariant().Replace(" ", null).TrimEnd('!', '.');

    //    switch (lowered)
    //    {
    //        case "deal":
    //            yield return null;
    //            yield return new[] { this.ButtonDeal };
    //            break;

    //        case "nodeal":
    //            yield return null;
    //            yield return new[] { this.ButtonRenew };
    //            break;

    //        default:
    //            yield break;
    //    }
    //}

    private T PickSeededRandom<T>(List<T> source)
    {
        return source[rand.Next(0, source.Count)];
    }
}


internal class Currency
{
    internal readonly string currencyName;
    internal readonly float currencyValue;

    private Currency() { }

    public Currency(string currencyName, float currencyValue)
    {
        this.currencyName = currencyName;
        this.currencyValue = currencyValue;
    }
}

internal class Unit
{
    internal readonly string unitName;
    internal readonly string unitNamePlural;
    internal readonly float unitValue;
    internal readonly bool countable;

    private Unit() { }

    public Unit(string unitName, string unitNamePlural, float unitvalue, bool countable)
    {
        this.unitName = unitName;
        this.unitNamePlural = unitNamePlural;
        this.unitValue = unitvalue;
        this.countable = countable;
    }
}

public class DealItem
{
    internal readonly string friendlyName;
    internal readonly string pluralFriendlyName;
    internal readonly float value;
    internal readonly bool countable;

    private DealItem() { }

    public DealItem(string friendlyName, string pluralFriendlyName, float value, bool countable)
    {
        this.friendlyName = friendlyName;
        this.pluralFriendlyName = pluralFriendlyName;
        this.value = value;
        this.countable = countable;
    }
}