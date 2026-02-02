using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screenshot : MonoBehaviour
{
    [SerializeField]
    private Camera screenshotCamera;

    public Sprite ScreenshotToSprite()
    {
        int width = Screen.width;
        int height = Screen.height;

        //width, height 크기의 RenderTexture생성
        RenderTexture renderTexture = new RenderTexture(width, height, 24);
        //Camera컴포넌트의 targetTexture 변수에 renderTexture등록(카메라에 촬영된 화면을 renderTexture에 지정)
        screenshotCamera.targetTexture = renderTexture;

        //Render메소드를 호출해 현재 카메라의 화면을 촬영하고(renderTexture에 저장됨),
        //renderTexture를 RenderTexture.active로 설정
        screenshotCamera.Render();
        RenderTexture.active = renderTexture;

        //현재 활성화 되어있는 renderTexture의 Pixels정보를 얻어와 Texture2D타입의 screenshot에 저장
        Texture2D screenshot = new Texture2D(width, width, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, (height - width) * 0.5f, width, width), 0, 0);
        screenshot.Apply();

        screenshotCamera.targetTexture = null;

        Rect rect = new Rect(0, 0, screenshot.width, screenshot.height);
        Sprite sprite = Sprite.Create(screenshot, rect, Vector2.one * 0.5f);

        return sprite;
    }
}
