using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static Enums;
using basics;
using Ast;

public static class HelperExtensions
{
    public static bool IsIdentifierLegal(this char c) => (char.IsLetter(c) || c == '_');

    public static T[] SubArray<T>(this T[] data, int index, int length)
    {
        T[] result = new T[length];
        Array.Copy(data, index, result, 0, length);
        return result;
    }
}

public static class Enums
{
    public enum ArithmeticOperators { Plus, Minus, Subtract, Divide, Multiply, Equal };
    public enum Block { OpenBr, CloseBr, Semicolon };
    public enum Parentheses { OpenPar, ClosePar, OpenBr, CloseBr };
    public enum Keywords { Function, Var, Print };
    public enum Special { Comma };
}

public sealed class Lexer
{
    private readonly List<object> result;

    public Lexer(TextReader input)
    {
        this.result = new List<object>();
        this.Scan(input);
    }

    public List<object> Tokens
    {
        get { return this.result; }
    }

    private void Scan(TextReader input)
    {
        while (input.Peek() != -1)
        {
            char ch = (char)input.Peek();

            // Scan individual tokens
            if (char.IsWhiteSpace(ch))
            {
                // eat the current char and skip ahead!
                input.Read();
            }
            else if (ch.IsIdentifierLegal())
            {
                // keyword or identifier

                StringBuilder accum = new StringBuilder();

                while (char.IsLetter(ch) || ch == '_')
                {
                    accum.Append(ch);
                    input.Read();

                    if (input.Peek() == -1)
                    {
                        break;
                    }
                    else
                    {
                        ch = (char)input.Peek();
                    }
                }

                switch(accum.ToString())
                {
                    case "function":
                        input.Read();
                        this.result.Add(Keywords.Function);
                        break;

                    case "var":
                        input.Read();
                        this.result.Add(Keywords.Var);
                        break;

                    case "print":
                        input.Read();
                        this.result.Add(Keywords.Print);
                        break;

                    default:
                        this.result.Add(new Identifier() { name = accum.ToString() });
                        break;
                }
            }
            else if (ch == '"')
            {
                // string literal
                StringBuilder accum = new StringBuilder();

                input.Read(); // skip the '"'

                if (input.Peek() == -1)
                {
                    throw new System.Exception("unterminated string literal");
                }

                while ((ch = (char)input.Peek()) != '"')
                {
                    accum.Append(ch);
                    input.Read();

                    if (input.Peek() == -1)
                    {
                        throw new System.Exception("unterminated string literal");
                    }
                }

                // skip the terminating "
                input.Read();
                this.result.Add(accum);
            }
            else if (char.IsDigit(ch))
            {
                // numeric literal

                StringBuilder accum = new StringBuilder();

                while (char.IsDigit(ch))
                {
                    accum.Append(ch);
                    input.Read();

                    if (input.Peek() == -1)
                    {
                        break;
                    }
                    else
                    {
                        ch = (char)input.Peek();
                    }
                }

                this.result.Add(int.Parse(accum.ToString()));
            }
            else switch (ch)
                {
                    case '+':
                        input.Read();
                        this.result.Add(ArithmeticOperators.Plus);
                        break;

                    case '-':
                        input.Read();
                        this.result.Add(ArithmeticOperators.Subtract);
                        break;

                    case '*':
                        input.Read();
                        this.result.Add(ArithmeticOperators.Multiply);
                        break;

                    case '/':
                        input.Read();
                        this.result.Add(ArithmeticOperators.Divide);
                        break;

                    case '=':
                        input.Read();
                        this.result.Add(ArithmeticOperators.Equal);
                        break;

                    case ';':
                        input.Read();
                        this.result.Add(Block.Semicolon);
                        break;

                    case '{':
                        input.Read();
                        this.result.Add(Block.OpenBr);
                        break;

                    case '}':
                        input.Read();
                        this.result.Add(Block.CloseBr);
                        break;

                    case '(':
                        input.Read();
                        this.result.Add(Parentheses.OpenPar);
                        break;

                    case ')':
                        input.Read();
                        this.result.Add(Parentheses.ClosePar);
                        break;

                    case '[':
                        input.Read();
                        this.result.Add(Parentheses.OpenBr);
                        break;

                    case ']':
                        input.Read();
                        this.result.Add(Parentheses.CloseBr);
                        break;

                    case ',':
                        input.Read();
                        this.result.Add(Special.Comma);
                        break;

                    default:
                        throw new System.Exception("Scanner encountered unrecognized character '" + ch + "'");
                }
        }
    }
}
