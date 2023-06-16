﻿using System;
using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;

namespace PSRMPoliceUtilities.Commands.Fine
{
    public class ActiveFines : IRocketCommand
    {
        public void Execute(IRocketPlayer caller, string[] command)
        {
            var player = (UnturnedPlayer)caller;

            var target = command.Length == 0 ? player : UnturnedPlayer.FromName(command[0]);

            if (target == null)
            {
                UnturnedChat.Say(player, PSRMPoliceUtilities.Instance.Translate("target_not_found"));
                return;
            }

            var activeFines = PSRMPoliceUtilities.Instance.FinesDatabase.FindActiveFines(target.Id);

            if (activeFines.Count == 0)
            {
                UnturnedChat.Say(player, PSRMPoliceUtilities.Instance.Translate("no_active_fines", target.CharacterName));
                return;
            }

            if (activeFines.Count > 4)
            {
                UnturnedChat.Say(player, PSRMPoliceUtilities.Instance.Translate("too_many_active_fines", target.CharacterName));
            }

            for (int i = 0; i < Math.Min(4, activeFines.Count); i++)
            {
                var fine = activeFines[i];
                UnturnedChat.Say(player, PSRMPoliceUtilities.Instance.Translate("active_fine", target.CharacterName, fine.FinedDate, fine.FinedAmount, "experience points", fine.Reason, fine.CaseID));
            }
        }

        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "activefines";
        public string Help => "Shows the active fines that you have.";
        public string Syntax => "/activefines";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "ps.policeutilities.activefines" };
    }
}
