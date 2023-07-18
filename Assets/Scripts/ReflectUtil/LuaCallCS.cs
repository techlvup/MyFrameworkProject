using System;
using UnityEngine;
using UnityEngine.UI;
using LuaInterface;

public static partial class LuaCallCS
{
    public static GameObject GetGameObject(UnityEngine.Object obj)
    {
        if (obj is GameObject)
        {
            var gameObject = obj as GameObject;
            return gameObject;
        }

        if (obj is Component)
        {
            var component = obj as Component;
            return component.gameObject;
        }

        return null;
    }

    public static Transform GetTransform(UnityEngine.Object obj)
    {
        if (obj is GameObject)
        {
            var gameObject = obj as GameObject;
            return gameObject.transform;
        }

        if (obj is Component)
        {
            var component = obj as Component;
            return component.transform;
        }

        return null;
    }

    public static GameObject CreateGameObject(string prefabPath, string name, UnityEngine.Object parent = null)
    {
        GameObject gameObject = null;
        Transform parentTrans = null;

        if (parent == null)
        {
            parentTrans = GameObject.Find("UI_Root").transform;
        }
        else
        {
            parentTrans = GetTransform(parent);
        }

        if (!string.IsNullOrEmpty(prefabPath))
        {
            gameObject = Resources.Load<GameObject>(prefabPath);

            if (gameObject != null)
            {
                string prefabName = gameObject.name;

                gameObject = GameObject.Instantiate(gameObject, parentTrans);

                gameObject.name = prefabName;

                return gameObject;
            }
        }
        else
        {
            gameObject = new GameObject(name);

            gameObject.transform.SetParent(parentTrans);

            gameObject.transform.localPosition = new Vector3(0, 0, 0);

            return gameObject;
        }

        return null;
    }

    public static Component GetComponent(UnityEngine.Object obj, string childPath, string componentName)
    {
        GameObject gameObject = GetGameObject(obj);
        Transform trans = null;

        if (gameObject != null)
        {
            trans = gameObject.transform;
        }
        else
        {
            return null;
        }

        if (!string.IsNullOrEmpty(childPath))
        {
            trans = trans.Find(childPath);
        }

        if (trans != null)
        {
            return trans.GetComponent(componentName);
        }

        return null;
    }

    public static Component AddComponent(UnityEngine.Object obj, string childPath, string componentName)
    {
        GameObject gameObject = GetGameObject(obj);

        if (gameObject == null)
        {
            return null;
        }

        if (!string.IsNullOrEmpty(childPath))
        {
            Transform trans = gameObject.transform.Find(childPath);

            if(trans == null)
            {
                return null;
            }

            gameObject = trans.gameObject;
        }

        Component component = gameObject.GetComponent(componentName);

        if(component == null)
        {
            Type type = Type.GetType(componentName);

            if(type == null)
            {
                type = FindTypeTool.GetComponentType(componentName);
            }

            if(type != null)
            {
                component = gameObject.AddComponent(type);
            }
        }

        return component;
    }

    public static GameObject Clone(UnityEngine.Object obj, string name = "cloneName", UnityEngine.Object parent = null)
    {
        GameObject objTrans = GetGameObject(obj);
        GameObject go = null;

        if (parent != null)
        {
            Transform parentTrans = GetTransform(parent);
            go = GameObject.Instantiate<GameObject>(objTrans, Vector3.zero, Quaternion.identity, parentTrans);
        }
        else
        {
           go = GameObject.Instantiate<GameObject>(objTrans, Vector3.zero, Quaternion.identity);
        }

        return go;
    }
}