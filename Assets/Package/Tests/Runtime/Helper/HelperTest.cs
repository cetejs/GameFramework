using GameFramework.Helper;
using NUnit.Framework;
using UnityEngine;

public class HelperTest
{
    [Test]
    public void TestBm()
    {
        string text = "abcacabcbcbacabc";
        string pattern = "cbacabc";

        int index = StringSearchHelper.SearchOfBm(text, pattern, true);
        Debug.Log(index);
    }
}