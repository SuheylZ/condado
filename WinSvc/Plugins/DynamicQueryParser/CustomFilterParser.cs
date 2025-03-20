using System;
using System.Collections.Generic;
using System.Linq;



/// <summary>
/// This is a class representation of a Custom Filter formula. Taken a literal string (or collection of characters), it return the bool of said valid expression.
/// </summary>
public class CustomFilterParser
{

    #region Static Members

    /// <summary>
    /// Constant for left association symbols
    /// </summary>
    private static readonly int LEFT_ASSOC = 0;

    /// <summary>
    /// Constant for right association symbols
    /// </summary>
    private static readonly int RIGHT_ASSOC = 1;

    /// <summary>
    /// Static list of operators in the formula
    /// </summary>
    private static Dictionary<String, int[]> OPERATORS = new Dictionary<string, int[]>();

    public List<string> listOperands = new List<string>();
    /// <summary>
    /// Static constructor.
    /// </summary>
    static CustomFilterParser()
    {
        OPERATORS.Add("AND", new int[] { 0, LEFT_ASSOC });
        OPERATORS.Add("OR", new int[] { 5, LEFT_ASSOC });
        OPERATORS.Add("NOT", new int[] { 10, RIGHT_ASSOC });        
    }

    /// <summary>
    /// Static method to check if a token is an operator.
    /// </summary>
    /// <param name="token">The token we want to check.</param>
    /// <returns>True if it is an operator, else false.</returns>
    private static bool isOperator(String token)
    {
        return OPERATORS.ContainsKey(token);
    }

    /// <summary>
    /// Static method to check if the type of operation is associative (left or right).
    /// </summary>
    /// <param name="token">The token operator.</param>
    /// <param name="type">The type of association (left or right).</param>
    /// <returns>True if it's associative, else false.</returns>
    private static bool isAssociative(String token, int type)
    {
        if (!isOperator(token))
            throw new ArgumentException("Invalid token: " + token);

        if (OPERATORS[token][1] == type)
            return true;

        return false;
    }

    /// <summary>
    /// Static method to compare operator precendece.
    /// </summary>
    /// <param name="token1">First operator.</param>
    /// <param name="token2">Second operator.</param>
    /// <returns>The value of precedence between the two operators.</returns>
    private static int comparePrecedence(String token1, String token2)
    {
        if (!isOperator(token1) || !isOperator(token2))
            throw new ArgumentException("Invalid token: " + token1 + " " + token2);

        return OPERATORS[token1][0] - OPERATORS[token2][0];
    }

    /// <summary>
    /// Static method to transfor a normal formula into an RPN formula.
    /// </summary>
    /// <param name="input">The normal infix formula.</param>
    /// <returns>The RPN formula.</returns>
    public String InfixToRPN(String input)
    {
        for (int i = 0; i < input.Length; i++)
        {
            if (isOperator(input[i].ToString()) || input[i] == '(' || input[i] == ')')
            {
                if (i != 0 && input[i - 1] != ' ')
                    input = input.Insert(i, " ");
                if (i != input.Length - 1 && input[i + 1] != ' ')
                    input = input.Insert(i + 1, " ");
            }
        }

        input = input.Trim();

        String[] inputTokens = input.Split(' ');
        List<string> outList = new List<string>();
        Stack<string> stack = new Stack<string>();

        foreach (string token in inputTokens)
        {
            if (token == " " || token == String.Empty)
                continue;

            if (isOperator(token))
            {
                while (stack.Count != 0 && isOperator(stack.Peek()))
                {
                    if ((isAssociative(token, LEFT_ASSOC) && comparePrecedence(token, stack.Peek()) <= 0) ||
                        (isAssociative(token, RIGHT_ASSOC) && comparePrecedence(token, stack.Peek()) < 0))
                    {
                        outList.Add(stack.Pop());
                        continue;
                    }
                    break;
                }
                stack.Push(token);
            }
            else if (token == "(")
            {
                stack.Push(token);
            }
            else if (token == ")")
            {
                while (stack.Count != 0 && stack.Peek() != "(")
                    outList.Add(stack.Pop());
                stack.Pop();
            }
            else
            {
                outList.Add(token);
            }
        }

        while (stack.Count != 0)
            outList.Add(stack.Pop());
        foreach (string item in outList)
        {
            if (!isOperator(item))
            {
                listOperands.Add(item);
            }
        }
        return String.Join(" ", outList.ToArray());
    }

    /// <summary>
    /// Static method to handle a single operation between two operands.
    /// </summary>
    /// <param name="val1">First operand.</param>
    /// <param name="val2">Second operand.</param>
    /// <param name="OP">Operator.</param>
    /// <returns>The result of the operation.</returns>
    private static float DoOperation(float val1, float val2, string OP)
    {
        switch (OP)
        {
            case "AND":
                return val1 + val2;
            case "OR":
                return val1 + val2;
            case "NOT":
                return val1 + val2;
            default:
                return 0;
        }
    }


    #endregion

    /// <summary>
    /// Environment for all the variables in the formula.
    /// </summary>
    private Dictionary<string, float> environment = new Dictionary<string, float>();

    /// <summary>
    /// Written formula in normal form.
    /// </summary>
    private string formula;

    /// <summary>
    /// Written formula in Reverse Polish Notation form.
    /// </summary>
    private string rpnFormula;

    /// <summary>
    /// Value of a parameter in the environment.
    /// </summary>
    /// <param name="param">Parameter name.</param>
    /// <returns>Value of the parameter.</returns>
    public float this[string param]
    {
        get
        {
            return environment[param];
        }
        set
        {
            environment[param] = value;
        }
    }

    /// <summary>
    /// Value of the formula.
    /// </summary>
    public float Value
    {
        get
        {
            return ParseInput();
        }
    }



    /// <summary>
    /// Method use to ParseInput the value of the formula.
    /// </summary>
    /// <returns>The floating point value of the formula.</returns>
    public float ParseInput()
    {
        String[] tokens = rpnFormula.Split(' ');
        Stack<float> values = new Stack<float>();
           
        foreach (string token in tokens)
        {
            if (!OPERATORS.ContainsKey(token))
            {
                float value;
                if (float.TryParse(token, out value))
                    values.Push(float.Parse(token, System.Globalization.CultureInfo.InvariantCulture));
                else
                    values.Push(environment[token]);
            }
            else
            {
                float val1 = values.Pop();
                float val2 = values.Pop();
                values.Push(DoOperation(val2, val1, token));
            }
        }

        if (values.Count != 1)
            throw new InvalidOperationException("Cannot ParseInput formula.");

        return values.Pop();
    }

    public Stack<CalculationOperands> ParseInputResult()
    {
        String[] tokens = rpnFormula.Split(' ');
        Stack<float> values = new Stack<float>();
        Stack<CalculationOperands> listOperandsWithOperators = new Stack<CalculationOperands>();  
        foreach (string token in tokens)
        {
            if (!OPERATORS.ContainsKey(token))
            {
                float value;
                if (float.TryParse(token, out value))
                    values.Push(float.Parse(token, System.Globalization.CultureInfo.InvariantCulture));
                else
                    values.Push(environment[token]);
            }
            else
            {
                float val1 = values.Pop();
                float val2 = values.Pop();
                values.Push(DoOperation(val2, val1, token));
                CalculationOperands nlistOperandsWithOperators = new CalculationOperands();
                nlistOperandsWithOperators.Operand1 = val1;
                listOperandsWithOperators.Push(nlistOperandsWithOperators);
                nlistOperandsWithOperators.Operand2 = val2;
                nlistOperandsWithOperators.OperationToken = token;
            }
        }

        if (values.Count != 1)
            throw new InvalidOperationException("Cannot ParseInput formula.");

        return listOperandsWithOperators;
    }

    /// <summary>
    /// String representation of the formula.
    /// </summary>
    public string Formula
    {
        get
        {
            return formula;
        }
        set
        {
            rpnFormula = InfixToRPN(value);
            formula = value;
        }
    }

    /// <summary>
    /// String representation of the formula.
    /// </summary>
    /// <returns>The string formula.</returns>
    public override string ToString()
    {
        return formula;
    }

    /// <summary>
    /// CTor.
    /// </summary>
    /// <param name="formula">Infix Notation of the formula.</param>
    public CustomFilterParser(string formula)
    {
        this.formula = formula;
        rpnFormula = InfixToRPN(formula);
    }

    /// <summary>
    /// Adds a parameter (variable) to the environment.
    /// </summary>
    /// <param name="param">Parameter name.</param>
    /// <param name="value">Paramatere value.</param>
    public void AddParameter(string param, float value)
    {
        environment.Add(param, value);
    }

    /// <summary>
    /// Checks if the environment contains the passed parameter.
    /// </summary>
    /// <param name="param">Name of the parameter.</param>
    /// <returns>True if it contains the parameter, else false.</returns>
    public bool ContainsParameter(string param)
    {
        return environment.ContainsKey(param);
    }
}

public class CalculationOperands
{
    public float Operand1 { get; set; }
    public float Operand2 { get; set; }
    public string OperationToken { get; set; }
}