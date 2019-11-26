using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameOfStuff.Data
{
    public class GameService
    {
        private readonly GameDbContext _db;

        public GameService(GameDbContext db)
        {
            _db = db;
        }

        public async Task<Game> GetGameModel(string gameID)
        {
            return await _db.Games
                .Include(g => g.Players)
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.GameID == gameID);
        }

        private string GetRandomQuestion()
        {
            string question = "";
            //TODO get random question from flatfile
            return question;
        }
        public async Task<Game> NewGame(string gameName, string password)
        {
            Game newGame = new Game()
            {
                GameID = gameName,
                Password = password
            };

            //Check if game name is unique
            if(_db.Games.Any(x => x.GameID == gameName)){
                throw new Exception("Game name already exists. Please try something else.");
            }

            try
            {
                var result = _db.Games.Add(newGame);
                await _db.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception ex)
            { 
                throw ex;
            }
        }

        public async Task<Game> SubmitAnswer(string userID, string answer)
        {
            var player = _db.Players.Find(userID);

            try
            {
                player.Answer = answer;
                _db.Update(answer);
                await _db.SaveChangesAsync();
                return await GetGameModel(player.GameID);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public bool RevealAnswers(string gameID)
        {
            var game = GetGameModel(gameID).Result;
            return !game.Players.Any(p => p.Answer == string.Empty);
        }

        public async Task<Game> AnswerGuessed(string userID)
        {
            var player = _db.Players.Find(userID);
            try
            {
                player.IsOut = true;
                _db.Update(player);
                await _db.SaveChangesAsync();
                return await GetGameModel(player.GameID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Game> AnswerReset(string userID)
        {
            var player = _db.Players.Find(userID);
            try
            {
                player.IsOut = false;
                _db.Update(player);
                await _db.SaveChangesAsync();
                return await GetGameModel(player.GameID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Game> JoinGame(string gameID, string password, string user)
        {
            var game = _db.Games.Find(gameID);
            if(game != null)
            {
                throw new Exception($@"GameID: {gameID} doesn't exist.");
            }

            if (game.Password == password)
            {
                throw new Exception($@"Incorrect password.");
            }

            try
            {
                Player player = new Player
                {
                    GameID = gameID,
                    PlayerID = user,
                    IsOut = false
                };
                _db.Players.Add(player);
                await _db.SaveChangesAsync();
                return game;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Game> ResetGame(string gameID)
        {
            var players = _db.Players.Where(x => x.GameID == gameID).ToList();
            if (players != null)
            {
                foreach (Player player in players)
                {
                    player.IsOut = false;
                    player.Answer = "";
                    _db.Players.Update(player);
                }
                await _db.SaveChangesAsync();
            }
            return await GetGameModel(gameID);
        }
    }
}
