using System;
using Antlr4.Runtime.Misc;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DEngine.Controller;

namespace DEngine.Model {

    public class DEngineListener : CSharpParserBaseListener {

        #region fields

        #region modifiers
        private bool PARTIAL;
        private bool STATIC;
        private bool PUBLIC;
        private bool PRIVATE;
        private bool PROTECTED;
        private bool INTERNAL;
        private bool READONLY;
        private bool VOLATILE;
        private bool VIRTUAL;
        private bool SEALED;
        private bool OVERRIDE;
        private bool ABSTRACT;
        private bool EXTERN;
        private bool UNSAFE;
        private bool ASYNC;
        #endregion

        private string className;
        private string methodName;
        private string structName;
        private List<string> modifiers;
        private GenericType type;
        private string[] subTypes;
        private string propertyName;
        private List<string> constants;
        private List<string> variables;
        private string constructor;
        private List<Parameter> parameters;
        private string classAttributes;
        private List<string> classModifiers;
        private List<string> classParameters;
        private List<string> interfaceParameters;
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

            PARTIAL = false;
            STATIC = false;
            propertyName = "";
            methodName = "";
            structName = "";
            modifiers = new List<string>();
            type = new GenericType("");
            subTypes = null;
            className = "";
            constants = new List<string>();
            variables = new List<string>();
            constructor = "";
            parameters = new List<Parameter>();
            classAttributes = "";
            classModifiers = new List<string>();
            classParameters = new List<string>();
            interfaceParameters = new List<string>();
            interfaceName = "";
            superClassName = "";
            interfaceBase = "";

            entities = new List<string>();
            wrapper = new Wrapper();
        }

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

            // Add the class to the List as soon as detected
            entities.Add(className);
            wrapper.AddClass(className);
            wrapper.AddParametersToModel(classParameters);
            if (PARTIAL) wrapper.IsPartial(PARTIAL);


            // Super Class handling
            var classBase = context.class_base();
            if(classBase != null) {
                ImplementedType superClass = ResolveSuperClass(classBase);
                wrapper.SetSuperClassName(superClass);
            }

            var interfaces = context.class_base()?.namespace_or_type_name();
            if(interfaces != null && interfaces.Length > 0) {
                List<ImplementedType> implementedInterfaces = new List<ImplementedType>();
                foreach(var implementedInterface in interfaces) {
                    wrapper.AddInterfaceToEntity(ResolveInterface(implementedInterface));
                }
            }
        }

        public override void ExitClass_definition([NotNull] CSharpParser.Class_definitionContext context) {

            entities.RemoveAt(entities.Count - 1);
            wrapper.FinishEntity();
        }

        public override void EnterType_declaration([NotNull] CSharpParser.Type_declarationContext context) {
            var modifiers = context.all_member_modifiers();
            if(modifiers != null) {
                ResolveModifier(modifiers);
            }
        }

        public override void ExitType_declaration([NotNull] CSharpParser.Type_declarationContext context) {
            ResetModifiers();
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

            propertyName = context.member_name().GetText();
        }

        public override void ExitProperty_declaration([NotNull] CSharpParser.Property_declarationContext context) {

            AccessModifier mod = AccessModifier.PRIVATE;
            StaticType attributeType = StaticType.NONE;
            Wrapper.ModifierMatch(modifiers, ref mod, ref attributeType);

            wrapper.AddAttributeTo(new Attribute(propertyName, type));
        }

        public override void EnterField_declaration([NotNull] CSharpParser.Field_declarationContext context) {

            variables.Clear();

            var fields = context.variable_declarators()?.variable_declarator();
            if(fields != null) {
                foreach (var field in fields) {
                    variables.Add(field.identifier().GetText());
                }
            }
        }

        public override void ExitField_declaration([NotNull] CSharpParser.Field_declarationContext context) {

            AccessModifier mod = AccessModifier.PRIVATE;
            StaticType attributeType = StaticType.NONE;
            Wrapper.ModifierMatch(modifiers, ref mod, ref attributeType);

            foreach (var variable in variables) {
                wrapper.AddAttributeTo(new Attribute(variable, type));
            }
        }

        public override void EnterConstant_declaration([NotNull] CSharpParser.Constant_declarationContext context) {

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

        public override void ExitClass_member_declaration([NotNull] CSharpParser.Class_member_declarationContext context) {
            ResetModifiers();
        }

        #endregion

        #region Interface Handling

        public override void EnterInterface_definition([NotNull] CSharpParser.Interface_definitionContext context) {

            interfaceBase = "";
            interfaceName = context.identifier().GetText();

            // Handle Parameters
            var parameters = context.variant_type_parameter_list()?.variant_type_parameter();
            if(parameters != null) {
                foreach (var parameter in parameters) {
                    interfaceParameters.Add(parameter.GetText());
                }
            }

            entities.Add(interfaceName);
            wrapper.AddInterface(interfaceName);
            wrapper.AddParametersToModel(interfaceParameters);

            // Base interfaces handling
            var bases = context.interface_base();
            if(bases != null) {
                List<ImplementedType> interfaces = new List<ImplementedType>();
                foreach(var implementedInterface in context.interface_base().interface_type_list().namespace_or_type_name()) {
                    wrapper.AddInterfaceToEntity(ResolveInterface(implementedInterface));
                }
            }
        }

        public override void ExitInterface_definition([NotNull] CSharpParser.Interface_definitionContext context) {
            entities.RemoveAt(entities.Count - 1);
            wrapper.FinishEntity();
        }

        public override void EnterInterface_member_declaration([NotNull] CSharpParser.Interface_member_declarationContext context) {

            // Type Handling
            var type = context.type();
            if(type != null)
                this.type = ResolveTypes(context.type());
            else
                this.type = new GenericType("void");

            // Name Handling
            methodName = context.identifier().GetText();

            var parameters = context.formal_parameter_list();
            if (parameters != null) {
                ResolveParameters(parameters);
            }
        }

        public override void ExitInterface_member_declaration([NotNull] CSharpParser.Interface_member_declarationContext context) {

            wrapper.AddMethodTo(new Method(methodName, type, parameters));
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

            // Resolve modifiers
            this.modifiers.Clear();
            var modifiers = context.all_member_modifiers();
            if(modifiers != null) {
                ResolveModifier(modifiers);
            }
        }

        #endregion

        #region common

        public override void EnterCommon_member_declaration([NotNull] CSharpParser.Common_member_declarationContext context) {

            if (context.typed_member_declaration()?.type() != null) {
                GenericType genericType = ResolveTypes(context.typed_member_declaration()?.type());
                type = genericType;
            }
        }

        public GenericType ResolveTypes(CSharpParser.TypeContext context) {

            string type;
            GenericType genericType = new GenericType("");

            // Type handling
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

            // Rank specifier handling
            genericType.rankSpecifier = context.rank_specifier() != null ? context.rank_specifier().Length : genericType.rankSpecifier;

            return genericType;
        }

        public GenericType ResolveArrayTypes(CSharpParser.Array_typeContext context) {

            string type;
            GenericType genericType = new GenericType("");

            // Type handling
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

            // Rank specifier handling
            genericType.rankSpecifier = context.rank_specifier() != null ? context.rank_specifier().Length : genericType.rankSpecifier;

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
            if (modifiers != null) {
                foreach (var modifier in modifiers) {
                    PARTIAL = modifier.PARTIAL() != null;
                    STATIC = modifier.STATIC() != null;
                    PUBLIC = modifier.PUBLIC() != null;
                    PRIVATE = modifier.PRIVATE() != null;
                    PROTECTED = modifier.PROTECTED() != null;
                    INTERNAL = modifier.INTERNAL() != null;
                    READONLY = modifier.READONLY() != null;
                    VOLATILE = modifier.VOLATILE() != null;
                    VIRTUAL = modifier.VIRTUAL() != null;
                    SEALED = modifier.SEALED() != null;
                    OVERRIDE = modifier.OVERRIDE() != null;
                    ABSTRACT = modifier.ABSTRACT() != null;
                    EXTERN = modifier.EXTERN() != null;
                    UNSAFE = modifier.UNSAFE() != null;
                    ASYNC = modifier.ASYNC() != null;
                }
            }
        }

        public ImplementedType ResolveSuperClass(CSharpParser.Class_baseContext context) {

            ImplementedType implementedType = new ImplementedType("");
            string name = "";
            var genericType = context.class_type()?.namespace_or_type_name();
            if(genericType != null) {
                var identifiers = genericType.identifier();
                if(identifiers != null && identifiers.Length > 0) { 
                    foreach(var identifier in identifiers) {
                        if (!string.IsNullOrEmpty(identifier.GetText())) {
                            name = identifier.GetText();
                            break;
                        }
                    }
                }

                var argumentsList = genericType.type_argument_list();
                if(argumentsList != null && argumentsList.Length > 0) { 
                    foreach(var argumentList in argumentsList) {
                        var arguments = argumentList.type();
                        if(arguments != null && arguments.Length > 0) { 
                            foreach(var argument in arguments) {
                                implementedType.AddType(argument.GetText());
                            }
                        }
                    }
                }
            }
            else { 
                if(context.class_type()?.OBJECT() != null) {
                    implementedType = new ImplementedType(context.class_type().OBJECT().GetText());
                }
                else if (context.class_type()?.DYNAMIC() != null) {
                    implementedType = new ImplementedType(context.class_type().DYNAMIC().GetText());
                }
                else if (context.class_type()?.STRING() != null) {
                    implementedType = new ImplementedType(context.class_type().STRING().GetText());
                }
            }

            return implementedType;
        }

        public ImplementedType ResolveInterface(CSharpParser.Namespace_or_type_nameContext context) {

            ImplementedType interface_ = new ImplementedType();
            string name = "";
            var identifiers = context.identifier();
            if (identifiers != null && identifiers.Length > 0) {
                foreach (var identifier in identifiers) {
                    if (!string.IsNullOrEmpty(identifier.GetText())) {
                        name = identifier.GetText();
                        Debug.Log(name);
                        break;
                    }
                }
            }

            var argumentsList = context.type_argument_list();
            if (argumentsList != null && argumentsList.Length > 0) {
                foreach (var argumentList in argumentsList) {
                    var arguments = argumentList.type();
                    if (arguments != null && arguments.Length > 0) {
                        foreach (var argument in arguments) {
                            Debug.Log(argument.GetText());
                            interface_.AddType(argument.GetText());
                        }
                    }
                }
            }
            return interface_;
        }

        public void ResetModifiers() {

            PARTIAL = false;
            STATIC = false;
            PUBLIC = false;
            PRIVATE = false;
            PROTECTED = false;
            INTERNAL = false;
            READONLY = false;
            VOLATILE = false;
            VIRTUAL = false;
            SEALED = false;
            OVERRIDE = false;
            ABSTRACT = false;
            EXTERN = false;
            UNSAFE = false;
            ASYNC = false;
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
    }
}

