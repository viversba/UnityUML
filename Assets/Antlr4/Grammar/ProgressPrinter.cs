using System;
using Antlr4.Runtime.Misc;
using UnityEngine;

namespace PL {
    public class ProgressPrinter: CSharpParserBaseListener {

        public override void EnterClass_definition([NotNull] CSharpParser.Class_definitionContext context) {
            base.EnterClass_definition(context);
            var name = context.identifier().IDENTIFIER().GetText();
            Debug.Log("HOLA " + name + ". Ke ase");
        }
    }
}
