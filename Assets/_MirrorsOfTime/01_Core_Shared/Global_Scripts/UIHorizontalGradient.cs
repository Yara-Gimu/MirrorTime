using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[AddComponentMenu("UI/Effects/Horizontal Gradient")]
public class UIHorizontalGradient : BaseMeshEffect
{
    [Header("--- إعدادات التدرج اللوني ---")]
    public Color rightColor = new Color(0f, 0f, 0f, 0.7f); // أسود شفاف جهة اليمين
    public Color leftColor = new Color(0f, 0f, 0f, 0f);    // شفاف تماماً جهة اليسار

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive()) return;

        int count = vh.currentVertCount;
        if (count == 0) return;

        UIVertex vertex = new UIVertex();
        float minX = float.MaxValue;
        float maxX = float.MinValue;

        // العثور على أطراف اللوحة لمعرفة العرض بالملي
        for (int i = 0; i < count; i++)
        {
            vh.PopulateUIVertex(ref vertex, i);
            if (vertex.position.x < minX) minX = vertex.position.x;
            if (vertex.position.x > maxX) maxX = vertex.position.x;
        }

        float width = maxX - minX;

        // تلوين النقاط بذكاء لإنتاج التدرج دون استخدام صور
        for (int i = 0; i < count; i++)
        {
            vh.PopulateUIVertex(ref vertex, i);
            float t = (vertex.position.x - minX) / width;
            vertex.color = Color.Lerp(leftColor, rightColor, t);
            vh.SetUIVertex(vertex, i);
        }
    }
}