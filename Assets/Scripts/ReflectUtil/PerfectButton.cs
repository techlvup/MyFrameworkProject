﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using LuaInterface;

public class PerfectButton :Button, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private int m_clickTimes = 0;

    private bool isCancelClick = false;

    private float m_startPressTime = 0;
    private float m_endPressTime = 0;

    private float m_startClickTime = 0;
    private float m_endClickTime = 0;

    private LuaFunction m_clickFunc = null;
    private LuaFunction m_doubleClickFunc = null;
    private LuaFunction m_downFunc = null;
    private LuaFunction m_upFunc = null;
    private LuaFunction m_longPressFun = null;

    private PointerEventData m_eventData = null;

    private ScrollRect m_scroll = null;

    private void Update()
    {
        CallLongPressListener();

        CallDoubleClickListener();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        if (m_longPressFun != null)
        {
            m_startPressTime = Time.time;
        }

        if (m_downFunc != null)
        {
            m_downFunc.Call();
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        if (m_longPressFun != null)
        {
            m_startPressTime = 0;
            m_endPressTime = 0;
        }

        if (m_upFunc != null)
        {
            m_upFunc.Call();
        }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (isCancelClick)
        {
            isCancelClick = false;
            m_eventData = null;
        }
        else
        {
            if (m_doubleClickFunc != null)
            {
                m_clickTimes++;
                m_eventData = eventData;
            }
            else if (m_clickFunc != null)
            {
                m_clickFunc.Call();
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(m_scroll != null)
        {
            m_scroll.OnBeginDrag(eventData);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        isCancelClick = true;

        if (m_scroll != null)
        {
            m_scroll.OnDrag(eventData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (m_scroll != null)
        {
            m_scroll.OnEndDrag(eventData);
        }
    }

    public void AddClickListener(LuaFunction luaFunction)
    {
        m_clickFunc = luaFunction;
    }

    public void ReleaseClickListener()
    {
        m_clickFunc = null;
    }

    public void AddDoubleClickListener(LuaFunction luaFunction)
    {
        m_doubleClickFunc = luaFunction;
    }

    public void ReleaseDoubleClickListener()
    {
        m_doubleClickFunc = null;
    }

    public void AddDownListener(LuaFunction luaFunction)
    {
        m_downFunc = luaFunction;
    }

    public void ReleaseDownListener()
    {
        m_downFunc = null;
    }

    public void AddUpListener(LuaFunction luaFunction)
    {
        m_upFunc = luaFunction;
    }

    public void ReleaseUpListener()
    {
        m_upFunc = null;
    }

    public void AddLongPressListener(LuaFunction luaFunction)
    {
        m_longPressFun = luaFunction;
    }

    public void ReleaseLongPressListener()
    {
        m_longPressFun = null;
    }

    public void AddDragScroll(ScrollRect scr)
    {
        m_scroll = scr;
    }

    public void ReleaseDragScroll()
    {
        m_scroll = null;
    }

    private void CallDoubleClickListener()
    {
        if(m_eventData != null)
        {
            if(m_startClickTime == 0)
            {
                m_startClickTime = Time.time;
            }

            m_endClickTime = Time.time;

            if (m_endClickTime - m_startClickTime >= 0.15)
            {
                if (m_clickTimes == 1)
                {
                    m_clickTimes = 0;

                    if (m_clickFunc != null)
                    {
                        m_clickFunc.Call();
                    }
                }
                else if (m_clickTimes >= 2)
                {
                    m_clickTimes = 0;

                    m_doubleClickFunc.Call();
                }

                m_eventData = null;
                m_startClickTime = 0;
                m_endClickTime = 0;
            }
        }
    }

    private void CallLongPressListener()
    {
        if (m_startPressTime != 0)
        {
            if (isCancelClick)
            {
                m_startPressTime = 0;
                m_endPressTime = 0;

                isCancelClick = false;
            }
            else
            {
                m_endPressTime = Time.time;

                if (m_endPressTime - m_startPressTime >= 0.2)
                {
                    m_startPressTime = 0;
                    m_endPressTime = 0;

                    isCancelClick = true;

                    m_longPressFun.Call();
                }
            }
        }
    }
}