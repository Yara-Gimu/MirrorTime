using System;
using System.Collections.Generic;
using UnityEngine;

// 🧠 كلاس Static يعني يعيش في الذاكرة طول الوقت، وما ينحط على أي مجسم
public static class EventManager
{
    // 📚 القاموس اللي بيحفظ كل "أسماء الأحداث" ومن جالس ينتظرها
    private static Dictionary<string, Action<Dictionary<string, object>>> eventDictionary = 
               new Dictionary<string, Action<Dictionary<string, object>>>();

    // 🎧 1. دالة الاستماع (اللي تبغى تنتظر حدث معين تستخدم هذي الدالة)
    public static void StartListening(string eventName, Action<Dictionary<string, object>> listener)
    {
        if (eventDictionary.ContainsKey(eventName))
        {
            eventDictionary[eventName] += listener;
        }
        else
        {
            eventDictionary.Add(eventName, listener);
        }
    }

    // 🔇 2. دالة إيقاف الاستماع (عشان ما يصير عندنا تسريب في الذاكرة)
    public static void StopListening(string eventName, Action<Dictionary<string, object>> listener)
    {
        if (eventDictionary.ContainsKey(eventName))
        {
            eventDictionary[eventName] -= listener;
        }
    }

    // 📢 3. دالة إطلاق الحدث (إرسال الرسالة في الجروب)
    public static void TriggerEvent(string eventName, Dictionary<string, object> messageData = null)
    {
        if (eventDictionary.ContainsKey(eventName))
        {
            eventDictionary[eventName]?.Invoke(messageData); // تشغيل كل الأكواد اللي تنتظر هذا الحدث
        }
        else
        {
            Debug.LogWarning("⚠️ EventManager: تم إطلاق حدث [" + eventName + "] لكن لا يوجد أحد يستمع له!");
        }
    }
}