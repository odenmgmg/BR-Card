
public enum SessionPhase
{
    None, //通常時
    Ready, // 準備フェーズ
    Battle, // 戦闘フェーズ
    Result // 結果発表フェーズ
}
public enum BattlePhase
{
    None, //開始前
    BattleStart, // 対戦開始
    TurnStart, // ターン開始
    Action, // 行動可能
    TurnEnd, // ターン終了
}
public enum CardArea
{
    None, //設定なし
    Deck, // 山札
    Hand, // 手札
    FieldFront, // 場：前列
    FieldBack, // 場：後列
    Trush, // 捨札
    CommonHand, //共通札
    Pool, //カードプール
    Picked, //ピック済
}
public enum CardState
{
    None, //設定なし
    Open, // 公開
    UserOnlyOpen, // 自分だけ公開
    Close, // 非公開
}

public enum CardType
{
    None, //設定なし
    Attack, // 物理
    Magic, // 魔法
    Support, // 補助
    Tactics, // 布石
    Unique, //特殊
}

public enum PlayerStatus
{
    Health,
    Mana,
    Attack,
    Deffence
}

public enum FuncButton
{
    TuenEnd,
    PickFinish,
    BattleEnd,
    Matching,
    OpenCardList,
    CloseCardList,
    Watching,

}

public enum Rule
{
    BLD,//構築
    BR1, BR2, BR3, BR4, BR5
}

public enum Special
{
    None,
    CommonAtk,
    CommonDef,
    CommonBst,
    Seal,
    AddCost,
    Freeze,
    GuardBreak,
    Cloak,
    TimeChange,
}


