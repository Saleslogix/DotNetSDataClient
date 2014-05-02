// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;
using Saleslogix.SData.Client.Framework;

namespace Saleslogix.SData.Client.Linq
{
    internal class SDataExpressionBuilderVisitor : ThrowingExpressionTreeVisitor
    {
        private static readonly IDictionary<ExpressionType, string> _binaryOperators =
            new Dictionary<ExpressionType, string>
                {
                    //Multiplicative
                    {ExpressionType.Multiply, "mul"},
                    {ExpressionType.Divide, "div"},
                    {ExpressionType.Modulo, "mod"},
                    //Additive
                    {ExpressionType.Add, "+"},
                    {ExpressionType.Subtract, "-"},
                    //Comparison
                    {ExpressionType.Equal, "eq"},
                    {ExpressionType.NotEqual, "ne"},
                    {ExpressionType.LessThan, "lt"},
                    {ExpressionType.LessThanOrEqual, "le"},
                    {ExpressionType.GreaterThan, "gt"},
                    {ExpressionType.GreaterThanOrEqual, "ge"},
                    //Logical
                    {ExpressionType.AndAlso, "and"},
                    {ExpressionType.OrElse, "or"}
                };

        private static readonly IDictionary<MethodMappingKey, Func<MethodCallExpression, IEnumerable>> _methodMappings =
            new Dictionary<MethodMappingKey, Func<MethodCallExpression, IEnumerable>>
                {
                    //Operators
                    {new MethodMappingKey(typeof (SDataOperatorExtensions), "Between"), RenderBetweenOperator},
                    {new MethodMappingKey(typeof (SDataOperatorExtensions), "In"), RenderInOperator},
                    {new MethodMappingKey(typeof (SDataOperatorExtensions), "Like"), RenderLikeOperator},
                    {new MethodMappingKey(typeof (string), "StartsWith"), RenderStartsWithOperator},
                    {new MethodMappingKey(typeof (string), "EndsWith"), RenderEndsWithOperator},
                    {new MethodMappingKey(typeof (string), "Contains"), RenderContainsOperator},
                    //String Functions
                    {new MethodMappingKey(typeof (string), "Concat"), RenderConcatFunction},
                    {new MethodMappingKey(typeof (SDataFunctionExtensions), "Left"), RenderLeftFunction},
                    {new MethodMappingKey(typeof (SDataFunctionExtensions), "Right"), RenderRightFunction},
                    {new MethodMappingKey(typeof (string), "Substring"), RenderSubstringFunction},
                    {new MethodMappingKey(typeof (string), "ToLower"), RenderLowerFunction},
                    {new MethodMappingKey(typeof (string), "ToUpper"), RenderUpperFunction},
                    {new MethodMappingKey(typeof (string), "Replace"), RenderReplaceFunction},
                    {new MethodMappingKey(typeof (string), "IndexOf"), RenderIndexOfFunction},
                    {new MethodMappingKey(typeof (string), "PadLeft"), RenderPadLeftFunction},
                    {new MethodMappingKey(typeof (string), "PadRight"), RenderPadRightFunction},
                    {new MethodMappingKey(typeof (string), "Trim"), RenderTrimFunction},
                    {new MethodMappingKey(typeof (SDataFunctionExtensions), "Ascii"), RenderAsciiFunction},
                    {new MethodMappingKey(typeof (SDataFunctionExtensions), "Char"), RenderCharFunction},
                    //Numeric Functions
                    {new MethodMappingKey(typeof (Math), "Abs"), RenderAbsFunction},
                    {new MethodMappingKey(typeof (Math), "Sign"), RenderSignFunction},
                    {new MethodMappingKey(typeof (Math), "Round"), RenderRoundFunction},
#if !SILVERLIGHT
                    {new MethodMappingKey(typeof (Math), "Truncate"), RenderTruncateFunction},
#endif
                    {new MethodMappingKey(typeof (Math), "Floor"), RenderFloorFunction},
                    {new MethodMappingKey(typeof (Math), "Ceiling"), RenderCeilingFunction},
                    {new MethodMappingKey(typeof (Math), "Pow"), RenderPowFunction},
                    //Date Functions
                    {new MethodMappingKey(typeof (DateTime), "AddDays"), RenderDateAddFunction},
                    {new MethodMappingKey(typeof (DateTimeOffset), "AddDays"), RenderDateAddFunction},
                    {new MethodMappingKey(typeof (DateTime), "AddMilliseconds"), RenderTimestampAddFunction},
                    {new MethodMappingKey(typeof (DateTimeOffset), "AddMilliseconds"), RenderTimestampAddFunction}
                };

        public static string BuildExpression(Expression expression, INamingScheme namingScheme = null)
        {
            var visitor = new SDataExpressionBuilderVisitor(namingScheme ?? NamingScheme.Default);
            visitor.VisitExpression(expression);
            return visitor._builder.ToString();
        }

        private readonly INamingScheme _namingScheme;
        private readonly StringBuilder _builder = new StringBuilder();

        private SDataExpressionBuilderVisitor(INamingScheme namingScheme)
        {
            _namingScheme = namingScheme;
        }

        protected override Expression VisitBinaryExpression(BinaryExpression expression)
        {
            string op;
            if (!_binaryOperators.TryGetValue(expression.NodeType, out op))
            {
                throw new NotSupportedException(string.Format("Binary operator '{0}' not supported", expression.NodeType));
            }

            Append("(", expression.Left, " ", op, " ", expression.Right, ")");
            return expression;
        }

        protected override Expression VisitConstantExpression(ConstantExpression expression)
        {
            Append(SDataUri.FormatConstant(expression.Value));
            return expression;
        }

        protected override Expression VisitMethodCallExpression(MethodCallExpression expression)
        {
            var method = expression.Method;
            Func<MethodCallExpression, IEnumerable> handler;

            if (method.IsSpecialName && method.Name == "get_Item" && expression.Arguments.Count == 1)
            {
                var arg = expression.Arguments[0];
                if (arg.NodeType == ExpressionType.Constant && arg.Type == typeof (string))
                {
                    if (!(expression.Object is QuerySourceReferenceExpression ||
                          expression.Object is ParameterExpression))
                    {
                        Append(expression.Object, ".");
                    }
                    Append((string) ((ConstantExpression) arg).Value);
                    return expression;
                }
            }

            if (!_methodMappings.TryGetValue(new MethodMappingKey(method.DeclaringType, method.Name), out handler))
            {
                throw new NotSupportedException(string.Format("Method '{0}.{1}' not supported", method.DeclaringType, method.Name));
            }

            Append(handler(expression));
            return expression;
        }

        protected override Expression VisitMemberExpression(MemberExpression expression)
        {
            if (expression.Member.DeclaringType == typeof (string) ||
                expression.Member.DeclaringType == typeof (DateTime) ||
                expression.Member.DeclaringType == typeof (DateTimeOffset))
            {
                switch (expression.Member.Name)
                {
                    case "Length":
                    case "Year":
                    case "Month":
                    case "Day":
                    case "Hour":
                    case "Minute":
                    case "Second":
                    case "Millisecond":
                        Append(expression.Member.Name.ToLowerInvariant(), "(", expression.Expression, ")");
                        return expression;
                }
            }

            if (expression.Member.DeclaringType == typeof (TimeSpan))
            {
                var memberExpr = expression.Expression as MemberExpression;
                if (memberExpr != null && memberExpr.Member.DeclaringType == typeof (DateTimeOffset) && memberExpr.Member.Name == "Offset")
                {
                    if (expression.Member.Name == "Hours")
                    {
                        Append("tzHour(", memberExpr.Expression, ")");
                        return expression;
                    }
                    if (expression.Member.Name == "Minutes")
                    {
                        Append("tzMinute(", memberExpr.Expression, ")");
                        return expression;
                    }
                }
            }

            if (!(expression.Expression is QuerySourceReferenceExpression ||
                  expression.Expression is ParameterExpression))
            {
                Append(expression.Expression, ".");
            }

            Append(_namingScheme.GetName(expression.Member));
            return expression;
        }

        protected override Expression VisitUnaryExpression(UnaryExpression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Not:
                    Append("not(", expression.Operand, ")");
                    return expression;
                case ExpressionType.Negate:
                    Append("-(", expression.Operand, ")");
                    return expression;
                case ExpressionType.Convert:
                    return BaseVisitUnaryExpression(expression);
                default:
                    throw new NotSupportedException(string.Format("Unary expression type '{0}' not supported", expression.NodeType));
            }
        }

        protected override Exception CreateUnhandledItemException<T>(T unhandledItem, string visitMethod)
        {
            throw new NotSupportedException(string.Format("Expression type '{0}' not supported", typeof (T)));
        }

        private void Append(params object[] parts)
        {
            Append((IEnumerable) parts);
        }

        private void Append(IEnumerable parts)
        {
            foreach (var part in parts)
            {
                var expr = part as Expression;
                if (expr != null)
                {
                    VisitExpression(expr);
                }
                else
                {
                    _builder.Append(part);
                }
            }
        }

        private static IEnumerable RenderBetweenOperator(MethodCallExpression expression)
        {
            return new object[]
                       {
                           "(",
                           expression.Arguments[0],
                           " between ",
                           expression.Arguments[1],
                           " and ",
                           expression.Arguments[2],
                           ")"
                       };
        }

        private static IEnumerable RenderInOperator(MethodCallExpression expression)
        {
            return new object[]
                       {
                           "(",
                           expression.Arguments[0],
                           " in ",
                           expression.Arguments[1],
                           ")"
                       };
        }

        private static IEnumerable RenderLikeOperator(MethodCallExpression expression)
        {
            return new object[]
                       {
                           "(",
                           expression.Arguments[0],
                           " like ",
                           expression.Arguments[1],
                           ")"
                       };
        }

        private static IEnumerable RenderStartsWithOperator(MethodCallExpression expression)
        {
            var argExpr = expression.Arguments[0] as ConstantExpression;
            if (argExpr == null)
            {
                throw new NotSupportedException("StartsWith must be passed a literal string");
            }

            return new object[]
                       {
                           "(",
                           expression.Object,
                           " like ",
                           SDataUri.FormatConstant(argExpr.Value + "%"),
                           ")"
                       };
        }

        private static IEnumerable RenderEndsWithOperator(MethodCallExpression expression)
        {
            var argExpr = expression.Arguments[0] as ConstantExpression;
            if (argExpr == null)
            {
                throw new NotSupportedException("EndsWith must be passed a literal string");
            }

            return new object[]
                       {
                           "(",
                           expression.Object,
                           " like ",
                           SDataUri.FormatConstant("%" + argExpr.Value),
                           ")"
                       };
        }

        private static IEnumerable RenderContainsOperator(MethodCallExpression expression)
        {
            var argExpr = expression.Arguments[0] as ConstantExpression;
            if (argExpr == null)
            {
                throw new NotSupportedException("Contains must be passed a literal string");
            }

            return new object[]
                       {
                           "(",
                           expression.Object,
                           " like ",
                           SDataUri.FormatConstant("%" + argExpr.Value + "%"),
                           ")"
                       };
        }

        private static IEnumerable RenderConcatFunction(MethodCallExpression expression)
        {
            var args = expression.Arguments;
            if (args.Count == 1 && expression.Arguments[0].Type.IsArray)
            {
                var newArrayExpr = args[0] as NewArrayExpression;
                if (newArrayExpr != null)
                {
                    args = newArrayExpr.Expressions;
                }
            }

            yield return "concat(";
            var first = true;
            foreach (var expr in args)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    yield return ",";
                }
                yield return expr;
            }
            yield return ")";
        }

        private static IEnumerable RenderLeftFunction(MethodCallExpression expression)
        {
            return new object[]
                       {
                           "left(",
                           expression.Arguments[0],
                           ",",
                           expression.Arguments[1],
                           ")"
                       };
        }

        private static IEnumerable RenderRightFunction(MethodCallExpression expression)
        {
            return new object[]
                       {
                           "right(",
                           expression.Arguments[0],
                           ",",
                           expression.Arguments[1],
                           ")"
                       };
        }

        private static IEnumerable RenderSubstringFunction(MethodCallExpression expression)
        {
            var parts = new List<object>
                            {
                                "substring(",
                                expression.Object,
                                ","
                            };

            var argExpr = expression.Arguments[0] as ConstantExpression;
            if (argExpr != null)
            {
                parts.Add(((int) argExpr.Value) + 1);
            }
            else
            {
                parts.Add(expression.Arguments[0]);
                parts.Add("+1");
            }

            parts.Add(",");

            if (expression.Arguments.Count > 1)
            {
                parts.Add(expression.Arguments[1]);
            }
            else
            {
                parts.Add("len(");
                parts.Add(expression.Object);
                parts.Add(")-");
                parts.Add(expression.Arguments[0]);
            }

            parts.Add(")");
            return parts;
        }

        private static IEnumerable RenderLowerFunction(MethodCallExpression expression)
        {
            return new object[]
                       {
                           "lower(",
                           expression.Object,
                           ")"
                       };
        }

        private static IEnumerable RenderUpperFunction(MethodCallExpression expression)
        {
            return new object[]
                       {
                           "upper(",
                           expression.Object,
                           ")"
                       };
        }

        private static IEnumerable RenderReplaceFunction(MethodCallExpression expression)
        {
            return new object[]
                       {
                           "replace(",
                           expression.Object,
                           ",",
                           expression.Arguments[0],
                           ",",
                           expression.Arguments[1],
                           ")"
                       };
        }

        private static IEnumerable RenderIndexOfFunction(MethodCallExpression expression)
        {
            if (expression.Arguments.Count != 1)
            {
                throw new NotSupportedException("Only the single parameter String.IndexOf overload is supported");
            }

            return new object[]
                       {
                           "(locate(",
                           expression.Arguments[0],
                           ",",
                           expression.Object,
                           ")-1)"
                       };
        }

        private static IEnumerable RenderPadLeftFunction(MethodCallExpression expression)
        {
            var parts = new List<object>
                            {
                                "lpad(",
                                expression.Object,
                                ",",
                                expression.Arguments[0]
                            };

            if (expression.Arguments.Count > 1)
            {
                parts.Add(",");
                parts.Add(expression.Arguments[1]);
            }

            parts.Add(")");
            return parts;
        }

        private static IEnumerable RenderPadRightFunction(MethodCallExpression expression)
        {
            var parts = new List<object>
                            {
                                "rpad(",
                                expression.Object,
                                ",",
                                expression.Arguments[0]
                            };

            if (expression.Arguments.Count > 1)
            {
                parts.Add(",");
                parts.Add(expression.Arguments[1]);
            }

            parts.Add(")");
            return parts;
        }

        private static IEnumerable RenderTrimFunction(MethodCallExpression expression)
        {
            if (expression.Arguments.Count > 0)
            {
                throw new NotSupportedException("Only the parameterless String.Trim overload is supported");
            }

            return new object[]
                       {
                           "trim(",
                           expression.Object,
                           ")"
                       };
        }

        private static IEnumerable RenderAsciiFunction(MethodCallExpression expression)
        {
            return new object[]
                       {
                           "ascii(",
                           expression.Arguments[0],
                           ")"
                       };
        }

        private static IEnumerable RenderCharFunction(MethodCallExpression expression)
        {
            return new object[]
                       {
                           "char(",
                           expression.Arguments[0],
                           ")"
                       };
        }

        private static IEnumerable RenderAbsFunction(MethodCallExpression expression)
        {
            return new object[]
                       {
                           "abs(",
                           expression.Arguments[0],
                           ")"
                       };
        }

        private static IEnumerable RenderSignFunction(MethodCallExpression expression)
        {
            return new object[]
                       {
                           "sign(",
                           expression.Arguments[0],
                           ")"
                       };
        }

        private static IEnumerable RenderRoundFunction(MethodCallExpression expression)
        {
            var parts = new List<object>
                            {
                                "round(",
                                expression.Arguments[0]
                            };

            if (expression.Arguments.Count > 1)
            {
                parts.Add(",");
                parts.Add(expression.Arguments[1]);
            }

            parts.Add(")");
            return parts;
        }

#if !SILVERLIGHT
        private static IEnumerable RenderTruncateFunction(MethodCallExpression expression)
        {
            return new object[]
                       {
                           "trunc(",
                           expression.Arguments[0],
                           ")"
                       };
        }
#endif

        private static IEnumerable RenderFloorFunction(MethodCallExpression expression)
        {
            return new object[]
                       {
                           "floor(",
                           expression.Arguments[0],
                           ")"
                       };
        }

        private static IEnumerable RenderCeilingFunction(MethodCallExpression expression)
        {
            return new object[]
                       {
                           "ceil(",
                           expression.Arguments[0],
                           ")"
                       };
        }

        private static IEnumerable RenderPowFunction(MethodCallExpression expression)
        {
            return new object[]
                       {
                           "pow(",
                           expression.Arguments[0],
                           ",",
                           expression.Arguments[1],
                           ")"
                       };
        }

        private static IEnumerable RenderDateAddFunction(MethodCallExpression expression)
        {
            var argExpr = expression.Arguments[0] as ConstantExpression;
            var value = argExpr != null ? (double) argExpr.Value : (double?) null;
            return new[]
                       {
                           "date",
                           value < 0 ? "Sub" : "Add",
                           "(",
                           expression.Object,
                           ",",
                           value < 0 ? (object) -value : expression.Arguments[0],
                           ")"
                       };
        }

        private static IEnumerable RenderTimestampAddFunction(MethodCallExpression expression)
        {
            var argExpr = expression.Arguments[0] as ConstantExpression;
            var value = argExpr != null ? (double) argExpr.Value : (double?) null;
            return new[]
                       {
                           "timestamp",
                           value < 0 ? "Sub" : "Add",
                           "(",
                           expression.Object,
                           ",",
                           value < 0 ? (object) -value : expression.Arguments[0],
                           ")"
                       };
        }

        #region Nested type: MethodMappingKey

        private class MethodMappingKey
        {
            private readonly Type _declaringType;
            private readonly string _methodName;

            public MethodMappingKey(Type declaringType, string methodName)
            {
                _declaringType = declaringType;
                _methodName = methodName;
            }

            public override bool Equals(object obj)
            {
                var other = obj as MethodMappingKey;
                return ReferenceEquals(this, obj) ||
                       (other != null &&
                        _declaringType == other._declaringType &&
                        _methodName == other._methodName);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (_declaringType != null ? _declaringType.GetHashCode() : 0)*397 ^
                           (_methodName != null ? _methodName.GetHashCode() : 0);
                }
            }
        }

        #endregion
    }
}