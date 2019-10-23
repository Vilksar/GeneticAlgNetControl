using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneticAlgNetControl.Helpers.ViewModels
{
    public class SolutionViewModel
    {
        public List<string> CompleteSolution { get; set; }

        public List<string> UniqueControlNodes { get; set; }

        public List<string> UniquePreferredNodes { get; set; }
    }
}
