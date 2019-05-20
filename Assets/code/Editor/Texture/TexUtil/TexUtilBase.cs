using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;

public abstract class TexUtilBase {

    public ETexUtil mMod = ETexUtil.ScreenShot;
    public string mDescr = "hello";

    public virtual void Draw() {

    }

}