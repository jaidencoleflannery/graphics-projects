using Silk.NET.Windowing;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace Program;
public class Program {

    // this loads the implementation of the opengl specification
    public static GL? gl = null;

    static void Main(string[] args) { 
        WindowOptions options = WindowOptions.Default with {
            Size = new Vector2D<int>(800, 600),
            Title = "Graphics Project"
        };
        // this gives you an IWindow object which contains the information for your window, like monitor, sizing, and a few events you can subscribe to
        var window = Window.Create(options); 
        // .Load is an Action, which means it is a delegate that returns void and runs when the window starts (it is also an event we can subscribe to)
        window.Load += () => { gl = window.CreateOpenGL(); };
        window.Run();
    }
    
}