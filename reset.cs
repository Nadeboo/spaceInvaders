using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace spaceInvaders
{
    public class Reset
    {
        private Game1 game;

        public Reset(Game1 game)
        {
            this.game = game;
        }

        public void ResetGame()
        {
            ResetLives();
            ResetScore();
            ResetGameState();
            ResetEnemies();
            ClearProjectiles();
            ResetPlayer();
            ResetMovement();
        }

        public void ResetEnemies()
        {
            game.Enemies.Clear();
            foreach (Vector2 position in game.InitialEnemyPositions)
            {
                game.Enemies.Add(new Enemy(game.GraphicsDevice, (int)position.X, (int)position.Y, game.EnemyWidth, game.EnemyHeight));
            }
            ResetMovement();
        }

        private void ResetLives()
        {
            game.Lives = 5;
        }

        private void ResetScore()
        {
            game.Score = 0;
        }

        private void ResetGameState()
        {
            game.CurrentGameState = Game1.GameState.InGame;
        }

        private void ClearProjectiles()
        {
            game.Projectiles.Clear();
        }

        private void ResetPlayer()
        {
            game.Player.ResetPosition();
        }

        private void ResetMovement()
        {
            game.Movement = game.InitialMovement;
        }

        public void DecrementLives()
        {
            game.Lives--;
            if (game.Lives <= 0)
            {
                game.CurrentGameState = Game1.GameState.GameOver;
            }
        }
    }
}