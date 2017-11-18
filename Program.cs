using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
class Player
{
	static void Main(string[] args)
	{

		// game loop
		while (true)
		{
			var gameContext = new GameContext();
			gameContext.MyReapers = new List<Reaper>();
			gameContext.EnemyReapers = new List<Reaper>();
			gameContext.Wrecks = new List<Wreck>();
			int myScore = int.Parse(Console.ReadLine());
			int enemyScore1 = int.Parse(Console.ReadLine());
			int enemyScore2 = int.Parse(Console.ReadLine());
			int myRage = int.Parse(Console.ReadLine());
			int enemyRage1 = int.Parse(Console.ReadLine());
			int enemyRage2 = int.Parse(Console.ReadLine());
			int unitCount = int.Parse(Console.ReadLine());
			for (int i = 0; i < unitCount; i++)
			{
				string[] inputs = Console.ReadLine().Split(' ');
				int unitId = int.Parse(inputs[0]);
				int unitType = int.Parse(inputs[1]);
				int player = int.Parse(inputs[2]);
				float mass = float.Parse(inputs[3]);
				int radius = int.Parse(inputs[4]);
				int x = int.Parse(inputs[5]);
				int y = int.Parse(inputs[6]);
				int vx = int.Parse(inputs[7]);
				int vy = int.Parse(inputs[8]);
				int extra = int.Parse(inputs[9]);
				int extra2 = int.Parse(inputs[10]);

				if (unitType == Constants.ReaperTypeId)
				{
					var reaper = new Reaper()
					{
						Id = unitId,
						PlayerId = player,
						Mass = mass,
						Radius = radius,
						Position = new Position(x, y)
					};

					if (reaper.PlayerId == 0)
					{
						gameContext.MyReapers.Add(reaper);
					}
					else
					{
						gameContext.EnemyReapers.Add(reaper);
					}
				}
				else if (unitType == Constants.WreckTypeId)
				{
					var wreck = new Wreck()
					{
						Id = unitId,
						Radius = radius,
						Position = new Position(x, y),
						WaterAmount = extra
					};

					gameContext.Wrecks.Add(wreck);
				}
			}

			gameContext.ProcessTurn();
			Console.WriteLine("WAIT");
			Console.WriteLine("WAIT");
		}
	}
}

public class GameContext
{
	public int Turn { get; set; }
	public List<Reaper> MyReapers { get; set; }
	public List<Reaper> EnemyReapers { get; set; }
	public List<Wreck> Wrecks { get; set; }

	public void ProcessTurn()
	{
		var targetWreck = this.Wrecks.OrderByDescending(w => w.WaterAmount).First();
		this.MyReapers.First().Throttle(targetWreck.Position, 100);
	}
}

public struct Position
{
	private readonly double x;
	private readonly double y;

	public double X { get { return x; } }
	public double Y { get { return y; } }

	public Position(double x, double y)
	{
		this.x = x;
		this.y = y;
	}

	public override bool Equals(Object other)
	{
		return other is Position && Equals((Position) other);
	}

	public bool Equals(Position other)
	{
		return X == other.X && Y == other.Y;
	}

	public override int GetHashCode()
	{
		return X.GetHashCode() ^ Y.GetHashCode();
	}

	public override string ToString()
	{
		return string.Format("{0},{1}", X, Y);
	}
}

public class Reaper
{
	public int Id { get; set; }

	public int PlayerId { get; set; }

	public float Mass { get; set; }

	public int Radius { get; set; }

	public Position Position { get; set; }

	public void Throttle(Position position, int throttle)
	{
		Console.WriteLine($"{position.X} {position.Y} {throttle}");
	}

	public void Wait()
	{
		Console.WriteLine("WAIT");
	}
}

public class Wreck
{
	public int Id { get; set; }

	public int Radius { get; set; }

	public Position Position { get; set; }

	public int WaterAmount { get; set; }
}

public static class Constants
{
	public const int Radius = 6000;
	public const int WaterPerTurn = 1;
	public const int WinWaterAmount = 50;
	public const int ReaperRadius = 400;
	public const double ReaperMass = 0.5;
	public const double ReaperFriction = 0.2;
	public const int ReaperTypeId = 0;
	public const int WreckTypeId = 4;
	public const int MaxThrottle = 300;
}