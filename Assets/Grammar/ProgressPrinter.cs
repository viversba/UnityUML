using System;
using Antlr4.Runtime.Misc;
using UnityEngine;

namespace PL {
    public class ProgressPrinter: CSharpParserBaseListener {


        public override void EnterUsing_directives([NotNull] CSharpParser.Using_directivesContext context) {
            base.EnterUsing_directives(context);
            string name = context.using_directive(0).GetText();
            Debug.Log(name);
        }

        public override void EnterClass_definition([NotNull] CSharpParser.Class_definitionContext context) {
            base.EnterClass_definition(context);
            var name = context.ToString();
            Debug.Log(name);
        }
    }
}
