using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Rendering.Universal.Internal;

public static class RPCSeriarizer
{
    private static int PARSE_DATA_LENGTH = 4;
    private static string PARSE_DELIMITER = ",";

    public static string convertListToString(List<int> source)
    {
        StringBuilder ret = new StringBuilder();
        foreach (int i in source)
        {
            if(ret.Length>0)ret.Append(PARSE_DELIMITER); //ãÊêÿÇËï∂éö
            ret.Append(i.ToString().PadLeft(PARSE_DATA_LENGTH, '0'));
        }
        Debug.Log(ret.ToString());
        return ret.ToString();
    }
    public static List<int> convertStringToList(string source)
    {
        List<int> ret = new List<int>();
        string[] arr = source.Split(PARSE_DELIMITER);
        foreach (string i in arr)
        {
            ret.Add(int.Parse(i));
        }
        Debug.Log(source);
        return ret;
    }
}
