using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
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

        private async Task<string> GetRandomQuestion()
        {
            var questions = await _db.Questions.ToListAsync();
            var r = new Random();
            var rIndex = r.Next(1, questions.Count());
            return questions[rIndex].Text;
        }

        public async Task<Game> NewGame(string gameName, string password)
        {
            Game newGame = new Game()
            {
                GameID = gameName,
                Password = password,
                Question = await GetRandomQuestion()
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

        public async Task<Game> SubmitAnswer(string gameID, string playerID, string answer)
        {
            var player = await _db.Players.FindAsync(playerID, gameID);

            try
            {
                player.Answer = answer;
                _db.Update(player);
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

        public async Task<Game> AnswerGuessed(string gameID, string playerID)
        {
            var player = await _db.Players.FindAsync(playerID, gameID);
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

        public async Task<Game> AnswerReset(string gameID, string playerID)
        {
            var player = await _db.Players.FindAsync(playerID, gameID);
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

        public async Task<Game> JoinGame(string gameID, string password, string playerID)
        {
            var game = await _db.Games.FindAsync(gameID);
            if(game == null)
            {
                throw new Exception($@"GameID: {gameID} doesn't exist.");
            }

            if (game.Password != password)
            {
                throw new Exception($@"Incorrect password.");
            }

            try
            {
                Player player = new Player
                {
                    GameID = gameID,
                    PlayerID = playerID,
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

        public async Task<Game> NewRound(string gameID)
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

            var game = await _db.Games.FindAsync(gameID);
            game.Question = await GetRandomQuestion();
            _db.Update(game);
            await _db.SaveChangesAsync();

            return game;
        }
    }
}
