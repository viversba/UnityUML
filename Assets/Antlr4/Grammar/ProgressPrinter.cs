using System;
using Antlr4.Runtime.Misc;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace PL {
    public class ProgressPrinter: CSharpParserBaseListener {

        private string className;
        private string methodName;
        private List<string> modifiers;
        private string type;
        private List<string> constants;
        private List<string> variables;

        public ProgressPrinter() {

            methodName = "";
            modifiers = new List<string>();
            type = "";
            className = "";
            constants = new List<string>();
            variables = new List<string>();
        }

        public override void EnterClass_definition([NotNull] CSharpParser.Class_definitionContext context) {
            base.EnterClass_definition(context);
            className = context.identifier().GetText();
            Debug.Log("CLASS: " + className);
        }

        public override void EnterMethod_declaration([NotNull] CSharpParser.Method_declarationContext context) {
            base.EnterMethod_declaration(context);
            methodName = context.method_member_name().GetText();
        }

        public override void ExitMethod_declaration([NotNull] CSharpParser.Method_declarationContext context) {
            foreach(string modifier in modifiers) {
                Debug.Log("Modifier: " + modifier);
            }
            Debug.Log("TYPE: " + type + "\n");
            Debug.Log("NAME: " + methodName + "\n");
            Debug.Log("------------------------------------");
        }

        public override void EnterProperty_declaration([NotNull] CSharpParser.Property_declarationContext context) {
            base.EnterProperty_declaration(context);

            Debug.Log("PROPERTY: " + context.member_name().GetText());
        }

        public override void EnterField_declaration([NotNull] CSharpParser.Field_declarationContext context) {
            base.EnterField_declaration(context);

            variables.Clear();
            try {
                variables = context.variable_declarators().GetText().Split(',').ToList<string>();
                foreach (string variable in variables) {
                    Debug.Log("FIELD: " + variable);
                }
            }
            catch(Exception e) {
                variables.Add(context.variable_declarators().GetText());
                Debug.Log("FIELD: " + variables[0]);
            }
        }

        public override void EnterConstant_declaration([NotNull] CSharpParser.Constant_declarationContext context) {
            base.EnterConstant_declaration(context);

            constants.Clear();
            try {
                constants = context.constant_declarators().GetText().Split(',').ToList<string>();
                foreach (string constant in constants) {
                    Debug.Log("CONSTANT: " + constant);
                }
            }
            catch {
                constants.Add(context.constant_declarators().GetText());
                Debug.Log("CONSTANT: " + constants[0]);
            }

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
