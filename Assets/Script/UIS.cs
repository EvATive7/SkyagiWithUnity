using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization;

using TMPro;
using UnityEngine.Networking;
using System.Threading.Tasks;

public class UIS : MonoBehaviour
{
    List<GameObject> Kong = new List<GameObject>();
    List<GameObject> Ying = new List<GameObject>();
    List<GameObject> Scene = new List<GameObject>();

    TMP_Text KongTextUI;
    TMP_InputField YingTextUI;

    bool _i = true;
    private bool Idle {
        get{
            return _i;
        }
        set {
            _i = value;
            Debug.Log($"Idle changed to {Idle}");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        void add(List<GameObject> ls, string name)
        {
            var a = GameObject.Find(name);
            ls.Add(a);
        }

        add(Kong, "t1");
        add(Kong, "t1a");
        add(Ying, "t2");
        add(Ying, "t2a");
        add(Scene, "t3");
        add(Scene, "t3a");

        YingTextUI = GameObject.Find("t2a").GetComponent<TMP_InputField>();
        KongTextUI = GameObject.Find("t1a").GetComponent<TMP_Text>();

        scene = 0;

        Environment.SetEnvironmentVariable("HTTP_PROXY", "");
        Environment.SetEnvironmentVariable("HTTPS_PROXY", "");

    }

    void SetIfHide(List<GameObject> ls, bool hide)
    {
        ls.ForEach(t => t.SetActive(!hide));
    }

    int _sc = 0;
    int scene
    {
        get
        {
            return _sc;
        }
        set
        {
            if (value == 3)
            {
                value -= 2;
            }
            _sc = value;
            SetIfHide(Kong, true);
            SetIfHide(Ying, true);
            SetIfHide(Scene, true);
            switch (value)
            {
                case 0:
                    SetIfHide(Scene, false);
                    break;
                case 1:
                    SetIfHide(Ying, false);
                    break;
                case 2:
                    SetIfHide(Kong, false);
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Next()
    {
        if (!Idle) return;
        scene++;
        try
        {
            if (scene == 2)
            {
                var text = YingTextUI.text;
                Debug.Log(YingTextUI.text);
                var end = "";

                KongTextUI.text = "[Generating...]";
                Idle = false;


                string url = "http://127.0.0.1:8000/msg";
                // 创建一个 UnityWebRequest 对象，指定请求的 URL 和方法为 POST
                UnityWebRequest request = UnityWebRequest.Get(url);

                // 将 bodyData 转换为字节数组，并设置为请求的上传处理器
                byte[] bodyBytes = System.Text.Encoding.UTF8.GetBytes(text);
                request.uploadHandler = new UploadHandlerRaw(bodyBytes);

                // 设置请求的 Content-Type 为 application/json（或其他合适的类型）
                request.SetRequestHeader("Content-Type", "application/text");

                // 发起请求
                var a = request.SendWebRequest();

                a.completed += (s) =>
                {
                    KongTextUI.text = "completed";
                    // 处理响应
                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        Debug.Log("Request was successful!");
                        Debug.Log("Response: " + request.downloadHandler.text);
                        end = request.downloadHandler.text.Replace("Kong said ", "").Replace('"', ' ');
                    }
                    else
                    {
                        Debug.LogWarning("Request failed with error: " + request.error);
                        Debug.LogWarning(request);
                    }

                    KongTextUI.text = end;
                    YingTextUI.text = "";
                    Idle = true;
                };


            }
            if (scene == 1)
            {

            }
            
        }
        catch (Exception e)
        {
            Debug.Log(e);
            KongTextUI.text = e.ToString();
        }
    }
}
