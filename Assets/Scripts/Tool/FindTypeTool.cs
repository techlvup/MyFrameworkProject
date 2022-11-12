using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class FindTypeTool
{
    private static Dictionary<string, string> m_nameDictionary = null;
    public static Type GetComponentType(string componentName)
    {
        if (m_nameDictionary == null)
        {
            InitNameDictionary();
        }

        if (m_nameDictionary.ContainsKey(componentName))
        {
            string className = m_nameDictionary[componentName];
            return Type.GetType(className);
        }

        return null;
    }

    private static void InitNameDictionary()
    {
        m_nameDictionary = new Dictionary<string, string>();

        m_nameDictionary.Add("RawImage", typeof(RawImage).AssemblyQualifiedName);
        m_nameDictionary.Add("Image", typeof(Image).AssemblyQualifiedName);
        m_nameDictionary.Add("Text", typeof(Text).AssemblyQualifiedName);
        m_nameDictionary.Add("Button", typeof(Button).AssemblyQualifiedName);
        m_nameDictionary.Add("ScrollRect", typeof(ScrollRect).AssemblyQualifiedName);
        m_nameDictionary.Add("Dropdown", typeof(Dropdown).AssemblyQualifiedName);
        m_nameDictionary.Add("Toggle", typeof(Toggle).AssemblyQualifiedName);
        m_nameDictionary.Add("Slider", typeof(Slider).AssemblyQualifiedName);
        m_nameDictionary.Add("Scrollbar", typeof(Scrollbar).AssemblyQualifiedName);
        m_nameDictionary.Add("InputField", typeof(InputField).AssemblyQualifiedName);
        m_nameDictionary.Add("GridLayoutGroup", typeof(GridLayoutGroup).AssemblyQualifiedName);
        m_nameDictionary.Add("HorizontalLayoutGroup", typeof(HorizontalLayoutGroup).AssemblyQualifiedName);
        m_nameDictionary.Add("VerticalLayoutGroup", typeof(VerticalLayoutGroup).AssemblyQualifiedName);
        m_nameDictionary.Add("Mask", typeof(Mask).AssemblyQualifiedName);
        m_nameDictionary.Add("RectMask2D", typeof(RectMask2D).AssemblyQualifiedName);
    }
}