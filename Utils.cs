using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using UnityEngine;
using HarmonyLib;
using Alta.Networking;
using Alta.Networking.Servers;
using Alta.WebApi.Models;

namespace ATT_Utils
{
    [BepInPlugin("com.christoffyw.plugins.attutils", "ATT Utils", "1.0.0.0")]
    public class Utils : BaseUnityPlugin
    {

        public void Awake()
        {
            //Declare Harmony Instance
            var harmony = new Harmony("com.christoffyw.plugins.attutils");

            //Apply Patches
            harmony.PatchAll(typeof(Events.SpawnLocalPatch));


            Logger.LogInfo("ATT Utils Loaded!");

        }

        static public class Events
        {
            //
            //Event Actions
            //
            static public event Action playerSpawnEvent; //This is called when the player spawns into the game
            static public event Action playerDieEvent; //This is called when a player dies
            

            static public void InvokeEvent(Action action)
            {
                action?.Invoke();
            }

            [HarmonyPatch(typeof(SpawnManager), "SpawnLocal")]
            public class SpawnLocalPatch
            {
                static public void Postfix(NetworkEntity __result)
                {
                    InvokeEvent(playerSpawnEvent);
                }


            [HarmonyPatch(typeof(Alta.StatSystem.StatBarVisibilityGroup), "PlayerDeath")]
            public class PlayerDiePatch
            {
                static public void Postfix(HealthObject health)
                {
                    InvokeEvent(playerDieEvent);
                }
            }
        }

        static public class Game
        {
            //
            // Classes
            //
            public class GamePlayer
            {
                public Player player;
                public Alta.Character.Hand rightHand;
                public Alta.Character.Hand leftHand;
                public PlayerController playerController;
                public LocomotionController locomotionController;
                public UserInfoAndRole userInfo;
            }

            //
            // Methods
            //
            static public GamePlayer GetLocalPlayer()
            {
                //Find all players in scene
                Player[] playerObjects = FindObjectsOfType<Player>();

                Player collectedPlayer = null;

                //Check if selected player is the local player
                foreach(Player player in playerObjects)
                {
                    if (player.IsLocalPlayer)
                    {
                        collectedPlayer = player;
                        break;
                    }
                    else
                        continue;
                }
                if (collectedPlayer)
                {
                    //Create new type for easy use
                    var newPlayer = new GamePlayer();

                    //Set common variables
                    newPlayer.player = collectedPlayer;
                    newPlayer.rightHand = collectedPlayer.PlayerController.RightController.Hand;
                    newPlayer.leftHand = collectedPlayer.PlayerController.LeftController.Hand;
                    newPlayer.playerController = collectedPlayer.PlayerController;
                    newPlayer.locomotionController = collectedPlayer.PlayerController.LocomotionController;
                    newPlayer.userInfo = collectedPlayer.UserInfo;

                    return newPlayer;
                }
                else
                    return null;
            }
        }
    }
}
