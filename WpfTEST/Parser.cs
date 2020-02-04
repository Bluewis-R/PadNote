using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;


public class Parser
{
  public Parser(Tokeniser _tokeniser)
  {
    m_tokeniser = _tokeniser;
  }
  Tokeniser m_tokeniser;

  public Node ParseExpression()
  {
    var expr = ParseAddSubtract();

    if (m_tokeniser.getToken != Token.EOF)
      throw new Exception("Unexpected characters at end of expression");

    return expr;
  }

  Node ParseAddSubtract()
  {
    // Parse the left hand side
    var lhs = ParseUnary();

    while (true)
    {
      // Work out the operator
      Func<double, double, double> op = null;
      if (m_tokeniser.getToken == Token.Add)
      {
        op = (a, b) => a + b;
      }
      else if (m_tokeniser.getToken == Token.Subtract)
      {
        op = (a, b) => a - b;
      }

      // Binary operator found?
      if (op == null)
        return lhs;             // no

      // Skip the operator
      m_tokeniser.NextToken();

      // Parse the right hand side of the expression
      var rhs = ParseUnary();

      // Create a binary node and use it as the left-hand side from now on
      lhs = new NodeBinary(lhs, rhs, op);
    }
  }

  Node ParseUnary()
  {
    // Positive operator is a no-op so just skip it
    if (m_tokeniser.getToken == Token.Add)
    {
      // Skip
      m_tokeniser.NextToken();
      return ParseUnary();
    }

    // Negative operator
    if (m_tokeniser.getToken == Token.Subtract)
    {
      // Skip
      m_tokeniser.NextToken();

      // Parse RHS 
      // Note this recurses to self to support negative of a negative
      var rhs = ParseUnary();

      // Create unary node
      return new NodeUnary(rhs, (a) => -a);
    }

    // No positive/negative operator so parse a leaf node
    return ParseLeaf();
  }

  // Parse a leaf node
  // (For the moment this is just a number)
  Node ParseLeaf()
  {
    // Is it a number?
    if (m_tokeniser.getToken == Token.Number)
    {
      var node = new NodeNumber(m_tokeniser.getNumber);
      m_tokeniser.NextToken();
      return node;
    }

    // Parenthesis?
    if (m_tokeniser.getToken == Token.OpenParens)
    {
      // Skip '('
      m_tokeniser.NextToken();

      // Parse a top-level expression
      var node = ParseAddSubtract();

      // Check and skip ')'
      if (m_tokeniser.getToken != Token.CloseParens)
        throw new Exception("Missing close parenthesis");
      m_tokeniser.NextToken();

      // Return
      return node;
    }
    // Variable
    if (m_tokeniser.getToken == Token.Identifier)
    {
      // Capture the name and skip it
      var name = m_tokeniser.getIdentifier;
      m_tokeniser.NextToken();

      // Parens indicate a function call, otherwise just a variable
      if (m_tokeniser.getToken != Token.OpenParens)
      {
        // Variable
        return new NodeVariable(name);
      }
      else
      {
        // Function call

        // Skip parens
        m_tokeniser.NextToken();

        // Parse arguments
        var arguments = new List<Node>();
        while (true)
        {
          // Parse argument and add to list
          arguments.Add(ParseAddSubtract());

          // Is there another argument?
          if (m_tokeniser.getToken == Token.Comma)
          {
            m_tokeniser.NextToken();
            continue;
          }

          // Get out
          break;
        }

        // Check and skip ')'
        if (m_tokeniser.getToken != Token.CloseParens)
          throw new Exception("Missing close parenthesis");
        m_tokeniser.NextToken();

        // Create the function call node
        return new NodeFunctionCall(name, arguments.ToArray());
      }
    }






    // Don't Understand
    throw new Exception($"Unexpect token: {m_tokeniser.getToken}");
  }
  #region Convenience Helpers

  // Static helper to parse a string
  public static Node Parse(string str)
  {
    return Parse(new Tokeniser(new StringReader(str)));
  }

  // Static helper to parse from a tokenizer
  public static Node Parse(Tokeniser _tokeniser)
  {
    var parser = new Parser(_tokeniser);
    return parser.ParseExpression();
  }     
  

  #endregion

}