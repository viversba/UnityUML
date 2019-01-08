using System;
using Antlr4.Runtime.Misc;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DEngine.Controller;

namespace DEngine.Model {

    public class ProgressPrinter : CSharpParserBaseListener {

        private string className;
        private string methodName;
        private List<string> modifiers;
        private string type;
        private string propertyName;
        private List<string> constants;
        private List<string> variables;
        private string constructor;
        private List<string[]> parameters;
        private string classAttributes;
        private List<string> classModifiers;
        private List<string[]> classParameters;
        private string interfaceName;
        
        /// <summary>
        /// The entities.
        /// </summary>
        private List<string> entities;
        private ClassWrapper wrapper;

        public ProgressPrinter() {

            propertyName = "";
            methodName = "";
            modifiers = new List<string>();
            type = "";
            className = "";
            constants = new List<string>();
            variables = new List<string>();
            constructor = "";
            parameters = new List<string[]>();
            classAttributes = "";
            classModifiers = new List<string>();
            classParameters = new List<string[]>();
            interfaceName = "";

            entities = new List<string>();

            wrapper = new ClassWrapper();
        }

        #region Class Handling

        public override void EnterClass_definition([NotNull] CSharpParser.Class_definitionContext context) {
            base.EnterClass_definition(context);
            
            classParameters.Clear();
            className = context.identifier().GetText();
            string[] params_ = { "", "" };
            try {
                var paramList = context.type_parameter_list().type_parameter();
                foreach (var param in paramList) {
                    try {
                        params_[0] = param.attributes().GetText();
                    }
                    catch (Exception e) {

                        ItDoesNothing("Class parameters Nested" + e);
                    }
                    params_[1] = param.identifier().GetText();
                    classParameters.Add(params_);
                }
            }
            catch (Exception e) {
                ItDoesNothing("Class parameters" + e);
            }

            entities.Add(className);
            wrapper.AddClass(className);
        }

        public override void ExitClass_definition([NotNull] CSharpParser.Class_definitionContext context) {
            base.ExitClass_definition(context);

            entities.RemoveAt(entities.Count - 1);
            wrapper.FinishEntity();
        }

        public override void EnterType_declaration([NotNull] CSharpParser.Type_declarationContext context) {
            base.EnterType_declaration(context);
            classModifiers.Clear();

            // Get the attributes
            try {
                classAttributes = context.attributes().GetText();
            }
            catch (Exception e) {
                ItDoesNothing("Class type " + e);
            }

            // Get the class modifiers
            try {
                var members = context.all_member_modifiers().all_member_modifier();
                foreach (var member in members) {
                    classModifiers.Add(member.GetText());
                }
            }
            catch (Exception e) {
                ItDoesNothing("Class modifier " + e);
            }
        }

        public override void EnterConstructor_declaration([NotNull] CSharpParser.Constructor_declarationContext context) {
            base.EnterConstructor_declaration(context);

            parameters.Clear();
            constructor = context.identifier().GetText();
            string[] pars = { "", "" };
            try {
                pars[0] = context.formal_parameter_list().parameter_array().GetText();
                pars[1] = "";
                parameters.Add(pars);
            }
            catch (Exception e) {

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
                catch (Exception e1) {
                    ItDoesNothing("Nested Constructor " + e1);
                }
            }
        }

        public override void ExitConstructor_declaration([NotNull] CSharpParser.Constructor_declarationContext context) {
            base.ExitConstructor_declaration(context);

            AccessModifier mod = AccessModifier.INTERNAL;
            MethodType methodType = MethodType.NONE;
            ClassWrapper.ModifierMatch(modifiers, ref mod, ref methodType);

            wrapper.AddConstructor(new Constructor(constructor, mod, methodType));

            //TODO add parameters to constructors
            //foreach(var par in parameters) {
            //    Debug.Log("PARAMETER TYPE: " + par[0]);
            //    Debug.Log("PARAMETER NAME: " + par[1]);
            //}
        }

        public override void EnterMethod_declaration([NotNull] CSharpParser.Method_declarationContext context) {
            base.EnterMethod_declaration(context);
            parameters.Clear();
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

            AccessModifier mod = AccessModifier.PRIVATE;
            MethodType methodType = MethodType.NONE;
            ClassWrapper.ModifierMatch(modifiers, ref mod, ref methodType);

            wrapper.AddMethodTo(new Method(methodName, mod, type, methodType));

            // TODO: take care of parameters
        }

        public override void EnterProperty_declaration([NotNull] CSharpParser.Property_declarationContext context) {
            base.EnterProperty_declaration(context);

            propertyName = context.member_name().GetText();
        }

        public override void ExitProperty_declaration([NotNull] CSharpParser.Property_declarationContext context) {
            base.ExitProperty_declaration(context);

            AccessModifier mod = AccessModifier.PRIVATE;
            AttributeType attributeType = AttributeType.NONE;
            ClassWrapper.ModifierMatch(modifiers, ref mod, ref attributeType);

            wrapper.AddAttributeTo(new Attribute(propertyName, mod, type, attributeType));
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


            AccessModifier mod = AccessModifier.PRIVATE;
            AttributeType attributeType = AttributeType.NONE;
            ClassWrapper.ModifierMatch(modifiers, ref mod, ref attributeType);

            foreach (var variable in variables) {
                wrapper.AddAttributeTo(new Attribute(variable, mod, type, attributeType));
            }
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

            AccessModifier mod = AccessModifier.PRIVATE;
            AttributeType attributeType = AttributeType.NONE;
            ClassWrapper.ModifierMatch(modifiers, ref mod, ref attributeType);

            foreach (var constant in constants) {
                wrapper.AddAttributeTo(new Attribute(constant, mod, type, attributeType));
            }
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

        #endregion

        #region Interface Handling

        public override void EnterInterface_definition([NotNull] CSharpParser.Interface_definitionContext context) {
            base.EnterInterface_definition(context);

            interfaceName = context.identifier().GetText();

            entities.Add(interfaceName);
            wrapper.AddInterface(interfaceName);
        }

        public override void ExitInterface_definition([NotNull] CSharpParser.Interface_definitionContext context) {
            base.ExitInterface_definition(context);

            entities.RemoveAt(entities.Count - 1);
            wrapper.FinishEntity();
        }

        public override void EnterInterface_member_declaration([NotNull] CSharpParser.Interface_member_declarationContext context) {

            //Debug.Log(interfaceName);

            // Type Handling
            try {
                type = context.type().GetText();
            }
            catch(Exception e) {
                type = context.VOID().GetText();
            }

            // Name Handling
            methodName = context.identifier().GetText();

            // Parameters Handling
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

        public override void ExitInterface_member_declaration([NotNull] CSharpParser.Interface_member_declarationContext context) {

            wrapper.AddMethodTo(new Method(methodName, AccessModifier.NONE, type, MethodType.NONE));
        }

        #endregion

        public List<BaseModel> GetAllEntities() {
            return wrapper.allEntities;
        }

        private void ItDoesNothing(string e) {

            //Debug.Log(e);
        }
    }
}

