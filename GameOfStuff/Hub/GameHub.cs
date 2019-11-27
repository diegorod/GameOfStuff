using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using GameOfStuff.Services;

namespace GameOfStuff
{
    public class GameHub : Hub
    {
        private readonly GameService _gs;
        public GameHub(GameService gs)
        {
            _gs = gs;
        }

        public async Task UpdatePlayers(string gameId)
        {
            await Clients.GroupExcept(gameId, Context.ConnectionId).SendAsync("GameUpdate");
        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("GameUpdate");
        }
    }
}
