using Silk.NET.Windowing;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Input;
using System.Drawing;

namespace Graphics.Handlers;
public class WindowHandler {

    private static uint _vbo;
    private static uint _vao;
    private static uint _ebo;

    // this loads the implementation of the opengl specification
    // only a function who was directly passed Set_gl has permission to augment _gl
    private static GL? _gl = null;
    private readonly static Action<GL> Set_gl = (gl) => { _gl = gl; };

    // same behavior as _gl
    private static IWindow? _currentWindow = null; 
    private readonly static Action<IWindow> Set_currentWindow = (window) => { _currentWindow = window; };

    static int Main(string[] args) { 
        // struct that holds basic window configuration settings that we pass into Window.Create
        WindowOptions options = WindowOptions.Default with {
            Size = new Vector2D<int>(1920, 1800),
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

        public static unsafe void OnLoad() {
            Set_gl(_currentWindow.CreateOpenGL()); 

            IInputContext input = _currentWindow!.CreateInput();
            static void KeyDown(IKeyboard keyboard, Key key, int keyCode) { 
                Console.WriteLine($"Key Pressed! {key}"); 
                _currentWindow!.Close();
            }; // function to be called when the key is pressed
            for(int key = 0; key < input.Keyboards.Count; key++)
                input.Keyboards[key].KeyDown += KeyDown; // KeyDown is an event on every keyboard key, we set it to KeyDown for every key and then that function gets called

            // a vao is metadata for a vbo (vertex with extra data)
            _vao = _gl!.GenVertexArray();
            // binding anything to the gl means that we put the opengl into the state to target that object
            _gl!.BindVertexArray(_vao); 

            // a vao is metadata for a vbo (vertex with extra data)
            _vbo = _gl.GenBuffer();
            _gl!.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);

            _gl!.ClearColor(Color.CornflowerBlue); 
            
            // a 2D square's (quad) coordinates for each point/corner
            // this is in R_3
            float[] vertices = {
                0.5f,  0.5f, 0.0f,
                0.5f, -0.5f, 0.0f,
                -0.5f, -0.5f, 0.0f,
                -0.5f,  0.5f, 0.0f
            };

            /*  
                indices for our index buffer to keep track of triangles
                here, we are assigning which coordinates from the quad are the points for each triangle
                (everything in graphics programming is a triangle)
                0 = R0,
                1 = R1,
                2 = R2,
                3 = R3
            */ 
            
            uint[] indices = {
                0, 1, 3, // top right triangle
                1, 2, 3 // bottom left triangle
            };

            fixed (float* buf = vertices) {
                _gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(vertices.Length * sizeof(float)), buf, BufferUsageARB.StaticDraw);
            }

            _ebo = _gl.GenBuffer();
            _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _ebo);

            fixed(uint* buf = indices) {
                _gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(indices.Length * sizeof(uint)), buf, BufferUsageARB.StaticDraw);
            }
                
            }

        public static unsafe void OnUpdate(double deltaTime) {
           // Console.WriteLine("Updated!");
        }

        public static unsafe void OnRender(double deltaTime) {
            // Console.WriteLine("Rendered!");
            _gl!.Clear(ClearBufferMask.ColorBufferBit);
        }

        public static unsafe void OnResize(Vector2D<int> size) {
            // _gl!.Viewport(0, 0, (uint)size.X, (uint)size.Y); Console.WriteLine("Resize!");
        }
    }

}