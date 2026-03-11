using Silk.NET.Windowing;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Input;
using Silk.NET.GLFW;
using Silk.NET.SPIRV.Cross;

namespace Graphics.Handlers;
public class WindowHandler {

    // this loads the implementation of the opengl specification
    // only a function who was directly passed Set_gl has permission to augment _gl
    private static GL? _gl = null;  
    private readonly static Action<GL> Set_gl = gl => { _gl = gl; };

    // same behavior as _gl
    private static IWindow? _currentWindow = null; 
    private readonly static Action<IWindow> Set_currentWindow = window => { _currentWindow = window; };

    static int Main(string[] args) { 
        // struct that holds basic window configuration settings that we pass into Window.Create
        WindowOptions options = WindowOptions.Default with {
            Size = new Vector2D<int>(800, 600),
            Title = "Graphics Project"
        };
        // this gives you an IWindow object which contains the information for your window, like monitor, sizing, and a few events you can subscribe to
        using (_currentWindow = Window.Create(options)) { 
            // .Load is an Action, which means it is a delegate that returns void and runs when the window starts (it is also an event we can subscribe to)
            _currentWindow.Load += EventHandlers.OnLoad;
            _currentWindow.Update += EventHandlers.OnUpdate;
            _currentWindow.Render += EventHandlers.OnRender;
            _currentWindow.Resize += EventHandlers.OnResize; 

            // starts an internal loop over glfw to keep the window open
            _currentWindow.Run();
        }

        return 0;
    }

    private static class EventHandlers {

        public static void OnLoad() {
            Set_gl(_currentWindow.CreateOpenGL()); 
            IInputContext input = _currentWindow!.CreateInput();
            static void KeyDown(IKeyboard keyboard, Key key, int keyCode) { Console.WriteLine($"Key Pressed! {key}"); }; // function to be called when the key is pressed
            for(int key = 0; key < input.Keyboards.Count; key++)
                input.Keyboards[key].KeyDown += KeyDown; // KeyDown is an event on every keyboard key, we set it to KeyDown for every key and then that function gets called
        }

        public static void OnUpdate(double deltaTime) {
           Console.WriteLine("Updated!"); 
        }

        public static void OnRender(double deltaTime) {
            Console.WriteLine("Rendered!");
        }

        public static void OnResize(Vector2D<int> size) {
            _gl!.Viewport(0, 0, (uint)size.X, (uint)size.Y); Console.WriteLine("Resize!");
        }
    }
    
}