using DEngine.Model;

public class ClassWrapper{

    public static ClassModel currentClass;

    public ClassWrapper() {

        currentClass = new ClassModel();
    }

    public void SetName(string name) {

        currentClass.SetName(name);
    }

    public void AddMethod(Method method) {

        currentClass.AddMethod(method);
    }

    public void AddAttribute(Attribute attribute) {

        currentClass.AddAttribute(attribute);
    }

    public void AddInterface(InterfaceModel interface_) {

        currentClass.AddInterface(interface_);
    }

    public void AddConstructor(Constructor constructor) {

        currentClass.AddConstructor(constructor);
    }
}
