using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace GameOfStuff
{
    public class GameHub : Hub
    {
        public void SubmitAnswer(GameModel gameModel)
        {
            Clients.Group(gameModel.SessionID).SendAsync("AddAnswer", gameModel.Answer);
        }

        public void JoinGroup(string groupName)
        {
            Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
    }

    public class GameModel
    {
        public string SessionID { get; set; }
        public string Answer { get; set; }
    }
}
