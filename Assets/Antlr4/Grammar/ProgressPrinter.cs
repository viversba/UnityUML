using System;
using Antlr4.Runtime.Misc;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DEngine.Controller;

namespace DEngine.Model {

    public class ProgressPrinter : CSharpParserBaseListener {

        #region fields

        private string className;
        private string methodName;
        private string structName;
        private List<string> modifiers;
        private string type;
        private string[] subTypes;
        private string rankSpecifier;
        private string propertyName;
        private List<string> constants;
        private List<string> variables;
        private string constructor;
        private List<string[]> parameters;
        private string classAttributes;
        private List<string> classModifiers;
        private List<string[]> classParameters;
        private string interfaceName;
        private string superClassName;
        private string interfaceBase;

        #endregion

        /// <summary>
        /// The entities.
        /// </summary>
        private List<string> entities;
        private ClassWrapper wrapper;

        public ProgressPrinter() {

            propertyName = "";
            methodName = "";
            structName = "";
            modifiers = new List<string>();
            type = "";
            subTypes = null;
            rankSpecifier = null;
            className = "";
            constants = new List<string>();
            variables = new List<string>();
            constructor = "";
            parameters = new List<string[]>();
            classAttributes = "";
            classModifiers = new List<string>();
            classParameters = new List<string[]>();
            interfaceName = "";
            superClassName = "";
            interfaceBase = "";

            entities = new List<string>();
            wrapper = new ClassWrapper();
        }

        #region Class Handling

        public override void EnterClass_definition([NotNull] CSharpParser.Class_definitionContext context) {
            base.EnterClass_definition(context);
            
            classParameters.Clear();
            superClassName = "";
            className = context.identifier().GetText();
            string[] params_ = { "", "" };

            // Class parameters handling
            try {
                var paramList = context.type_parameter_list().type_parameter();
                className += context.type_parameter_list().GetText();
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

            // SuperClass Handling
            try {
                superClassName = context.class_base().class_type().GetText();
            }
            catch(Exception e) {
                ItDoesNothing(e.ToString());
            }

            entities.Add(className);
            wrapper.AddClass(className);

            // Interface handling
            try {
                foreach(var interface_ in context.class_base().namespace_or_type_name()) {
                    try {
                        foreach (var identifier in interface_.identifier()) {
                            //Debug.Log(className + " implements " + identifier.GetText());
                            if (!string.IsNullOrEmpty(identifier.GetText())) {
                                interfaceBase = identifier.GetText();
                                try { 
                                    foreach(var argument in interface_.type_argument_list()) {
                                        interfaceBase += argument.GetText();
                                    }
                                }
                                catch(Exception e) {
                                    ItDoesNothing("Interface argument list in Class:" + e);
                                }
                                wrapper.AddInterfaceToEntity(interfaceBase);
                                break;
                            }
                        }
                    }
                    catch (Exception e) {
                        ItDoesNothing("Nested interface handling in Class " + e);
                    }
                }
            }
            catch(Exception e) {
                ItDoesNothing(e.ToString());
            }
            if(superClassName != "")
                wrapper.SetSuperClassName(superClassName);
        }

        public override void ExitClass_definition([NotNull] CSharpParser.Class_definitionContext context) {

            entities.RemoveAt(entities.Count - 1);
            wrapper.FinishEntity();
        }

        public override void EnterType_declaration([NotNull] CSharpParser.Type_declarationContext context) {
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

            wrapper.AddMethodTo(new Method(methodName, mod, type, methodType, subTypes));

            // TODO: take care of parameters
        }

        public override void EnterProperty_declaration([NotNull] CSharpParser.Property_declarationContext context) {
            base.EnterProperty_declaration(context);

            propertyName = context.member_name().GetText();
        }

        public override void ExitProperty_declaration([NotNull] CSharpParser.Property_declarationContext context) {
            base.ExitProperty_declaration(context);

            AccessModifier mod = AccessModifier.PRIVATE;
            StaticType attributeType = StaticType.NONE;
            ClassWrapper.ModifierMatch(modifiers, ref mod, ref attributeType);

            wrapper.AddAttributeTo(new Attribute(propertyName, mod, type, attributeType, subTypes));
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
            StaticType attributeType = StaticType.NONE;
            ClassWrapper.ModifierMatch(modifiers, ref mod, ref attributeType);

            foreach (var variable in variables) {
                wrapper.AddAttributeTo(new Attribute(variable, mod, type, attributeType, subTypes));
            }
        }

        public override void EnterConstant_declaration([NotNull] CSharpParser.Constant_declarationContext context) {
            base.EnterConstant_declaration(context);

            string test = context.type()?.base_type()?.GetText();
            if (test != null) {
                Debug.Log(test);
            }

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
            StaticType attributeType = StaticType.NONE;
            ClassWrapper.ModifierMatch(modifiers, ref mod, ref attributeType);

            foreach (var constant in constants) {
                wrapper.AddAttributeTo(new Attribute(constant, mod, type, attributeType, subTypes));
            }
        }

        public override void EnterStruct_member_declaration([NotNull] CSharpParser.Struct_member_declarationContext context) {

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
        }

        public override void EnterClass_member_declaration([NotNull] CSharpParser.Class_member_declarationContext context) {

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
        }

        public override void EnterCommon_member_declaration([NotNull] CSharpParser.Common_member_declarationContext context) {

            string type;
            type = context.typed_member_declaration()?.type()?.base_type()?.simple_type()?.GetText();
            if (type == null) {
                type = context.VOID()?.GetText();
                if (type == null) {
                    var typeNames = context.typed_member_declaration()?.type()?.base_type()?.class_type()?.namespace_or_type_name()?.identifier();
                    if (typeNames != null) {
                        foreach (var name in typeNames) {
                            type = name.GetText();
                        }
                        var rawSubTypes = context.typed_member_declaration()?.type()?.base_type()?.class_type()?.namespace_or_type_name()?.type_argument_list();
                        if (rawSubTypes != null && rawSubTypes.Length != 0) {
                            var finalSubTypes = context?.typed_member_declaration()?.type()?.base_type()?.class_type()?.namespace_or_type_name()?.type_argument_list()[0].type();
                            if (finalSubTypes != null && finalSubTypes.Length != 0) {
                                subTypes = new string[finalSubTypes.Length];
                                for (int i = 0; i < finalSubTypes.Length; i++) {
                                    subTypes[i] = finalSubTypes[i].GetText();
                                }
                            }
                            else {
                                subTypes = null;
                            }
                        }
                        else {
                            subTypes = null;
                        }
                    }
                    else {
                        type = context.typed_member_declaration()?.type()?.base_type()?.class_type()?.GetText();
                    }
                }
            }

            this.type = type;

            // Get the rank specifiers
            var rankSpecifiers = context.typed_member_declaration()?.type()?.rank_specifier();
            if (rankSpecifiers != null) {
                rankSpecifier = "";
                foreach (var specifier in rankSpecifiers) {
                    rankSpecifier += specifier.GetText();
                }
            }
            else {
                rankSpecifier = null;
            }
        }

        #endregion

        #region Interface Handling

        public override void EnterInterface_definition([NotNull] CSharpParser.Interface_definitionContext context) {

            interfaceName = context.identifier().GetText();

            // Parameters list
            try {
                interfaceName += context.variant_type_parameter_list().GetText();
            }
            catch(Exception e) {
                ItDoesNothing("Interface declaration parameters " + e);
            }
            interfaceBase = "";

            entities.Add(interfaceName);
            wrapper.AddInterface(interfaceName);

            // Add each interface implementation
            try {
                foreach (var interface_ in context.interface_base().interface_type_list().namespace_or_type_name()) {
                    try {
                        foreach (var identifier in interface_.identifier()) {
                            //Debug.Log(className + " implements " + identifier.GetText());
                            if (!string.IsNullOrEmpty(identifier.GetText())) {
                                interfaceBase = identifier.GetText();
                                try {
                                    foreach (var argument in interface_.type_argument_list()) {
                                        interfaceBase += argument.GetText();
                                    }
                                }
                                catch (Exception e) {
                                    ItDoesNothing("Interface argument list in Interface:" + e);
                                }
                                wrapper.AddInterfaceToEntity(interfaceBase);
                                break;
                            }
                        }
                    }
                    catch (Exception e) {
                        ItDoesNothing("Nested interface handling in Interface " + e);
                    }
                }
            }
            catch(Exception e) {
                ItDoesNothing(e.ToString());
            }
        }

        public override void ExitInterface_definition([NotNull] CSharpParser.Interface_definitionContext context) {
            base.ExitInterface_definition(context);

            entities.RemoveAt(entities.Count - 1);
            wrapper.FinishEntity();
        }

        public override void EnterInterface_member_declaration([NotNull] CSharpParser.Interface_member_declarationContext context) {

            // Type Handling
            try {
                type = context.type().GetText();
            }
            catch {
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

        #region Struct handling

        public override void EnterStruct_definition([NotNull] CSharpParser.Struct_definitionContext context) {

            structName = context.identifier().GetText();
            entities.Add(structName);
            wrapper.AddStruct(structName);
        }

        public override void ExitStruct_definition([NotNull] CSharpParser.Struct_definitionContext context) {

            entities.RemoveAt(entities.Count - 1);
            wrapper.FinishEntity();
        }

        #endregion

        #region Namespace Handling

        public override void EnterNamespace_declaration([NotNull] CSharpParser.Namespace_declarationContext context) {

            // Namespace identification goes from global to specific.
            // For example DEngine.Model will be { DEngine , Model }
            var names = context.qualified_identifier().identifier();
            string[] name = new string[names.Length];
            for(int i=0; i < names.Length; i++) {
                name[i] = names[i].GetText();
            }

            wrapper.AddNamespace(name);
        }

        public override void ExitNamespace_declaration([NotNull] CSharpParser.Namespace_declarationContext context) {

            wrapper.FinishNamespace();
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

