using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class CSVDataIO : MonoBehaviour
{
    [SerializeField]
    CardDataBase CardDataBase;
    private string path;
    private string writeTxt = "hello";
    private string fileName = "BR_CardData - Data.csv";
    private string userFileName = "BR_UserData.csv";

    private string cardDataPath = "/GameData/CardData/";
    private string userFilePath = "/GameData/UserData/";
    private string spriteDataPath = "/GameData/CardImages/";
    private string avatarSpriteDataPath = "/GameData/AvatarImages/";

    [SerializeField]
    private Sprite defaultLocalSprite = null;

    void Start()
    {
        path = Application.dataPath + cardDataPath + fileName;
        Debug.Log(path);
        ReadFile();
        //WriteFile(writeTxt);
    }

    public void SaveUserFile(LocalData localData)
    {
        string txt = localData.userName;
        txt += "," + localData.avatarID.ToString();
        txt += "," + localData.bgm.ToString();
        txt += "," + localData.se.ToString();
        Debug.Log("save:" + txt);

        path = Application.dataPath + userFilePath + userFileName;
        //FileInfo fi = new FileInfo(path);
        using (StreamWriter sw = new StreamWriter(path, false, Encoding.GetEncoding("UTF-8")))
        {
            sw.Write(txt);
            sw.Flush();
        }
    }
    public LocalData LoadUserFile(LocalData localData)
    {
        localData.userName = "player";
        localData.avatarID = 0;
        localData.bgm = 4;
        localData.se = 4;

        path = Application.dataPath + userFilePath + userFileName;
        FileInfo fi = new FileInfo(path);
        using (StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.UTF8))
        {
            string readTxt = sr.ReadToEnd();
            Debug.Log("load:" + readTxt);
            string[] lines = readTxt.Split('\n');
            string[] datas = lines[0].Split(",");
            if (datas != null && datas.Length > 0)
            {
                localData.userName = datas[0];
                localData.avatarID = int.Parse(datas[1]);
                localData.bgm = float.Parse(datas[2]);
                localData.se = float.Parse(datas[3]);

            }
            else
            {
                int i = 0;
                localData.userName = "load:" + datas[i++] + "," + datas[i++] + "," + datas[i++] + "," + datas[i++];
            }

        }
        return localData;
    }

    void ReadFile()
    {
        FileInfo fi = new FileInfo(path);
        try
        {
            using (StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.UTF8))
            {
                string readTxt = sr.ReadToEnd();
                Debug.Log(readTxt);
                //�@Asset��������ǂݍ��ށiResources�ł͂Ȃ��̂Œ��Ӂj
                //TextAsset textasset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                //�@������ScriptableObject�t�@�C����ǂݍ��ށB�Ȃ��ꍇ�͐V���ɍ��B
                string assetfile = path.Replace(".csv", ".asset");

                CardDataBase.data.Clear();
                CardDataBase = ParseFile(readTxt, CardDataBase);
                CardDataBase = FetchSpriteData(CardDataBase);
                //cd.data.AddRange(CSVSerializer.Deserialize<CardData>(textasset.text));
            }

        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    private CardDataBase ParseFile(string data, CardDataBase source)
    {
        CardDataBase ret = source;
        string[] datas = data.Split('\n');
        int index = -1;
        foreach (string line in datas)
        {
            index++;
            if (index == 0) continue;

            CardData tmp = ParseContent(line);
            if (tmp != null)
            {
                ret.data.Add(tmp);
            }
        }
        return ret;
    }

    private CardData ParseContent(string data)
    {
        CardData ret = new CardData();
        string[] datas = data.Split(",");

        if (datas[0] == "") return null;
        //0�O���[�v
        //1ID
        //2�J�[�h���
        //3�J�[�h���ID
        //4�J�[�h��
        //5���~�e�b�h
        //6�R�X�g
        //7�z��_�U����1
        //8�z��_�U����2
        //9�z��_�h���
        //10����
        //11�������
        //12�������ID
        //13���ʗ�
        //14���ʃe�L�X�g
        //15���ʃe�L�X�g�i�^�O�t���j


        //0�O���[�v
        ret.groupID = datas[0];
        //1ID
        ret.cardID = int.Parse(datas[1]);
        //2�J�[�h���
        //3�J�[�h���ID
        ret.cardType = (CardType)int.Parse(datas[3]);
        //4�J�[�h��
        ret.cardName = datas[4];
        //5���~�e�b�h
        ret.limited = datas[5];
        //6�R�X�g
        ret.cost = datas[6];
        //7�z��_�U����1
        ret.unit_atk1 = datas[7];
        //8�z��_�U����2
        ret.unit_atk2 = datas[8];
        //9�z��_�h���
        ret.unit_def = datas[9];
        //10����
        ret.coin = datas[10];
        //11�������
        //12�������ID
        ret.specialID = (Special)int.Parse(datas[12]);
        //13���ʗ�
        ret.specialValue = datas[13];
        //14���ʃe�L�X�g
        //15���ʃe�L�X�g�i�^�O�t���j
        ret.effectText = datas[15];
        //ret.cardImage = FetchSpriteData(ret);

        return ret;
    }

    private CardDataBase FetchSpriteData(CardDataBase cardDataBase)
    {
        try
        {
            // Only get files that begin with the letter "c".
            //string filePath = Application.dataPath + spriteDataPath + $"{data.groupID}_{data.cardID}_*.png";
            string[] dirs = Directory.GetFiles($"{Application.dataPath}{spriteDataPath}", "*.png");
            Sprite defaultSprite = null;
            Debug.Log($"The number of files starting with * is {dirs.Length}.");
            foreach (string dir in dirs)
            {
                Debug.Log($"{dir}");
                if (!File.Exists(dir)) continue;
                Debug.Log($"{dir}:file exists");

                string fileName = Path.GetFileName(dir);
                string[] ID = fileName.Split('_');

                if (ID[1] == "XX")
                {
                    byte[] tmpFileData = File.ReadAllBytes(dir); // �t�@�C���f�[�^���o�C�g�z��Ƃ��ēǂݍ���
                    if (tmpFileData == null) continue;
                    Debug.Log($"{dir}: texture exists");

                    Texture2D tmpTexture = new Texture2D(2, 2); // ��̃e�N�X�`�����쐬����
                    tmpTexture.LoadImage(tmpFileData); // �e�N�X�`���Ƀt�@�C���f�[�^�����[�h����
                    defaultSprite = Sprite.Create(tmpTexture, new Rect(0, 0, tmpTexture.width, tmpTexture.height), Vector2.zero); // �I�u�W�F�N�g�̃}�e���A���Ƀe�N�X�`�����Z�b�g����

                    continue;
                }
                try { int.Parse(ID[1]); }
                catch { continue; }
                CardData cardData = cardDataBase.data.Find(x => x.groupID == ID[0] & x.cardID == int.Parse(ID[1]));
                if (cardData == null) { continue; }
                Debug.Log($"{dir}:data exists");

                byte[] fileData = File.ReadAllBytes(dir); // �t�@�C���f�[�^���o�C�g�z��Ƃ��ēǂݍ���
                if (fileData == null) continue;
                Debug.Log($"{dir}: texture exists");

                Texture2D texture = new Texture2D(2, 2); // ��̃e�N�X�`�����쐬����
                texture.LoadImage(fileData); // �e�N�X�`���Ƀt�@�C���f�[�^�����[�h����
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero); // �I�u�W�F�N�g�̃}�e���A���Ƀe�N�X�`�����Z�b�g����
                cardData.cardImage = sprite;
            }
            foreach (CardData data in cardDataBase.data)
            {
                if (data.cardImage != null) continue;
                if (data.cardImage == null & defaultSprite != null) data.cardImage = defaultSprite;
                if (data.cardImage == null & defaultSprite == null) data.cardImage = defaultLocalSprite;
            }

        }
        catch (Exception e)
        {
            Console.WriteLine("The process failed: {0}", e.ToString());
        }



        return cardDataBase;
    }

    public SpriteDataBase FetchAvatarSpriteData(SpriteDataBase ret)
    {
        ret.data.Clear();
        try
        {
            // Only get files that begin with the letter "c".
            //string filePath = Application.dataPath + spriteDataPath + $"{data.groupID}_{data.cardID}_*.png";
            string[] dirs = Directory.GetFiles($"{Application.dataPath}{avatarSpriteDataPath}", "*.png");
            Debug.Log($"The number of files starting with * is {dirs.Length}.");
            foreach (string dir in dirs)
            {
                Debug.Log($"{dir}");
                if (!File.Exists(dir)) continue;
                Debug.Log($"{dir}:file exists");

                string fileName = Path.GetFileName(dir);

                byte[] fileData = File.ReadAllBytes(dir); // �t�@�C���f�[�^���o�C�g�z��Ƃ��ēǂݍ���
                if (fileData == null) continue;
                Debug.Log($"{dir}: texture exists");

                Texture2D texture = new Texture2D(2, 2); // ��̃e�N�X�`�����쐬����
                texture.LoadImage(fileData); // �e�N�X�`���Ƀt�@�C���f�[�^�����[�h����
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero, 100f, 1, SpriteMeshType.FullRect); // �I�u�W�F�N�g�̃}�e���A���Ƀe�N�X�`�����Z�b�g����

                SpriteData spriteData = new SpriteData
                {
                    sprite = sprite,
                    ID = int.Parse(fileName.Substring(7, 2)),
                    fileName = fileName,
                };

                ret.data.Add(spriteData);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("The process failed: {0}", e.ToString());
        }



        return ret;
    }
}