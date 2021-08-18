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
            harmony.PatchAll(typeof(SpawnLocalPatch));


            Logger.LogInfo("ATT Utils Loaded!");

        }

        [HarmonyPatch(typeof(SpawnManager), "SpawnLocal")]
        class SpawnLocalPatch
        {
            static void Postfix(NetworkEntity __result)
            {
                Events.SpawnLocal();
            }
        }

        static public class Events
        {
            static public event Action playerSpawnEvent;

            static public void SpawnLocal()
            {
                playerSpawnEvent?.Invoke();
            }
        }

        static public class Game
        {
            static public GamePlayer GetLocalPlayer()
            {
                Player[] playerObjects = FindObjectsOfType<Player>();

                Player collectedPlayer = null;

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
                    var newPlayer = new GamePlayer();

                    newPlayer.player = collectedPlayer;
                    newPlayer.rightHand = collectedPlayer.PlayerController.RightController.Hand;
                    newPlayer.leftHand = collectedPlayer.PlayerController.LeftController.Hand;

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
