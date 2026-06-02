using System;
using SCPCB360;

try
{
    Console.WriteLine("Starting SCPCB360...");
    using var game = new SCPCB360Game();
    game.Run();
    Console.WriteLine("Game exited normally.");
}
catch (Exception ex)
{
    Console.WriteLine("CRASH:");
    Console.WriteLine(ex);
    Console.ReadLine();
}