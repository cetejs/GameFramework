using UnityEngine;
using UnityEngine.UI;

namespace GameFramework
{
    internal class FpsDrawer : MonoBehaviour
    {
        [SerializeField]
        private Text text;
        [SerializeField]
        private float refreshTime = 0.5f;

        private float fpsDeltaTime;
        private float refreshEndTime;
        private string fpsText;

        private void Update()
        {
            fpsDeltaTime += (Time.unscaledDeltaTime - fpsDeltaTime) * 0.1f;
            float ms = fpsDeltaTime * 1000f;
            float fps = 1f / fpsDeltaTime;
            if (Time.unscaledTime - refreshEndTime > refreshTime)
            {
                Color color = Color.red;
                if (fps >= 50f)
                {
                    color = Color.blue;
                }
                else if (fps > 30f)
                {
                    color = Color.yellow;
                }

                fpsText = $"{ms:0.0} ms ({fps:0.0} fps)";
                refreshEndTime = Time.unscaledTime;
                text.text = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}><b>{fpsText}</b></color>";
            }
        }
    }
}