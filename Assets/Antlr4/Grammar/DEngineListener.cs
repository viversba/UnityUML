using System;
using Antlr4.Runtime.Misc;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DEngine.Controller;

namespace DEngine.Model {

    public class DEngineListener : CSharpParserBaseListener {

        #region fields

        private bool isPartial;
        private bool isStatic;
        private string className;
        private string methodName;
        private string structName;
        private List<string> modifiers;
        private GenericType type;
        private string[] subTypes;
        private string rankSpecifier;
        private string propertyName;
        private List<string> constants;
        private List<string> variables;
        private string constructor;
        private List<Parameter> parameters;
        private string classAttributes;
        private List<string> classModifiers;
        private List<string> classParameters;
        private string interfaceName;
        private string superClassName;
        private string interfaceBase;

        #endregion

        /// <summary>
        /// The entities.
        /// </summary>
        private List<string> entities;
        private Wrapper wrapper;

        public DEngineListener() {

            isPartial = false;
            isStatic = false;
            propertyName = "";
            methodName = "";
            structName = "";
            modifiers = new List<string>();
            type = new GenericType("");
            subTypes = null;
            rankSpecifier = null;
            className = "";
            constants = new List<string>();
            variables = new List<string>();
            constructor = "";
            parameters = new List<Parameter>();
            classAttributes = "";
            classModifiers = new List<string>();
            classParameters = new List<string>();
            interfaceName = "";
            superClassName = "";
            interfaceBase = "";

            entities = new List<string>();
            wrapper = new Wrapper();
        }


        #region Type handling

        public override void EnterNamespace_member_declaration([NotNull] CSharpParser.Namespace_member_declarationContext context) {

            var typeDeclaration = context.type_declaration();
            if(typeDeclaration != null) { 
                if(typeDeclaration.class_definition() != null || typeDeclaration.struct_definition() != null || typeDeclaration.interface_definition() != null) {
                    var modifiers = typeDeclaration.all_member_modifiers()?.all_member_modifier();
                    if (modifiers != null) { 
                        foreach(var modifier in modifiers) { 
                            if(modifier.PARTIAL() != null) {
                                isPartial = true;
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Class Handling

        #region definition

        public override void EnterClass_definition([NotNull] CSharpParser.Class_definitionContext context) {
            base.EnterClass_definition(context);

            classParameters.Clear();
            superClassName = "";
            className = context.identifier().GetText();

            // Class parameters handling
            var paramList = context.type_parameter_list()?.type_parameter();
            if (paramList != null) {
                foreach (var param in paramList) {
                    classParameters.Add(param.identifier().GetText());
                }
            }

            // Super Class handling
            var supClassName = context.class_base()?.class_type()?.GetText();
            superClassName = supClassName ?? "";

            entities.Add(className);
            wrapper.AddClass(className);
            wrapper.AddParametersToModel(classParameters);


            var interfaces = context.class_base()?.namespace_or_type_name();
            if (interfaces != null) {
                foreach (var interface_ in interfaces) {
                    Debug.Log(interface_.GetText());
                }
            }

            // Interface handling
            try {
                foreach (var interface_ in context.class_base().namespace_or_type_name()) {
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
            catch (Exception e) {
                ItDoesNothing(e.ToString());
            }
            if (superClassName != "")
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

            var modifiers = context.all_member_modifiers();
            ResolveModifier(modifiers);
        }

        #endregion

        #region constructor

        public override void EnterConstructor_declaration([NotNull] CSharpParser.Constructor_declarationContext context) {
            base.EnterConstructor_declaration(context);

            this.parameters.Clear();
            constructor = context.identifier().GetText();

            // Parameter handling
            var parameters = context.formal_parameter_list();
            if(parameters != null) {
                ResolveParameters(parameters);
            }
        }

        public override void ExitConstructor_declaration([NotNull] CSharpParser.Constructor_declarationContext context) {

            //Wrapper.ModifierMatch(modifiers, ref mod, ref methodType);

            wrapper.AddConstructor(new Constructor(constructor, parameters));
        }

        #endregion

        #region methods

        public override void EnterMethod_declaration([NotNull] CSharpParser.Method_declarationContext context) {

            this.parameters.Clear();
            methodName = context.method_member_name().GetText();

            // Parameter handling
            var parameters = context.formal_parameter_list();
            if (parameters != null) {
                ResolveParameters(parameters);
            }
        }

        public override void ExitMethod_declaration([NotNull] CSharpParser.Method_declarationContext context) {
        
            //Wrapper.ModifierMatch(modifiers, ref mod, ref methodType);

            wrapper.AddMethodTo(new Method(methodName, type, parameters));
        }

        #endregion

        #region properties-fields-constants

        public override void EnterProperty_declaration([NotNull] CSharpParser.Property_declarationContext context) {
            base.EnterProperty_declaration(context);

            propertyName = context.member_name().GetText();
        }

        public override void ExitProperty_declaration([NotNull] CSharpParser.Property_declarationContext context) {
            base.ExitProperty_declaration(context);

            AccessModifier mod = AccessModifier.PRIVATE;
            StaticType attributeType = StaticType.NONE;
            Wrapper.ModifierMatch(modifiers, ref mod, ref attributeType);

            wrapper.AddAttributeTo(new Attribute(propertyName, type));
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
            Wrapper.ModifierMatch(modifiers, ref mod, ref attributeType);

            foreach (var variable in variables) {
                wrapper.AddAttributeTo(new Attribute(variable, type));
            }
        }

        public override void EnterConstant_declaration([NotNull] CSharpParser.Constant_declarationContext context) {
            base.EnterConstant_declaration(context);

            constants.Clear();
            type = ResolveTypes(context.type());

            var consts = context.constant_declarators()?.constant_declarator();
            if(consts != null) {
                foreach (var constant in consts) {
                    constants.Add(constant.identifier().GetText());
                }
            }
        }

        public override void ExitConstant_declaration([NotNull] CSharpParser.Constant_declarationContext context) {
            base.ExitConstant_declaration(context);

            //Wrapper.ModifierMatch(modifiers, ref mod, ref attributeType);

            foreach (var constant in constants) {
                wrapper.AddAttributeTo(new Attribute(constant, type));
            }
        }

        #endregion

        public override void EnterClass_member_declaration([NotNull] CSharpParser.Class_member_declarationContext context) {

            this.modifiers.Clear();
            // Get the access modifiers
            var modifiers = context.all_member_modifiers();
            if(modifiers != null) {
                ResolveModifier(modifiers);
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
            //try {
            //    type = context.type().GetText();
            //}
            //catch {
            //    type = context.VOID().GetText();
            //}

            //// Name Handling
            //methodName = context.identifier().GetText();

            //// Parameters Handling
            //string[] pars = new string[2];
            //try {
            //    pars[0] = context.formal_parameter_list().parameter_array().GetText();
            //    pars[1] = "NONE";
            //    parameters.Add(pars);
            //}
            //catch (Exception e) {

            //    ItDoesNothing("Method " + e);
            //    try {
            //        // This is going to hold every parameter of the list of parameters
            //        var params_ = context.formal_parameter_list().fixed_parameters().fixed_parameter();
            //        foreach (var param in params_) {
            //            // For each one, extract the type of parameter and the name
            //            pars[0] = param.arg_declaration().type().GetText();
            //            pars[1] = param.arg_declaration().identifier().GetText();
            //            parameters.Add(pars);
            //        }
            //    }
            //    catch (Exception e1) {
            //        ItDoesNothing("Nested Method " + e1);
            //    }
            //}
        }

        public override void ExitInterface_member_declaration([NotNull] CSharpParser.Interface_member_declarationContext context) {

            //wrapper.AddMethodTo(new Method(methodName, AccessModifier.NONE, type, MethodType.NONE));
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

        #endregion

        #region common

        public override void EnterCommon_member_declaration([NotNull] CSharpParser.Common_member_declarationContext context) {

            if (context.typed_member_declaration()?.type() != null) {
                GenericType genericType = ResolveTypes(context.typed_member_declaration()?.type());
                type = genericType;
            }

            // Get the rank specifiers
            var rankSpecifiers = context.typed_member_declaration()?.type()?.rank_specifier();
            if (rankSpecifiers != null) {
                rankSpecifier = "";
                Debug.Log("Rank specifiers length: " + rankSpecifiers.Length);
                foreach (var specifier in rankSpecifiers) {
                    rankSpecifier += specifier.GetText();
                }
            }
            else {
                rankSpecifier = null;
            }
        }

        public GenericType ResolveTypes(CSharpParser.TypeContext context) {

            string type;
            GenericType genericType = new GenericType("");
            type = context.base_type()?.simple_type()?.GetText();
            if(type == null) {
                type = context.base_type()?.VOID()?.GetText();
                if (type == null) {
                    var typeNames = context.base_type()?.class_type()?.namespace_or_type_name()?.identifier();
                    if(typeNames != null) { 
                        foreach(var name in typeNames) {
                            if (!string.IsNullOrEmpty(name.GetText())) {
                                type = name.GetText();
                                genericType = new GenericType(type);
                                break;
                            }
                        }
                        var rawSubtypes = context.base_type()?.class_type()?.namespace_or_type_name()?.type_argument_list();
                        if(rawSubtypes != null && rawSubtypes.Length > 0) {
                            var finalSubTypes = context.base_type()?.class_type()?.namespace_or_type_name()?.type_argument_list()[0].type();
                            if(finalSubTypes != null && finalSubTypes.Length > 0) { 
                                for(int i = 0; i < finalSubTypes.Length; i++) {
                                    genericType.AddGenericType(ResolveTypes(finalSubTypes[i]));
                                }
                            }
                        }
                    }
                    else {
                        type = context.base_type()?.class_type()?.GetText();
                    }
                }
            }
            return genericType;
        }

        public GenericType ResolveArrayTypes(CSharpParser.Array_typeContext context) {

            string type;
            GenericType genericType = new GenericType("");
            type = context.base_type()?.simple_type()?.GetText();
            if (type == null) {
                type = context.base_type()?.VOID()?.GetText();
                if (type == null) {
                    var typeNames = context.base_type()?.class_type()?.namespace_or_type_name()?.identifier();
                    if (typeNames != null) {
                        foreach (var name in typeNames) {
                            if (!string.IsNullOrEmpty(name.GetText())) {
                                type = name.GetText();
                                genericType = new GenericType(type);
                                break;
                            }
                        }
                        var rawSubtypes = context.base_type()?.class_type()?.namespace_or_type_name()?.type_argument_list();
                        if (rawSubtypes != null && rawSubtypes.Length > 0) {
                            var finalSubTypes = context.base_type()?.class_type()?.namespace_or_type_name()?.type_argument_list()[0].type();
                            if (finalSubTypes != null && finalSubTypes.Length > 0) {
                                for (int i = 0; i < finalSubTypes.Length; i++) {
                                    genericType.AddGenericType(ResolveTypes(finalSubTypes[i]));
                                }
                            }
                        }
                    }
                    else {
                        type = context.base_type()?.class_type()?.GetText();
                    }
                }
            }
            return genericType;
        }

        public void ResolveParameters(CSharpParser.Formal_parameter_listContext context) {

            var params_ = context.fixed_parameters()?.fixed_parameter();
            if (params_ != null) {
                foreach (var param in params_) {
                    string name = param.arg_declaration()?.identifier()?.GetText();
                    if (!string.IsNullOrEmpty(name)) {
                        GenericType type = ResolveTypes(param.arg_declaration().type());
                        Parameter parameter = new Parameter(type, name, false);
                        this.parameters.Add(parameter);
                    }
                }
            }

            // With the params keyword
            var params_array = context.parameter_array();
            if (params_array != null) {
                string name = context.parameter_array()?.identifier()?.GetText();
                if (!string.IsNullOrEmpty(name)) {
                    GenericType arrayType = ResolveArrayTypes(context.parameter_array()?.array_type());
                    Parameter parameter = new Parameter(arrayType, name, true);
                    this.parameters.Add(parameter);
                }
            }
        }

        public void ResolveModifier(CSharpParser.All_member_modifiersContext context) {

            var modifiers = context.all_member_modifier();
            if(modifiers != null) {
                foreach (var modifier in modifiers) {
                    Debug.Log(modifier.GetText() + "    " + className);
                }
            }
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

