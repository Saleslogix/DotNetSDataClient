// Copyright (c) 1997-2014, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;
using Remotion.Linq.Parsing.Structure;
using Saleslogix.SData.Client.Content;

namespace Saleslogix.SData.Client.Linq
{
    internal class PropertyPathExtractionVisitor : ThrowingExpressionTreeVisitor
    {
        private readonly INodeTypeProvider _nodeTypeProvider;
        private readonly bool _selectMode;
        private readonly bool _includeProtocolProps;
        private readonly INamingScheme _namingScheme;
        private readonly IList<string> _parts = new List<string>();
        private readonly IList<IList<string>> _paths = new List<IList<string>>();
        private bool _convertedToSimple;

        public static IList<string> ExtractPaths(Expression expression, INodeTypeProvider nodeTypeProvider, bool selectMode, bool includeProtocolProps, INamingScheme namingScheme, string separator)
        {
            var paths = ExtractPaths(expression, nodeTypeProvider, selectMode, includeProtocolProps, namingScheme);
            if (selectMode)
            {
                var lookup = paths.ToLookup(path => path[0] == "*");
                var dupes = lookup[true].SelectMany(wildcard => lookup[false].Select(path => new {wildcard, path}))
                    .Where(pair => pair.wildcard.Skip(1).SequenceEqual(pair.path.Skip(1)))
                    .Select(pair => pair.path)
                    .ToList();
                paths = paths.Except(dupes).ToList();
            }
            return paths
                .Select(path => string.Join(separator, path.Reverse().ToArray()))
                .Distinct()
                .ToList();
        }

        private static IList<IList<string>> ExtractPaths(Expression expression, INodeTypeProvider nodeTypeProvider, bool selectMode, bool includeProtocolProps, INamingScheme namingScheme)
        {
            var visitor = new PropertyPathExtractionVisitor(nodeTypeProvider, selectMode, includeProtocolProps, namingScheme);
            visitor.VisitExpression(expression);
            return visitor._paths;
        }

        private PropertyPathExtractionVisitor(INodeTypeProvider nodeTypeProvider, bool selectMode, bool includeProtocolProps, INamingScheme namingScheme)
        {
            _nodeTypeProvider = nodeTypeProvider;
            _selectMode = selectMode;
            _includeProtocolProps = includeProtocolProps;
            _namingScheme = namingScheme;
        }

        protected override Expression VisitMemberExpression(MemberExpression expression)
        {
            if (expression.Member is PropertyInfo || expression.Member is FieldInfo)
            {
                if (_selectMode && _parts.Count == 0 && ContentHelper.IsObject(expression.Type))
                {
                    _parts.Add("*");
                }
                _parts.Add(_namingScheme.GetName(expression.Member));
                return BaseVisitMemberExpression(expression);
            }

            return base.VisitMemberExpression(expression);
        }

        protected override Expression VisitMethodCallExpression(MethodCallExpression expression)
        {
            if (!expression.Method.IsStatic && expression.Method.IsSpecialName && expression.Method.Name == "get_Item" && expression.Arguments.Count == 1)
            {
                var arg = expression.Arguments[0] as ConstantExpression;
                if (arg != null && arg.Type == typeof (string))
                {
                    if (_selectMode && _parts.Count == 0 && !_convertedToSimple)
                    {
                        _parts.Add("*");
                    }
                    _parts.Add((string) arg.Value);
                    VisitExpression(expression.Object);
                    return expression;
                }
            }
            else if (_nodeTypeProvider.IsRegistered(expression.Method))
            {
                Expression source;
                IList<Expression> args;
                if (expression.Object != null)
                {
                    source = expression.Object;
                    args = expression.Arguments;
                }
                else
                {
                    source = expression.Arguments[0];
                    args = expression.Arguments.Skip(1).ToList();
                }

                var sourcePaths = ExtractPaths(source, _nodeTypeProvider, _selectMode, _includeProtocolProps, _namingScheme);
                var argPaths = args.SelectMany(arg => ExtractPaths(arg, _nodeTypeProvider, _selectMode, _includeProtocolProps, _namingScheme)).ToList();

                foreach (var sourcePath in sourcePaths)
                {
                    var isPassthrough = expression.Method.Name != "Select" &&
                                        expression.Method.Name != "SelectMany" &&
                                        (expression.Type.IsAssignableFrom(source.Type) ||
                                         typeof (IEnumerable<>).MakeGenericType(expression.Type).IsAssignableFrom(source.Type));
                    if (isPassthrough)
                    {
                        _paths.Add(sourcePath.ToList());
                    }
                    if (sourcePath[0] == "*")
                    {
                        sourcePath.RemoveAt(0);
                    }
                    if (!isPassthrough && argPaths.Count == 0)
                    {
                        _paths.Add(sourcePath.ToList());
                    }
                    foreach (var argPath in argPaths)
                    {
                        _paths.Add(argPath.Concat(sourcePath).ToList());
                    }
                }

                return expression;
            }

            return base.VisitMethodCallExpression(expression);
        }

        protected override Expression VisitUnaryExpression(UnaryExpression expression)
        {
            if (expression.NodeType == ExpressionType.Convert)
            {
                _convertedToSimple = !ContentHelper.IsObject(expression.Type);
                try
                {
                    return BaseVisitUnaryExpression(expression);
                }
                finally
                {
                    _convertedToSimple = false;
                }
            }

            return base.VisitUnaryExpression(expression);
        }

        protected override Expression VisitLambdaExpression(LambdaExpression expression)
        {
            VisitExpression(expression.Body);
            return expression;
        }

        protected override Expression VisitQuerySourceReferenceExpression(QuerySourceReferenceExpression expression)
        {
            CompletePath();
            return BaseVisitQuerySourceReferenceExpression(expression);
        }

        protected override Expression VisitParameterExpression(ParameterExpression expression)
        {
            CompletePath();
            return BaseVisitParameterExpression(expression);
        }

        private void CompletePath()
        {
            if (_parts.Count > 0)
            {
                if (!_includeProtocolProps && _parts[0].StartsWith("$"))
                {
                    _parts.RemoveAt(0);
                }

                if (_parts.Count > 0)
                {
                    _paths.Add(_parts.ToList());
                    _parts.Clear();
                }
            }
            else if (_selectMode)
            {
                _paths.Add(new[] {"*"});
            }
        }

        protected override Expression VisitNewExpression(NewExpression expression)
        {
            return BaseVisitNewExpression(expression);
        }

        protected override TResult VisitUnhandledItem<TItem, TResult>(TItem unhandledItem, string visitMethod, Func<TItem, TResult> baseBehavior)
        {
            _parts.Clear();
            return _selectMode
                ? baseBehavior(unhandledItem)
                : base.VisitUnhandledItem(unhandledItem, visitMethod, baseBehavior);
        }

        protected override Exception CreateUnhandledItemException<T>(T unhandledItem, string visitMethod)
        {
            throw new NotSupportedException(string.Format("Expression type '{0}' not supported", typeof (T)));
        }
    }
}