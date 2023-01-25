//Mauro Martínez Montes
//José Tomás Gómez Becerra
using System.Xml.Linq;

namespace Numberama;
class Numberama
{
    const int MAX_NUM = 400; // número máximo de eltos en la secuencia
    const int LONG_FIL = 9;  // longitud de las líneas
    static void Main()
    {
        // 𝐈𝐧𝐢𝐜𝐢𝐚𝐥𝐢𝐳𝐚𝐜𝐢ó𝐧 𝐝𝐞 𝐥𝐚𝐬 𝐯𝐚𝐫𝐢𝐚𝐛𝐥𝐞𝐬.

        int[] nums = new int[MAX_NUM];  // secuencia de dígitos
        int cont = 0,   // número de dígitos de la secuencia = primera posición libre en la secuencia
        act = 0,    // posición del cursor en la secuencia
        sel = -1,   // posición de la casilla seleccionada; -1 si no hay selección
        lin = 4,    // número de líneas en ese momento
        cant = 18;  // número máximo 
        bool close = false;

        Genera(ref nums, ref cont, cant);
        Render(nums, cont, act, sel, lin);

        if (File.Exists("save.txt") == true)    // confirma la existencia de un save antes de leerlo
        {
            Lee(ref nums, ref cont);
        }

        // 𝐁𝐮𝐜𝐥𝐞 𝐩𝐫𝐢𝐧𝐜𝐢𝐩𝐚𝐥.

        while (Terminado(nums, cont) == false && close == false)      // comprueba que no todos los digitos sean 0
        {
            char ch = LeeInput();

            ProcesaInput(ch, ref nums, ref cont, ref act, ref sel, cant, ref close);
            Render(nums, cont, act, sel, lin);
            Thread.Sleep(100);
        }
        Guarda(nums, cont);
    }

    static void Render(int[] nums, int cont, int act, int sel, int lin) // renderizador (funciona)
    {
        int tamlin = 0, // limitador del tamaño cada linea
        curX,           // posicion del cursor eje X
        curY;           // posicion del cursor eje Y

        Console.SetCursorPosition(0, 0);
        for (int i = 0; i < cont; i++)  // escribe los números en orden en sus respectivas casillas
        {
            Console.Write(nums[i]);
            tamlin++;

            if (tamlin == LONG_FIL)   // cambio de linea
            {
                Console.WriteLine("");
                tamlin = 0;
            }
        }

        curX = act % LONG_FIL;                          // columna en la que se encuentra el cursor        
        curY = act / LONG_FIL;                          // fila en la que se encuentra el cursor
        Console.SetCursorPosition(curX, curY);
        Console.BackgroundColor = ConsoleColor.Yellow;
        Console.Write(nums[act]);
        Console.BackgroundColor = ConsoleColor.Black;

        if (sel != -1)
        {                                                  // renderizado de la selección
            Console.SetCursorPosition(sel % LONG_FIL, sel / LONG_FIL);
            Console.BackgroundColor = ConsoleColor.Green;
            Console.Write(nums[sel]);
            Console.BackgroundColor = ConsoleColor.Black;
        }
        Console.SetCursorPosition(0, 0);

        if (Terminado(nums, cont))
        {
            Console.Write("Felicidades, has ganado");   // mensaje de victoria
            for (int i = 1; i < cont; i++)              // escribe las casillas vacías
            {
                Console.Write(" ");
                tamlin++;

                if (tamlin == LONG_FIL)   // cambio de linea
                {
                    Console.WriteLine("");
                    tamlin = 0;
                }
            }
        }

        Console.SetCursorPosition(LONG_FIL, cont / LONG_FIL);         // colocación del cursor en la esquina inferior derecha de las casillas totales
        tamlin = 0;
    }

    static void Genera(ref int[] nums, ref int cont, int cant) // generador de numeros y casillas (funciona)
    {

        for (int i = 1; i <= cant; i++)
        {
            if (i < 10)                  // asigna los numeros de un digito (primeros 9)
            {
                nums[cont] = i;
                cont++;
            }

            else if (i > 10 && i != 10)    // asigna los numeros superioes a 10
            {
                nums[cont] = 1;         // asigna los unos
                cont++;
                nums[cont] = i - 10;    // asigna los digitos a la derecha
                cont++;
            }
        }
    }

    static char LeeInput()  // lector de inputs (funciona)
    {
        char ch = ' ';
        if (Console.KeyAvailable)
        {
            string dir = Console.ReadKey(true).Key.ToString();
            if (dir == "A" || dir == "LeftArrow") ch = 'l';         // izquierda
            else if (dir == "D" || dir == "RightArrow") ch = 'r';   // derecha
            else if (dir == "W" || dir == "UpArrow") ch = 'u';      // arriba
            else if (dir == "S" || dir == "DownArrow") ch = 'd';    //abajo                  
            else if (dir == "X" || dir == "Spacebar") ch = 'x';     // marcar   
            else if (dir == "G" || dir == "Intro") ch = 'g';        // generar
            else if (dir == "P") ch = 'p';                          // pista 
            else if (dir == "Q" || dir == "Escape") ch = 'q';       // salir

            while (Console.KeyAvailable) Console.ReadKey();         // limpiamos buffer
        }
        return ch;
    }

    static void ProcesaInput(char ch, ref int[] nums, ref int cont, ref int act, ref int sel, int cant, ref bool close)   //procesador de inputs (funciona) (expandir)
    {
        if (ch == 'l' && act - 1 >= 0)                      // desplazamiento hacia la izquierda
        {
            act--;
            ch = ' ';
        }
        else if (ch == 'r' && act + 1 < cont)               // desplazamiento hacia la derecha
        {
            act++;
            ch = ' ';
        }
        if (ch == 'u' && act - LONG_FIL >= 0)               // desplazamiento hacia arriba
        {
            act = act - LONG_FIL;
            ch = ' ';
        }
        else if (ch == 'd' && act + LONG_FIL + 1 <= cont)   // desplazamiento hacia abajo
        {
            act = act + LONG_FIL;
            ch = ' ';
        }

        else if (ch == 'x')       // eliminación de casilla
        {
            if (sel == -1)      // checkea que no haya nada seleccionado
            {
                sel = act;      // si no lo hay selecciona la casilla
            }
            else if (EliminaPar(nums, cont, act, sel))  // elimina las dos casillas
            {
                nums[sel] = 0;
                nums[act] = 0;
                sel = -1;           // resetea la selección
            }
            else if (act == sel) // checkea que la casilla seleccionada no haya sido seleccionada antes y si lo fue, anula la selección
            {
                sel = -1;
            }
            else                // cambia la casilla seleccionada si ya lo estaba antes y no es o contigua o eliminables
            {
                sel = act;
            }
            ch = ' ';
        }

        else if (ch == 'g')                      // añade los digitos nuevos
        {
            Genera(ref nums, ref cont, cant);
            ch = ' ';
        }

        else if (ch == 'q')
        {
            close = true;
        }
    }
    static bool Contiguos(int[] nums, int cont, int act, int sel)   // comprobador de espacios
    {
        int min = Math.Min(act, sel),           // asigna mayor
            max = Math.Max(act, sel),           // asigna menor
            i = 0;

        if (min + 1 == max || min + LONG_FIL == max) return true;   // checkea proximidad immediata tanto horizontal como vertical

        i = min + 1;
        while (i < max && nums[i] == 0) // checkeador de filas, si detecta una casilla diferente entre min y maxcancela la busqueda y pasa a la siguiente
        {
            i++;
        }
        if (i == max) return true;      // si la busqueda no es interrumpida devolverá "true"

        i = min + 9;
        while (i < max && nums[i] == 0) // checkeador de columnas, misma idea detras del de filas
        {
            i = i + LONG_FIL;
        }
        if (i == max) return true;

        return false;                   // si no hay casillas horizontalmente o horizontalmente libres se devuelve "false"
    }
    static bool EliminaPar(int[] nums, int cont, int act, int sel)  // comprueba que la pareja elegida sea eliminable
    {
        if (Contiguos(nums, cont, act, sel) && (nums[act] == nums[sel] || nums[act] + nums[sel] == 10)) // condiciones: contiguas y mismo valor o la suma de estos es 10
        {
            return true;
        }
        else return false;
    }
    static bool Terminado(int[] nums, int cont) // detecta si todos las casillas presentes han sido eliminadas
    {
        int i = 0;
        while (nums[i] == 0 && i < cont)    // busca al menos una casilla que no es igual a 0, si lo encuentra interrumpe la búsqueda
        {
            i++;
        }
        if (i == cont) return true; // si la busqueda no es interrumpida retorna "true"

        return false;   // si es interrumpoda retorna "false"
    }
    static void Guarda(int[] nums, int cont)    // guarda la partida en el archivo "save"
    {
        StreamWriter guardado;                      // declaración de flujo de salida

        guardado = new StreamWriter("save.txt");    // asignación del archivo

        guardado.WriteLine(cont);           // se registra el número de casillas presentes
        for (int i = 0; i < cont; i++)
        {
            guardado.WriteLine(nums[i]);    // se registra cada casilla por separado
        }
        guardado.Close();   // cierra el flujo
    }
    static void Lee(ref int[] nums, ref int cont)
    {
        StreamReader cargar;                    // declaración del flujo de entrada

        cargar = new StreamReader("save.txt");  // asiganción del archivo

        string[] lines = File.ReadAllLines("save.txt"); // almacena todas las líneas en forma de string

        cont = lines.Length - 1;                // asigna cont (número de casillas ocupadas)

        for (int i = 0; i < cont; i++)
        {
            nums[i] = int.Parse(lines[i + 1]);    //  asigna a cada casilla su numero
        }
        cargar.Close();     // cierra el flujo
    }
}