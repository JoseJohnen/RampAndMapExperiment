using Stride.Engine;

namespace RampAndMapExperiment
{
    class RampAndMapExperimentApp
    {
        static void Main(string[] args)
        {
            using (var game = new Game())
            {
                game.Run();
            }
        }
    }
}
