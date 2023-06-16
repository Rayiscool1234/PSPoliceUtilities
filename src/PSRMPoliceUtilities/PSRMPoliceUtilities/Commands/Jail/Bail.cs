using System;
using System.Collections.Generic;
using PSRMPoliceUtilities.Models;
using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace PSRMPoliceUtilities.Commands.JailCommands
{
    public class Bail : IRocketCommand
    {
        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer unturnedPlayer = (UnturnedPlayer)caller;

            var jailedPlayer = UnturnedPlayer.FromName(command[0]);

            if (command[0].Length < 1) jailedPlayer = unturnedPlayer;

            if (jailedPlayer == null)
            {
                ChatManager.serverSendMessage($"Player does not exist.", Color.red, null, unturnedPlayer.SteamPlayer(), EChatMode.SAY, null, true);
                return;
            }

            JailTime jailedTime = new JailTime();
            if (!PSRMPoliceUtilities.Instance.JailTimeService.IsPlayerJailed(jailedPlayer.CSteamID.ToString(), out jailedTime)) return;
            if (jailedTime == null)
            {
                ChatManager.serverSendMessage($"{jailedPlayer} is not in jail.", Color.red, null, unturnedPlayer.SteamPlayer(), EChatMode.SAY, null, true);
                return;
            }

            var currentExperience = jailedPlayer.Experience;
            var requiredExperience = PSRMPoliceUtilities.Instance.Configuration.Instance.ExperiencePerMinute * (jailedTime.ExpireDate - DateTime.UtcNow).Minutes;
            if (currentExperience < requiredExperience)
            {
                ChatManager.serverSendMessage($"You need {requiredExperience} experience points, but you only have {currentExperience} experience points!", Color.red, null, null, EChatMode.GLOBAL, null, true);
                return;
            }

            jailedPlayer.Experience -= (uint)requiredExperience;
            ChatManager.serverSendMessage($"You bailed {jailedPlayer.CharacterName} for {requiredExperience} experience points.", Color.red, null, null, EChatMode.GLOBAL, null, true);

            ChatManager.serverSendMessage($"{unturnedPlayer.CharacterName} bailed {jailedPlayer.CharacterName} from {jailedTime.JailName}", Color.blue, null, null, EChatMode.GLOBAL, null, true);

            jailedPlayer.Teleport(new Vector3(PSRMPoliceUtilities.Instance.Configuration.Instance.ReleaseLocation.x, PSRMPoliceUtilities.Instance.Configuration.Instance.ReleaseLocation.y, PSRMPoliceUtilities.Instance.Configuration.Instance.ReleaseLocation.z), 0);
            PSRMPoliceUtilities.Instance.JailTimesDatabase.Data.Remove(jailedTime);
        }

        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "isjailed";
        public string Help => "Check if someone is jailed.";
        public string Syntax => "/bail [player]";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "ps.policeutilities.bail" };
    }
}
