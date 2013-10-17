using System;

namespace IsometricProject
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (ScreenHandler game = new ScreenHandler())
            {
                game.Run();
            }
        }
    }
#endif
}

