using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HykbFcm;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    private HykbContext hykbContext;
    private AndroidJavaClass fcmSDKClass;
    void Start()
    {
        hykbContext = HykbContext.GetInstance();
        InitSDK();
    }

    public void InitSDK()
    {

        //获取HykbFcmSDKConfig的class
        AndroidJavaObject buidlerObject = new AndroidJavaObject("com.m3839.union.fcm.UnionFcmParam$Builder");
        AndroidJavaObject paramObject = buidlerObject
                    .Call<AndroidJavaObject>("setGameId", "17753")  //填写游戏ID
                    .Call<AndroidJavaObject>("setDefaultPassword", "8888")  //设置默认密码，必须是4位数，不填写默认是3839
                    .Call<AndroidJavaObject>("setContact", "110@qq.test")  //设置找回密码的联系方式，必须填写
                    .Call<AndroidJavaObject>("setOrientation", 1) //0是横屏，1是竖屏
                    .Call<AndroidJavaObject>("build");

        //获取HykbFcmSDK的类
        fcmSDKClass = new AndroidJavaClass("com.m3839.union.fcm.UnionFcmSDK");

        //调用初始化方法      
        fcmSDKClass.CallStatic("initSDK", hykbContext.GetActivity(), paramObject, new UnionFcmListenerProxy());
        //日志开关      
        fcmSDKClass.CallStatic("setDebug", true);
    }

    /*
     *回调   
     */
    internal class UnionFcmListenerProxy : AndroidJavaProxy
    {

        public UnionFcmListenerProxy() : base("com.m3839.union.fcm.UnionFcmListener")
        {

        }

        /**
         * 反沉迷的回调
         */
        void onFcm(int code, string msg)
        {
            Debug.Log("code = " + code + ",msg = " + msg);
            if(code == 100)
            {
                //表示登录成功，获取登录信息
                AndroidJavaClass fcmSDKClass = new AndroidJavaClass("com.m3839.union.fcm.UnionFcmSDK");
                UnionFcmUser user = new UnionFcmUser(fcmSDKClass.CallStatic<AndroidJavaObject>("getUser")); 
                if (user != null)
                {
                    //ToastUtils.showToast(user.toString());
                }
            }
            else if (code == 2005)
            {
                //这边只是个回调退出游戏，游戏可自己实现退出游戏的方式
                ///HykbContext.GetInstance().GetActivity().Call("finish");
                //获取LaunchActivity的类
                AndroidJavaClass launchActivityClass = new AndroidJavaClass("com.m3839.fcm.sdk.LaunchActivity");
                //获取LaunchActivity的对象
                AndroidJavaObject launchActivityObject = launchActivityClass.GetStatic<AndroidJavaObject>("launchActivity");
                HykbContext.GetInstance().GetActivity().Call("finish");
                launchActivityClass.CallStatic("exit");
                if (launchActivityObject == null)
                {
                    Debug.Log("launchobject 为空");
                    //ToastUtils.showToast("获取不到对象");
                }
                else
                {
                    //ToastUtils.showToast("find the 到对象");
                    Debug.Log("launchobject 不为空");
                    //launchActivityObject.Call("exit");
                    Application.Quit();
                }

            }
            else if (code == 1001)
            {
                //ToastUtils.showToast("青少年模式已关闭");
            }
            else if (code == 1002)
            {
                //ToastUtils.showToast("青少年模式已开启");
            }
            else if (code == 1003)
            {
                //ToastUtils.showToast("游戏未设置找回密码联系方式");
            }
        }

        
    }

    bool isHomeOrMenu;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HykbContext.GetInstance().GetActivity().Call("finish");
            //获取LaunchActivity的类
            AndroidJavaClass launchActivityClass = new AndroidJavaClass("com.m3839.fcm.sdk.LaunchActivity");
            //获取LaunchActivity的对象
            launchActivityClass.CallStatic("exit");
            Application.Quit();
        }

    }


    void OnDestroy()
    {
        //销毁    
        fcmSDKClass.CallStatic("releaseSDK");
        Debug.Log("destroy : 游戏对象被销毁了");
    }

}
