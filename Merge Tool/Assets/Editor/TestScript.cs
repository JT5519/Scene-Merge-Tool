using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TestScript : ScriptableWizard
{
    public GameObject obj1;
    public GameObject obj2;

    [MenuItem("Merge Tool/Test..")]
    static void testobj()
    {
        ScriptableWizard.DisplayWizard<TestScript>("Test Wizard", "Compare");
    }
    private void OnWizardCreate()
    {
        if (obj1.Equals(obj2))
            Debug.Log("Samesies");
        else
            Debug.Log("NOT Samesies");
    }
}
