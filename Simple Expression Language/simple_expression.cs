/*
 * Lexical analysis (scanner) for the Simple Expression Language.
 * 2019-09-02
 * 
 * Syntactic analysis (parser)
 * 2019-09-23
 * 
 *  LL(1) Grammar:
 * 
 *      Prog  ::= Exp "EOF"
 *      Exp   ::= Term ("+" Term)*
 *      Term  ::= Pow ("*" Pow)*
 *      Pow   ::= Fact ("**" Pow)?
 *      Fact  ::= "int" | "(" Exp ")"
 * 
 * AST construction
 * 2019-10-07
 * 
 * AST traversal using the (modified) Visitor pattern
 * 2019-10-14
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

public enum TokenCategory {
    INT, PLUS, TIMES, POW, OPEN_PAR, CLOSE_PAR, EOF, BAD_TOKEN
}

public class Token {
    public TokenCategory Category { get; }
    public String Lexeme { get; }
    public Token(TokenCategory category, String lexeme) {
        Category = category;
        Lexeme = lexeme;
    }
    public override String ToString() {
        return $"[{Category}, \"{Lexeme}\"]";
    }
}

public class Scanner {
    readonly String input;
    static readonly Regex regex = new Regex(
        @"(\d+)|([+])|([*][*])|([*])|([(])|([)])|(\s)|(.)");
    public Scanner(String input) {
        this.input = input;
    }
    public IEnumerable<Token> Start() {
        foreach (Match m in regex.Matches(input)) {
            if (m.Groups[1].Success) {
                yield return new Token(TokenCategory.INT, m.Value);
            } else if (m.Groups[2].Success) {
                yield return new Token(TokenCategory.PLUS, m.Value);
            } else if (m.Groups[3].Success) {
                yield return new Token(TokenCategory.POW, m.Value);
            } else if (m.Groups[4].Success) {
                yield return new Token(TokenCategory.TIMES, m.Value);
            } else if (m.Groups[5].Success) {
                yield return new Token(TokenCategory.OPEN_PAR, m.Value);
            } else if (m.Groups[6].Success) {
                yield return new Token(TokenCategory.CLOSE_PAR, m.Value);
            } else if (m.Groups[7].Success) {
                continue; // Skip spaces
            } else if (m.Groups[8].Success) {
                yield return new Token(TokenCategory.BAD_TOKEN, m.Value);
            }
        }
        yield return new Token(TokenCategory.EOF, null);
    }
}

public class SyntaxError: Exception { }

public class Parser {
    IEnumerator<Token> tokenStream;

    public Parser(IEnumerator<Token> tokenStream) {
        this.tokenStream = tokenStream;
        this.tokenStream.MoveNext();
    }

    public TokenCategory Current {
        get { return tokenStream.Current.Category; }
    }

    public Token Expect(TokenCategory category) {
        if (Current == category) {
            Token current = tokenStream.Current;
            tokenStream.MoveNext();
            return current;
        } else {
            throw new SyntaxError();
        }
    }

    public Node Prog() {
        var result = new Prog() {
            Exp()  // Add method called implicitly.
        };
        Expect(TokenCategory.EOF);
        return result;
    }

    public Node Exp() {
        var result = Term();
        while (Current == TokenCategory.PLUS) {
            var node = new Plus() {
                AnchorToken = Expect(TokenCategory.PLUS)
            };
            node.Add(result);
            node.Add(Term());
            result = node;
        }
        return result;
    }

    public Node Term() {
        var result = Pow();
        while (Current == TokenCategory.TIMES) {
            var node = new Times() {
                AnchorToken = Expect(TokenCategory.TIMES)
            };
            node.Add(result);
            node.Add(Pow());
            result = node;
        }
        return result;
    }

    public Node Pow() {
        var result = Fact();
        if (Current == TokenCategory.POW) {
            var node = new Pow() {
                AnchorToken = Expect(TokenCategory.POW)
            };
            node.Add(result);
            node.Add(Pow());
            result = node;
        }
        return result;
    }

    public Node Fact() {
        switch (Current) {
        case TokenCategory.INT:
            var token = Expect(TokenCategory.INT);
            return new Int() {
                AnchorToken = token
            };

        case TokenCategory.OPEN_PAR:
            Expect(TokenCategory.OPEN_PAR);
            var result = Exp();
            Expect(TokenCategory.CLOSE_PAR);
            return result;

        default:
            throw new SyntaxError();
        }
    }
}

public class Node: IEnumerable<Node> {

    IList<Node> children = new List<Node>();

    public Node this[int index] {
        get {
            return children[index];
        }
    }

    public Token AnchorToken { get; set; }

    public void Add(Node node) {
        children.Add(node);
    }

    public IEnumerator<Node> GetEnumerator() {
        return children.GetEnumerator();
    }

    System.Collections.IEnumerator
    System.Collections.IEnumerable.GetEnumerator() {
        throw new NotImplementedException();
    }

    public override string ToString() {
        return String.Format("{0} {1}", GetType().Name, AnchorToken);
    }

    public string ToStringTree() {
        var sb = new StringBuilder();
        TreeTraversal(this, "", sb);
        return sb.ToString();
    }

    static void TreeTraversal(Node node, string indent, StringBuilder sb) {
        sb.Append(indent);
        sb.Append(node);
        sb.Append('\n');
        foreach (var child in node.children) {
            TreeTraversal(child, indent + "  ", sb);
        }
    }
}

public class Prog:  Node { }
public class Plus:  Node { }
public class Times: Node { }
public class Pow:   Node { }
public class Int:   Node { }

public class EvalVisitor {

    public int Visit(Prog node) {
        return Visit((dynamic) node[0]);
    }

    public int Visit(Plus node) {
        return Visit((dynamic) node[0]) 
            + Visit((dynamic) node[1]);
    }

    public int Visit(Times node) {
        return Visit((dynamic) node[0])
            * Visit((dynamic) node[1]);
    }

    public int Visit(Pow node) {
        return (int) Math.Pow(
            Visit((dynamic) node[0]),
            Visit((dynamic) node[1]));
    }

    public int Visit(Int node) {
        return Int32.Parse(node.AnchorToken.Lexeme);
    }
}

public class LispVisitor {

    public String Visit(Prog node) {
        return "(require '[clojure.math.numeric-tower :refer [expt]])\n"
            + Visit((dynamic) node[0]);
    }

    public String Visit(Plus node) {
        return "(+ " + Visit((dynamic) node[0])
            + " " + Visit((dynamic) node[1]) + ")"; 
    }

    public String Visit(Times node) {
        return "(* " + Visit((dynamic) node[0])
            + " " + Visit((dynamic) node[1]) + ")";
    }

    public String Visit(Pow node) {
        return "(expt " + Visit((dynamic) node[0])
            + " " + Visit((dynamic) node[1]) + ")";
    }

    public String Visit(Int node) {
        return node.AnchorToken.Lexeme;
    }
}

public class CVisitor {

    public String Visit(Prog node) {
        return @"#include <stdio.h>
#include <math.h>

int main(void) {
    printf(""%d"","
            + Visit((dynamic) node[0])
            + @");
    return 0;
}";
    }

    public String Visit(Plus node) {
        return "(" + Visit((dynamic) node[0])
            + "+" + Visit((dynamic) node[1]) + ")";
    }

    public String Visit(Times node) {
        return "(" + Visit((dynamic) node[0])
            + "*" + Visit((dynamic) node[1]) + ")";
    }

    public String Visit(Pow node) {
        return "((int) pow(" + Visit((dynamic) node[0])
            + ", " + Visit((dynamic) node[1]) + "))";
    }

    public String Visit(Int node) {
        return node.AnchorToken.Lexeme;
    }
}

public class Driver {
    public static void Main() {
        Console.Write("> ");
        var line = Console.ReadLine();
        var parser = new Parser(new Scanner(line).Start().GetEnumerator());
        try {
            var ast = parser.Prog();

            Console.WriteLine("Result of evaluation:\n");
            Console.WriteLine(new EvalVisitor().Visit((dynamic) ast));
            Console.WriteLine();

            Console.WriteLine("Translation to Lisp:\n");
            Console.WriteLine(new LispVisitor().Visit((dynamic) ast));
            Console.WriteLine();

            Console.WriteLine("Translation to C:\n");
            Console.WriteLine(new CVisitor().Visit((dynamic) ast));

        } catch (SyntaxError) {
            Console.WriteLine("Bad syntax!");
        }
    }
}
