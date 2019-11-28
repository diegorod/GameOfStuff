using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using GameOfStuff.Services;
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

        public async Task UpdatePlayers(string gameId)
        {
            await Clients.Group(gameId).SendAsync("GameUpdate");
        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Game game = await _gs.LeaveGame(Context.ConnectionId);
            if(game != null) //There are still players left
            {
                await UpdatePlayers(game.GameID);
            }
            await base.OnDisconnectedAsync(exception);
        }

    }
}
