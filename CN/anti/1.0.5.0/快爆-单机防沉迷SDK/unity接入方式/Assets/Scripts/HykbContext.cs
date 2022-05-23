using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HykbFcm
{
    public sealed class HykbContext : MonoBehaviour
    {

        private AndroidJavaObject currentActivity;

        private static readonly HykbContext _HykbContext = new HykbContext();

        /*
         * 获取当前实例       
         */
        public static HykbContext GetInstance()
        {
            return _HykbContext;
        }


        /*
         * 获取当前Activity       
         */
        public AndroidJavaObject GetActivity()
        {
            if (null == currentActivity)
            {
                AndroidJavaClass unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                currentActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
            }

            return currentActivity;
        }

        /*
         * 运行在主UI线程       
         */
        public void RunOnUIThread(AndroidJavaRunnable runnable)
        {
            GetActivity().Call("runOnUiThread", runnable);
        }
    }
}


