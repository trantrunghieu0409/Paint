using IContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint.AdvancedFeature
{
    public class RedoCommand : Command
    {
        public RedoCommand(MainWindow app) : base(app)
        {
        }

        public override bool Execute()
        {
            Command? c = _redoHistory.pop();
            if (c != null)
            {
                _app._drawnShapes = new List<IShapeEntity>(c._backup);
                _undoHistory.push(c);
            }
            return false;
        }
    }
}
