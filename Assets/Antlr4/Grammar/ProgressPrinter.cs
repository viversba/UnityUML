using System;
using Antlr4.Runtime.Misc;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace DEngine.Model {
    public class ProgressPrinter : CSharpParserBaseListener {

        private string className;
        private string methodName;
        private List<string> modifiers;
        private string type;
        private string name;
        private List<string> constants;
        private List<string> variables;
        private string constructor;
        private List<string[]> parameters;

        public ProgressPrinter() {

            name = "";
            methodName = "";
            modifiers = new List<string>();
            type = "";
            className = "";
            constants = new List<string>();
            variables = new List<string>();
            constructor = "";
            parameters = new List<string[]>();
        }

        public override void EnterClass_definition([NotNull] CSharpParser.Class_definitionContext context) {
            base.EnterClass_definition(context);
            className = context.identifier().GetText();
            Debug.Log("CLASS: " + className);
        }

        public override void EnterConstructor_declaration([NotNull] CSharpParser.Constructor_declarationContext context) {
            base.EnterConstructor_declaration(context);

            parameters.Clear();
            constructor = context.identifier().GetText();
            string[] pars = new string[2];
            try {
                pars[0] = context.formal_parameter_list().parameter_array().GetText();
                pars[1] = "NONE";
                parameters.Add(pars);
            }
            catch(Exception e) {

                ItDoesNothing("Constructor " + e);
                try {
                    // This is going to hold every parameter of the list of parameters
                    var params_ = context.formal_parameter_list().fixed_parameters().fixed_parameter();
                    foreach (var param in params_) {
                        // For each one, extract the type of parameter and the name
                        pars[0] = param.arg_declaration().type().GetText();
                        pars[1] = param.arg_declaration().identifier().GetText();
                        parameters.Add(pars);
                    }
                }
                catch(Exception e1) {
                    ItDoesNothing("Nested Constructor " + e1);
                }
            }
        }

        public override void ExitConstructor_declaration([NotNull] CSharpParser.Constructor_declarationContext context) {
            base.ExitConstructor_declaration(context);

            foreach (var modifier in modifiers) {
                Debug.Log("MODIFIER: " + modifier);
            }
            Debug.Log("CONSTRUCTOR: " + constructor);
            foreach(var par in parameters) {
                Debug.Log("PARAMETER TYPE: " + par[0]);
                Debug.Log("PARAMETER NAME: " + par[1]);
            }
        }

        public override void EnterMethod_declaration([NotNull] CSharpParser.Method_declarationContext context) {
            base.EnterMethod_declaration(context);
            methodName = context.method_member_name().GetText();

            string[] pars = new string[2];
            try {
                pars[0] = context.formal_parameter_list().parameter_array().GetText();
                pars[1] = "NONE";
                parameters.Add(pars);
            }
            catch (Exception e) {

                ItDoesNothing("Method " + e);
                try {
                    // This is going to hold every parameter of the list of parameters
                    var params_ = context.formal_parameter_list().fixed_parameters().fixed_parameter();
                    foreach (var param in params_) {
                        // For each one, extract the type of parameter and the name
                        pars[0] = param.arg_declaration().type().GetText();
                        pars[1] = param.arg_declaration().identifier().GetText();
                        parameters.Add(pars);
                    }
                }
                catch (Exception e1) {
                    ItDoesNothing("Nested Method " + e1);
                }
            }
        }

        public override void ExitMethod_declaration([NotNull] CSharpParser.Method_declarationContext context) {
            foreach (string modifier in modifiers) {
                //Debug.Log("Modifier: " + modifier);
            }
            //Debug.Log("TYPE: " + type + "\n");
            //Debug.Log("NAME: " + methodName + "\n");
            //Debug.Log("------------------------------------");
        }

        public override void EnterProperty_declaration([NotNull] CSharpParser.Property_declarationContext context) {
            base.EnterProperty_declaration(context);

            name = context.member_name().GetText();
            //Debug.Log("PROPERTY: " + context.member_name().GetText());
        }

        public override void ExitProperty_declaration([NotNull] CSharpParser.Property_declarationContext context) {
            base.ExitProperty_declaration(context);

            //foreach(var modifier in modifiers) {
            //    Debug.Log("MODIFIER: " + modifier);
            //}
            //Debug.Log("TYPE: " + type + "\n");
            //Debug.Log("NAME: " + name + "\n");
        }

        public override void EnterField_declaration([NotNull] CSharpParser.Field_declarationContext context) {
            base.EnterField_declaration(context);

            // Clear the current variables array
            variables.Clear();
            try {
                //Get the whole variable declarations array
                var vars = context.variable_declarators().variable_declarator();
                foreach (var variable in vars) {
                    // Add all of the one by one to the global variables array
                    variables.Add(variable.identifier().GetText());
                }
            }
            catch (Exception e) {
                ItDoesNothing("Field " + e.ToString());
            }
        }

        public override void ExitField_declaration([NotNull] CSharpParser.Field_declarationContext context) {
            base.ExitField_declaration(context);

            //foreach (var modifier in modifiers) {
            //    Debug.Log("MODIFIER: " + modifier);
            //}
            //Debug.Log("TYPE: " + type + "\n");
            //foreach (var field in variables) {
            //    Debug.Log("VARIABLE: " + field);
            //}
        }

        public override void EnterConstant_declaration([NotNull] CSharpParser.Constant_declarationContext context) {
            base.EnterConstant_declaration(context);

            constants.Clear();
            try {

                type = context.type().GetText();
                var consts = context.constant_declarators().constant_declarator();
                foreach (var constant in consts) {
                    constants.Add(constant.identifier().GetText());
                }
            }
            catch (Exception e) {
                ItDoesNothing("Constant " + e.ToString());
            }
        }

        public override void ExitConstant_declaration([NotNull] CSharpParser.Constant_declarationContext context) {
            base.ExitConstant_declaration(context);

            //foreach (var modifier in modifiers) {
            //    Debug.Log("MODIFIER: " + modifier);
            //}
            //Debug.Log("const\nTYPE: " + type + "\n");
            //foreach (var constant in constants) {
            //    Debug.Log("CONSTANT: " + constant);
            //}
        }

        public override void EnterClass_member_declaration([NotNull] CSharpParser.Class_member_declarationContext context) {
            base.EnterClass_member_declaration(context);

            modifiers.Clear();
            // Get the access modifiers
            try {
                foreach (var modifier in context.all_member_modifiers().all_member_modifier()) {
                    modifiers.Add(modifier.GetText());
                }
            }
            catch (Exception e) {
                ItDoesNothing("Member " + e.ToString());
            }

            // Get the return type
            try {
                try {
                    type = context.common_member_declaration().VOID().GetText();
                }
                catch (Exception e) {

                    type = context.common_member_declaration().typed_member_declaration().type().GetText();
                    ItDoesNothing("Nested return type " + e.ToString());
                }
            }
            catch (Exception e) {
                ItDoesNothing("Return type " + e.ToString());
            }
        }

        private void ItDoesNothing(string e) {

            //Debug.Log(e);
        }
    }
}
