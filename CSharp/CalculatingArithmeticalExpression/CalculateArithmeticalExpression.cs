using System;
using System.Collections.Generic;
using System.Threading;
using System.Globalization;

class CalculateArithmeticalExpression
{
    public struct PredefinedFunction
    {
        public int parametersCount;
        public bool isLeftAssociated;
        public Function.Calculate calculateFunction;
        public int priority;
        public PredefinedFunction(int parametersCount, bool isLeftAssociated, Function.Calculate calculateFunction, int priority)
            : this()
        {
            this.parametersCount = parametersCount;
            this.isLeftAssociated = isLeftAssociated;
            this.calculateFunction = calculateFunction;
            this.priority = priority;
        }
    }

    public class Function
    {
        public delegate double Calculate(double[] parameters);        

        private double[] parameters;
        private string functionName;
        private static Dictionary<string, PredefinedFunction> functionsLibrary;

        static Function()
        {
            functionsLibrary = new Dictionary<string, PredefinedFunction>();
            functionsLibrary.Add("+", new PredefinedFunction(2, true, new Calculate(x => x[0] + x[1]), 1));
            functionsLibrary.Add("-", new PredefinedFunction(2, true, new Calculate(x => x[0] - x[1]), 1));
            functionsLibrary.Add("*", new PredefinedFunction(2, true, new Calculate(x => x[0] * x[1]), 2));
            functionsLibrary.Add("/", new PredefinedFunction(2, true, new Calculate(x => x[0] / x[1]), 2));
            functionsLibrary.Add("^", new PredefinedFunction(2, false, new Calculate(x => Math.Pow(x[0], x[1])), 3));
            functionsLibrary.Add("pow", new PredefinedFunction(2, true, new Calculate(x => Math.Pow(x[0], x[1])), 4));
            functionsLibrary.Add("ln", new PredefinedFunction(1, true, new Calculate(x => Math.Log(x[0])), 4));
            functionsLibrary.Add("sqrt", new PredefinedFunction(1, true, new Calculate(x => Math.Sqrt(x[0])), 4));
        }

        public Function(string functionName)
        {
            this.functionName = functionName;
            this.parameters = new double[functionsLibrary[functionName].parametersCount];
        }

        public static Dictionary<string, PredefinedFunction> Library
        {
            get
            {
                return functionsLibrary;
            }
        }

        public string FunctionName
        {
            get
            {
                return this.functionName;
            }
        }

        public double this[int index]
        {
            get
            {
                return this.parameters[index];
            }
            set
            {
                this.parameters[index] = value;
            }
        }

        public int ParameterCount
        {
            get
            {
                return functionsLibrary[functionName].parametersCount;
            }
        }

        public int Precedence
        {
            get
            {
                return functionsLibrary[functionName].priority;
            }
        }

        public bool IsLeftAssociated
        {
            get
            {
                return functionsLibrary[functionName].isLeftAssociated;
            }
        }

        public double Result
        {
            get
            {
                return functionsLibrary[this.functionName].calculateFunction(this.parameters);
            }
        }        
    }

    public static Stack<string> RPN;

    static void Main()
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        PrintTestInput("2 + 3 * 5 / ( 4 - 5 ) - 1");        
        PrintTestInput("2 + 3 * 5 + ( 4 * pow ( ( sqrt ( 4 + ln ( 1 ) ) - 1 ) , 5 ) - 5 ) - 1");

        PrintTestInput("2 + 3 * 5 + ( 4 * pow ( ( sqrt ( 4 ) ) , 5 ) - 5 ) - 1");
        PrintTestInput("2 + 3 * 5 + ( 4 * ( sqrt ( 4 ) ^ 5 ) - 5 ) - 1");
        PrintTestInput("2 + 3 * 5 + ( 4 * sqrt ( 4 ) ^ 5 - 5 ) - 1");
        PrintTestInput("2 + 3 * 5 + ( 2 ^ 2 * sqrt ( 4 ) ^ 5 - 5 ) - 1");
        PrintTestInput("2 + 3 * 5 + ( 2 ^ 2 * sqrt ( 4 ) ^ 2 ^ 2 * 2 - 5 ) - 1");
    }

    public static void PrintTestInput(string input)
    {
        Console.WriteLine("At the begining we have the input expression:\n{0}", input);
        RPN = GenerateRNP(input);
        Console.WriteLine("First we generate its Polish Notation equivalent:");
        PrintRNP();
        Console.WriteLine();
        RPN = GenerateRNP(input);
        Console.WriteLine("Finally the calculated result: {0}", CalculateRPN());
        Console.WriteLine();
    }

    public static void PrintFunctionLibrary()
    {
        foreach (string name in Function.Library.Keys)
        {
            Console.WriteLine("function {0}, priority {1}", name, Function.Library[name].priority);
        }
    }

    public static Stack<string> GenerateRNP(string expression)
    {
        Stack<string> output = new Stack<string>();
        Stack<string> operators = new Stack<string>();
        string[] tokens = expression.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < tokens.Length; i++)
        {
            ShuntingYard(tokens[i], output, operators);
        }
        while (operators.Count > 0)
        {
            output.Push(operators.Pop());
        }
        return output;
    }

    public static void ShuntingYard(string token, Stack<string> output, Stack<string> operators)
    {
        #region Number
        double tempNumber;
        if(double.TryParse(token, out tempNumber))
        {
            output.Push(token);
            return;
        }
        #endregion Number

        #region Function or Operator
        if (Function.Library.ContainsKey(token))
        {
            if (Function.Library[token].priority == 4)
            {
                operators.Push(token);
            }
            else
            {
                while (IsThereHigherOperatorWaiting(token, operators))
                {
                    output.Push(operators.Pop());
                }
                operators.Push(token);
            }
            return;
        }
        #endregion Function or Operator

        #region Parenthesis
        if (token == "(")
        {
            operators.Push(token);
            return;
        }
        if (token == ")")
        {
            GoToPreviousLeftParenthesisAndRemoveThem(output, operators);
            return;
        }
        #endregion Parenthesis

        #region Comma
        if (token == ",")
        {
            GoToPreviousLeftParenthesis(output, operators);
            return;
        }
        #endregion Comma
    }

    private static void GoToPreviousLeftParenthesis(Stack<string> output, Stack<string> operators)
    {
        while (operators.Count > 0)
        {
            if (operators.Peek() == "(")
            {
                return;
            }
            output.Push(operators.Pop());
        }
    }

    private static void GoToPreviousLeftParenthesisAndRemoveThem(Stack<string> output, Stack<string> operators)
    {
        while (operators.Count > 0)
        {
            if (operators.Peek() == "(")
            {
                operators.Pop();
                if (operators.Count > 0 && Function.Library.ContainsKey(operators.Peek()))
                {
                    if (Function.Library[operators.Peek()].priority == 4)
                    {
                        output.Push(operators.Pop());
                    }
                }
                return;
            }
            output.Push(operators.Pop());
        }
    }

    private static bool IsThereHigherOperatorWaiting(string token, Stack<string> operators)
    {
        if (operators.Count == 0) return false;
        if(Function.Library.ContainsKey(operators.Peek()) && Function.Library[operators.Peek()].priority < 4)
        {
            if (Function.Library[token].isLeftAssociated)
            {
                if (Function.Library[token].priority <= Function.Library[operators.Peek()].priority)
                {
                    return true;
                }
            }
            else
            {
                if (Function.Library[token].priority < Function.Library[operators.Peek()].priority)
                {
                    //Console.WriteLine("No left associatives in this homework!");
                    return true;
                }
            }
        }
        return false;
    }

    public static void GenerateRNP()
    {
        RPN = new Stack<string>();
        RPN.Push("2");
        RPN.Push("3");
        RPN.Push("pow");
        RPN.Push("2.5");
        RPN.Push("/");
    }

    public static void PrintRNP()
    {
        while (RPN.Count > 0)
        {
            Console.Write("{0} ", RPN.Pop());
        }
    }

    public static double CalculateRPN()
    {
        return ParseNumberOrFunction();
    }

    private static double ParseNumberOrFunction()
    {
        double result = 0;
        if (double.TryParse(RPN.Peek(), out result))
        {
            RPN.Pop();
            return result;
        }

        Function function = new Function(RPN.Pop());
        if (function.IsLeftAssociated)
        {
            for (int i = function.ParameterCount - 1; i >= 0; i--)
            {
                function[i] = ParseNumberOrFunction();
            }
        }
        else
        {
            for (int i = function.ParameterCount - 1; i >= 0; i--)
            {
                function[i] = ParseNumberOrFunction();
            }            
        }

        result = function.Result;
        return result;
    }
}
