using System;
using UnityEngine;
using Model = DEngine.Model;

namespace DEngine.View {

    public static class Drawer {

        public static GUIStyle widthStyle;
        public static GUIStyle header;
        public static GUIStyle public_;
        public static GUIStyle private_;
        public static GUIStyle protected_;
        public static GUIStyle internal_;

        static Drawer() {

            widthStyle = new GUIStyle();
            header = new GUIStyle();
            public_ = new GUIStyle();
            private_ = new GUIStyle();
            protected_ = new GUIStyle();
            internal_ = new GUIStyle();

            widthStyle.wordWrap = false;
            widthStyle.stretchWidth = false;
            header.normal.textColor = new Color(0.188f, 0.258f, 0.733f, 1f);
            public_.normal.textColor = Color.green;
            private_.normal.textColor = Color.red;
            protected_.normal.textColor = Color.magenta;
            internal_.normal.textColor = Color.cyan;
        }

        public static void DrawMethod(Model::Method method) {

            DrawAccessModifier(method.modifier);
            DrawType(method.returnType);
            GUILayout.Label(method.name, widthStyle);
            GUILayout.Label("(", widthStyle);
            if (method.parameters != null && method.parameters.Count > 0) {
                for (int i = 0; i < method.parameters.Count - 1; i++) {
                    DrawParameter(method.parameters[i]);
                    GUILayout.Label(",", widthStyle);
                }
                DrawParameter(method.parameters[method.parameters.Count - 1]);
            }
            GUILayout.Label(")", widthStyle);
        }

        public static void DrawParameter(Model::Parameter parameter) {

            GUILayout.Label(parameter.type.name, widthStyle);
            if (parameter.type.type != null && parameter.type.type.Count > 0) {
                GUILayout.Label("<", widthStyle);
                for (int i = 0; i < parameter.type.type.Count - 1; i++) {
                    DrawType(parameter.type.type[i]);
                    GUILayout.Label(",", widthStyle);
                }
                Drawer.DrawType(parameter.type.type[parameter.type.type.Count - 1]);
                GUILayout.Label(">", widthStyle);
            }
            GUILayout.Label(parameter.name, widthStyle);
        }

        public static void DrawType(Model::GenericType type) {

            GUILayout.Label(type.name, widthStyle);
            if (type.type != null && type.type.Count > 0) {
                GUILayout.Label("<", widthStyle);
                for (int i = 0; i < type.type.Count - 1; i++) {
                    DrawType(type.type[i]);
                    GUILayout.Label(",", widthStyle);
                }
                DrawType(type.type[type.type.Count - 1]);
                GUILayout.Label(">", widthStyle);
            }
        }

        public static void DrawConstructor(Model::Constructor constructor) {

            DrawAccessModifier(constructor.modifier);
            GUILayout.Label(constructor.name, widthStyle);
            GUILayout.Label("(", widthStyle);
            if (constructor.parameters != null && constructor.parameters.Count > 0) {
                for (int i = 0; i < constructor.parameters.Count - 1; i++) {
                    DrawParameter(constructor.parameters[i]);
                    GUILayout.Label(",", widthStyle);
                }
                DrawParameter(constructor.parameters[constructor.parameters.Count - 1]);
            }
            GUILayout.Label(")", widthStyle);
        }

        public static void DrawAttribute(Model::Attribute attribute) {

            DrawAccessModifier(attribute.modifier);
            DrawType(attribute.returnType);
            GUILayout.Label(attribute.name, widthStyle);
        }

        public static void DrawAccessModifier(Model::AccessModifier accessModifier) {

            switch (accessModifier) {
                case Model::AccessModifier.PRIVATE:
                    GUILayout.Label("- ", private_, GUILayout.Width(5));
                    break;
                case Model::AccessModifier.PUBLIC:
                    GUILayout.Label("+ ", public_, GUILayout.Width(5));
                    break;
                case Model::AccessModifier.PROTECTED:
                    GUILayout.Label("# ", protected_, GUILayout.Width(5));
                    break;
                case Model::AccessModifier.INTERNAL:
                case Model::AccessModifier.PROTECTED_INTERNAL:
                    GUILayout.Label("~ ", internal_, GUILayout.Width(5));
                    break;
            }
        }
    }
}