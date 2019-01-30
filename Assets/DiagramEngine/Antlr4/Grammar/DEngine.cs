
namespace DEngine {

    public class DEngine {

        private static DEngine engine = new DEngine();

        private Program program;

        private DEngine() {

            program = new Program();
        }

        public static DEngine getInstance() {
            return engine;
        }

        public void Run() {

            program.Start();
        }
    }

}