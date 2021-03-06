﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

public enum Token
{
  EOF,
  Add,
  Subtract,
  Multiply,
  Divie,
  OpenParens,
  CloseParens,
  Comma,
  Identifier,
  Number,
}

public class Tokeniser
{
  public Tokeniser(TextReader reader)
  {
    _reader = reader;
    NextChar();
    NextToken();
  }

  TextReader _reader;
  char _currentChar;
  Token _currentToken;
  double _number;
  string _identifier;



  public Token getToken
  {
    get { return _currentToken; }
  }

  public double getNumber
  {
    get { return _number; }
  }

  public string getIdentifier
  {
    get { return _identifier; }
  }


  // Read the next character from the input strem
  // and store it in _currentChar, or load '\0' if EOF
  void NextChar()
  {
    int ch = _reader.Read();
    _currentChar = ch < 0 ? '\0' : (char)ch;
  }

  // Read the next token from the input stream
  public void NextToken()
  {
    // Skip whitespace
    while (char.IsWhiteSpace(_currentChar))
    {
      NextChar();
    }

    // Special characters
    switch (_currentChar)
    {
      case '\0':
        _currentToken = Token.EOF;
        return;

      case '+':
        NextChar();
        _currentToken = Token.Add;
        return;

      case '-':
        NextChar();
        _currentToken = Token.Subtract;
        return;

      case '*':
        NextChar();
        _currentToken = Token.Multiply;
        return;

      case '(':
        NextChar();
        _currentToken = Token.OpenParens;
        return;

      case ')':
        NextChar();
        _currentToken = Token.CloseParens;
        return;

      case ',':
        NextChar();
        _currentToken = Token.Comma;
        return;
    }

    // Number?
    if (char.IsDigit(_currentChar) || _currentChar == '.')
    {
      // Capture digits/decimal point
      var sb = new StringBuilder();
      bool haveDecimalPoint = false;
      while (char.IsDigit(_currentChar) || (!haveDecimalPoint && _currentChar == '.'))
      {
        sb.Append(_currentChar);
        haveDecimalPoint = _currentChar == '.';
        NextChar();
      }

      // Parse it
      _number = double.Parse(sb.ToString(), CultureInfo.InvariantCulture);
      _currentToken = Token.Number;
      return;
    }
    // Identifier - starts with letter or underscore
    if (char.IsLetter(_currentChar) || _currentChar == '_')
    {
      var sb = new StringBuilder();

      // Accept letter, digit or underscore
      while (char.IsLetterOrDigit(_currentChar) || _currentChar == '_')
      {
        sb.Append(_currentChar);
        NextChar();
      }

      // Setup token
      _identifier = sb.ToString();
      _currentToken = Token.Identifier;
      return;
    }

      throw new InvalidDataException($"Unexpected character: {_currentChar}");
  }
}







