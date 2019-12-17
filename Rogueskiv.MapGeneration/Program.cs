using System;

namespace Rogueskiv.MapGeneration
{
    class Program
    {
        private const int WIDTH = 64;
        private const int HEIGHT = 32;
        private const float EXPAND_PROBABILITY = 0.33f;
        private const float TURN_PROBABILITY = 0.1f;
        private const float MIN_DENSITY = 0.33f;
        private const int INITIAL_ROOMS = 15;
        private const int MIN_ROOM_SIZE = 3;

        static void Main(string[] args)
        {
            while (true)
            {
                var map = MapGenerator.GenerateMap(
                    WIDTH, HEIGHT, EXPAND_PROBABILITY, TURN_PROBABILITY, MIN_DENSITY, INITIAL_ROOMS, MIN_ROOM_SIZE
                );
                Console.WriteLine(map);

                var k = Console.ReadLine();
                if (k.ToUpper() == "Q")
                    return;
            }
        }
    }
}
