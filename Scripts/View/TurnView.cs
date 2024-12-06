using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TurnView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textTurnCount, textTurnPlayer, textEndButton;
    [SerializeField]
    private Image image;
    [SerializeField]
    private Color playerColor, enemyColor;

    public void UpdateView(int count, int player, BattlePhase battlePhase, int localPlayer)
    {
        textTurnCount.text = $"�^�[��{count.ToString()}:[{battlePhase}]";
        if (player == localPlayer)
        {
            textTurnPlayer.text = $"�����̔�";
            image.color = playerColor;
            textEndButton.text = "�^�[���I��";
        }
        else
        {
            textTurnPlayer.text = $"����̔�";
            image.color = enemyColor;
            textEndButton.text = "����̍s����...";

        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
