// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 
using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Parsing.Structure.NodeTypeProviders;
using Remotion.Linq.Utilities;

namespace Remotion.Linq.Parsing.Structure.IntermediateModel
{
  /// <summary>
  /// Represents a <see cref="MethodCallExpression"/> for <see cref="Queryable.Contains{TSource}(System.Linq.IQueryable{TSource},TSource)"/> and
  /// <see cref="Enumerable.Contains{TSource}(System.Collections.Generic.IEnumerable{TSource},TSource)"/>. 
  /// It is generated by <see cref="ExpressionTreeParser"/> when an <see cref="Expression"/> tree is parsed.
  /// When this node is used, it marks the beginning (i.e. the last node) of an <see cref="IExpressionNode"/> chain that represents a query.
  /// </summary>
  internal class ContainsExpressionNode : ResultOperatorExpressionNodeBase
  {
    public static readonly MethodInfo[] SupportedMethods = new[]
        {
          GetSupportedMethod (() => Queryable.Contains<object> (null, null)),
          GetSupportedMethod (() => Enumerable.Contains<object>(null, null))
        };

    public static readonly NameBasedRegistrationInfo[] SupportedMethodNames = new[]
        {
          // Note: We only detect supported/unsupported types heuristically - real interface implementation checks (via GetInterfaceMap) would be
          // too costly.
          new NameBasedRegistrationInfo (
              "Contains",
              mi => mi.DeclaringType != typeof (string) 
                  && typeof (IEnumerable).GetTypeInfo().IsAssignableFrom (mi.DeclaringType.GetTypeInfo())
                  && !typeof (IDictionary).GetTypeInfo().IsAssignableFrom (mi.DeclaringType.GetTypeInfo())
                  && (mi.IsStatic && mi.GetParameters().Length == 2 || !mi.IsStatic && mi.GetParameters().Length == 1))
        };

    public ContainsExpressionNode (MethodCallExpressionParseInfo parseInfo, Expression item)
        : base(parseInfo, null, null)
    {
      ArgumentUtility.CheckNotNull ("item", item);
      Item = item;
    }

    public Expression Item { get; private set; }


    public override Expression Resolve (ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
    {
      // no data streams out from this node, so we cannot resolve any expressions
      throw CreateResolveNotSupportedException ();
    }

    protected override ResultOperatorBase CreateResultOperator (ClauseGenerationContext clauseGenerationContext)
    {
      return new ContainsResultOperator(Item);
    }
  }
}
