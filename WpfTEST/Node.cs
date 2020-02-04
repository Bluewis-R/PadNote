using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

// might be worth splitting this doc into lots of class docs
//
//
public abstract class Node
{
  public abstract double Eval(IContext _ctx);
}

public interface IContext
{
  double ResolveVariable(string _name);
  double CallFunction(string _name, double[] _arguments);
}

class NodeNumber : Node
{
  double m_number;
  public NodeNumber(double _number)
  {
    m_number = _number;
  }
  
  public override double Eval(IContext ctx)
  {
    return m_number;
  }
}

class NodeBinary : Node
{
  public NodeBinary(Node _lhs, Node _rhs, Func<double, double, double> _op)
  {
    m_lhs = _lhs;
    m_rhs = _rhs;
    m_op = _op;
  }
  Node m_lhs;
  Node m_rhs;
  Func<double, double, double> m_op;

  public override double Eval(IContext _ctx)
  {
    var lhsVal = m_lhs.Eval(_ctx);
    var rhsVal = m_rhs.Eval(_ctx);

    var result = m_op(lhsVal, rhsVal);
    return result;
  }
}

class NodeUnary : Node
{
  Node m_rhs;                              // Right hand side of the operation
  Func<double, double> m_op;               // The callback operator

  // Constructor accepts the two nodes to be operated on and function
  // that performs the actual operation
  public NodeUnary(Node _rhs, Func<double, double> _op)
  {
    m_rhs = _rhs;
    m_op = _op;
  }

  public override double Eval(IContext _ctx)
  {
    // Evaluate RHS
    var rhsVal = m_rhs.Eval(_ctx);

    // Evaluate and return
    var result = m_op(rhsVal);
    return result;
  }
}
// Represents a variable (or a constant) in an expression.  eg: "2 * pi"
public class NodeVariable : Node
{
  string m_variableName;

  public NodeVariable(string _variableName)
  {
    m_variableName = _variableName;
  }

  public override double Eval(IContext ctx)
  {
    return ctx.ResolveVariable(m_variableName);
  }
}

public class NodeFunctionCall : Node
{
  string m_functionName;
  Node[] m_arguments;

  public NodeFunctionCall(string _functionName, Node[] _arguments)
  {
    m_functionName = _functionName;
    m_arguments = _arguments;
  }

  public override double Eval(IContext ctx)
  {
    // Evaluate all arguments
    var argVals = new double[m_arguments.Length];
    for (int i = 0; i < m_arguments.Length; i++)
    {
      argVals[i] = m_arguments[i].Eval(ctx);
    }

    // Call the function
    return ctx.CallFunction(m_functionName, argVals);
  }
}


class MyFunctionContext : IContext
{
  public MyFunctionContext()
  {
  }

  public double ResolveVariable(string _name)
  {
    throw new InvalidDataException($"Unknown variable: '{_name}'");
  }

  public double CallFunction(string _name, double[] arguments)
  {
    if (_name == "rectArea")
    {
      return arguments[0] * arguments[1];
    }

    if (_name == "rectPerimeter")
    {
      return (arguments[0] + arguments[1]) * 2;
    }

    throw new InvalidDataException($"Unknown function: '{_name}'");
  }
}
