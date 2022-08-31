using System;
using TMPro;
//using Unity.GameCore;
using UnityEngine;
using UnityEngine.UI;

public class UserBar : MonoBehaviour
{
    [SerializeField]
    private Canvas _canvas;

    [SerializeField]
    private RawImage _icon;

    [SerializeField]
    private TextMeshProUGUI _label;

    // private XboxUser _user;
    //
    // public void Initialize()
    // {
    //     _canvas.enabled = false;
    // }
    //
    // public void DeInitialize()
    // {
    //     if(_user != null)
    //         _user.OnUserUpdated -= UpdateBar;
    //     
    //     //_canvas.enabled = false;
    //     _user = null;
    // }
    //
    // public void Construct(XboxUser user)
    // {
    //     _user = user;
    //
    //     user.OnUserUpdated += UpdateBar;
    // }
    //
    // private void UpdateBar(XboxUser.UserData data)
    // {
    //     _canvas.enabled = true;
    //     
    //     UpdateLabel(data.userGamertag);
    //     UpdateAvatar(data.imageBuffer);
    // }
    //
    // private void UpdateLabel(string text) => 
    //     _label.text = text;
    //
    // private void UpdateAvatar(byte[] imageBuffer)
    // {
    //     if (imageBuffer == null)
    //         return;
    //
    //     Texture2D myTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
    //     myTexture.filterMode = FilterMode.Point;
    //     myTexture.LoadImage(imageBuffer);
    //     _icon.texture = myTexture;
    // }
}