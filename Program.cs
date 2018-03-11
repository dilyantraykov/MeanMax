using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

class Player
{
    static void Main(string[] args)
    {
        while (true)
        {
            var gameContext = new GameContext();
            int myScore = int.Parse(Console.ReadLine());
            gameContext.MyScore = myScore;
            int enemyScore1 = int.Parse(Console.ReadLine());
            gameContext.EnemyOneScore = enemyScore1;
            int enemyScore2 = int.Parse(Console.ReadLine());
            gameContext.EnemyTwoScore = enemyScore2;
            int myRage = int.Parse(Console.ReadLine());
            gameContext.MyRage = myRage;
            int enemyRage1 = int.Parse(Console.ReadLine());
            gameContext.EnemyOneRage = enemyRage1;
            int enemyRage2 = int.Parse(Console.ReadLine());
            gameContext.EnemyTwoRage = enemyRage2;
            int unitCount = int.Parse(Console.ReadLine());
            for (int i = 0; i < unitCount; i++)
            {
                string[] inputs = Console.ReadLine().Split(' ');

                var unitDetails = new UnitDetails()
                {
                    UnitId = int.Parse(inputs[0]),
                    UnitType = int.Parse(inputs[1]),
                    PlayerId = int.Parse(inputs[2]),
                    Mass = float.Parse(inputs[3]),
                    Radius = int.Parse(inputs[4]),
                    Position = new Position(int.Parse(inputs[5]), int.Parse(inputs[6])),
                    Speed = new Speed(int.Parse(inputs[7]), int.Parse(inputs[8])),
                    Extra = int.Parse(inputs[9]),
                    WaterCapacity = int.Parse(inputs[10])
                };

                gameContext.InitializeUnit(unitDetails);
            }

            gameContext.ProcessTurn();
        }
    }
}

public struct UnitDetails
{
    public int UnitId { get; internal set; }
    public int UnitType { get; internal set; }
    public int PlayerId { get; internal set; }
    public float Mass { get; internal set; }
    public int Radius { get; internal set; }
    public Position Position { get; internal set; }
    public Speed Speed { get; internal set; }
    public int Extra { get; internal set; }
    public int WaterCapacity { get; internal set; }
}

public class GameContext
{
    private static Random rand = new Random();

    public GameContext()
    {
        this.Wrecks = new List<Wreck>();
        this.Tankers = new List<Tanker>();
        this.MyReapers = new List<Reaper>();
        this.EnemyReapers = new Dictionary<int, Reaper>();
        this.MyDestroyers = new List<Destroyer>();
        this.EnemyDestroyers = new Dictionary<int, Destroyer>();
        this.MyDoofs = new List<Doof>();
        this.EnemyDoofs = new Dictionary<int, Doof>();
        this.TarPools = new List<TarPool>();
        this.OilPools = new List<OilPool>();
    }

    public int Turn { get; set; }

    public int MyScore { get; set; }

    public int EnemyOneScore { get; set; }

    public int EnemyTwoScore { get; set; }

    public int MyRage { get; set; }

    public int EnemyOneRage { get; set; }

    public int EnemyTwoRage { get; set; }

    public List<Reaper> MyReapers { get; set; }

    public Dictionary<int, Reaper> EnemyReapers { get; set; }

    public List<Destroyer> MyDestroyers { get; set; }

    public Dictionary<int, Destroyer> EnemyDestroyers { get; set; }

    public List<Doof> MyDoofs { get; set; }

    public Dictionary<int, Doof> EnemyDoofs { get; set; }

    public List<Wreck> Wrecks { get; set; }
    public List<Tanker> Tankers { get; set; }
    public List<TarPool> TarPools { get; set; }
    public List<OilPool> OilPools { get; set; }

    public void ProcessTurn()
    {
        this.ProcessReapers();
        this.ProcessDestroyers();
        this.ProcessDoofs();
    }

    private void ProcessDoofs()
    {
        var h1 = new PlaceOilOnWreckHandler();
        var h2 = new BlockEnemyReapersHandler();
        h1.SetSuccessor(h2);
        h1.ProcessTurn(this);
    }

    private void ProcessDestroyers()
    {
        var h1 = new ThrowGrenadeHandler();
        var h2 = new MoveToClosestTankerHandler();
        var h3 = new BlockTopPlayersReaperHandler();
        h1.SetSuccessor(h2);
        h2.SetSuccessor(h3);
        h1.ProcessTurn(this);
    }

    private void ProcessReapers()
    {
        var h1 = new HarvestNearbyWreckHandler();
        var h2 = new ThrottleTowardsOptimalWreckHandler();
        var h3 = new FollowClosestDestroyerHandler();
        //h1.SetSuccessor(h2);
        h2.SetSuccessor(h3);
        h2.ProcessTurn(this);
    }

    internal int GetScoreByPlayerId(int id)
    {
        switch (id)
        {
            case 1:
                return EnemyOneScore;
            default:
                return EnemyTwoScore;
        }
    }

    internal void InitializeUnit(UnitDetails unitDetails)
    {
        switch (unitDetails.UnitType)
        {
            case Constants.ReaperTypeId:
                {
                    var reaper = new Reaper(unitDetails);

                    if (reaper.PlayerId == 0)
                    {
                        this.MyReapers.Add(reaper);
                    }
                    else
                    {
                        this.EnemyReapers.Add(reaper.PlayerId, reaper);
                    }

                    break;
                }
            case Constants.DestroyerTypeId:
                {
                    var destroyer = new Destroyer(unitDetails);

                    if (destroyer.PlayerId == 0)
                    {
                        this.MyDestroyers.Add(destroyer);
                    }
                    else
                    {
                        this.EnemyDestroyers.Add(destroyer.PlayerId, destroyer);
                    }

                    break;
                }
            case Constants.DoofTypeId:
                {
                    var doof = new Doof(unitDetails);

                    if (doof.PlayerId == 0)
                    {
                        this.MyDoofs.Add(doof);
                    }
                    else
                    {
                        this.EnemyDoofs.Add(doof.
                            PlayerId, doof);
                    }

                    break;
                }
            case Constants.TankerTypeId:
                {
                    var tanker = new Tanker(unitDetails);
                    this.Tankers.Add(tanker);
                    break;
                }
            case Constants.WreckTypeId:
                {
                    var wreck = new Wreck(unitDetails);
                    this.Wrecks.Add(wreck);
                    break;
                }
            case Constants.TarPoolTypeId:
                {
                    var pool = new TarPool(unitDetails);
                    this.TarPools.Add(pool);
                    break;
                }
            case Constants.OilPoolTypeId:
                {
                    var pool = new OilPool(unitDetails);
                    this.OilPools.Add(pool);
                    break;
                }
            default:
                break;
        }
    }

    internal bool ProcessTargetIfPossible(Looter looter, Unit target)
    {
        if (target != null)
        {
            var throttle = Utils.GetDesiredThrottle(looter, target);
            looter.Throttle(target.Position);
            return true;
        }

        return false;
    }
}

#region ReaperActions
internal class FollowClosestDestroyerHandler : ActionsHandler
{
    public override bool CanProcessTurn(GameContext context)
    {
        var reaper = context.MyReapers.First();
        var destroyers = context.MyDestroyers.Concat(context.EnemyDestroyers.Values);
        var target = Utils.GetClosestUnit(reaper, destroyers);
        return context.ProcessTargetIfPossible(reaper, target);
    }
}

internal class ThrottleTowardsOptimalWreckHandler : ActionsHandler
{
    public override bool CanProcessTurn(GameContext context)
    {
        var reaper = context.MyReapers.First();
        var otherUnits = new List<Unit>();
        otherUnits.AddRange(context.OilPools);
        otherUnits.AddRange(context.EnemyReapers.Values);
        otherUnits.AddRange(context.EnemyDestroyers.Values);
        otherUnits.AddRange(context.EnemyDoofs.Values);
        otherUnits.AddRange(context.MyDestroyers);
        otherUnits.AddRange(context.MyDoofs);
        otherUnits.AddRange(context.Tankers);
        var target = context.Wrecks
            .OrderByDescending(w => Utils.CalculateWreckRoi(reaper, w, otherUnits))
            .ThenBy(w => otherUnits.Where(u => Utils.IsInRange(w, u, w.Radius)).Count())
            .FirstOrDefault();
        return context.ProcessTargetIfPossible(reaper, target);
    }
}

internal class HarvestNearbyWreckHandler : ActionsHandler
{
    public override bool CanProcessTurn(GameContext context)
    {
        var reaper = context.MyReapers.First();
        var target = context.Wrecks
            .Where(w => !context.OilPools.Any(o => Utils.IsInRange(w, o, o.Radius)))
            .FirstOrDefault(w => Utils.IsInRange(w, reaper, Constants.ReaperRadius * 3)); // TODO: Optimize this radius
        return context.ProcessTargetIfPossible(reaper, target);
    }
}
#endregion

#region DestroyerActions
internal class BlockTopPlayersReaperHandler : ActionsHandler
{
    public override bool CanProcessTurn(GameContext context)
    {
        var destroyer = context.MyDestroyers.First();
        var target = context.EnemyReapers.Values.OrderByDescending(r => context.GetScoreByPlayerId(r.PlayerId)).First();
        return context.ProcessTargetIfPossible(destroyer, target);
    }
}

internal class MoveToClosestTankerHandler : ActionsHandler
{
    public override bool CanProcessTurn(GameContext context)
    {
        var destroyer = context.MyDestroyers.First();
        var reaper = context.MyReapers.First();
        Unit target = Utils.GetClosestUnit(reaper, context.Tankers.Where(t => Utils.IsInWaterTown(t)));
        if (target != null)
        {
            destroyer.Throttle(target.Position);
            return true;
        }

        return false;
    }
}

internal class ThrowGrenadeHandler : ActionsHandler
{
    public override bool CanProcessTurn(GameContext context)
    {
        var destroyer = context.MyDestroyers.First();
        var reaper = context.MyReapers.First();
        var hasEnoughRage = context.MyRage >= Constants.NitroGrenadeRageCost;
        var anyEnemyReapersInRange = context.EnemyReapers.Values.Any(r => Utils.IsInRange(destroyer, r, Constants.SkillRadius));
        var areMyReapersInRange = context.MyReapers.Any(r => Utils.IsInRange(destroyer, r, Constants.SkillRange));
        var isMyReaperNearWreck = context.Wrecks.Any(w => Utils.IsInRange(reaper, w, w.Radius * 3));

        if (hasEnoughRage &&
            anyEnemyReapersInRange &&
            !areMyReapersInRange)
        {
            destroyer.Skill(destroyer.Position);
            return true;
        }
        else if (hasEnoughRage &&
            areMyReapersInRange &&
            isMyReaperNearWreck)
        {
            destroyer.Skill(reaper.Position);
            return true;
        }

        return false;
    }
}
#endregion

#region DoofActions
internal class BlockEnemyReapersHandler : ActionsHandler
{
    public override bool CanProcessTurn(GameContext context)
    {
        var doof = context.MyDoofs.First();
        var target = context.EnemyReapers.Values.OrderByDescending(r => context.GetScoreByPlayerId(r.PlayerId)).First();
        doof.Throttle(target.Position);

        return true;
    }
}

public class PlaceOilOnWreckHandler : ActionsHandler
{
    public override bool CanProcessTurn(GameContext context)
    {
        var doof = context.MyDoofs.First();
        foreach (var wreck in context.Wrecks)
        {
            var enemyReaperOnWreck = context.EnemyReapers.Values.Any(r => Utils.IsInRange(r, wreck, wreck.Radius * 2));
            var doesWreckAlreadyHaveOilOver = context.OilPools.Any(o => Utils.IsInRange(wreck, o, wreck.Radius));
            var ownReaperOnWreck = Utils.IsInRange(wreck, context.MyReapers.First(), wreck.Radius * 2);
            if (!ownReaperOnWreck &&
                !doesWreckAlreadyHaveOilOver &&
                context.MyRage >= Constants.OilPoolRageCost &&
                enemyReaperOnWreck &&
                Utils.IsInRange(doof, wreck, Constants.SkillRange))
            {
                doof.Skill(wreck.Position);
                return true;
            }
        }

        return false;
    }
}
#endregion

public abstract class ActionsHandler
{
    protected ActionsHandler successor;

    public void SetSuccessor(ActionsHandler successor)
    {
        this.successor = successor;
    }

    public void ProcessTurn(GameContext context)
    {
        var result = this.CanProcessTurn(context);
        if (result)
        {
            Console.Error.WriteLine(this.GetType().Name);
            return;
        }
        else if (successor != null)
        {
            successor.ProcessTurn(context);
        }
    }

    public abstract bool CanProcessTurn(GameContext context);
}

public static class Utils
{
    public static bool IsInWaterTown(Unit unit)
    {
        return Math.Abs(unit.Position.X) <= Constants.TownRadius &&
            Math.Abs(unit.Position.Y) <= Constants.TownRadius;
    }

    public static bool IsInRange(Position origin, Position target, int range)
    {
        return GetDistance(origin, target) <= range;
    }

    public static bool IsInRange(Unit origin, IEnumerable<Unit> targets, int range)
    {
        return targets.Any(t => IsInRange(origin.Position, t.Position, range));
    }

    public static bool IsInRange(Unit origin, Unit target, int range)
    {
        return IsInRange(origin.Position, target.Position, range);
    }

    public static int CalculateRage(Speed speed)
    {
        var rage = Math.Sqrt(speed.VX * speed.VX + speed.VY * speed.VY) * 100;
        return (int) Math.Floor(rage);
    }

    public static Unit GetClosestUnit(Unit unit, IEnumerable<Unit> units)
    {
        return units?.OrderBy(w => Utils.GetDistance(unit.Position, w.Position)).FirstOrDefault();
    }

    public static double GetDistance(Unit a, Unit b)
    {
        return GetDistance(a.Position, b.Position);
    }

    public static double GetDistance(Position a, Position b)
    {
        var distance = Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
        return distance;
    }

    public static float GetAcceleration(int throttle, float mass)
    {
        var acceleration = throttle / mass;
        return acceleration;
    }

    public static float GetSpeedAfterFriction(float currentSpeed, float friction)
    {
        var speed = currentSpeed * (1 - friction);
        return speed;
    }

    public static int GetDesiredThrottle(Looter looter, Unit target)
    {
        return Constants.MaxThrottle;
    }

    private static int GetSpeedAfterThrottle(Looter looter, int throttle)
    {
        var speed = throttle / looter.Mass;
        return (int) Math.Round(speed * (1 - looter.Friction));
    }

    private static int CalculateUnitsPerTurnWithCurrentSpeed(Speed speed)
    {
        var units = Math.Sqrt(Math.Pow(Math.Abs(speed.VX), 2) + Math.Pow(Math.Abs(speed.VY), 2));
        return (int) units;
    }

    internal static object CalculateWreckRoi(Looter looter, Wreck wreck, IEnumerable<Unit> otherUnits)
    {
        var turnsToWreck = Math.Ceiling(Utils.GetDistance(looter, wreck) / Utils.CalculateUnitsPerTurnWithCurrentSpeed(looter.Speed));
        var unitCount = otherUnits.Where(u => Utils.IsInRange(u, wreck, wreck.Radius * 2)).Count();
        return wreck.WaterAmount - turnsToWreck;
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

public struct Speed
{
    private readonly int vx;
    private readonly int vy;

    public int VX { get { return vx; } }
    public int VY { get { return vy; } }

    public Speed(int vx, int vy)
    {
        this.vx = vx;
        this.vy = vy;
    }
}

public class Unit
{
    public Unit(UnitDetails unitDetails)
    {
        this.Id = unitDetails.UnitId;
        this.Radius = unitDetails.Radius;
        this.Position = unitDetails.Position;
    }

    public int Id { get; set; }

    public int Radius { get; set; }

    public Position Position { get; set; }
}

public class Looter : Unit
{
    public Looter(UnitDetails unitDetails)
        : base(unitDetails)
    {
        this.PlayerId = unitDetails.PlayerId;
        this.Mass = unitDetails.Mass;
        this.Speed = unitDetails.Speed;
    }

    public int PlayerId { get; set; }

    public float Mass { get; set; }

    public float Friction { get; set; }

    public Speed Speed { get; set; }

    public void Throttle(Position position)
    {
        var targetPositionWithCurrentSpeed = new Position(position.X - this.Speed.VX, position.Y - this.Speed.VY);
        Console.WriteLine($"{targetPositionWithCurrentSpeed.X} {targetPositionWithCurrentSpeed.Y} {Constants.MaxThrottle}");
    }

    public void Skill(Position position)
    {
        Console.WriteLine($"SKILL {position.X} {position.Y}");
    }

    public void Wait()
    {
        Console.WriteLine("WAIT");
    }
}

public class Doof : Looter
{
    public Doof(UnitDetails unitDetails)
        : base(unitDetails)
    {
        this.Friction = Constants.DoofFriction;
    }
}

public class Destroyer : Looter
{
    public Destroyer(UnitDetails unitDetails)
        : base(unitDetails)
    {
        this.Friction = Constants.DestroyerFriction;
    }
}

public class Reaper : Looter
{
    public Reaper(UnitDetails unitDetails)
        : base(unitDetails)
    {
        this.Friction = Constants.ReaperFriction;
    }
}

public class Tanker : Looter
{
    public Tanker(UnitDetails unitDetails)
        : base(unitDetails)
    {
        this.WaterAmount = unitDetails.Extra;
        this.WaterCapacity = unitDetails.WaterCapacity;
    }

    public int WaterAmount { get; set; }

    public int WaterCapacity { get; set; }
}

public class Wreck : Unit
{
    public Wreck(UnitDetails unitDetails)
        : base(unitDetails)
    {
        this.WaterAmount = unitDetails.Extra;
    }

    public int WaterAmount { get; set; }
}

public class TarPool : Unit
{
    public TarPool(UnitDetails unitDetails)
        : base(unitDetails)
    {
        this.RemainingDuration = unitDetails.Extra;
    }

    public int RemainingDuration { get; private set; }
}

public class OilPool : Unit
{
    public OilPool(UnitDetails unitDetails)
        : base(unitDetails)
    {
        this.RemainingDuration = unitDetails.Extra;
    }

    public int RemainingDuration { get; private set; }
}

public static class Constants
{
    public const int MapRadius = 6000;
    public const int TownRadius = 3000;
    public const int ReaperRadius = 400;

    public const int WaterPerTurn = 1;
    public const int WinWaterAmount = 50;

    public const float ReaperMass = 0.5f;
    public const float ReaperFriction = 0.2f;
    public const float DestroyerMass = 1.5f;
    public const float DestroyerFriction = 0.3f;
    public const float TankerFriction = 0.4f;
    public const float DoofMass = 1f;
    public const float DoofFriction = 0.25f;

    public const int TankerThrottle = 500;
    public const int MaxThrottle = 300;

    public const int ReaperTypeId = 0;
    public const int DestroyerTypeId = 1;
    public const int DoofTypeId = 2;
    public const int TankerTypeId = 3;
    public const int WreckTypeId = 4;
    public const int TarPoolTypeId = 5;
    public const int OilPoolTypeId = 6;

    public const int SkillRange = 2000;
    public const int SkillRadius = 1000;

    public const int NitroGrenadeRageCost = 60;

    public const int TarPoolMassIncrease = 10;
    public const int TarPoolRageCost = 30;

    public const int OilPoolRageCost = 30;

    public const int MaxRage = 300;
}