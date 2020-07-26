using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLAgent.Criteria
{
    public enum LogicalOperators
    {
        AND,
        OR,
        NOT,
        OpenParenthesis,
        CloseParenthesis,
        Unknown
    }
}
