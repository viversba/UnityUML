
public class UMLEngine{

    private static UMLEngine engine = new UMLEngine();

    private Program program;

    private UMLEngine() {

        program = new Program();
    }

    public static UMLEngine getInstance() {
        return engine;
    }

    public void Run() {

        program.Start();
    }
}
