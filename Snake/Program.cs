// Namespaces in C# are used to orgnize too many classes so that it can be easy to handle the application.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Threading;
using System.ComponentModel;
//System or library for adding media
using System.Media;
using System.Windows.Media;
using System.Net;
 
//namespace
namespace Snake
{   
    struct Position
    {       
        //Variable
        public int row;
        public int col;
        public Position(int row, int col)
        {
            this.row = row;
            this.col = col;
        }
        //this is a structure type entity which holds the data for various positions which will be used as the coordinates on the console screen      
    }
   
    class Game
    {
        public string gameOverScreen(int userPoints, int snakeLives)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            // Re-position the "game over" text at the center of the screen as shown in Figure 1.
            string gameover = "Game over!";
            Console.SetCursorPosition((Console.WindowWidth - gameover.Length) / 2, (Console.WindowHeight / 2) - 2);
            Console.WriteLine(gameover);

            //Re-position the the result of the game
            string statuspoint = "Your points are: {0}";
            Console.SetCursorPosition((Console.WindowWidth - statuspoint.Length) / 2, (Console.WindowHeight / 2) - 1);
            Console.WriteLine(statuspoint, userPoints);
            
            string snakelivesstatus ="Snake lives left: 0";
            Console.SetCursorPosition((Console.WindowWidth - snakelivesstatus.Length) / 2, (Console.WindowHeight / 2) - 3);
            Console.WriteLine(snakelivesstatus, snakeLives);

            //Add instructions at the end of the game and re-position it
            string msg = "Enter player name:";
            string name;
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition((Console.WindowWidth - msg.Length) / 2, (Console.WindowHeight / 2));
            Console.WriteLine(msg);
            Console.SetCursorPosition((Console.WindowWidth - msg.Length) / 2, (Console.WindowHeight / 2) + 1);
            name = Console.ReadLine();
            return name;
        }

        public string gameWon()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;

            //Declare that the player wins in the middle of the screen
            string winning = "CONGRATULATIONS YOU WIN!";
            Console.SetCursorPosition((Console.WindowWidth - winning.Length) / 2, (Console.WindowHeight / 2) - 1);           
            Console.WriteLine(winning);

            //Add instructions at the end of the game and re-position it
            string msg = "Enter player name:";
            string name;
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition((Console.WindowWidth - msg.Length) / 2, (Console.WindowHeight / 2));
            Console.WriteLine(msg);
            Console.SetCursorPosition((Console.WindowWidth - msg.Length) / 2, (Console.WindowHeight / 2) + 1);
            name = Console.ReadLine();
            return name;
        }

        public void createFood(Position food)
        {
            Console.SetCursorPosition(food.col, food.row);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.OutputEncoding = Encoding.UTF8;
            Console.Write("\u2665\u2665");
        }
        
        public void createBonusFood(Position BonusFood)
        {
            Console.SetCursorPosition(BonusFood.col, BonusFood.row);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("#");
        }

        public void createObstacle(Position obstacle)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.SetCursorPosition(obstacle.col, obstacle.row);
            Console.OutputEncoding = Encoding.UTF8;
            Console.Write("\u2592");
        }

        public void createSnakeBody(Position position)
        {
            Console.SetCursorPosition(position.col, position.row);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("*");
        }
    }
    
    //Main Menu class
    class MainMenu
    {
        //Console Output
        public void mainMenu()
        {
            Console.WriteLine("Welcome to the Snake Game");
            Console.WriteLine("1) Play");
            Console.WriteLine("2) Scoreboard");
            Console.WriteLine("3) Exit");
        }

    }
    
    
    
    class Program
    {
        //the main compiler
        
        static void Main(string[] args)
        {
            MainMenu startgame = new MainMenu();
            byte right = 0;
            byte left = 1;
            byte down = 2;
            byte up = 3;
            int lastFoodTime = 0;
            int lastBonusFoodTime = 0;
            //The food relocate's timer (adjusted)
            int foodDissapearTime = 15000;
            int BonusFoodDisappearTime = 9000;
            int negativePoints = 0;
            int userPoints = 0;
            double sleepTime = 100;
            //Snake contain three lives
            int snakeLives = 3;
            

            Game game1 = new Game();

            //Background Music (Looping)
            //Continue the background music after snake eat the food
            MediaPlayer backgroundMusic = new MediaPlayer();
            backgroundMusic.Open(new System.Uri(Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wii.wav")));

            
            
            bool gameLoop = false;

            
            startgame.mainMenu();
            string userOption = Console.ReadLine();

            if (userOption == "1")
            {
                Console.Clear();
                gameLoop = true;

            }
            else if (userOption == "2")
            {
                Console.Clear();
                string lines = File.ReadAllText("Score.txt");
                string[] items = lines.Split(' ', '\n');


                int n = items.Length;
                for (int i = 1; i < n -1 ; i+=2)
                {
                    int key = int.Parse(items[i]);
                    int j = i - 2;

                    while (j >= 0 && int.Parse(items[j]) < key)
                    {
                        items[j + 2] = items[j];
                        j = j - 2;
                    }
                    items[j + 2] = key.ToString();
                }

                
                for (int x = 0; x < n - 1; x += 2)
                {
                    Console.WriteLine("Name = {0}, Score = {1}",
                        items[x], items[x + 1]);
                }
                
                Console.ReadLine();
                    
            }
            else if (userOption == "3")
            { 
                gameLoop = false;
                Environment.Exit(0);
            }
            
            
            
            Position[] directions = new Position[]
            {
                new Position(0, 1), // right
                new Position(0, -1), // left
                new Position(1, 0), // down
                new Position(-1, 0), // up
            };                       
            
            //this defaults the snake's direction to right when the game starts
            int direction = right; 
            Random randomNumbersGenerator = new Random();
            Console.BufferHeight = Console.WindowHeight;
            lastFoodTime = Environment.TickCount;

            //Make a list of coordinates of the position
            List<Position> obstacles = new List<Position>();

            //5 obstacles have been created while the game start running with different position  
            for (int i = 0; i < 5; i++)
            {             
                obstacles.Add(new Position(randomNumbersGenerator.Next(1, Console.WindowHeight), randomNumbersGenerator.Next(0, Console.WindowWidth)));               
            }

            
            // For loop for creating the obstacles
            foreach (Position obstacle in obstacles)
            {
                game1.createObstacle(obstacle);
            }

        
            Queue<Position> snakeElements = new Queue<Position>();
            //change 5 to 3 to make the default size of the snake to 3 upon start
            for (int i = 0; i <= 3; i++)
            {
                snakeElements.Enqueue(new Position(1, i));
            }
            
            
            //creating food item
            Position food;
            do
            {
                food = new Position(randomNumbersGenerator.Next(1, Console.WindowHeight),
                    randomNumbersGenerator.Next(0, Console.WindowWidth));
            }
            while (snakeElements.Contains(food) || obstacles.Contains(food));

            game1.createFood(food);
            
            Position BonusFood;
            do
            {
                BonusFood = new Position(randomNumbersGenerator.Next(1, Console.WindowHeight), randomNumbersGenerator.Next(0, Console.WindowWidth));
            }
            while (snakeElements.Contains(BonusFood) || obstacles.Contains(BonusFood));
            
            game1.createBonusFood(BonusFood);
            
            //the body of the snake
            foreach (Position position in snakeElements)
            {
                game1.createSnakeBody(position);
            }
            
            while (gameLoop)
            {
                negativePoints++;
                //background music (looping)
                backgroundMusic.Play();
                if (backgroundMusic.Position >= new TimeSpan(0, 0, 29))
                {
                    backgroundMusic.Position = new TimeSpan(0, 0, 0);
                }

                //control keys
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo userInput = Console.ReadKey();
                    if (userInput.Key == ConsoleKey.LeftArrow)
                    {
                        if (direction != right)
                        {
                            direction = left;
                        }
                    }
                    if (userInput.Key == ConsoleKey.RightArrow)
                    {
                        if (direction != left)
                        {
                            direction = right;
                        }
                    }
                    if (userInput.Key == ConsoleKey.UpArrow)
                    {
                        if (direction != down)
                        {
                            direction = up;
                        }
                    }
                    if (userInput.Key == ConsoleKey.DownArrow)
                    {
                        if (direction != up)
                        {
                            direction = down;
                        }
                    }
                }
                //apart from giving the snake directions on where to move, it also ensures that the snake doesnt move the opposite direction directly
               
                Position snakeHead = snakeElements.Last();
                Position nextDirection = directions[direction];

                Position snakeNewHead = new Position(snakeHead.row + nextDirection.row,
                snakeHead.col + nextDirection.col);
                
                //This is to display the player score
                Console.SetCursorPosition(0, 0);
                Console.ForegroundColor = ConsoleColor.White;
                //if (userPoints < 0) userPoints = 0;
                userPoints = Math.Max(userPoints, 0);
                Console.WriteLine("Your points are: {0} \t REACH 10 POINTS TO WIN", userPoints);
                
                //This is to display the snake lives.
                Console.SetCursorPosition(100, 0);
                Console.ForegroundColor = ConsoleColor.White;
                snakeLives = Math.Max(snakeLives, 0);
                Console.WriteLine("Snake lives left: {0} \t", snakeLives);
                
                //this is the game over scene after the player loses the game either by the snake colliding with itself or the snake colliding with obstacles                
                if (snakeElements.Contains(snakeNewHead) || obstacles.Contains(snakeNewHead))
                {
                    snakeLives--;   
                } 
                else if (userPoints >= 10) //winning condition
                {
                    //This sound plays when the player wins
                    SoundPlayer sound2 = new SoundPlayer("gamestart.wav");
                    sound2.Play();

                    string player_name = game1.gameWon();

                    using (StreamWriter file = new StreamWriter("Score.txt", true))
                    {
                        file.WriteLine(player_name + " " + userPoints);
                    }
                    return;
                }
                //when the snake died
                if(snakeLives == 0 || (snakeNewHead.col < 0) || (snakeNewHead.row < 1) || (snakeNewHead.row >= Console.WindowHeight) || (snakeNewHead.col >= Console.WindowWidth))
                {
                    //This is to display the snake lives.
                    Console.SetCursorPosition(100, 0);
                    Console.ForegroundColor = ConsoleColor.White;          
                    Console.WriteLine("Snake lives left: 0 ");

                    SoundPlayer sound1 = new SoundPlayer("die.wav");
                    sound1.Play();

                    string player_name = game1.gameOverScreen(userPoints, snakeLives);

                    using (StreamWriter file = new StreamWriter("Score.txt", true))
                    {
                        file.WriteLine(player_name + " " + userPoints);
                    }
                    return;
                }
                
                           
                game1.createSnakeBody(snakeHead);
               
                //whenever the snake changes direction, the head of the snake changes according to the direction
                snakeElements.Enqueue(snakeNewHead);
                Console.SetCursorPosition(snakeNewHead.col, snakeNewHead.row);
                Console.ForegroundColor = ConsoleColor.Gray;
                if (direction == right)
                {
                    Console.Write(">");
                }
                if (direction == left)
                {
                    Console.Write("<");
                }
                if (direction == up)
                {
                    Console.Write("^");
                }
                if (direction == down)
                {
                    Console.Write("v");
                }               
                
                // feeding the snake
                if ((snakeNewHead.col == food.col && snakeNewHead.row == food.row)|| (snakeNewHead.col == food.col+1 && snakeNewHead.row == food.row)) //if snake head's coordinates is same with food
                {
                    Console.SetCursorPosition(food.col, food.row);
                    Console.Write(" ");

                    Console.SetCursorPosition(food.col+1, food.row);
                    Console.Write(" ");

                    //Increase snake movement speed when eating the food
                    sleepTime -= 10.00;

                    //Snake eat food sound effect
                    SoundPlayer sound3 = new SoundPlayer("food.wav");
                    sound3.Play();       
                    
                    //add one point when food is eaten
                    userPoints++;
                    
                    //creates new food 
                    do
                    {
                          food = new Position(randomNumbersGenerator.Next(1, Console.WindowHeight),
                          randomNumbersGenerator.Next(0, Console.WindowWidth));
                    }
                    while (snakeElements.Contains(food) || obstacles.Contains(food));
                    
                    lastFoodTime = Environment.TickCount;
                    game1.createFood(food);
                    sleepTime--;
                    
                    //creates new obstacle
                    Position obstacle = new Position();
                    do
                    {
                         obstacle = new Position(randomNumbersGenerator.Next(1, Console.WindowHeight),
                         randomNumbersGenerator.Next(0, Console.WindowWidth));
                    }
                    while (snakeElements.Contains(obstacle) || obstacles.Contains(obstacle) || (food.row != obstacle.row && food.col != obstacle.row));
                         obstacles.Add(obstacle);
                         game1.createObstacle(obstacle);
                }
                // snake eat bonus food
                else if (snakeNewHead.col == BonusFood.col && snakeNewHead.row == BonusFood.row) //if snake head's coordinates is same with food
                {
                    //Increase snake movement speed when eating the food
                    sleepTime -= 10.00;

                    //Snake eat food sound effect
                    SoundPlayer sound3 = new SoundPlayer("food.wav");
                    sound3.Play();

                    //add one point when food is eaten
                    userPoints += 2;

                    //creates new Bonusfood 
                    do
                    {
                        BonusFood = new Position(randomNumbersGenerator.Next(1, Console.WindowHeight),
                        randomNumbersGenerator.Next(0, Console.WindowWidth));
                    }
                    while (snakeElements.Contains(BonusFood) || obstacles.Contains(BonusFood));

                    lastBonusFoodTime = Environment.TickCount;
                    game1.createBonusFood(BonusFood);
                    sleepTime--;

                    //creates new obstacle
                    Position obstacle = new Position();
                    do
                    {
                        obstacle = new Position(randomNumbersGenerator.Next(1, Console.WindowHeight),
                        randomNumbersGenerator.Next(0, Console.WindowWidth));
                    }
                    while (snakeElements.Contains(obstacle) || obstacles.Contains(obstacle) || (BonusFood.row != obstacle.row && BonusFood.col != obstacle.row));
                    obstacles.Add(obstacle);
                    game1.createObstacle(obstacle);
                }
                else
                {
                    // moving...
                    Position last = snakeElements.Dequeue();
                    Console.SetCursorPosition(last.col, last.row);
                    Console.Write(" ");
                }
                
                if (Environment.TickCount - lastFoodTime >= foodDissapearTime)
                {
                    negativePoints = negativePoints + 50;
                    Console.SetCursorPosition(food.col, food.row);
                    Console.Write(" ");
                    do
                    {
                        food = new Position(randomNumbersGenerator.Next(1, Console.WindowHeight),
                            randomNumbersGenerator.Next(0, Console.WindowWidth));
                    }
                    while (snakeElements.Contains(food) || obstacles.Contains(food));
                    lastFoodTime = Environment.TickCount;
                }

                game1.createFood(food);
                
                if (Environment.TickCount - lastBonusFoodTime >= BonusFoodDisappearTime)
                {
                    negativePoints = negativePoints + 50;
                    Console.SetCursorPosition(BonusFood.col, BonusFood.row);
                    Console.Write(" ");
                    do
                    {
                        BonusFood = new Position(randomNumbersGenerator.Next(1, Console.WindowHeight),
                            randomNumbersGenerator.Next(0, Console.WindowWidth));
                    }
                    while (snakeElements.Contains(BonusFood) || obstacles.Contains(BonusFood));
                    lastBonusFoodTime = Environment.TickCount;
                }
                game1.createBonusFood(BonusFood);
                
                Console.SetCursorPosition(0, 0);  
             
                Thread.Sleep((int)sleepTime);
            }
        }
    }
}
