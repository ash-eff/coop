using UnityEngine;
using TMPro;
using Photon.Pun;

public class PlayerEndStats : MonoBehaviourPunCallbacks, IPunObservable
{
    public bool available;
    string playerName;
    string deathTime;
    int zombiesKilled;
    string accuracy;
    int numOfReloads;
    int shotsFired;
    int damageTaken;
    int friendlyFireTaken;
    int friendlyFireCaused;
    int damageHealed;

    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI deathTimeText;
    public TextMeshProUGUI zombiesKilledText;
    public TextMeshProUGUI accuracyText;
    public TextMeshProUGUI numOfReloadsText;
    public TextMeshProUGUI shotsFiredText;
    public TextMeshProUGUI damageTakenText;
    public TextMeshProUGUI friendlyFireTakenText;
    public TextMeshProUGUI friendlyFireCausedText;
    public TextMeshProUGUI damageHealedText;

    public string PlayerName
    {
        set { playerName = value; }
    }

    public string DeathTime
    {
        set { deathTime = value; }
    }

    public int ZombiesKilled
    {
        set { zombiesKilled = value; }
    }

    public string Accuracy
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

    public int DamageTaken
    {
        set { damageTaken = value; }
    }

    public int FriendlyFireTaken
    {
        set { friendlyFireTaken = value; }
    }

    public int FriendlyFireCaused
    {
        set { friendlyFireCaused = value; }
    }

    public int DamageHealed
    {
        set { damageHealed = value; }
    }

    public void Populate()
    {
        playerNameText.text = playerName;
        deathTimeText.text = deathTime;
        zombiesKilledText.text = zombiesKilled.ToString();
        accuracyText.text = accuracy;
        numOfReloadsText.text = numOfReloads.ToString();
        shotsFiredText.text = shotsFired.ToString();
        damageTakenText.text = damageTaken.ToString();
        friendlyFireTakenText.text = friendlyFireTaken.ToString();
        friendlyFireCausedText.text = friendlyFireCaused.ToString();
        damageHealedText.text = damageHealed.ToString();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(this.available);
        }
        else
        {
            // Network player, receive data
            this.available = (bool)stream.ReceiveNext();
        }
    }
}
