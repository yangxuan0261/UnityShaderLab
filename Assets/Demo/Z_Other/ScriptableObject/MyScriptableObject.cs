using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 定义创建菜单, 在 create -> ScriptableObject -> MyScriptableObject 可以创建该类的序列化资源, 
// 也可以参考 OtherScriptableObject.cs 程序创建资源
[CreateAssetMenu(menuName = "ScriptableObject/MyScriptableObject")]
public class MyScriptableObject : ScriptableObject {

    public string myname = "yangx";
    public int age = 11;
    [Range(0, 20)]
    public float raduis = 12.3f;

    public Content content = new Content();

    public List<Texture> texList = new List<Texture>();

}

[System.Serializable]
public class Content {
    public string en;
    public string cn;
    public string jp;
}