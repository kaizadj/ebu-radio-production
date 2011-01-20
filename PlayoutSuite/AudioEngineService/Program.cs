using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AudioServiceLibrary;
using AudioEngineDll;
using System.Collections;

namespace AudioEngineService
{
    class Program
    {
        static void Main(string[] args)
        {
            AudioManager.GetInstance();
        }
    }
}
