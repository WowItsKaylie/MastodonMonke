using ComputerInterface;
using ComputerInterface.ViewLib;
using Mastonet;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using System.IO;

namespace MastodonMonke
{
    
    public class MastodonMonkePostView : ComputerView
    {


        private void Awake()
        {
            Debug.Log("loading mastodonmonke config");
            var customFile = new ConfigFile(Path.Combine(Paths.ConfigPath, "MastodonMonke.cfg"), true);
            Debug.Log("loading instance and token from config");
            var instance = customFile.Bind("Login", "Instance", "https://mastodon.social", "Mastodon instance to use");
            var accessToken = customFile.Bind("Login", "Token", "PutYourTokenHere", "Access token to use. Get this from the Development category in your Mastodon settings. Don't share this with anyone, anybody with this token can freely access your application and usually full write access in turn!!!!!");
            Debug.Log("creating authClient and client");
            var authClient = new AuthenticationClient(instance.Value);
            var client = new MastodonClient(instance.Value, accessToken.Value);
        }
        
        public override void OnShow(object[] args)
        {
            Text = "Enter your post below\n";
        }

        // you can do something on keypresses by overriding "OnKeyPressed"
        // it get's an EKeyboardKey passed as a parameter which wraps the old character string
        public override void OnKeyPressed(EKeyboardKey key)
        {
            // private readonly UITextInputHandler _textInputHandler;

            // changing the Text property will fire an PropertyChanged event
            // which lets the computer know the text has changed and update it


            var _textInputHandler = new UITextInputHandler();
            if (_textInputHandler.HandleKey(key))
            {
                Text = "Enter your post below\n" + _textInputHandler.Text;
                return;
            }
            switch (key)
            {
                case EKeyboardKey.Enter:
                    client.PublishStatus(_textInputHandler.Text, Visibility.Public);
                case EKeyboardKey.Delete:
                    _textInputHandler.Text = "";
                    ReturnToMainMenu();
                    break;
            }
        }
    }
}