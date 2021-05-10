using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp.NEAT
{
    /// <summary>
    /// Simple representation of an organism as it's stored within the species controller.
    /// </summary>
    public struct OrganismStruct
    {
        public int Id;
        public double Fitness;

        public OrganismStruct(int id, double fitness)
        {
            Id = id;
            Fitness = fitness;
        }

        public override bool Equals(object obj)
        {
            return obj is OrganismStruct other &&
                   Id == other.Id &&
                   Fitness == other.Fitness;
        }

        public override string ToString()
        {
            return $"({Id}) {Math.Round(Fitness,2)}..";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Fitness);
        }

        public void Deconstruct(out int id, out double fitness)
        {
            id = Id;
            fitness = Fitness;
        }

        public static implicit operator int(OrganismStruct item)
        {
            return item.Id;
        }

        public static implicit operator OrganismStruct((int Id, double Fitness) value)
        {
            return new OrganismStruct(value.Id, value.Fitness);
        }

        public static implicit operator (int Id, double Fitness)(OrganismStruct item)
        {
            return (item.Id, item.Fitness);
        }

        public static bool operator ==(OrganismStruct left, OrganismStruct right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(OrganismStruct left, OrganismStruct right)
        {
            return !(left == right);
        }
    }
}
