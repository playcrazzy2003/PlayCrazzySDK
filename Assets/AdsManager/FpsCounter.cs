using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsCounter : MonoBehaviour
{
    private float count;

    private IEnumerator Start() {
        GUI.depth = 2;
        while (true) {
            count = 1f / Time.unscaledDeltaTime;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnGUI() {
        Rect location = new Rect(5,500
            , 230, 90);
        string text = $"FPS: {Mathf.Round(count)}";
        Texture black = Texture2D.linearGrayTexture;
        GUI.DrawTexture(location, black, ScaleMode.StretchToFill);
        GUI.color = Color.black;
        GUI.skin.label.fontSize = 40;
        GUI.Label(location, text);
    }

}
