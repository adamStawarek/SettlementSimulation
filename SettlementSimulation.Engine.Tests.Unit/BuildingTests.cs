﻿using NUnit.Framework;
using SettlementSimulation.Engine.Enumerators;
using SettlementSimulation.Engine.Models.Buildings;
using SettlementSimulation.Engine.Models.Buildings.FirstType;
using SettlementSimulation.Engine.Models.Buildings.SecondType;
using SettlementSimulation.Engine.Models.Buildings.ThirdType;

namespace SettlementSimulation.Engine.Tests.Unit
{
    public class BuildingTests
    {
        [Test]
        [Retry(10)]
        public void At_First_Epoch_GetRandomGene_Generates_At_Least_One_Residence_In_10_Tries()
        {
            var gene = Building.GetRandom(Epoch.First);
            Assert.IsInstanceOf<Residence>(gene);
        }

        [Test]
        [Retry(30)]
        public void At_First_Epoch_GetRandomGene_Generates_At_Least_One_Tavern_In_30_Tries()
        {
            var gene = Building.GetRandom(Epoch.First);
            Assert.IsInstanceOf<Tavern>(gene);
        }

        [Test]
        [Retry(100)]
        public void At_Second_Epoch_GetRandomGene_Generates_At_Least_One_School_In_100_Tries()
        {
            var gene = Building.GetRandom(Epoch.Second);
            Assert.IsInstanceOf<School>(gene);
        }

        [Test]
        [Retry(200)]
        public void At_Second_Epoch_GetRandomGene_Generates_At_Least_One_Church_In_200_Tries()
        {
            var gene = Building.GetRandom(Epoch.Second);
            Assert.IsInstanceOf<Church>(gene);
        }

        [Test]
        [Retry(200)]
        public void At_Second_Epoch_GetRandomGene_Generates_At_Least_One_AdministrationOffice_In_200_Tries()
        {
            var gene = Building.GetRandom(Epoch.Second);
            Assert.IsInstanceOf<Administration>(gene);
        }

        [Test]
        [Retry(1000)]
        public void At_Third_Epoch_GetRandomGene_Generates_At_Least_One_University_In_1000_Tries()
        {
            var gene = Building.GetRandom(Epoch.Third);
            Assert.IsInstanceOf<University>(gene);
        }

        [Test]
        [Retry(1000)]
        public void At_Third_Epoch_GetRandomGene_Generates_At_Least_One_Port_In_1000_Tries()
        {
            var gene = Building.GetRandom(Epoch.Third);
            Assert.IsInstanceOf<Port>(gene);
        }
    }
}
