using GameFramework.Generic;
using GameFramework.UIService;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework
{
    public class TestWindow : UIWindow
    {
        [SerializeField]
        private Text text;
    
        public override void OnInitData()
        {
            GameLogger.Log($"{GetType().Name}.OnInitData Layer = {Layer} Depth = {Depth} GetData() = {GetData()}");
        }

        public override void OnCreatWindow()
        {
            text.text = $"WindowName = {name},Layer = {Layer} Depth = {Depth} GetData() = {GetData()}";
            GameLogger.Log($"{GetType().Name}.OnCreatWindow Layer = {Layer} Depth = {Depth} GetData() = {GetData()}");
        }

        public override void OnCloseWindow()
        {
            GameLogger.Log($"{GetType().Name}.OnCloseWindow Layer = {Layer} Depth = {Depth} GetData() = {GetData()}");
        }

        public override void OnShowWindow()
        {
            GameLogger.Log($"{GetType().Name}.OnShowWindow Layer = {Layer} Depth = {Depth} GetData() = {GetData()}");
        }

        public override void OnHideWindow()
        {
            GameLogger.Log($"{GetType().Name}.OnHideWindow Layer = {Layer} Depth = {Depth} GetData() = {GetData()}");
        }

        public override void OnUpdateWindow()
        {
            if (Time.frameCount % Application.targetFrameRate == 0)
            {
                GameLogger.Log($"{GetType().Name}.OnUpdateWindow Layer = {Layer} Depth = {Depth} GetData() = {GetData()}");
            }
        }
    }
}