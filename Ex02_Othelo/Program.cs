using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ex02_Othelo
{
    public class Program
    {
        public static void Main()
        {
            GameUi ui = new GameUi();
            ui.ChooseSettings();
            ui.GameMainLoop();
        }
    }
}
