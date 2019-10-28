using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.RPG_System.Scripts.Combat.CalcSpeak
{
    public class Expression
    {

        private int scanIndex = 0;
        protected string expressionText = string.Empty;

        private StringBuilder sb = new StringBuilder();

        public bool isValid { get; private set; }

        public string LastError { get; private set; }

        private List<float> values = new List<float>();
        private List<char> operators = new List<char>();

        private Dictionary<string, float> constantValues;

        private void LogError(string message)
        {
            LastError = message;
            sb.AppendLine(message);
            scanIndex = expressionText.Length;
            isValid = false;
        }

        public Expression(string expression, Dictionary<string, float> constantValues)
        {
            isValid = true;
            expressionText = expression.Trim().ToUpper() + " ";     // Tack on the space at the end to ensure token read termination.
            this.constantValues = constantValues;
        }

        enum ScanState
        {
            AwaitingExpression,
            AwaitingOperator
        }

        public void Parse()
        {
            ScanState currentState = ScanState.AwaitingExpression;
            while (scanIndex < expressionText.Length)
            {
                currentState = (values.Count == operators.Count ? ScanState.AwaitingExpression : ScanState.AwaitingOperator);

                char c = expressionText[scanIndex];

                switch (currentState)
                {
                    case ScanState.AwaitingExpression:
                        if (char.IsDigit(c) || c == '.')
                        {
                            values.Add(ReadImmediateValue());
                        }
                        else if (c == '-')
                        {
                            values.Add(-1);
                            operators.Add('*');
                            sb.AppendLine("Unary negation of following expression");
                            scanIndex++;
                            break;
                        }
                        else if (c == '(')
                        {
                            int depth = 1, startingIndex = scanIndex;
                            while (depth > 0 && scanIndex < expressionText.Length - 1)
                            {
                                scanIndex++;
                                if (expressionText[scanIndex] == '(') depth++;
                                if (expressionText[scanIndex] == ')') depth--;
                            }
                            if (depth == 0)
                            {
                                string subExpressionText = expressionText.Substring(startingIndex + 1, scanIndex - startingIndex - 1);
                                sb.AppendLine("Subexpression found: " + subExpressionText);
                                Expression subExpression = new Expression(subExpressionText, constantValues);
                                subExpression.Parse();
                                if (subExpression.isValid)
                                {
                                    values.Add(subExpression.Evaluate());
                                    scanIndex++;
                                }
                                else
                                {
                                    LogError(subExpression.GetLastAnalysis());
                                }
                            }
                            else
                            {
                                LogError("Unclosed parenthesis at position " + startingIndex);
                            }
                            break;
                        }
                        else if (char.IsLetter(c))
                        {
                            values.Add(ReadVariableValue());
                        }
                        else if (c != ' ')
                        {
                            LogError("Parse error - an expression was expected but the invalid character " + c + " was encountered at position " + scanIndex);
                        }
                        else
                        {
                            scanIndex++;
                        }
                        break;
                    case ScanState.AwaitingOperator:
                        if (!" +-/*^".Contains(c))
                        {
                            LogError("Parse error - an operator was expected but the invalid character " + c + " was encountered at position " + scanIndex);
                        }
                        else if (c != ' ')
                        {
                            operators.Add(c);
                            scanIndex++;
                            sb.AppendLine("Operator " + c + " found");
                        }
                        else
                        {
                            scanIndex++;
                        }
                        break;
                }
            }
            if (values.Count > 0 && operators.Count == values.Count)
            {
                LogError("Parse error - expression terminates in an operator, expression expected");
            }
        }

        public float Evaluate()
        {
            if (!isValid) return 0;
            if (values.Count == 0) return 0;

            // E MD AS
            for (int i = 0; i < operators.Count; i++)
            {
                if (operators[i] == '^')
                {
                    // 1 + 5
                    // value 0 = 1, value 1 = 5
                    // operator 0 = +
                    // 
                    // Binary operators are:
                    // value[opIndex] operators[opIndex] value[opIndex + 1]
                    //
                    // Operator is removed
                    // value[opIndex + 1] is removed
                    // value [opIndex] becomes calculated value

                    float calculatedValue = (float)Math.Pow(values[i], values[i + 1]);
                    values.RemoveAt(i + 1);
                    values[i] = calculatedValue;
                    operators.RemoveAt(i--);
                }
            }

            for (int i = 0; i < operators.Count; i++)
            {
                if (operators[i] == '*')
                {
                    float calculatedValue = values[i] * values[i + 1];
                    values.RemoveAt(i + 1);
                    values[i] = calculatedValue;
                    operators.RemoveAt(i--);
                }
                else if (operators[i] == '/')
                {
                    float calculatedValue = values[i] / values[i + 1];
                    values.RemoveAt(i + 1);
                    values[i] = calculatedValue;
                    operators.RemoveAt(i--);
                }
            }

            for (int i = 0; i < operators.Count; i++)
            {
                if (operators[i] == '+')
                {
                    float calculatedValue = values[i] + values[i + 1];
                    values.RemoveAt(i + 1);
                    values[i] = calculatedValue;
                    operators.RemoveAt(i--);
                }
                else if (operators[i] == '-')
                {
                    float calculatedValue = values[i] - values[i + 1];
                    values.RemoveAt(i + 1);
                    values[i] = calculatedValue;
                    operators.RemoveAt(i--);
                }
            }

            return values[0];
        }

        public string GetLastAnalysis()
        {
            sb.AppendLine();
            sb.AppendLine("Final evaluation: " + Evaluate());
            return sb.ToString();
        }

        public bool IsTerminatingToken(char c)
        {
            if (" +-/*^".Contains(c.ToString())) return true;
            return false;
        }

        protected float ReadVariableValue()
        {
            // Read characters and underscore; anything else is invalid

            string variableName = string.Empty;
            while (scanIndex < expressionText.Length)
            {
                char c = expressionText[scanIndex];
                if (char.IsLetter(c) || c == '_')
                {
                    variableName += c;
                    scanIndex++;
                }
                else
                {
                    break;
                }
            }

            // Found the end, try to find it
            if (constantValues.ContainsKey(variableName))
            {
                return constantValues[variableName];
            }
            else
            {
                LogError("Invalid variable " + variableName + " encountered");
                return 0f;
            }


        }

        protected float ReadImmediateValue()
        {
            string readValue = string.Empty;
            bool decimalFound = false;
            int startPosition = scanIndex;
            while (scanIndex < expressionText.Length)
            {
                char c = expressionText[scanIndex];
                if (IsTerminatingToken(c))
                {
                    if (readValue.Last() == '.')
                    {
                        LogError("Decimal termination error in immediate value starting at position " + startPosition + ": " + readValue);
                    }
                    else
                    {
                        sb.AppendLine("Valid immediate value read at position " + startPosition + ": " + readValue);
                        scanIndex++;
                    }
                    return float.Parse(readValue);
                }

                if (char.IsDigit(c))
                {
                    readValue += c;
                }
                else
                {
                    if (c == '.')
                    {
                        if (decimalFound)
                        {
                            LogError("Double decimal error in immediate value starting at position " + startPosition + ": " + readValue + ".");
                            return float.Parse(readValue);
                        }
                        else
                        {
                            decimalFound = true;
                            readValue += c;
                        }
                    }
                    else
                    {
                        // Not a terminating token, not a digit, not a decimal - so not valid
                        LogError("Parse error after immediate value starting at position " + startPosition + ": " + readValue + c);
                        return float.Parse(readValue);
                    }
                }

                scanIndex++;

            }
            sb.AppendLine("Read value " + readValue);
            return float.Parse(readValue);
        }
    }
}
