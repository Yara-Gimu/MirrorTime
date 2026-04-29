using UnityEngine;
using UnityEngine.Assertions;

public class ArabicSupportTester : MonoBehaviour 
{
    public ExpectedFixedText[] ExpectedTexts;    

    void Start()    
    {        
        // التعديل هنا: استخدام الكود الحديث والأسرع من يونتي
        ExpectedTexts = FindObjectsByType<ExpectedFixedText>(FindObjectsSortMode.None);        
        
        foreach (var expectedText in ExpectedTexts)        
        {       
            expectedText.Fix();            
            Assert.AreEqual(expectedText.Expected, expectedText.Fixed);        
        }    
    }
}