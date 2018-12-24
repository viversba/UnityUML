using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using DFA = Antlr4.Runtime.Dfa.DFA;
using System;
using System.IO;
using System.Collections.Generic;
using PL;
using UnityEngine;

public class Program {

    public void Start() {

        try {

            //Debug.Log("Ola k ase");
            string[] files = Directory.GetFiles("./Assets/Antlr4/Grammar", "*.cs");
            foreach (string file in files) {

                //Debug.Log(file + "---------------------------------------");

                //Read the file
                string text = File.ReadAllText(file);

                //Create the lexer
                CSharpLexer lexer = new CSharpLexer(new AntlrInputStream(text));

                var tokens = lexer.GetAllTokens();
                List<IToken> codeTokens = new List<IToken>();
                List<IToken> commentTokens = new List<IToken>();

                var directiveTokens = new List<IToken>();
                var directiveTokenSource = new ListTokenSource(directiveTokens);
                var directiveTokenStream = new CommonTokenStream(directiveTokenSource, CSharpLexer.DIRECTIVE);
                CSharpPreprocessorParser preprocessorParser = new CSharpPreprocessorParser(directiveTokenStream);

                int index = 0;
                bool compiliedTokens = true;
                while (index < tokens.Count) {
                    var token = tokens[index];
                    if (token.Type == CSharpLexer.SHARP) {
                        directiveTokens.Clear();
                        int directiveTokenIndex = index + 1;
                        // Collect all preprocessor directive tokens.
                        while (directiveTokenIndex < tokens.Count &&
                               tokens[directiveTokenIndex].Type != CSharpLexer.Eof &&
                               tokens[directiveTokenIndex].Type != CSharpLexer.DIRECTIVE_NEW_LINE &&
                               tokens[directiveTokenIndex].Type != CSharpLexer.SHARP) {
                            if (tokens[directiveTokenIndex].Channel == CSharpLexer.COMMENTS_CHANNEL) {
                                commentTokens.Add(tokens[directiveTokenIndex]);
                            }
                            else if (tokens[directiveTokenIndex].Channel != Lexer.Hidden) {
                                //Debug.Log(allTokens[directiveTokenIndex] + "  HOLA");
                                directiveTokens.Add(tokens[directiveTokenIndex]);
                            }
                            directiveTokenIndex++;
                        }

                        directiveTokenSource = new ListTokenSource(directiveTokens);
                        directiveTokenStream = new CommonTokenStream(directiveTokenSource, CSharpLexer.DIRECTIVE);
                        preprocessorParser.TokenStream = directiveTokenStream;
                        //preprocessorParser.SetInputStream(directiveTokenStream);
                        preprocessorParser.Reset();
                        // Parse condition in preprocessor directive (based on CSharpPreprocessorParser.g4 grammar).
                        CSharpPreprocessorParser.Preprocessor_directiveContext directive = preprocessorParser.preprocessor_directive();
                        // if true than next code is valid and not ignored.
                        compiliedTokens = directive.value;
                        
                        String directiveStr = tokens[index + 1].Text.Trim();
                        if ("line".Equals(directiveStr) || "error".Equals(directiveStr) || "warning".Equals(directiveStr) || "define".Equals(directiveStr) || "endregion".Equals(directiveStr) || "endif".Equals(directiveStr) || "pragma".Equals(directiveStr)) {
                            //Debug.Log(directiveStr);
                            compiliedTokens = true;
                        }
                        String conditionalSymbol = null;
                        if ("define".Equals(tokens[index + 1].Text)) {
                            // add to the conditional symbols 
                            conditionalSymbol = tokens[index + 2].Text;
                            preprocessorParser.ConditionalSymbols.Add(conditionalSymbol);
                        }
                        if ("undef".Equals(tokens[index + 1].Text)) {
                            conditionalSymbol = tokens[index + 2].Text;
                            preprocessorParser.ConditionalSymbols.Remove(conditionalSymbol);
                        }

                        //This code deletes the directive tokens from the input so that they don't interfere with the parsing process
                        // In all of the cases, we have to remove at least two positions of the tokens array
                        tokens.RemoveAt(directiveTokenIndex - 1);
                        tokens.RemoveAt(directiveTokenIndex - 2);

                        if ("pragma".Equals(directiveStr) || "warning".Equals(directiveStr) || "region".Equals(directiveStr) || "error".Equals(directiveStr)) {
                            // Remove three positions before
                            tokens.RemoveAt(directiveTokenIndex - 3);
                            directiveTokenIndex--;
                        }
                        else if("define".Equals(directiveStr) || "undef".Equals(directiveStr) || "if".Equals(directiveStr) || "elif".Equals(directiveStr) || "line".Equals(directiveStr)) {
                            // Remove four positions before
                            tokens.RemoveAt(directiveTokenIndex - 3);
                            tokens.RemoveAt(directiveTokenIndex - 4);
                            directiveTokenIndex -= 2;
                        }
                        directiveTokenIndex -= 2;
                        index = directiveTokenIndex - 1;
                    }
                    else if (token.Channel == CSharpLexer.COMMENTS_CHANNEL) {
                        commentTokens.Add(token); // Colect comment tokens (if required).
                    }
                    else if (token.Channel != Lexer.Hidden && token.Type != CSharpLexer.DIRECTIVE_NEW_LINE && compiliedTokens) {
                        codeTokens.Add(token); // Collect code tokens.
                    }
                    index++;
                }

                // At second stage tokens parsed in usual way.
                var codeTokenSource = new ListTokenSource(tokens);
                var codeTokenStream = new CommonTokenStream(codeTokenSource);
                CSharpParser parser = new CSharpParser(codeTokenStream);

                ////Create the token stream
                //CommonTokenStream tokens = new CommonTokenStream(lexer);
                //CSharpParser parser = new CSharpParser(tokens);
                IParseTree tree = parser.compilation_unit();

                ////Walk the tree
                ParseTreeWalker walker = new ParseTreeWalker();
                walker.Walk(new ProgressPrinter(), tree);
            }
        }
        catch (Exception e) {
            Debug.LogError("Error (Program.cs): " + e);
        }
    }

    public int hola() {
        return 1;
    }

    public struct nico {
        int hola;
    }
}