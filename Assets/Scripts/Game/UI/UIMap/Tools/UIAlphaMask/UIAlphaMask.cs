using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAlphaMask : MonoBehaviour
{
    public List<Image> m_chilsImg = new List<Image>();
    [HideInInspector] public bool m_IsCacheImg = false;
    [HideInInspector] public string m_DefaultCachePath = "";
    [HideInInspector] public string m_CachePath = "";
    [HideInInspector] public string m_CacheImageName = "UIAlphaMask_CreatePNG";

    [HideInInspector] public string m_NewObjectName = "Create Object";
    public List<GameObject> m_CreateGameObject = new List<GameObject>();
    [HideInInspector] public bool m_NewObjIsDesOldObj = true;

    [HideInInspector] public string m_NamePrefix = "_____";
    private void OnEnable()
    {
        if (!m_IsCacheImg)
        {
            Create();
        }
    }
    public bool GetIsCreateObject(GameObject obj)
    {
        bool isCreateObj = false;
        var itemName = obj.name;
        var prefix = itemName.Substring(0, Mathf.Min(m_NamePrefix.Length, itemName.Length));
        if (string.Equals(prefix, m_NamePrefix))
        {
            isCreateObj = true;
        }
        return isCreateObj;
    }
    void GetChildComponent<T>(List<T> list, Transform target)
    {
        for (int i = 0; i < target.childCount; i++)
        {
            var child = target.GetChild(i);
            var isCreateObj = GetIsCreateObject(child.gameObject);
            if (!isCreateObj && child.TryGetComponent<T>(out var img))
            {
                list.Add(img);
            }
            if (child.childCount > 0)
            {
                GetChildComponent(list, child);
            }
        }
    }
    public Texture2D Create()
    {

        var thisRect = GetComponent<RectTransform>();
        Image thisImg = GetComponent<Image>();

        var chilsImg = new List<Image>();
        GetChildComponent(chilsImg, thisRect);



        Rect newRect = new Rect(new Vector2(0, 0), thisRect.sizeDelta);
        Texture2D newTex = new Texture2D((int)thisRect.sizeDelta.x, (int)thisRect.sizeDelta.y);

        Vector2 GetLocalPixed(RectTransform img, RectTransform parent, Vector2 offset)
        {
            var scaleX = img.localScale.x;
            var scaleY = img.localScale.y;
            var imgWidth = img.sizeDelta.x * scaleX;
            var imgHeight = img.sizeDelta.y * scaleY;
            var parWidth = parent.sizeDelta.x;
            var parHeight = parent.sizeDelta.y;

            var imgOffset = new Vector2
            {
                x = ((imgWidth - parWidth) * img.pivot.x + offset.x - img.anchoredPosition3D.x) / scaleX,
                y = ((imgHeight - parHeight) * img.pivot.y + offset.y - img.anchoredPosition3D.y) / scaleY
            };

            return imgOffset;
        }

        var maxPixX = newTex.width;
        var maxPixY = newTex.height;
        for (int i = 0; i < maxPixX; i++)
        {
            for (int j = 0; j < maxPixY; j++)
            {
                var pixelColor = new Color(0, 0, 0, 0);
                for (int k = chilsImg.Count - 1; k >= 0; k--)
                {
                    var item = chilsImg[k];
                    var itemMaxPixelX = item.sprite.texture.width;
                    var itemMaxPixelY = item.sprite.texture.height;
                    var pixelPoint = GetLocalPixed(item.GetComponent<RectTransform>(), thisRect, new Vector2(i, j));
                    var itemPixelX = (int)(pixelPoint.x / maxPixX * itemMaxPixelX);
                    var itemPixelY = (int)(pixelPoint.y / maxPixY * itemMaxPixelY);
                    if (itemPixelX < 0 || itemPixelY < 0 || itemPixelX > itemMaxPixelX * item.transform.localScale.x || itemPixelY > itemMaxPixelY * item.transform.localScale.y)
                    {
                        continue;
                    }
                    var itemColor = item.sprite.texture.GetPixel(itemPixelX, itemPixelY);

                    var blendValue = (1 - pixelColor.a);
                    pixelColor.a += itemColor.a * blendValue;
                    pixelColor.r += itemColor.r * (1 - pixelColor.r) * blendValue;
                    pixelColor.g += itemColor.g * (1 - pixelColor.g) * blendValue;
                    pixelColor.b += itemColor.b * (1 - pixelColor.b) * blendValue;
                }


                var thisPosX = thisImg.sprite.texture.width;
                var thisPosY = thisImg.sprite.texture.height;
                var pixelX = (int)((float)i / maxPixX * thisPosX);
                var pixelY = (int)((float)j / maxPixY * thisPosY);
                var thisPixelColor = thisImg.sprite.texture.GetPixel(pixelX, pixelY);
                var maskAlpha = thisPixelColor.a * thisImg.color.a;
                pixelColor.a *= maskAlpha;

                newTex.SetPixel(i, j, pixelColor);
            }
        }
        newTex.Apply();



        Sprite newSprite = Sprite.Create(newTex, newRect, new Vector2(0, 0));

        GameObject newObject;
        if (m_CreateGameObject.Count > 0 && m_CreateGameObject[m_CreateGameObject.Count - 1] != null && m_NewObjIsDesOldObj)
        {
            newObject = m_CreateGameObject[m_CreateGameObject.Count - 1];
        }
        else
        {
            newObject = new GameObject($"{m_NamePrefix}{m_NewObjectName}");
            var newObjRect = newObject.AddComponent<RectTransform>();
            newObject.transform.SetParent(transform);
            newObjRect.anchoredPosition3D = Vector3.zero;
            newObjRect.localScale = Vector3.one;
            var newObjImg = newObject.AddComponent<Image>();
            newObjImg.raycastTarget = false;
            m_CreateGameObject.Add(newObject);
        }
        var ObjImg = newObject.GetComponent<Image>();
        ObjImg.sprite = newSprite;
        ObjImg.SetNativeSize();

        m_chilsImg = chilsImg;

        return newTex;
    }
    public void SetCreateObjSprite(Sprite sprite)
    {
        if(m_CreateGameObject.Count > 0)
        {
            var img = m_CreateGameObject[m_CreateGameObject.Count - 1]?.GetComponent<Image>();
            img.sprite = sprite;
        }
    }
    public void SetActiveAll(bool active)
    {
        foreach (var item in m_chilsImg)
        {
            item.gameObject.SetActive(active);
        }
    }
    public void ResetCcachePath()
    {
        m_CachePath = m_DefaultCachePath;
    }
    public void ClearAllCreateObject()
    {
        while (m_CreateGameObject.Count > 0)
        {
            GameObject.DestroyImmediate(m_CreateGameObject[0]);
            m_CreateGameObject.RemoveAt(0);
        }
        m_CreateGameObject = new List<GameObject>();
    }
}
