using IContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint.AdvancedFeature
{
    public class UndoCommand : Command
    {
        public UndoCommand(MainWindow app) : base(app)
        {
        }

        public override bool Execute()
        {
            Command? c = _undoHistory.pop();
            if (c != null)
            {
                _app._drawnShapes = new List<IShapeEntity>(c._backup);
                _redoHistory.push(c);
            }
            return false;
        }
    }
}
