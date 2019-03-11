using UnityEngine;
using TMPro;

public class PlayerEndStats : MonoBehaviour
{
    public bool available;
    string playerName;
    float deathTime;
    int zombiesKilled;
    float accuracy;
    int numOfReloads;
    int shotsFired;
    float damageTaken;
    float damageHealed;

    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI deathTimeText;
    public TextMeshProUGUI zombiesKilledText;
    public TextMeshProUGUI accuracyText;
    public TextMeshProUGUI numOfReloadsText;
    public TextMeshProUGUI shotsFiredText;
    public TextMeshProUGUI damageTakenText;
    public TextMeshProUGUI damageHealedText;

    public string PlayerName
    {
        set { playerName = value; }
    }

    public float DeathTime
    {
        set { deathTime = value; }
    }

    public int ZombiesKilled
    {
        set { zombiesKilled = value; }
    }

    public float Accuracy
    {
        set { accuracy = value; }
    }

    public int NumOfReloads
    {
        set { numOfReloads= value; }
    }

    public int ShotsFired
    {
        set { shotsFired = value; }
    }

    public float DamageTaken
    {
        set { damageTaken = value; }
    }

    public float DamageHealed
    {
        set { damageHealed = value; }
    }

    public void Populate()
    {
        playerNameText.text = playerName;
        deathTimeText.text = deathTime.ToString();
        zombiesKilledText.text = zombiesKilled.ToString();
        accuracyText.text = accuracy.ToString();
        numOfReloadsText.text = numOfReloads.ToString();
        shotsFiredText.text = shotsFired.ToString();
        damageTakenText.text = damageTaken.ToString();
        damageHealedText.text = damageHealed.ToString();
    }
}
