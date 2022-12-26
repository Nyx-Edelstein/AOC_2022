namespace AOC_2022.Puzzles
{
    internal class Day21
    {
        public static double SolutionA(string input)
        {
            var data = Parse(input);
            var monkeyDict = data.ToDictionary(x => x.Name);
            var rootMonkey = monkeyDict["root"];

            while (!rootMonkey.Expression.CanBeSolved())
            {
                data = data.Where(x => !x.Expression.CanBeSolved()).ToArray();
                foreach (var m in data)
                    m.TrySimplify(monkeyDict);
            }

            var result = ((ValueExpression)rootMonkey.Expression).Value;
            return result;
        }

        public static double SolutionB(string input)
        {
            var data = Parse(input);
            var monkeyDict = data.ToDictionary(x => x.Name);
            var rootMonkey = monkeyDict["root"];
            
            var humn = monkeyDict["humn"];
            humn.Expression = new VariableExpression("X");
            ((OperationExpression)rootMonkey.Expression).Op = "=";

            while (!rootMonkey.Expression.CanBeSolved())
            {
                data = data.Where(x => !x.Expression.CanBeSolved()).ToArray();
                foreach (var m in data)
                    m.TrySimplify(monkeyDict);
            }

            var rootExpression = (OperationExpression)rootMonkey.Expression;
            var result = SolveExpression(rootExpression);
            return result;
        }

        private static MonkeyExpression[] Parse(string input) =>
            input.Split("\r\n", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Split(" ", StringSplitOptions.RemoveEmptyEntries))
                .Select(x => new MonkeyExpression
                {
                    Name = x[0][..^1],
                    Expression = x.Length == 2
                        ? new ValueExpression(double.Parse(x[1]))
                        : new OperationExpression(x[1], x[3], x[2])
                }).ToArray();

        private static double SolveExpression(OperationExpression expr) => expr.LHS is ValueExpression
            ? SolveExpression(((ValueExpression)expr.LHS).Value, expr.RHS)
            : SolveExpression(((ValueExpression)expr.RHS).Value, expr.LHS);

        private static double SolveExpression(double value, Expression toSolve)
        {
            while (true)
            {
                if (toSolve is VariableExpression) return value;

                var expr = (OperationExpression)toSolve;
                if (expr.LHS is ValueExpression lhsValExpr)
                {
                    var lhsVal = lhsValExpr.Value;
                    if (expr.Op == "+")
                        value = value - lhsVal;
                    else if (expr.Op == "-")
                        value = lhsVal - value;
                    else if (expr.Op == "*")
                        value = value / lhsVal;
                    else // "/'
                        value = lhsVal / value;

                    toSolve = expr.RHS;
                    continue;
                }

                if (expr.RHS is ValueExpression rhsValExpr)
                {
                    var rhsVal = rhsValExpr.Value;
                    if (expr.Op == "+")
                        value = value - rhsVal;
                    else if (expr.Op == "-")
                        value = value + rhsVal;
                    else if (expr.Op == "*")
                        value = value / rhsVal;
                    else // "/"
                        value = value * rhsVal;

                    toSolve = expr.LHS;
                    continue;
                }

                throw new Exception("Should be unreachable...");
            }
        }

        private class MonkeyExpression
        {
            internal string Name;
            internal Expression Expression;

            internal void TrySimplify(Dictionary<string, MonkeyExpression> monkeyDict)
            {
                TryReplaceReferences(monkeyDict);
                TrySimplifyToValue();
            }

            private void TryReplaceReferences(Dictionary<string, MonkeyExpression> monkeyDict)
            {
                if (Expression is not OperationExpression opExpr) return;

                void TryReplace(ref Expression expr)
                {
                    if (expr is not ReferenceExpression reference) return;

                    var refMonkey = monkeyDict[reference.Name];
                    if (refMonkey.Expression.CanBeSolved())
                        expr = refMonkey.Expression;
                }

                TryReplace(ref opExpr.LHS);
                TryReplace(ref opExpr.RHS);
            }

            private void TrySimplifyToValue()
            {
                if (Expression is not OperationExpression { LHS: ValueExpression lhs, RHS: ValueExpression rhs } expr) return;
                
                var val1 = lhs.Value;
                var val2 = rhs.Value;
                var op = expr.Op;

                var result = op switch
                {
                    "+" => val1 + val2,
                    "-" => val1 - val2,
                    "*" => val1 * val2,
                    "/" => val1 / val2,
                };

                Expression = new ValueExpression(result);
            }

            public override string ToString() => $"{Name}: {Expression}";
        }

        private abstract class Expression
        {
            internal abstract bool CanBeSolved();
        }

        private class ValueExpression : Expression
        {
            internal readonly double Value;
            internal ValueExpression(double value) => Value = value;
            public override string ToString() => Value.ToString();
            internal override bool CanBeSolved() => true;
        }

        private class VariableExpression : Expression
        {
            internal readonly string Variable;
            internal VariableExpression(string variable) => Variable = variable;
            public override string ToString() => Variable;
            internal override bool CanBeSolved() => true;
        }

        private class ReferenceExpression : Expression
        {
            internal readonly string Name;
            internal ReferenceExpression(string name) => Name = name;
            public override string ToString() => Name;
            internal override bool CanBeSolved() => false;
        }

        private class OperationExpression : Expression
        {
            internal Expression LHS;
            internal Expression RHS;
            internal string Op;

            internal OperationExpression(string ref1, string ref2, string op)
            {
                LHS = new ReferenceExpression(ref1);
                RHS = new ReferenceExpression(ref2);
                Op = op;
            }

            public override string ToString() => $"({LHS} {Op} {RHS})";
            internal override bool CanBeSolved() => LHS.CanBeSolved() && RHS.CanBeSolved();
        }
    }
}
