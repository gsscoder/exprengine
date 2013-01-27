#region License
//
// Expression Engine Library: LexerFixture.cs
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
using ExpressionEngine.Internal;
using Xunit;
using FluentAssertions;
#endregion

namespace ExpressionEngine.Tests
{
    public class LexerFixture
    {
        [Fact]
        public void Should_lex_expression_with_nested_parentheses()
        {
            // Given when
            var scanner = new Lexer(Text.OfString("((10 + 1 - (3 - .234)) * 300)  /  10.1"));

            // Then
            scanner.NextToken().Type.Should().Be(TokenType.LeftParenthesis);
            scanner.NextToken().Type.Should().Be(TokenType.LeftParenthesis);
            ((LiteralToken) scanner.NextToken()).Value.Should().Be(10D);
            scanner.NextToken().Type.Should().Be(TokenType.Plus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(1D);
            scanner.NextToken().Type.Should().Be(TokenType.Minus);
            scanner.NextToken().Type.Should().Be(TokenType.LeftParenthesis);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(3D);
            scanner.NextToken().Type.Should().Be(TokenType.Minus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(.234D);
            scanner.NextToken().Type.Should().Be(TokenType.RightParenthesis);
            scanner.NextToken().Type.Should().Be(TokenType.RightParenthesis);
            scanner.NextToken().Type.Should().Be(TokenType.Multiply);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(300D);
            scanner.NextToken().Type.Should().Be(TokenType.RightParenthesis);
            scanner.NextToken().Type.Should().Be(TokenType.Divide);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(10.1D);
            scanner.NextToken().Should().BeNull();
        }

        [Fact]
        public void Should_not_move_forward_when_using_peek()
        {
            // Given when
            var scanner = new Lexer(Text.OfString("1 + 3"));

            // Then
            ((LiteralToken)scanner.PeekToken()).Value.Should().Be(1D);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(1D);

            // Then
            scanner.PeekToken().Type.Should().Be(TokenType.Plus);
            scanner.NextToken().Type.Should().Be(TokenType.Plus);

            // Then
            ((LiteralToken)scanner.PeekToken()).Value.Should().Be(3D);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(3D);

            // Then
            scanner.PeekToken().Should().BeNull();
            scanner.NextToken().Should().BeNull();
        }

        [Fact]
        public void Should_lex_expression_with_modulo()
        {
            // Given when
            var scanner = new Lexer(Text.OfString("10 % 2"));
            ((LiteralToken)scanner.PeekToken()).Value.Should().Be(10D);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(10D);

            // Then
            scanner.PeekToken().Type.Should().Be(TokenType.Modulo);
            scanner.NextToken().Type.Should().Be(TokenType.Modulo);
        }

        [Fact]
        public void Should_lex_expression_formed_by_a_single_number()
        {
            // Given when
            var scanner = new Lexer(Text.OfString("123"));

            // Then
            ((LiteralToken)scanner.PeekToken()).Value.Should().Be(123D);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(123D);
        }

        [Fact]
        public void Should_lex_expression_formed_by_a_single_decimal_number_with_zero_omitted()
        {
            // Give when
            var scanner = new Lexer(Text.OfString(".123"));

            // Then
            ((LiteralToken)scanner.PeekToken()).Value.Should().Be(.123D);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(.123D);
        }

        [Fact]
        public void Should_lex_expression_formed_by_a_single_positive_number_with_plus_sign_explicit()
        {
            // Given when
            var scanner = new Lexer(Text.OfString("+123"));

            // Then
            scanner.PeekToken().Type.Should().Be(TokenType.Plus);
            scanner.NextToken().Type.Should().Be(TokenType.Plus);
            ((LiteralToken)scanner.PeekToken()).Value.Should().Be(123D);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(123D);

        }

        [Fact]
        public void Should_lex_expression_formed_by_a_single_positive_decimal_number_with_zero_omitted_and_plus_sign_explicit()
        {
            // Given when
            var scanner = new Lexer(Text.OfString("+.123"));

            // Than
            scanner.PeekToken().Type.Should().Be(TokenType.Plus);
            scanner.NextToken().Type.Should().Be(TokenType.Plus);
            ((LiteralToken)scanner.PeekToken()).Value.Should().Be(.123D);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(.123D);
        }

        [Fact]
        public void Should_lex_expression_formed_by_a_single_negative_number()
        {
            // Given when
            var scanner = new Lexer(Text.OfString("-123"));

            // Than
            scanner.PeekToken().Type.Should().Be(TokenType.Minus);
            scanner.NextToken().Type.Should().Be(TokenType.Minus);
            ((LiteralToken)scanner.PeekToken()).Value.Should().Be(123D);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(123D);
        }

        [Fact]
        public void Should_lex_expression_formed_by_a_single_negative__decimal_number_with_zero_omitted()
        {
            // Given when
            var scanner = new Lexer(Text.OfString("-.123"));

            // Than
            scanner.PeekToken().Type.Should().Be(TokenType.Minus);
            scanner.NextToken().Type.Should().Be(TokenType.Minus);
            ((LiteralToken)scanner.PeekToken()).Value.Should().Be(.123D);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(.123D);
        }

        [Fact]
        public void Should_lex_expression_formed_by_negative_and_positive_decimal_number_with_zero_omitted_and_explicit_sign()
        {
            // Given when
            var scanner = new Lexer(Text.OfString(".2 / +9.99 * (-.123 + -2)"));

            // Than
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(.2D);
            scanner.NextToken().Type.Should().Be(TokenType.Divide);
            scanner.NextToken().Type.Should().Be(TokenType.Plus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(9.99D);
            scanner.NextToken().Type.Should().Be(TokenType.Multiply);
            scanner.NextToken().Type.Should().Be(TokenType.LeftParenthesis);
            scanner.NextToken().Type.Should().Be(TokenType.Minus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(.123D);
            scanner.NextToken().Type.Should().Be(TokenType.Plus);
            scanner.NextToken().Type.Should().Be(TokenType.Minus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(2D);
            scanner.NextToken().Type.Should().Be(TokenType.RightParenthesis);
        }

        [Fact]
        public void Should_lex_expression_with_sum_near_unary_operators()
        {
            // Given when
            var scanner = new Lexer(Text.OfString("-.23 + +.99 * ((-.123 + -2.1)--333)"));

            // Than
            scanner.NextToken().Type.Should().Be(TokenType.Minus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(.23D);
            scanner.NextToken().Type.Should().Be(TokenType.Plus);
            scanner.NextToken().Type.Should().Be(TokenType.Plus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(.99D);
            scanner.NextToken().Type.Should().Be(TokenType.Multiply);
            scanner.NextToken().Type.Should().Be(TokenType.LeftParenthesis);
            scanner.NextToken().Type.Should().Be(TokenType.LeftParenthesis);
            scanner.NextToken().Type.Should().Be(TokenType.Minus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(.123D);
            scanner.NextToken().Type.Should().Be(TokenType.Plus);
            scanner.NextToken().Type.Should().Be(TokenType.Minus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(2.1D);
            scanner.NextToken().Type.Should().Be(TokenType.RightParenthesis);
            scanner.NextToken().Type.Should().Be(TokenType.Minus); ;
            scanner.NextToken().Type.Should().Be(TokenType.Minus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(333D);
            scanner.NextToken().Type.Should().Be(TokenType.RightParenthesis);
        }

        [Fact]
        public void Should_lex_expression_with_function()
        {
            // Given when
            var scanner = new Lexer(Text.OfString("3 + pow(10, 2)"));

            // Than
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(3D);
            scanner.NextToken().Type.Should().Be(TokenType.Plus);
            ((IdentifierToken)scanner.NextToken()).Text.Should().Be("pow");
            scanner.NextToken().Type.Should().Be(TokenType.LeftParenthesis);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(10D);
            scanner.NextToken().Type.Should().Be(TokenType.Comma);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(2D);
            scanner.NextToken().Type.Should().Be(TokenType.RightParenthesis);
        }

        [Fact]
        public void Should_lex_identifier_with_underscore()
        {
            // Given when
            var scanner = new Lexer(Text.OfString("var_name"));

            // Than
            ((IdentifierToken)scanner.NextToken()).Text.Should().Be("var_name");
        }

        [Fact]
        public void Should_lex_equality_operator()
        {
            // Given when
            var scanner = new Lexer(Text.OfString("var1 == 10.3"));

            // Than
            ((IdentifierToken)scanner.NextToken()).Text.Should().Be("var1");
            scanner.NextToken().Type.Should().Be(TokenType.Equality);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(10.3D);
        }

        [Fact]
        public void Should_lex_inequality_operator()
        {
            // Given when
            var scanner = new Lexer(Text.OfString(".3 != 0.003003"));

            // Than
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(0.3D);
            scanner.NextToken().Type.Should().Be(TokenType.Inequality);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(0.003003D);
        }

        [Fact]
        public void Should_lex_less_than_operator()
        {
            // Given when
            var scanner = new Lexer(Text.OfString("10 < (10 * 2.2)"));

            // Than
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(10D);
            scanner.NextToken().Type.Should().Be(TokenType.LessThan);
            scanner.NextToken().Type.Should().Be(TokenType.LeftParenthesis);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(10D);
            scanner.NextToken().Type.Should().Be(TokenType.Multiply);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(2.2D);
            scanner.NextToken().Type.Should().Be(TokenType.RightParenthesis);
        }

        [Fact]
        public void Should_lex_less_than_or_equal_operator()
        {
            // Given when
            var scanner = new Lexer(Text.OfString(".3 <= .9"));

            // Than
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(0.3D);
            scanner.NextToken().Type.Should().Be(TokenType.LessThanOrEqual);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(0.9D);
        }

        [Fact]
        public void Should_lex_greater_than_operator()
        {
            // Given when
            var scanner = new Lexer(Text.OfString("10 > (10 * 2.2)"));

            // Than
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(10D);
            scanner.NextToken().Type.Should().Be(TokenType.GreaterThan);
            scanner.NextToken().Type.Should().Be(TokenType.LeftParenthesis);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(10D);
            scanner.NextToken().Type.Should().Be(TokenType.Multiply);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(2.2D);
            scanner.NextToken().Type.Should().Be(TokenType.RightParenthesis);
        }

        [Fact]
        public void Should_lex_greater_than_or_equal_operator()
        {
            // Given when
            var scanner = new Lexer(Text.OfString("0.1 >= +5.9"));

            // Than
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(0.1D);
            scanner.NextToken().Type.Should().Be(TokenType.GreaterThanOrEqual);
            scanner.NextToken().Type.Should().Be(TokenType.Plus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(5.9D);
        }

        [Fact]
        public void Should_lex_true_literal()
        {
            // Given when
            var scanner = new Lexer(Text.OfString("true == 1"));

            // Than
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(true);
            scanner.NextToken().Type.Should().Be(TokenType.Equality);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(1D);
        }

        [Fact]
        public void Should_lex_false_literal()
        {
            // Given when
            var scanner = new Lexer(Text.OfString("false != .01"));

            // Than
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(false);
            scanner.NextToken().Type.Should().Be(TokenType.Inequality);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(0.01D);
        }

        [Fact]
        public void Should_lex_number_with_exponent()
        {
            // Given when
            var scanner = new Lexer(Text.OfString("1 + 10E2 + 3"));

            // Than
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(1D);
            scanner.NextToken().Type.Should().Be(TokenType.Plus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(1000D);
            scanner.NextToken().Type.Should().Be(TokenType.Plus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(3D);
        }

        [Fact]
        public void Should_lex_number_with_exponent_and_plus_sign()
        {
            // Given when
            var scanner = new Lexer(Text.OfString("1 + 10E+2"));

            // Than
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(1D);
            scanner.NextToken().Type.Should().Be(TokenType.Plus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(1000D);
        }

        [Fact]
        public void Should_lex_number_with_lower_case_exponent()
        {
            // Given when
            var scanner = new Lexer(Text.OfString("10e2 + 4"));

            // Than
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(1000D);
            scanner.NextToken().Type.Should().Be(TokenType.Plus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(4D);
        }

        [Fact]
        public void Should_lex_number_with_lower_case_exponent_and_plus_sign()
        {
            // Given when
            var scanner = new Lexer(Text.OfString("1 + 10e+2 + 4"));

            // Than
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(1D);
            scanner.NextToken().Type.Should().Be(TokenType.Plus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(1000D);
            scanner.NextToken().Type.Should().Be(TokenType.Plus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(4D);
        }

        [Fact]
        public void Should_lex_number_with_lower_case_exponent_and_minus_sign()
        {
            // Given when
            var scanner = new Lexer(Text.OfString("10E-11 + 10e-11"));

            // Than
            ((LiteralToken) scanner.NextToken()).Value.Should().Be(0.0000000001D);
            scanner.NextToken().Type.Should().Be(TokenType.Plus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(0.0000000001D);
        }

        [Fact]
        public void Should_lex_string_literal()
        {
            // Given when
            var scanner = new Lexer(Text.OfString("\"hello, csharp\""));

            // Than
            ((LiteralToken)scanner.NextToken()).Value.Should().Be("hello, csharp");
        }

        [Fact]
        public void Should_lex_expression_containing_string_literal()
        {
            // Given when
            var scanner = new Lexer(Text.OfString(".3 + \"hello, fsharp\" + 0.999"));

            // Than
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(0.3D);
            scanner.NextToken().Type.Should().Be(TokenType.Plus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be("hello, fsharp");
            scanner.NextToken().Type.Should().Be(TokenType.Plus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(0.999D);
        }

        [Fact]
        public void Should_lex_string_literal_with_escape_characters()
        {
            // Given when
            var scanner = new Lexer(Text.OfString(@"0 + ""\t \n \r"" + 0"));

            // Than
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(0D);
            scanner.NextToken().Type.Should().Be(TokenType.Plus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be("\t \n \r");
            scanner.NextToken().Type.Should().Be(TokenType.Plus);
            ((LiteralToken)scanner.NextToken()).Value.Should().Be(0D);
        }

        [Fact]
        public void Should_lex_string_literal_with_decimal_escape()
        {
            // Given when
            var scanner = new Lexer(Text.OfString(@" ""\048\048\055"" "));

            // Than
            ((LiteralToken)scanner.NextToken()).Value.Should().Be("007");
        }

        [Fact]
        public void Should_lex_string_literal_with_double_quote_escape_character()
        {
            // Given when
            var scanner = new Lexer(Text.OfString(@" ""a\""bc"" "));

            // Than
            ((LiteralToken)scanner.NextToken()).Value.Should().Be("a\"bc");
        }

        [Fact]
        public void Should_not_allow_line_ending_in_expression()
        {
            // Given when
            var text = Text.OfString(string.Concat(new string(' ', 10), Environment.NewLine));

            // Than
            Assert.Throws<EvaluatorException>(
                () => new Lexer(text));
        }
    }
}