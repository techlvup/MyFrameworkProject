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

    public static LuaTable OpenPrefabPanel(string prefabPath, string luaPath)
    {
        LuaTable luaClass;

        int startIndex = prefabPath.LastIndexOf("/") + 1;
        string prefabName = prefabPath.Substring(startIndex, prefabPath.Length - startIndex);

        if (LuaManager.m_luaClassList.ContainsKey(prefabName))
        {
            luaClass = LuaManager.m_luaClassList[prefabName];
        }
        else
        {
            luaClass = LuaManager.m_luaState.DoFile<LuaTable>(luaPath);

            LuaManager.m_luaClassList[prefabName] = luaClass;

            GameObject obj = CreateGameObject(prefabPath, prefabName);

            obj.AddComponent<PrefabInstance>();
        }

        return luaClass;
    }

    public static void ClosePrefabPanel(string prefabName)
    {
        if (LuaManager.m_luaClassList.ContainsKey(prefabName))
        {
            GameObject.Destroy((GameObject)LuaManager.m_luaClassList[prefabName]["gameObject"]);
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