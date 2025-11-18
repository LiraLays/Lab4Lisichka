using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLab4.Models.AstNodes;
using ClassLab4.Exceptions;

namespace ClassLab4.Parser
{
    public class FormulaParcer
    {
        private readonly string _input;
        private int _position;
        private readonly List<Token> _tokens;
        private int _tokenIndex;

        public FormulaParcer(string input)
        {
            _input = input?.Replace(" ", "") ?? throw new ArgumentNullException(nameof(input));
            _position = 0;
            _tokens = Tokenize();
            _tokenIndex = 0;
        }

        public Node Parse()
        {
            if (_tokens.Count == 0)
                throw new FormulaParseException("Пустая формула");

            return ParseException();
        }

        private List<Token> Tokenize()
        {
            var tokens = new List<Token>();

            while (_position < _input.Length)
            {
                var current = _input[_position];

                switch (current)
                {
                    case '&':
                    case '∧':
                        tokens.Add(new Token(TokenType.And, current));
                        _position++;
                        break;
                    case '|':
                    case '∨':
                        tokens.Add(new Token(TokenType.Or, current));
                        _position++;
                        break;
                    case '!':
                    case '¬':
                        tokens.Add(new Token(TokenType.Not, current));
                        _position++;
                        break;
                    case '(':
                        tokens.Add(new Token(TokenType.LeftParen, current));
                        _position++;
                        break;
                    case ')':
                        tokens.Add(new Token(TokenType.RightParen, current));
                        _position++;
                        break;
                    default:
                        if (char.IsLetter(current))
                        {
                            tokens.Add(new Token(TokenType.Variable, value: current.ToString()));
                            _position++;
                        }
                        else
                        {
                            throw new FormulaParseException($"Неизвестный символ: {current}");
                        }
                        break;
                }
            }

            tokens.Add(new Token(TokenType.End));
            return tokens;
        }

        private Node ParseException()
        {
            var left = ParseTerm();

            while (CurrentToken.Type == TokenType.Or)
            {
                Consume(TokenType.Or);
                var right = ParseTerm();
                left = new OrNode(left, right);
            }

            return left;
        }

        private Node ParseTerm()
        {
            var left = ParseFactor();

            while (CurrentToken.Type == TokenType.And)
            {
                Consume(TokenType.And);
                var right = ParseFactor();
                left = new AndNode(left, right);
            }

            return left;
        }

        private Node ParseFactor()
        {
            if (CurrentToken.Type == TokenType.Not)
            {
                Consume(TokenType.Not);
                var operand = ParseFactor();
                return new NotNode(operand);
            }

            if (CurrentToken.Type == TokenType.LeftParen)
            {
                Consume(TokenType.LeftParen);
                var expression = ParseException();
                Consume(TokenType.RightParen);
                return expression;
            }

            if (CurrentToken.Type == TokenType.Variable)
            {
                var variableName = CurrentToken.Value[0];
                Consume(TokenType.Variable);
                return new VariableNode(variableName);
            }

            throw new FormulaParseException($"Ожидалась переменная или выражение, получен: {CurrentToken.Type}");
        }

        private Token CurrentToken => _tokens[_tokenIndex];

        private void Consume(TokenType expectedType)
        {
            if (CurrentToken.Type == expectedType)
            {
                _tokenIndex++;
            }
            else
            {
                throw new FormulaParseException($"Ожидался {expectedType}, получен {CurrentToken.Type}");
            }
        }
    }
}
