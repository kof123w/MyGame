using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RefRoot : MonoBehaviour
{  
    public List<Object> RefList = new List<Object>(); 
    public void AddRef(Object obj)
    {
        RefList.Add(obj);
    }

    public Text GetText(int index)
    {
        return RefList[index] as Text;
    }

    public Image GetImage(int index)
    {
        return RefList[index] as Image;
    }

    public Button GetButton(int index)
    {
        return RefList[index] as Button;
    }

    public InputField GetInputField(int index)
    {
        return RefList[index] as InputField;
    }

    public RectTransform GetRectTransform(int index)
    {
        return RefList[index] as RectTransform;
    }

    public Transform GetTransform(int index)
    {
        return RefList[index] as Transform;
    }

    public GameObject GetGameObject(int index)
    {
        return RefList[index] as GameObject;
    }
}
