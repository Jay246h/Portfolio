using TMPro;
using UnityEngine;

public class Player2 : MonoBehaviour
{
    public static Player2 instance;

    public int Health;
    public int Exp;

    public TextMeshProUGUI HealthText;
    public TextMeshProUGUI ExpText;

    private void Awake()
    {
        instance = this;
    }

    public void IncreaseHealth(int value)
    {
        Health += value;
        HealthText.text = $"HP : {Health}";
    }

    public void IncreaseExp(int value)
    {
        Exp += value;
        ExpText.text = $"EXP : {Exp}";
    }
}
