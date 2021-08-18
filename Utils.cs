using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using UnityEngine;
using HarmonyLib;
using Alta.Networking;

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
            static public event Action playerSpawnEvent; //This event is called when the player prefab is spawned into the game

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
            }
        }

        static public class Game
        {
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

                    return newPlayer;
                }
                else
                    return null;
            }

            public class GamePlayer
            {
                public Player player;
                public Alta.Character.Hand rightHand;
                public Alta.Character.Hand leftHand;
                public PlayerController playerController;
                public LocomotionController locomotionController;
            }
        }
    }
}
