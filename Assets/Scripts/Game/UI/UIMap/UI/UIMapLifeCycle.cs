using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMapLifeCycle : MonoBehaviour
{
    public float m_CurTime = 0.0f;
    public float m_Frames = 0.0f;
    public Action UpdateFrameRate00 = null;
    public Action UpdateFrameRate01 = null;
    public Action UpdateFrameRate03 = null;
    public Action UpdateFrameRate05 = null;
    public Action UpdateFrameRate10 = null;
    public Action UpdateFrameRate30 = null;
    public Action UpdateFrameRate50 = null;
    private void OnEnable()
    {
        m_CurTime = 0.0f;
        //StartCoroutine(IEUpdate00());
        StartCoroutine(IEUpdate01());
        StartCoroutine(IEUpdate03());
        StartCoroutine(IEUpdate05());
        StartCoroutine(IEUpdate10());
        StartCoroutine(IEUpdate30());
        StartCoroutine(IEUpdate50());
    }
    private void Update()
    {
        m_CurTime += Time.deltaTime;
        m_Frames = 1f / Time.deltaTime;
        UpdateFrameRate00?.Invoke();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }


    //private IEnumerator IEUpdate00()
    //{
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(0.0f);
    //        UpdateFrameRate00?.Invoke();
    //    }
    //}
    private IEnumerator IEUpdate01()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            UpdateFrameRate01?.Invoke();
        }
    }
    private IEnumerator IEUpdate03()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.3f);
            UpdateFrameRate03?.Invoke();
        }
    }
    private IEnumerator IEUpdate05()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            UpdateFrameRate05?.Invoke();
        }
    }

    private IEnumerator IEUpdate10()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f);
            UpdateFrameRate10?.Invoke();
        }
    }
    private IEnumerator IEUpdate30()
    {
        while (true)
        {
            yield return new WaitForSeconds(3.0f);
            UpdateFrameRate30?.Invoke();
        }
    }
    private IEnumerator IEUpdate50()
    {
        while (true)
        {
            yield return new WaitForSeconds(5.0f);
            UpdateFrameRate50?.Invoke();
        }
    }

}
