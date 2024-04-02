using UnityEngine;
using LuaInterface;



public class PrefabInstance : MonoBehaviour
{
    private LuaTable m_luaTable = null;
    private LuaFunction m_updateFunc = null;
    private float m_time = 0;



    private void Awake()
    {
        if (LuaManager.Instance.m_luaClassList.ContainsKey(gameObject.name))
        {
            m_luaTable = LuaManager.Instance.m_luaClassList[gameObject.name];

            LuaFunction awake = (LuaFunction)m_luaTable["Awake"];

            if (awake != null)
            {
                awake.Call(this);
            }

            m_updateFunc = (LuaFunction)m_luaTable["Update"];

            m_luaTable["gameObject"] = gameObject;
        }
    }

    private void OnEnable()
    {
        LuaFunction enable = (LuaFunction)m_luaTable["OnEnable"];

        if (enable != null)
        {
            enable.Call();
        }
    }

    private void Start()
    {
        LuaFunction start = (LuaFunction)m_luaTable["Start"];

        if (start != null)
        {
            start.Call();
        }
    }

    private void Update()
    {
        m_time += Time.deltaTime;

        if (m_updateFunc != null)
        {
            m_updateFunc.Call(m_time);
        }
    }

    private void OnDisable()
    {
        LuaFunction disable = (LuaFunction)m_luaTable["OnDisable"];

        if (disable != null)
        {
            disable.Call();
        }
    }

    private void OnDestroy()
    {
        LuaFunction destroy = (LuaFunction)m_luaTable["OnDestroy"];

        if (destroy != null)
        {
            destroy.Call();
        }

        if (LuaManager.Instance.m_luaClassList.ContainsKey(gameObject.name))
        {
            LuaManager.Instance.m_luaClassList.Remove(gameObject.name);
        }
    }



    public LuaTable GetLuaTable()
    {
        return m_luaTable;
    }

    public float GetTime()
    {
        return m_time;
    }
}