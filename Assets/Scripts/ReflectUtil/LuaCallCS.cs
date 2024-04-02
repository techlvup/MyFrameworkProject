using System;
using UnityEngine;
using UnityEngine.UI;
using LuaInterface;



public static partial class LuaCallCS
{
    public static GameObject GetGameObject(UnityEngine.Object obj, string childPath = "")
    {
        GameObject gameObject = null;

        if (obj is GameObject)
        {
            gameObject = obj as GameObject;
        }
        else if (obj is Component)
        {
            var component = obj as Component;
            gameObject = component.gameObject;
        }

        if (!string.IsNullOrEmpty(childPath))
        {
            Transform trans = gameObject.transform.Find(childPath);

            if (trans != null)
            {
                return trans.gameObject;
            }

            return null;
        }

        return gameObject;
    }

    public static Transform GetTransform(UnityEngine.Object obj, string childPath = "")
    {
        var gameObject = GetGameObject(obj, childPath);

        if (gameObject != null)
        {
            return gameObject.transform;
        }

        return null;
    }

    public static LuaTable OpenPrefabPanel(string prefabPath, string luaPath)
    {
        LuaTable luaClass;

        int startIndex = prefabPath.LastIndexOf("/") + 1;
        string prefabName = prefabPath.Substring(startIndex, prefabPath.Length - startIndex);

        if (LuaManager.Instance.m_luaClassList.ContainsKey(prefabName))
        {
            luaClass = LuaManager.Instance.m_luaClassList[prefabName];
        }
        else
        {
            luaClass = LuaManager.Instance.m_luaState.DoFile<LuaTable>(luaPath);

            LuaManager.Instance.m_luaClassList[prefabName] = luaClass;

            GameObject obj = CreateGameObject(prefabPath, prefabName);

            obj.AddComponent<PrefabInstance>();
        }

        return luaClass;
    }

    public static void ClosePrefabPanel(string prefabName)
    {
        if (LuaManager.Instance.m_luaClassList.ContainsKey(prefabName))
        {
            GameObject.Destroy((GameObject)LuaManager.Instance.m_luaClassList[prefabName]["gameObject"]);
            LuaManager.Instance.m_luaClassList.Remove(prefabName);
        }
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

            if (trans == null)
            {
                return null;
            }

            gameObject = trans.gameObject;
        }

        Component component = gameObject.GetComponent(componentName);

        if (component == null)
        {
            Type type = Type.GetType(componentName);

            if (type == null)
            {
                type = FindTypeTool.GetComponentType(componentName);
            }

            if (type != null)
            {
                component = gameObject.AddComponent(type);
            }
        }

        return component;
    }

    public static GameObject Clone(UnityEngine.Object obj, string name = "cloneName", UnityEngine.Object parent = null)
    {
        GameObject item = GetGameObject(obj);
        GameObject clone;

        if (parent != null)
        {
            Transform parentTrans = GetTransform(parent);
            clone = GameObject.Instantiate<GameObject>(item, Vector3.zero, Quaternion.identity, parentTrans);
        }
        else
        {
            clone = GameObject.Instantiate<GameObject>(item, Vector3.zero, Quaternion.identity);
        }

        clone.name = name;

        return clone;
    }

    public static void SetActive(UnityEngine.Object obj, bool isActive = false)
    {
        GameObject item = GetGameObject(obj);
        item.SetActive(isActive);
    }

    public static void SetActive(UnityEngine.Object obj, string childPath, bool isActive = false)
    {
        GameObject item = GetGameObject(obj);

        if (!string.IsNullOrEmpty(childPath))
        {
            item = item.transform.Find(childPath).gameObject;
        }

        item.SetActive(isActive);
    }
}