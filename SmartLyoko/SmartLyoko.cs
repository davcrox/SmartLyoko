using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LyokoAPI.Plugin;
using LyokoAPI.Events;

namespace SmartLyoko
{
    public class SmartLyokoObject : LyokoAPIPlugin
    {
        public override string Name { get; } = "SmartLyoko";
        public override string Author { get; } = "Davcrox";

        private static string _token = "";
        protected override bool OnEnable()
        {
            Listener.StartListening(); //Starting to listen to events in the OnEnable() is recommended
            return true; //nothing went wrong, so we return true to show the plugin has been enabled properly
        }

        protected override bool OnDisable()
        {
            Listener.StopListening(); //Definitely stop listening to events OnDisable(), since code will still be run if you dont.
            return true; //disabled succesfully, returning true.
        }
        /*
        * This method is run when a gamesession starts in the Application (if it has game sessions.)
        *
        */
        public override void OnGameStart(bool storyMode)
        {
            LyokoAPI.Events.LyokoLogger.Log("test", "OnGameStart");
            Listener.StopListening(); //if your listners somehow break immersion, do this.
        }

        public override void OnGameEnd(bool failed)
        {
            LyokoAPI.Events.LyokoLogger.Log("test", "OnGameEnd");
            Listener.StartListening(); //start listening because we stopped in OnGameEnd

        }
    }
}
