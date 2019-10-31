namespace GeneticAlgorithm
{
    public enum Epoch
    {
        First, Second, Third
    }

    public abstract class Building
    {
        public Location Location { get; set; }
        public abstract double Probability { get; }
    }

    public abstract class FirstTypeBuilding : Building
    {
        //list of rules
    }

    public abstract class SecondTypeBuilding : Building
    {
        //list of rules
    }

    public abstract class ThirdTypeBuilding : Building
    {
        //list of rules
    }

    public class EmptyArea : FirstTypeBuilding
    {
        public override double Probability => 0.5;
    }

    public class Residence : FirstTypeBuilding
    {
        public override double Probability => 0.4;
    }

    public class School : SecondTypeBuilding
    {
        public override double Probability => 0.03;
    }

    public class Market : SecondTypeBuilding
    {
        public override double Probability => 0.03;
    }

    public class AdministrationBuilding : SecondTypeBuilding
    {
        public override double Probability => 0.03;
    }

    public class Court : ThirdTypeBuilding
    {
        public override double Probability => 0.005;
    }

    public class University : ThirdTypeBuilding
    {
        public override double Probability => 0.005;
    }


    public class Location
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}