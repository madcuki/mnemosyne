using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mnemosyne
{
    class ExceptionMessage
    {
        protected string Context;
        protected string Problem;
        protected string Solution;

        public ExceptionMessage(string context, string problem, string solution)
        {
            Context = context;
            Problem = problem;
            Solution = solution;
        }

        public override string ToString()
        {
            return  String.Format("\n\nContext: {0}\nProblem: {1}\nSolution: {2}\n", Context, Problem, Solution);
        }
    }
}
