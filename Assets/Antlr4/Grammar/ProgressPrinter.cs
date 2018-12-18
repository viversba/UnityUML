using System;
using Antlr4.Runtime.Misc;
using UnityEngine;

namespace PL {
    public class ProgressPrinter: CSharpParserBaseListener {

        public override void EnterClass_base([NotNull] CSharpParser.Class_baseContext context) {
            base.EnterClass_base(context);
            var name = context.GetText();
            Debug.Log(name);
        }
    }
}
