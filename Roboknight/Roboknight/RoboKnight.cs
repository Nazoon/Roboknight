//Author: Ilan Segal
//File Name: RoboKnight.cs
//Project Name: RoboKnight
//Creation Date: November 7, 2015
//Modified Date: December 17, 2015
//Description: Runs the game.

using System;

namespace Roboknight
{
#if WINDOWS || XBOX
    static class RoboKnight
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Driver game = new Driver())
            {
                game.Run();
            }
        }
    }
#endif
}

