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
        textTurnCount.text = $"ターン{count.ToString()}:[{battlePhase}]";
        if (player == localPlayer)
        {
            textTurnPlayer.text = $"自分の番";
            image.color = playerColor;
            textEndButton.text = "ターン終了";
        }
        else
        {
            textTurnPlayer.text = $"相手の番";
            image.color = enemyColor;
            textEndButton.text = "相手の行動中...";

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
