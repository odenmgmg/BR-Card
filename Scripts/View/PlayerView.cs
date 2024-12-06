using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerView : MonoBehaviour
{
    [SerializeField]
    public GameObject handArea, unitAreaFront, unitAreaBack, trushArea, commonHandArea, popValue;
    [SerializeField]
    private TextMeshProUGUI textName, textHealth, textMana, textAttack, textDeffence;
    [SerializeField]
    public Image avatar, healthGauge;
    public int playerId;

    [SerializeField]
    public List<StatusObject> statusObjects;

    [SerializeField]
    private Animator animator;

    public void UpdateView(PlayerData playerData, LocalManager localManager)
    {

        playerId = playerData.playerId;
        textName.text = $"{playerData.userName}";
        avatar.sprite = localManager.avatarSprites.data.Find(x => x.ID == playerData.avatarId).sprite;
        if (textHealth.text != $"{playerData.Health}")
        {
            int diff = playerData.Health - int.Parse(textHealth.text);
            if (diff < 0) animator.SetTrigger("damage");
            ValueChanged(PlayerStatus.Health, diff);
        }
        if (textMana.text != $"{playerData.Mana}")
        {
            int diff = playerData.Mana - int.Parse(textMana.text);
            ValueChanged(PlayerStatus.Mana, diff);
        }
        if (textAttack.text != $"{playerData.Attack}")
        {
            int diff = playerData.Attack - int.Parse(textAttack.text);
            ValueChanged(PlayerStatus.Attack, diff);
        }
        if (textDeffence.text != $"{playerData.Deffence}")
        {
            int diff = playerData.Deffence - int.Parse(textDeffence.text);
            ValueChanged(PlayerStatus.Deffence, diff);
        }
        textHealth.text = $"{playerData.Health}";
        healthGauge.fillAmount = playerData.Health / 15f;
        Color32 healthColor = new Color32();
        if (playerData.Health >= 15)
        {
            healthColor = localManager.staticData.uIColors.Find(x => x.text == "LifeMax").color;

        }
        else if (playerData.Health >= 8)
        {
            healthColor = localManager.staticData.uIColors.Find(x => x.text == "LifeHigh").color;
        }
        else if (playerData.Health >= 3)
        {
            healthColor = localManager.staticData.uIColors.Find(x => x.text == "LifeMid").color;
        }
        else
        {
            healthColor = localManager.staticData.uIColors.Find(x => x.text == "LifeLow").color;
        }
        textHealth.color = healthColor;
        healthGauge.color = healthColor;

        textMana.text = $"{playerData.Mana}";
        textAttack.text = $"{playerData.Attack}";
        textDeffence.text = $"{playerData.Deffence}";

        foreach (var item in statusObjects)
        {
            item.Init(playerId);
        }

        animator.SetBool("active", localManager.GetTurnPlayer() == playerId);
    }

    public void ValueChanged(PlayerStatus status, int value)
    {
        StatusObject tmp = statusObjects.Find(x => x.playerStatus == status);
        if (value > 0)
        {
            tmp.animator.SetTrigger("change");
            //Instantiate(popValue, tmp.transform.position, Quaternion.identity, this.transform);
        }
        else
        {
            tmp.animator.SetTrigger("change");
        }

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //foreach (UnitArea child in unitAreaFront.transform)
        //{
        //    child.cardArea = CardArea.FieldFront;
        //    child.potisionX = child.transform.GetSiblingIndex(); ;
        //}
        //foreach (UnitArea child in unitAreaBack.transform)
        //{
        //    child.cardArea = CardArea.FieldBack;
        //    child.potisionX = child.transform.GetSiblingIndex(); ;
        //}
    }

    // Update is called once per frame
    void Update()
    {

    }
}
