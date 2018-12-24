using System;
using Antlr4.Runtime.Misc;
using UnityEngine;
using System.Collections.Generic;

namespace PL {
    public class ProgressPrinter: CSharpParserBaseListener {

        private string className;
        private string name;
        private List<string> modifiers;
        private string type;

        public ProgressPrinter() {

            name = "";
            modifiers = new List<string>();
            type = "";
            className = "";
        }

        public override void EnterClass_definition([NotNull] CSharpParser.Class_definitionContext context) {
            base.EnterClass_definition(context);
            className = context.identifier().GetText();
            Debug.Log(className);
        }

        public override void EnterMethod_declaration([NotNull] CSharpParser.Method_declarationContext context) {
            base.EnterMethod_declaration(context);
            name = context.method_member_name().GetText();
        }

        public override void ExitMethod_declaration([NotNull] CSharpParser.Method_declarationContext context) {
            foreach(string modifier in modifiers) {
                Debug.Log("Modifier: " + modifier);
            }
            Debug.Log("TYPE: " + type + "\n");
            Debug.Log("NAME: " + name + "\n");
            Debug.Log("------------------------------------");
        }

        public override void EnterClass_member_declaration([NotNull] CSharpParser.Class_member_declarationContext context) {
            base.EnterClass_member_declaration(context);

            modifiers = new List<string>();
            // Get the access modifiers
            try {
                foreach(var modifier in context.all_member_modifiers().all_member_modifier()) {
                    modifiers.Add(modifier.GetText());
                }
            }
            catch(Exception e) {
                ItDoesNothing();
            }

            // Get the return type of the method
            try {
                try {
                    type = context.common_member_declaration().VOID().GetText();
                }
                catch(Exception e) {

                    type = context.common_member_declaration().typed_member_declaration().type().GetText();
                }
            }
            catch(Exception e) {
                ItDoesNothing();
            }
        }

        private void ItDoesNothing() {}
    }
}
