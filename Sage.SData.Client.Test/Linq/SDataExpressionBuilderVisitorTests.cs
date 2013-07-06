using System;
using System.Linq.Expressions;
using NUnit.Framework;
using Remotion.Linq.Parsing.ExpressionTreeVisitors;
using Remotion.Linq.Parsing.ExpressionTreeVisitors.Transformation;
using Sage.SData.Client.Linq;
using Sage.SData.Client.Test.Model;

// ReSharper disable InconsistentNaming

namespace Sage.SData.Client.Test.Linq
{
    [TestFixture]
    public class SDataExpressionBuilderVisitorTests
    {
        private static readonly ExpressionTransformerRegistry _tranformationProvider = ExpressionTransformerRegistry.CreateDefault();

        #region Constants

        [Test]
        public void Constant_Null_Test()
        {
            AssertSimple(() => (object) null, "null");
        }

        [Test]
        public void Constant_BooleanTrue_Test()
        {
            AssertSimple(() => true, "true");
        }

        [Test]
        public void Constant_BooleanFalse_Test()
        {
            AssertSimple(() => false, "false");
        }

        [Test]
        public void Constant_String_Test()
        {
            AssertSimple(() => "Hello", "'Hello'");
        }

        [Test]
        public void Constant_String_EscapeQuotes_Test()
        {
            AssertSimple(() => "Ma'am", "'Ma''am'");
        }

        [Test]
        public void Constant_DateTime_Test()
        {
            AssertSimple(() => new DateTime(2001, 2, 3, 4, 5, 6), "@2001-02-03T04:05:06@");
        }

        [Test]
        public void Constant_DateTimeOffset_Test()
        {
            AssertSimple(() => new DateTimeOffset(2001, 2, 3, 4, 5, 6, TimeSpan.FromHours(8)), "@2001-02-03T04:05:06+08:00@");
        }

        [Test]
        public void Constant_TimeSpan_Test()
        {
            AssertSimple(() => new TimeSpan(4, 5, 6), "@04:05:06@");
        }

        [Test]
        public void Constant_Guid_Test()
        {
            AssertSimple(() => new Guid("134068ea-3e53-3037-a010-d98aa9f566bc"), "'134068ea-3e53-3037-a010-d98aa9f566bc'");
        }

        [Test]
        public void Constant_Char_Test()
        {
            AssertSimple(() => 'a', "'a'");
        }

        [Test]
        public void Constant_Enum_Test()
        {
            AssertSimple(() => ContactCivility.Mr, "'Mr'");
        }

        [Test]
        public void Constant_Decimal_Test()
        {
            AssertSimple(() => 3.14M, "3.14");
        }

        [Test]
        public void Constant_Array_Test()
        {
            AssertSimple(() => new[] {"Active", "Retired"}, "('Active','Retired')");
        }

        #endregion

        #region Operators

        [Test]
        public void Operator_Minus_Test()
        {
            AssertObject((SalesOrder o) => -o.SubTotal, "-(SubTotal)");
        }

        [Test]
        public void Operator_Not_Test()
        {
            AssertObject((Contact c) => !c.Active, "not(Active)");
        }

        [Test]
        public void Operator_Multiply_Test()
        {
            AssertObject((SalesOrder o) => o.SubTotal*3.14M, "(SubTotal mul 3.14)");
        }

        [Test]
        public void Operator_Divide_Test()
        {
            AssertObject((SalesOrder o) => o.SubTotal/3.14M, "(SubTotal div 3.14)");
        }

        [Test]
        public void Operator_Modulo_Test()
        {
            AssertObject((SalesOrder o) => o.SubTotal%3.14M, "(SubTotal mod 3.14)");
        }

        [Test]
        public void Operator_Add_Test()
        {
            AssertObject((SalesOrder o) => o.SubTotal + 3.14M, "(SubTotal + 3.14)");
        }

        [Test]
        public void Operator_Subtract_Test()
        {
            AssertObject((SalesOrder o) => o.SubTotal - 3.14M, "(SubTotal - 3.14)");
        }

        [Test]
        public void Operator_Equal_Test()
        {
            AssertObject((Contact c) => c.LastName == "Smith", "(LastName eq 'Smith')");
        }

        [Test]
        public void Operator_NotEqual_Test()
        {
            AssertObject((Contact c) => c.LastName != "Smith", "(LastName ne 'Smith')");
        }

        [Test]
        public void Operator_LessThan_Test()
        {
            AssertObject((SalesOrder o) => o.SubTotal < 3.14M, "(SubTotal lt 3.14)");
        }

        [Test]
        public void Operator_LessThanOrEqual_Test()
        {
            AssertObject((SalesOrder o) => o.SubTotal <= 3.14M, "(SubTotal le 3.14)");
        }

        [Test]
        public void Operator_GreaterThan_Test()
        {
            AssertObject((SalesOrder o) => o.SubTotal > 3.14M, "(SubTotal gt 3.14)");
        }

        [Test]
        public void Operator_GreaterThanOrEqual_Test()
        {
            AssertObject((SalesOrder o) => o.SubTotal >= 3.14M, "(SubTotal ge 3.14)");
        }

        [Test]
        public void Operator_AndAlso_Test()
        {
            AssertObject((Contact c) => (bool) c.Active && (bool) c.Active, "(Active and Active)");
        }

        [Test]
        public void Operator_OrElse_Test()
        {
            AssertObject((Contact c) => (bool) c.Active || (bool) c.Active, "(Active or Active)");
        }

        [Test]
        public void Operator_Between_Test()
        {
            AssertObject((SalesOrder o) => o.SubTotal.Value.Between(30, 40), "(SubTotal between 30 and 40)");
        }

        [Test]
        public void Operator_Between_Nullable_Test()
        {
            AssertObject((SalesOrder o) => o.SubTotal.Between(30, 40), "(SubTotal between 30 and 40)");
        }

        [Test]
        public void Operator_In_Test()
        {
            AssertObject((Contact c) => c.LastName.In("John", "Jane"), "(LastName in ('John','Jane'))");
        }

        [Test]
        public void Operator_In_Nullable_Test()
        {
            AssertObject((Contact c) => c.Civility.In(ContactCivility.Mrs, ContactCivility.Ms), "(Civility in ('Mrs','Ms'))");
        }

        [Test]
        public void Operator_Like_Test()
        {
            AssertObject((Contact c) => c.LastName.Like("S%e"), "(LastName like 'S%e')");
        }

        [Test]
        public void Operator_Like_StartsWith_Test()
        {
            AssertObject((Contact c) => c.LastName.StartsWith("S"), "(LastName like 'S%')");
        }

        [Test]
        public void Operator_Like_EndsWith_Test()
        {
            AssertObject((Contact c) => c.LastName.EndsWith("S"), "(LastName like '%S')");
        }

        [Test]
        public void Operator_Like_Contains_Test()
        {
            AssertObject((Contact c) => c.LastName.Contains("S"), "(LastName like '%S%')");
        }

        #endregion

        #region Functions

        [Test]
        public void Function_Concat_Test()
        {
            AssertObject((Contact c) => string.Concat(c.FirstName, " ", c.LastName), "concat(FirstName,' ',LastName)");
        }

        [Test]
        public void Function_Concat_Array_Test()
        {
            AssertObject((Contact c) => string.Concat(new[] {c.FirstName, " ", c.LastName}), "concat(FirstName,' ',LastName)");
        }

        [Test]
        public void Function_Left_Test()
        {
            AssertObject((Contact c) => c.LastName.Left(3), "left(LastName,3)");
        }

        [Test]
        public void Function_Right_Test()
        {
            AssertObject((Contact c) => c.LastName.Right(3), "right(LastName,3)");
        }

        [Test]
        public void Function_Substring_1_Test()
        {
            AssertObject((SalesOrder o) => o.OrderNumber.Substring(2), "substring(OrderNumber,3,len(OrderNumber)-2)");
        }

        [Test]
        public void Function_Substring_2_Test()
        {
            AssertObject((SalesOrder o) => o.OrderNumber.Substring(o.LineCount.Value), "substring(OrderNumber,LineCount+1,len(OrderNumber)-LineCount)");
        }

        [Test]
        public void Function_Substring_3_Test()
        {
            AssertObject((SalesOrder o) => o.OrderNumber.Substring(2, 4), "substring(OrderNumber,3,4)");
        }

        [Test]
        public void Function_Substring_4_Test()
        {
            AssertObject((SalesOrder o) => o.OrderNumber.Substring(o.LineCount.Value, 4), "substring(OrderNumber,LineCount+1,4)");
        }

        [Test]
        public void Function_Lower_Test()
        {
            AssertObject((Contact c) => c.LastName.ToLower(), "lower(LastName)");
        }

        [Test]
        public void Function_Upper_Test()
        {
            AssertObject((Contact c) => c.LastName.ToUpper(), "upper(LastName)");
        }

        [Test]
        public void Function_Replace_Char_Test()
        {
            AssertObject((Contact c) => c.LastName.Replace('a', 'b'), "replace(LastName,'a','b')");
        }

        [Test]
        public void Function_Replace_String_Test()
        {
            AssertObject((Contact c) => c.LastName.Replace("a", "b"), "replace(LastName,'a','b')");
        }

        [Test]
        public void Function_Length_Test()
        {
            AssertObject((Contact c) => c.FirstName.Length, "length(FirstName)");
        }

        [Test]
        public void Function_IndexOf_Test()
        {
// ReSharper disable StringIndexOfIsCultureSpecific.1
            AssertObject((Contact c) => c.LastName.IndexOf("a"), "(locate('a',LastName)-1)");
// ReSharper restore StringIndexOfIsCultureSpecific.1
        }

        [Test]
        public void Function_PadLeft_Test()
        {
            AssertObject((Contact c) => c.FirstName.PadLeft(3), "lpad(FirstName,3)");
        }

        [Test]
        public void Function_PadLeft_Custom_Test()
        {
            AssertObject((Contact c) => c.FirstName.PadLeft(3, '*'), "lpad(FirstName,3,'*')");
        }

        [Test]
        public void Function_PadRight_Test()
        {
            AssertObject((Contact c) => c.FirstName.PadRight(3), "rpad(FirstName,3)");
        }

        [Test]
        public void Function_PadRight_Custom_Test()
        {
            AssertObject((Contact c) => c.FirstName.PadRight(3, '*'), "rpad(FirstName,3,'*')");
        }

        [Test]
        public void Function_Trim_Test()
        {
            AssertObject((Contact c) => c.FirstName.Trim(), "trim(FirstName)");
        }

        [Test]
        public void Function_Ascii_Test()
        {
            AssertObject((Contact c) => c.FirstName.Ascii(), "ascii(FirstName)");
        }

        [Test]
        public void Function_Char_Test()
        {
            AssertObject((SalesOrder o) => o.LineCount.Value.Char(), "char(LineCount)");
        }

        [Test]
        public void Function_Char_Nullable_Test()
        {
            AssertObject((SalesOrder o) => o.LineCount.Char(), "char(LineCount)");
        }

        [Test]
        public void Function_Abs_Test()
        {
            AssertObject((SalesOrder o) => Math.Abs(o.SubTotal.Value), "abs(SubTotal)");
        }

        [Test]
        public void Function_Sign_Test()
        {
            AssertObject((SalesOrder o) => Math.Sign(o.SubTotal.Value), "sign(SubTotal)");
        }

        [Test]
        public void Function_Round_Test()
        {
            AssertObject((SalesOrder o) => Math.Round(o.SubTotal.Value), "round(SubTotal)");
        }

        [Test]
        public void Function_Round_Custom_Test()
        {
            AssertObject((SalesOrder o) => Math.Round(o.SubTotal.Value, 3), "round(SubTotal,3)");
        }

#if !SILVERLIGHT
        [Test]
        public void Function_Truncate_Test()
        {
            AssertObject((SalesOrder o) => Math.Truncate(o.SubTotal.Value), "trunc(SubTotal)");
        }
#endif

        [Test]
        public void Function_Floor_Test()
        {
            AssertObject((SalesOrder o) => Math.Floor((double) o.SubTotal.Value), "floor(SubTotal)");
        }

        [Test]
        public void Function_Ceiling_Test()
        {
            AssertObject((SalesOrder o) => Math.Ceiling((double) o.SubTotal.Value), "ceil(SubTotal)");
        }

        [Test]
        public void Function_Pow_Test()
        {
            AssertObject((SalesOrder o) => Math.Pow((double) o.SubTotal.Value, 3), "pow(SubTotal,3)");
        }

        [Test]
        public void Function_Year_Test()
        {
            AssertObject((SalesOrder o) => o.OrderDate.Value.Year, "year(OrderDate)");
        }

        [Test]
        public void Function_Month_Test()
        {
            AssertObject((SalesOrder o) => o.OrderDate.Value.Month, "month(OrderDate)");
        }

        [Test]
        public void Function_Day_Test()
        {
            AssertObject((SalesOrder o) => o.OrderDate.Value.Day, "day(OrderDate)");
        }

        [Test]
        public void Function_Hour_Test()
        {
            AssertObject((SalesOrder o) => o.OrderDate.Value.Hour, "hour(OrderDate)");
        }

        [Test]
        public void Function_Minute_Test()
        {
            AssertObject((SalesOrder o) => o.OrderDate.Value.Minute, "minute(OrderDate)");
        }

        [Test]
        public void Function_Second_Test()
        {
            AssertObject((SalesOrder o) => o.OrderDate.Value.Second, "second(OrderDate)");
        }

        [Test]
        public void Function_Millisecond_Test()
        {
            AssertObject((SalesOrder o) => o.OrderDate.Value.Millisecond, "millisecond(OrderDate)");
        }

        [Test]
        public void Function_TzHour_Test()
        {
            AssertObject((SalesOrder o) => o.OrderDate.Value.Offset.Hours, "tzHour(OrderDate)");
        }

        [Test]
        public void Function_TzMinute_Test()
        {
            AssertObject((SalesOrder o) => o.OrderDate.Value.Offset.Minutes, "tzMinute(OrderDate)");
        }

        [Test]
        public void Function_AddDays_Test()
        {
            AssertObject((SalesOrder o) => o.OrderDate.Value.AddDays(7), "dateAdd(OrderDate,7)");
        }

        [Test]
        public void Function_SubDays_Test()
        {
            AssertObject((SalesOrder o) => o.OrderDate.Value.AddDays(-7), "dateSub(OrderDate,7)");
        }

        [Test]
        public void Function_TimestampAdd_Test()
        {
            AssertObject((SalesOrder o) => o.OrderDate.Value.AddMilliseconds(5000), "timestampAdd(OrderDate,5000)");
        }

        [Test]
        public void Function_TimestampSub_Test()
        {
            AssertObject((SalesOrder o) => o.OrderDate.Value.AddMilliseconds(-5000), "timestampSub(OrderDate,5000)");
        }

        #endregion

        [Test]
        public void Property_Test()
        {
            AssertObject((Contact c) => c.LastName, "LastName");
        }

        [Test]
        public void Property_Nested_Test()
        {
            AssertObject((Contact c) => c.Address.City, "Address.City");
        }

        [Test]
        public void Property_NullableBoolean_Test()
        {
            AssertObject((Contact c) => (bool) c.Active && c.Active.Value, "(Active and Active)");
        }

        private static void AssertSimple<T>(Expression<Func<T>> lambda, string expected)
        {
            AssertLambda(lambda, expected);
        }

        private static void AssertObject<TSource, TResult>(Expression<Func<TSource, TResult>> lambda, string expected)
        {
            AssertLambda(lambda, expected);
        }

        private static void AssertLambda(LambdaExpression lambda, string expected)
        {
            var expression = lambda.Body;
            expression = PartialEvaluatingExpressionTreeVisitor.EvaluateIndependentSubtrees(expression);
            expression = TransformingExpressionTreeVisitor.Transform(expression, _tranformationProvider);
            var result = SDataExpressionBuilderVisitor.BuildExpression(expression);
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}