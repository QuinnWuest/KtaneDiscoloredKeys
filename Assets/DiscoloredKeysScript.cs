using System.Collections;
using System.Linq;
using UnityEngine;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using Rnd = UnityEngine.Random;

public class DiscoloredKeysScript : MonoBehaviour
{

    public KMAudio Audio;
    public KMBombInfo BombInfo;
    public KMBombModule Module;

    public KMSelectable[] ButtonSels;
    public GameObject[] ButtonObjs;
    public Material[] ButtonMats;
    public TextMesh DisplayText;
    public Color[] DisplayColors;

    private int _moduleId;
    private static int _moduleIdCounter = 1;
    private bool _moduleSolved;

    private static readonly string[] _colorNames = new string[] { "blue", "cyan", "green", "lime", "magenta", "orange", "purple", "red", "white", "yellow" };
    private int[] _btnColors = new int[4];

    private char[] _grid = new char[196];

    private void Start()
    {
        _moduleId = _moduleIdCounter++;
        for (int btn = 0; btn < ButtonSels.Length; btn++)
            ButtonSels[btn].OnInteract += ButtonPress(btn);
        for (int i = 0; i < 4; i++)
        {
            _btnColors[i] = Rnd.Range(1, _colorNames.Length);
            ButtonObjs[i].GetComponent<MeshRenderer>().material = ButtonMats[_btnColors[i]];
        }
    }

    private KMSelectable.OnInteractHandler ButtonPress(int btn)
    {
        return delegate ()
        {
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, ButtonSels[btn].transform);
            if (_moduleSolved)
                return false;
            return false;
        };
    }

    private IEnumerator SolveAnim()
    {
        Audio.PlaySoundAtTransform("success", transform);
        int i = 0;
        DisplayText.text = "CORRECT";
        DisplayText.color = DisplayColors[5];
        yield return new WaitForSeconds(0.5f);
        while (i != 50)
        {
            DisplayText.text = _colorNames[Rnd.Range(0, _colorNames.Length)].ToUpper();
            DisplayText.color = DisplayColors[Rnd.Range(0, DisplayColors.Length)];
            for (int j = 0; j < 4; j++)
                ButtonObjs[j].transform.localPosition = ButtonObjs[j].transform.localPosition + Vector3.up * -0.0005f;
            i++;
            yield return new WaitForSeconds(0.02f);
        }
        DisplayText.text = "";
    }

#pragma warning disable 0414
    private readonly string TwitchHelpMessage = "!{0} press TL [Presses top left button.] | Buttons are TL, TR, BL, BR. | 'press' is optional.";
#pragma warning restore 0414

    private static readonly string[] _tpBtnPos = new string[] { "tl", "tr", "bl", "br" };

    private IEnumerator ProcessTwitchCommand(string command)
    {
        var parameters = command.ToLowerInvariant().Split(' ');
        int ix = parameters[0] == "press" || parameters[0] == "submit" ? 1 : 0;
        if (parameters.Length != ix + 1)
            yield break;
        int buttonIx = Array.IndexOf(_tpBtnPos, parameters[ix]);
        if (buttonIx == -1)
            yield break;
        yield return null;
        ButtonSels[buttonIx].OnInteract();
    }
}