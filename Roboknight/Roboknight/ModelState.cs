//Author: Ilan Segal
//File Name: ModelState.cs
//Project Name: RoboKnight
//Creation Date: January 9, 2015
//Modified Date: January 9, 2016
//Description: Used to distinguish separate states that the Model can be in at any time

namespace Roboknight
{
    enum ModelState
    {
        Game,
        MainMenu,
        PauseMenu,
        GameOver,
        Quit
    }
}
