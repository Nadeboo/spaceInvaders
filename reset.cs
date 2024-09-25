using Microsoft.Xna.Framework;
using System;
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
            game.hardEnemies.Clear();
            Random random = new Random();
            foreach (Vector2 position in game.InitialEnemyPositions)
            {
                if (random.Next(2) == 0)
                {
                    game.Enemies.Add(new Enemy(game.GraphicsDevice, (int)position.X, (int)position.Y, game.EnemyWidth, game.EnemyHeight));
                }
                else
                {
                    game.hardEnemies.Add(new Enemy(game.GraphicsDevice, (int)position.X, (int)position.Y, game.EnemyWidth, game.EnemyHeight));
                }
            }
            ResetMovement();
        }

        private void ResetLives()
        {
            game.Lives = 1;
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