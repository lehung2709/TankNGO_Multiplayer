using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class LeaderBoardEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text displayText;

    private FixedString32Bytes displayName;

    public int TeamIndex { get; private set; }
    public ulong ClientId { get; private set; }
    public int Scores { get; private set; }

    public void Initialise(ulong clientId, FixedString32Bytes displayName, int scores)
    {
        ClientId = clientId;
        this.displayName = displayName;

        UpdateScores(scores);
    }

    public void Initialise(int teamIndex, FixedString32Bytes displayName, int scores)
    {
        TeamIndex = teamIndex;
        this.displayName = displayName;

        UpdateScores(scores);
    }

    public void SetColour(Color colour)
    {
        displayText.color = colour;
    }

    public void UpdateScores(int scores)
    {
        Scores = scores;

        UpdateText();
    }

    public void UpdateText()
    {
        displayText.text = $"{transform.GetSiblingIndex() + 1}. {displayName} ({Scores})";
    }
}
