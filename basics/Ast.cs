using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ast
{
    public class Script
    {
        public Environment globalEnv;
        public List<Function> functions;
    }

    public class Environment
    {
        public List<Variable> variables;
    }

    public class Statement { }

    public class Expression { }

    public class IntLiteral : Expression
    {
        public int value;
    }

    public class DeclareVariable : Statement
    {
        public Identifier name;
        public Expression value;
    }

    public class Identifier
    {
        public string name;
    }

    public class Variable
    {
        public Identifier identifier { get; set; }
        public object value { get; set; }
    }

    public class DoubleVar : Variable
    {
        public new double value;
    }

    public class ArrayVar : Variable
    {
        public new List<double> value;
    }

    public class Function
    {
        public string name;
        public string signature;
        public List<object[]> statements;
    }

    public class Sequence : Statement
    {
        public Statement first;
        public Sequence rest;
    }
}
