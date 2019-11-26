using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using GameOfStuff.Data;

namespace GameOfStuff
{
    public class GameHub : Hub
    {
        private readonly GameService _gs;
        public GameHub(GameService gs)
        {
            _gs = gs;
        }

        public void UpdatePlayers(Game gameModel)
        {
            Clients.Group(gameModel.GameID).SendAsync("GameUpdate", gameModel);
        }

        public void JoinGroup(string groupName)
        {
            Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
    }
}
