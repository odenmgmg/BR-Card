
public enum SessionPhase
{
    None, //�ʏ펞
    Ready, // �����t�F�[�Y
    Battle, // �퓬�t�F�[�Y
    Result // ���ʔ��\�t�F�[�Y
}
public enum BattlePhase
{
    None, //�J�n�O
    BattleStart, // �ΐ�J�n
    TurnStart, // �^�[���J�n
    Action, // �s���\
    TurnEnd, // �^�[���I��
}
public enum CardArea
{
    None, //�ݒ�Ȃ�
    Deck, // �R�D
    Hand, // ��D
    FieldFront, // ��F�O��
    FieldBack, // ��F���
    Trush, // �̎D
    CommonHand, //���ʎD
    Pool, //�J�[�h�v�[��
    Picked, //�s�b�N��
}
public enum CardState
{
    None, //�ݒ�Ȃ�
    Open, // ���J
    UserOnlyOpen, // �����������J
    Close, // ����J
}

public enum CardType
{
    None, //�ݒ�Ȃ�
    Attack, // ����
    Magic, // ���@
    Support, // �⏕
    Tactics, // �z��
    Unique, //����
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
    BLD,//�\�z
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


