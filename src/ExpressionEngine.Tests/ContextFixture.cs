#region License
//
// Expression Engine Library: ContextFixture.cs
//
// Author:
//   Giacomo Stelluti Scala (gsscoder@gmail.com)
//
// Copyright (C) 2012 - 2013 Giacomo Stelluti Scala
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
#endregion
#region Using Directives
using System;
using ExpressionEngine.Tests.Fakes;
using Xunit;
using FluentAssertions;
#endregion

namespace ExpressionEngine.Tests
{
    public class ContextFixture
    {
        [Fact]
        public void Should_throws_exception_with_null_expression()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Context().Evaluate(null));
        }

        [Fact]
        public void Should_throws_exception_with_empty_expression()
        {
            Assert.Throws<EvaluatorException>(() =>
                new Context().Evaluate(""));
        }

        [Fact]
        public void Should_evaluate_plain_addition()
        {
            new Context().Evaluate("1 + 2").Should().Be(3D);
        }

        [Fact]
        public void Should_evaluate_plain_addition_with_three_operands()
        {
            new Context().Evaluate("1 + 2 + 3").Should().Be(6D);
        }

        [Fact]
        public void Should_evaluate_plain_addition_with_four_operands()
        {
            new Context().Evaluate("1 + 2 + 3 + 4").Should().Be(10D);
        }

        [Fact]
        public void Should_evaluate_plain_subtraction()
        {
            new Context().Evaluate("3 - 4").Should().Be(-1D);
        }

        [Fact]
        public void Should_evaluate_plain_multiplication()
        {
            new Context().Evaluate("3 * 9").Should().Be(27D);
        }

        [Fact]
        public void Should_evaluate_plain_division()
        {
            new Context().Evaluate("16 / 4").Should().Be(4D);
        }

        [Fact]
        public void Should_evaluate_plain_modulo()
        {
            new Context().Evaluate("10 % 4").Should().Be(2D);
        }

        [Fact]
        public void Should_evaluate_expression_with_four_operands()
        {
            new Context().Evaluate("1 + 2 * 3 / 1").Should().Be(7D);
        }

        [Fact]
        public void Should_evaluate_expression_with_decimal_number()
        {
            new Context().Evaluate(".90 - 3 * 4 + 11.123").Should().Be(0.022999999999999687D);
        }

        [Fact]
        public void Should_evaluate_expression_with_modulo()
        {
            new Context().Evaluate("+0.90 - 3 * 4 + 11 - (123 % 23)").Should().Be(-8.1D);
        }

        [Fact]
        public void Should_evaluate_expression_with_unary_operators()
        {
            new Context().Evaluate("-3 + -1").Should().Be(-4D);
        }

        [Fact]
        public void Should_evaluate_expression_when_subtracting_from_a_function()
        {
            new Context().Evaluate("sqrt(10) - 1").Should().Be(2.1622776601683795D);
        }

        [Fact]
        public void Should_evaluate_expression_when_dividing_from_a_function()
        {
            new Context().Evaluate("sqrt(10000) / 2").Should().Be(50D);
        }

        [Fact]
        public void Should_evaluate_expression_when_calculating_modulo_to_a_function()
        {
            new Context().Evaluate("sqrt(1000) % 3").Should().Be(1.6227766016837926D);
        }

        [Fact]
        public void Should_evaluate_expression_when_calculating_modulo_of_a_decimal_number_to_a_function()
        {
            new Context().Evaluate("cos(999) % .3").Should().Be(0.099649852980826459D);
        }

        [Fact]
        public void Should_evaluate_expression_of_a_function_with_unary_operator()
        {
            new Context().Evaluate("-sqrt(100)").Should().Be(-10D);
        }

        [Fact]
        public void Should_evaluate_expression_with_five_operands()
        {
            new Context().Evaluate("3 * 0.31 / 19 + 10 - .7").Should().Be(9.3489473684210527D);
        }

        [Fact]
        public void Should_evaluate_expression_with_two_operands_between_parentheses()
        {
            new Context().Evaluate("1 + (2 - 1)").Should().Be(2D);
        }

        [Fact]
        public void Should_evaluate_expression_with_operands_between_nested_parentheses()
        {
            new Context().Evaluate("3 * 0.31 / ((19 + 10) - .7)").Should().Be(0.032862190812720848D);
        }

        [Fact]
        public void Should_evaluate_expression_with_nested_parentheses_and_functions()
        {
            new Context().Evaluate("3 * 0.31 / ((19 + sqrt(1000.5 / 10)) - pow(.7, 2)) + 3").Should().Be(3.0326172734832215D);
        }

        [Fact]
        public void Should_evaluate_unary_expression_between_parentheses()
        {
            new Context().Evaluate("-(1 + 2)").Should().Be(-3D);
        }

        [Fact]
        public void Should_evaluate_expression_with_nested_functions()
        {
            new Context().Evaluate("pow(2, cos(90))").Should().Be(0.73302097391658683D);
        }

        [Fact]
        public void Should_evaluate_expression_with_functions_having_different_parameters_and_same_name()
        {
            new Context().Evaluate("log(log(10, 3))").Should().Be(0.73998461763125689D);
        }

        [Fact]
        public void Should_evaluate_expression_with_nested_functions_and_unary_operators()
        {
            new Context().Evaluate("cos(-log(100))").Should().Be(-0.10701348355876977D);
        }

        [Fact]
        public void Should_evaluate_expression_with_nested_functions_having_two_parameters_and_unary_operators()
        {
            new Context().Evaluate("pow(2, -atan(-33))").Should().Be(2.9089582019337388);
        }

        [Fact]
        public void Should_evaluate_expression_with_multiple_nested_functions()
        {
            new Context().Evaluate("sin(10.3) * cos(sqrt(sin(301 - sqrt(10))))").Should().Be(-0.55707344143373561D);
        }

        [Fact]
        public void Should_evaluate_function_with_nested_expression()
        {
            new Context().Evaluate("sqrt(sqrt(10 * 3) - sqrt(1 + 3))").Should().Be(1.8647320384043551D);
        }

        [Fact]
        public void Should_evaluate_function_having_two_parameters_with_nested_expression()
        {
            new Context().Evaluate("pow((10 * 3), -(1 + log(10 * 10)))").Should().Be(0.0000000052539263674376282D);
        }

        [Fact]
        public void Should_evaluate_expression_with_unary_operators_and_decimal_numbers()
        {
            new Context().Evaluate(".2 / +9.99 * (-.123 + -2)").Should().Be(-0.042502502502502509D);
        }

        [Fact]
        public void Should_evaluate_a_more_complex_expression_with_unary_operators_and_decimal_numbers()
        {
            new Context().Evaluate("-.23 + +.99 * ((-.123 + -2.1)--333)").Should().Be(327.23922999999996D);
        }

        [Fact]
        public void Should_evaluate_exponent()
        {
            new Context().Evaluate("pow(10, 2)").Should().Be(100D);
        }

        [Fact]
        public void Should_evaluate_exponent_with_nested_expression()
        {
            new Context().Evaluate("pow((10 * 3), -(1 + 3))").Should().Be(0.0000012345679012345679D);
        }

        [Fact]
        public void Should_evaluate_variable()
        {
            new Context().Evaluate("pi - 3").Should().Be(0.14159265358979312D);
        }

        [Fact]
        public void Should_evaluate_expression_with_variable()
        {
            new Context().Evaluate("(e + pi) - 5.8598744820488378").Should().Be(0D);
        }

        [Fact]
        public void Should_evaluate_variable_with_unary_operator()
        {
            new Context().Evaluate("-e").Should().Be(-2.7182818284590451D);
        }

        [Fact]
        public void Should_evaluate_expression_with_variables_and_functions()
        {
            new Context().Evaluate("10 - (pi * atan(10)) - -e").Should().Be(8.0965979343737935D);
        }

        [Fact]
        public void Should_throws_exception_when_an_open_parenthesis_is_not_closed()
        {
            Assert.Throws<EvaluatorException>(() =>
                new Context().Evaluate("3 + 1 - ("));
        }

        [Fact]
        public void Should_throws_exception_when_operator_between_operands_is_missing()
        {
            Assert.Throws<EvaluatorException>(() =>
                new Context().Evaluate("1 2"));
        }

        [Fact]
        public void Should_throws_exception_when_an_open_parenthesis_is_not_closed_and_an_operator_has_a_missing_operand()
        {
            Assert.Throws<EvaluatorException>(() =>
                new Context().Evaluate("3 + (1 -"));
        }

        [Fact]
        public void Should_throws_exception_when_an_open_parenthesis_is_not_closed_and_terminates_with_an_operand()
        {
            Assert.Throws<EvaluatorException>(() =>
                new Context().Evaluate("3 + 3 / (1"));
        }

        [Fact]
        public void Should_evaluate_expression_with_user_defined_functions_and_variables()
        {
            // Given
            var context = new Context()
                .SetVariable("G", 6.67428D)
                .SetVariable("earth_mass", Evaluate.As<double>("5.97219 * pow(10,24)")) // 5.97219E+24 kg
                .SetVariable("lunar_mass", Evaluate.As<double>("7.34767309 * pow(10,22)")) // 7.34767309E+22 kg
                .SetVariable("perigee_dist", 356700000D) // moon-earth distance at perigee in m
                .SetFunction("calc_force", args => (
                    TypeConverter.ToNumber(args[0]) * TypeConverter.ToNumber(args[1]) / Math.Pow(TypeConverter.ToNumber(args[2]), 2)));

            // When
            var result = context.EvaluateAs<double>("G * calc_force(earth_mass, lunar_mass, perigee_dist)");
            
            // Than
            result.Should().Be(2.3018745174107073E+31D);
        }

        [Fact]
        public void Should_evaluate_expression_with_equality_operator()
        {
            new Context().EvaluateAs<bool>("10 == 10").Should().Be(true);
        }

        [Fact]
        public void Should_evaluate_expression_with_inequality_operator()
        {
            new Context().EvaluateAs<bool>("10 != (5 + 5)").Should().Be(false);
        }

        [Fact]
        public void Should_evaluate_expression_with_lesser_than_operator()
        {
            new Context().EvaluateAs<bool>("pow(10, 2) < 9.9").Should().Be(false);
        }

        [Fact]
        public void Should_evaluate_expression_with_greater_than_operator()
        {
            new Context().EvaluateAs<bool>("pow(10, 2) > 9.9").Should().Be(true);
        }

        [Fact]
        public void Should_evaluate_expression_with_lesser_than_or_equal_operator()
        {
            new Context().EvaluateAs<bool>("pi <= pi").Should().Be(true);
        }

        [Fact]
        public void Should_evaluate_expression_with_greater_than_or_equal_operator()
        {
            new Context().EvaluateAs<bool>("pi >= (pi + 0.0001)").Should().Be(false);
        }

        [Fact]
        public void Should_evaluate_equality_between_two_expressions()
        {
            new Context().EvaluateAs<bool>("(10 == 1) == (1 == 10)").Should().Be(true);
        }

        [Fact]
        public void Should_evaluate_inequality_between_two_expressions()
        {
            new Context().EvaluateAs<bool>("(10 == 1) != (10 == 1)").Should().Be(false);
        }

        [Fact]
        public void Should_evaluate_equality_between_an_expression_and_boolean_literal()
        {
            new Context().EvaluateAs<bool>("(10 == 10) == true").Should().Be(true);
        }

        [Fact]
        public void Should_evaluate_true_when_true_is_matched_against_true()
        {
            new Context().EvaluateAs<bool>("true == true").Should().Be(true);
        }

        [Fact]
        public void Should_evaluate_inequality_between_an_expression_and_boolean_literal()
        {
            new Context().EvaluateAs<bool>("false != (10 != 10)").Should().Be(false);
        }

        [Fact]
        public void Should_evaluate_true_when_false_is_matched_against_false()
        {
            new Context().EvaluateAs<bool>("false == false").Should().Be(true);
        }

        [Fact]
        public void Should_sum_a_boolean_literal_with_a_number()
        {
            new Context().EvaluateAs<double>("true + 1").Should().Be(2D);
        }

        [Fact]
        public void Should_subtract_a_boolean_literal_with_a_number()
        {
            new Context().EvaluateAs<double>("false - 1").Should().Be(-1D);
        }

        [Fact]
        public void Should_sum_a_boolean_literal_with_an_expression()
        {
            new Context().EvaluateAs<double>("true - (1 + 2)").Should().Be(-2D);
        }

        [Fact]
        public void Should_sum_a_boolean_literal_with_an_equality_expression()
        {
            new Context().EvaluateAs<double>("(10 == 10) + false").Should().Be(1D);
        }

        [Fact]
        public void Should_evaluate_exponent_when_a_boolean_literal_is_used_as_a_number()
        {
            new Context().EvaluateAs<double>("pow(10, true+1)").Should().Be(100D);
        }

        [Fact]
        public void Should_evaluate_function_when_a_boolean_literal_is_used_as_a_number()
        {
            new Context().EvaluateAs<double>("log(10, 10+false)").Should().Be(1D);
        }

        [Fact]
        public void Should_convert_number_to_string_with_plus_operator()
        {
            // Given
            var expression = "\"hello, \" + 1234";

            // When
            var result = new Context().EvaluateAs<string>(expression);

            // Than
            result.Should().Be("hello, 1234");
        }

        [Fact]
        public void Should_throws_exception_when_unary_plus_operator_is_applied_to_string()
        {
            Assert.Throws<EvaluatorException>(
                () => new Context().Evaluate("+\"this is a string\"") );
        }

        [Fact]
        public void Should_throws_exception_when_unary_minus_operator_is_applied_to_string()
        {
            Assert.Throws<EvaluatorException>(
                () => new Context().Evaluate("-\"this is a string\""));
        }

        [Fact]
        public void Should_convert_all_numbers_to_string_with_plus_operator()
        {
            // Given
            var expression = "1234 + \" hello, \" + 4567 + \"! \" + 8910";

            // When
            var result = new Context().EvaluateAs<string>(expression);

            // Than
            result.Should().Be("1234 hello, 4567! 8910");
        }

        //[Fact]
        //public void Should_access_string_field_from_registred_object()
        //{
        //    // Given
        //    var expression = "my_fake.FakeStringField";
        //    var fake = new FakeObject {FakeStringField = "exactly this"};
        //    var context = new Context();

        //    // When
        //    context.SetObject("my_fake", fake);
        //    var result = context.EvaluateAs<string>(expression);

        //    // Than
        //    result.Should().Be("exactly this");
        //}
    }
}