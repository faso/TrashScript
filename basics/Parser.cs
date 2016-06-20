using System.Collections.Generic;
using System.IO;
using System.Text;
using basics;
using static Enums;
using static HelperExtensions;
using System;
using Ast;
using System.Linq;

public sealed class Parser
{
    private readonly List<object> tokens;
    private Ast.Environment globalEnv;

    public Parser(List<object> input)
    {
        this.tokens = input;
        this.Parse();
    }

    public void Parse()
    {
        var script = new Script();
        //script.globalEnv = GetGlobalEnv(tokens);

        var functionTokens = SeparateFunctions();
        var boop = ParseFunctions(functionTokens);
        globalEnv = new Ast.Environment();
        globalEnv.variables = new List<Variable>();
        InterpretFunction(boop, "main");
    }

    public void InterpretFunction(List<Function> functions, string name)
    {
        var func = functions.SingleOrDefault(o => o.name == name);

        var currentEnv = new Ast.Environment();
        currentEnv.variables = new List<Variable>();

        foreach (var stat in func.statements)
        {
            if (stat[0].Equals(Keywords.Var))
            {
                if (!(stat[1] is Identifier))
                    throw new FormatException("Expected an identifier after var", null);

                if (!(stat[2].Equals(ArithmeticOperators.Equal)))
                    throw new FormatException("Expected = after identifier", null);

                if ((stat[3] is int))
                    currentEnv.variables.Add(new DoubleVar() { identifier = stat[1] as Identifier, value = (int)stat[3] });
                else if (stat[3].Equals(Parentheses.OpenBr))
                {
                    List<Variable> arr = new List<Variable>();
                    var i = 3;
                    while (!stat[i].Equals(Parentheses.CloseBr))
                        i++;
                    var content = stat.Skip(4).Take(i - 4).Where(o => !o.Equals(Special.Comma)).Select(o => Convert.ToDouble(o));
                    currentEnv.variables.Add(new ArrayVar() { identifier = stat[1] as Identifier, value = content.ToList() });
                }
            }
            else if (stat[0] is Identifier && stat[1].Equals(ArithmeticOperators.Equal))
            {
                if (!(stat[2] is int))
                    throw new FormatException("Expected an integer after =", null);

                currentEnv.variables.FirstOrDefault(o => o.identifier.name == (stat[0] as Identifier).name).value = (int)stat[2];
            }
            else if (stat[0] is Identifier && (stat[1].Equals(Parentheses.OpenPar)))
            {
                if(!(stat[1].Equals(Parentheses.OpenPar)))
                    throw new FormatException("Invocation parentheses not closed", null);

                InterpretFunction(functions, (stat[0] as Identifier).name);
            }
            else if (stat[0].Equals(Keywords.Print))
            {
                if (stat[1] is Identifier)
                    Console.WriteLine($"Function {name} printed: {currentEnv.variables.FirstOrDefault(o => o.identifier.name == (stat[1] as Identifier).name).value}\n");

                if (stat[1] is int)
                    Console.WriteLine((int)stat[1]);
            }
        }

        Console.WriteLine($"Function {name}, ended. Environment: ");
        foreach (var variable in currentEnv.variables)
        {
            Console.WriteLine($"    {variable.identifier.name} = {variable.value}");
        }
        Console.WriteLine();
    }

    private List<List<object>> SeparateFunctions()
    {
        var result = new List<List<object>>();
        var currentFunction = new List<object>();
        foreach (var token in this.tokens)
        {
            currentFunction.Add(token);
            if (token.Equals(Block.CloseBr))
            {
                result.Add(currentFunction);
                currentFunction = new List<object>();
            }
        }
        return result;
    }

    private List<Function> ParseFunctions(List<List<object>> functions)
    {
        var result = new List<Function>();
        foreach (var func in functions)
        {
            var curFunc = new Function();
            var tokens = func.ToArray();

            int pos = 0;
            pos++;
            curFunc.name = (tokens[pos] as Identifier).name; // read the name
            pos += 3; //skipping () {
            curFunc.signature = "";
            curFunc.statements = SeparateStatements(tokens.SubArray(pos, tokens.Length - pos));
            result.Add(curFunc);
        }
        return result;
    }

    private List<object[]> SeparateStatements(object[] statementTokens)
    {
        var result = new List<object[]>();
        var currentFunction = new List<object>();
        foreach (var token in statementTokens)
        {
            if (token.Equals(Block.CloseBr))
                break;
            if (token.Equals(Block.OpenBr))
                continue;
            currentFunction.Add(token);
            if (token.Equals(Block.Semicolon))
            {
                result.Add(currentFunction.ToArray());
                currentFunction = new List<object>();
            }
        }
        return result;
    }

    //public Environment GetGlobalEnv(List<object>)
    //{
    //    var result = new Environment();
    //    result.variables = new List<Variable>();

    //    while()
    //}

}
