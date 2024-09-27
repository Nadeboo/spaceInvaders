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
            // clear the arrays by setting all elements to null
            for (int i = 0; i < game.Enemies.GetLength(0); i++)
            {
                for (int j = 0; j < game.Enemies.GetLength(1); j++)
                {
                    game.Enemies[i, j] = null;
                    game.hardEnemies[i, j] = null;
                }
            }

            //basically the same as what's happening in enemy.cs 
            //resets enemies to their original positions rather than creating new ones
            //though it also very clearly creates new enemies so like i don't really know?
            //game breaks if its removed thats what matters
            Random random = new Random();
            for (int i = 0; i < game.InitialEnemyPositions.GetLength(0); i++)
            {
                for (int j = 0; j < game.InitialEnemyPositions.GetLength(1); j++)
                {
                    Vector2 position = game.InitialEnemyPositions[i, j];
                    if (random.Next(2) == 0)
                    {
                        game.Enemies[i, j] = new Enemy(game.GraphicsDevice, (int)position.X, (int)position.Y, game.EnemyWidth, game.EnemyHeight);
                    }
                    else
                    {
                        game.hardEnemies[i, j] = new Enemy(game.GraphicsDevice, (int)position.X, (int)position.Y, game.EnemyWidth, game.EnemyHeight);
                    }
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