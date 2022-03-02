using System;
using System.Text;

namespace yes_polish_draughts
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            var game = new Game();
            game.Start();
        }
    }
}
