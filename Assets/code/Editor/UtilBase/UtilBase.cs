using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public abstract class UtilBase<E> {

    public E mMod;
    public string mDescr = "UtilBase";

    public virtual void Draw() { }
    public virtual void OnUtilEnter() { }
    public virtual void OnUtilExit() { }

}