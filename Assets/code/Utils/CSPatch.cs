using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// csharp 补丁
public static class CSPatch {

    public static string Shuffle(this string str) {
        char[] array = str.ToCharArray();
        System.Random rng = new System.Random();
        int n = array.Length;
        while (n > 1) {
            n--;
            int k = rng.Next(n + 1);
            var value = array[k];
            array[k] = array[n];
            array[n] = value;
        }
        return new string(array);
    }
}