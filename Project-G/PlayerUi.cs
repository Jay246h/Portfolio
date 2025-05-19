using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Playables;

public class PlayerUi : MonoBehaviour
{
    [Header("Image")]
    public Image frontGageBar;
    public Image backGageBar;
    public Image frontHealthBar;
    public Image backHealthBar;

    [Header("MaxValue")]
    public float maxGage;
    public float maxHealth;

    [Header("CurrentValue")]
    public float currentGage;
    public float currentHealth;

    [Header("Score")]
    public TMP_Text scoreText;

    [Header("ScoreValue")]
    public int score;

    [Header("Slot")]
    [SerializeField] private GameObject selectedSlot;
    [SerializeField] private GameObject unSelectedSlot;
    [SerializeField] private GameObject slot1;
    [SerializeField] private GameObject slot2;


    private float lerpTimer;
    private readonly float gagelerpSpeed = 200f;
    private readonly float hplerpSpeed = 200f;

    public PlayerState state;

    public SkillInventory inventory;

    private void Start()
    {
        for (int i = 0; i < inventory.slots.Length; i++)
        {
            inventory.slots[i].Init();
        }

    }

    public void StartGetScore()
    {
        StartCoroutine(GetScore());
    }

    private void Update()
    {
        if (GameManager.Instance.gameState == GameState.InGame)
        {
            currentGage = Mathf.Clamp(currentGage, 0, maxGage);
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            if (currentGage <= 0)
            {
                state = PlayerState.NoGageLeft;
            }
            else
            {
                state = PlayerState.GageLeft;
            }

            if (currentHealth <= 0)
            {
                state = PlayerState.Die;
                UIManager.Instance.GameOver();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                inventory.SlotChange();
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                inventory.UseSkill();
            }
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.gameState == GameState.InGame)
            UpdateOreStateUI();
    }

    public void UpdateOreStateUI()
    {
        float gageFillFront = frontGageBar.fillAmount;
        float gageFillBack = backGageBar.fillAmount;

        float gageFraction = currentGage / maxGage;

        if (gageFillBack > gageFraction)
        {
            frontGageBar.fillAmount = gageFraction;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / gagelerpSpeed;
            backGageBar.fillAmount = Mathf.Lerp(gageFillBack, gageFraction, percentComplete);
        }

        if (gageFillFront < gageFraction)
        {
            backGageBar.fillAmount = gageFraction;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / gagelerpSpeed;
            frontGageBar.fillAmount = Mathf.Lerp(gageFillFront, backGageBar.fillAmount, percentComplete);
        }

        float hpFillFront = frontHealthBar.fillAmount;
        float hpFillBack = backHealthBar.fillAmount;

        float hpFraction = currentHealth / maxHealth;

        if (hpFillBack > hpFraction)
        {
            frontHealthBar.fillAmount = hpFraction;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / hplerpSpeed;
            backHealthBar.fillAmount = Mathf.Lerp(hpFillBack, hpFraction, percentComplete);
        }

        if (hpFillFront < hpFraction)
        {
            backHealthBar.fillAmount = hpFraction;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / hplerpSpeed;
            frontHealthBar.fillAmount = Mathf.Lerp(hpFillFront, backHealthBar.fillAmount, percentComplete);
        }
    }

    public IEnumerator GetScore()
    {
        score += 1;
        scoreText.text = score.ToString();
        yield return new WaitForSeconds(0.05f);
        StartCoroutine(GetScore());
    }

    public void TickGage(float value, bool isAdd)
    {
        if (isAdd)
        {
            currentGage += value * Time.deltaTime;
        }
        else
        {
            currentGage -= value * Time.deltaTime;
        }
    }

    public void AddGage(float value)
    {
        currentGage += value;
    }

    public void Damaged(float value)
    {
        currentHealth -= value;
    }

    public void Healed(float value)
    {
        currentHealth += value;
    }
}
