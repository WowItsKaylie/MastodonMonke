using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using ComputerInterface;
using UnityEngine;
using Zenject;
using Mastonet;
using System.IO;

namespace MastodonMonke
{
    internal class MastodonMonkeCommandManager : IInitializable, IDisposable
    {
        private readonly CommandHandler _commandHandler;
        private List<CommandToken> _commandTokens;

        // Request the CommandHandler
        // This gets resolved by zenject since we bind MyModCommandManager in the container
        public MastodonMonkeCommandManager(CommandHandler commandHandler)
        {
            _commandHandler = commandHandler;
        }

        public void Initialize()
        {
            _commandTokens = new List<CommandToken>();
            Debug.Log("loading mastodonmonke config");
            var customFile = new ConfigFile(Path.Combine(Paths.ConfigPath, "MastodonMonke.cfg"), true);
            Debug.Log("loading instance and token from config");
            var instance = customFile.Bind("Login", "Instance", "https://mastodon.social", "Mastodon instance to use");
            var accessToken = customFile.Bind("Login", "Token", "PutYourTokenHere", "Access token to use. Get this from the Development category in your Mastodon settings. Don't share this with anyone, anybody with this token can freely access your application and usually full write access in turn!!!!!");
            Debug.Log("creating authClient and client");
            var authClient = new AuthenticationClient(instance.Value);
            var client = new MastodonClient(instance.Value, accessToken.Value);

            // Add a command
            // You can pass null in argumentsType if you aren't expecting any
            // RegisterCommand(new Command(name: "testpost", argumentTypes: null, args =>
            // {
            //     client.PublishStatus("Test post from my Gorilla Tag Mastodon mod", Visibility.Public);
            //     return "attempted to post ong";
            // }));

            // Pass null in the argumentTypes array for any type that doesn't need to be converted (i.e. it's a string)
            RegisterCommand(new Command(name: "post", argumentTypes: new Type[]{null}, args =>
            {
                if (accessToken.Value != accessToken.DefaultValue.ToString())
                {
                    client.PublishStatus((string)args[0], Visibility.Public);
                    Debug.Log("Attempted to post: " + (string)args[0]);
                    return "Attempted to post:\n" + (string)args[0];
                }
                else
                {
                    return "Placeholder token still supplied in config\nNot attempting to post anything";
                }
            }));

            // Pass the types of the arguments you are expecting and the command handler will try to find an converter
            // and convert them to for you one the command is executed
            // the types are checked during adding and if the command handler doesn't find an converter for the type it will log an error
            // RegisterCommand(new Command(name: "add", argumentTypes: new [] { typeof(int), typeof(int) }, args =>
            // {
            //     return ((int) args[0] + (int) args[1]).ToString();
            // }));

            // you can add your own converters like this
            // NOTE: this will add the converter in BepInEx globally
            // that means you can use the type as a config entry too.
            // only use this if you need to convert the type often
            // in your program or if you are using it in configs too.

            // already handled types:
            // all c# predefined data types like int, float, bool and so on
            // all unity vector lengths (vector2, vector3, ...)
            // unity color
            // unity quaternion
            var converter = new TypeConverter()
            {
                ConvertToString = (obj, type) =>
                {
                    var color = (MyOwnColorClass) obj;
                    return ColorUtility.ToHtmlStringRGB(new Color(color.R, color.G, color.B));
                },
                ConvertToObject = (str, type) =>
                {
                    ColorUtility.TryParseHtmlString((str.StartsWith("#")?"":"#")+str, out var unityColor);
                    return new MyOwnColorClass((int)(unityColor.r*255), (int)(unityColor.g * 255), (int)(unityColor.b * 255));
                }
            };

            TomlTypeConverter.AddConverter(typeof(MyOwnColorClass), converter);

            // RegisterCommand(new Command(name: "hextorgb", new []{typeof(MyOwnColorClass)}, args =>
            // {
            //     var color = (MyOwnColorClass) args[0];
            //     return $"{color.R}, {color.G}, {color.B}";
            // }));
        }

        public void RegisterCommand(Command cmd)
        {
            var token = _commandHandler.AddCommand(cmd);
            _commandTokens.Add(token);
        }

        public void UnregisterAllCommands()
        {
            foreach (var token in _commandTokens)
            {
                token.UnregisterCommand();
            }
        }

        public void Dispose()
        {
            UnregisterAllCommands();
        }
    }

    public class MyOwnColorClass
    {
        public int R;
        public int G;
        public int B;

        public MyOwnColorClass(int r, int g, int b)
        {
            R = r;
            G = g;
            B = b;
        }
    }
}