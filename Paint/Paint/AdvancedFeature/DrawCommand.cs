using IContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint.AdvancedFeature
{
    internal class DrawCommand : Command
    {
        public DrawCommand(MainWindow app) : base(app)
        {
        }

        public override bool Execute()
        {
            saveBackup(); // save current state before doing anything
            
            // your action here
            _app._drawnShapes.Add((IShapeEntity)_app._preview.Clone());
            
            return true; // this command change the state of canvas so returns true
        }
    }
}
