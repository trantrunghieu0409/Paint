using IContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint.AdvancedFeature
{
    internal class CopyCommand : Command
    {
        public CopyCommand(MainWindow app) : base(app)
        {
        }

        public override bool Execute()
        {
            if(_app._choosenShape != null)
            {
                _app._clipboard = (IShapeEntity)_app._choosenShape.Clone();
                return false;
            }
            return true;
        }
    }
}
