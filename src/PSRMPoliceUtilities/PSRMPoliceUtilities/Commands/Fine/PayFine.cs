using System;
using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace PSRMPoliceUtilities.Commands.Fine
{
    public class PayFine : IRocketCommand // Declaration of class PayFine via Rocket Command
    {
        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer unturnedPlayer = (UnturnedPlayer)caller;

            if (command.Length < 1)
            {
                ChatManager.serverSendMessage($"Incorrect usage of command.", Color.red, null, unturnedPlayer.SteamPlayer(), EChatMode.SAY, null, true);
                return;
            }

            if (!Decimal.TryParse(command[0], out var caseId) || !PSRMPoliceUtilities.Instance.FinesDatabase.Collection.Exists(x => x.CaseID == caseId))
            {
                ChatManager.serverSendMessage($"Invalid case ID.", Color.red, null, unturnedPlayer.SteamPlayer(), EChatMode.SAY, null, true);
                return;
            }

            Models.Fine selectedFine = PSRMPoliceUtilities.Instance.FinesDatabase.Collection.FindOne(x => x.Active && x.CaseID == caseId);
            PSRMPoliceUtilities.Instance.FinesDatabase.DeactivateFine(selectedFine);

            // If player doesn't have enough experience, send a message and return
            if (unturnedPlayer.Experience < Convert.ToUInt32(selectedFine.FinedAmount))
            {
                ChatManager.serverSendMessage($"You don't have enough Money to pay this fine.", Color.red, null, unturnedPlayer.SteamPlayer(), EChatMode.SAY, null, true);
                return;
            }

            // Decrease experience instead of using Uconomy
            unturnedPlayer.Experience -= Convert.ToUInt32(selectedFine.FinedAmount);

            ChatManager.serverSendMessage($"Paid fine with the Case ID of {selectedFine.CaseID}.", Color.blue, null, unturnedPlayer.SteamPlayer(), EChatMode.SAY, null, true);
        }

        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "payfine";
        public string Help => "Pays off a fine.";
        public string Syntax => "/payfine <FineID>";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "ps.policeutilities.payfine" };
    }
}
