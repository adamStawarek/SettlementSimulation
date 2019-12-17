using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using SettlementSimulation.Engine.Interfaces;

namespace SettlementSimulation.Engine.Helpers
{
    public class RuleDistributor : IRuleDistributor
    {
        public IRule GetRule<T>() where T : IRule
        {
            return (IRule)Assembly.Load("SettlementSimulation.Engine")
                .GetTypes()?
                .Where(t => t == typeof(T))
                .Select(Activator.CreateInstance)
                .FirstOrDefault();
        }
    }
}
