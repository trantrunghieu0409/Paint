using IContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint.AdvancedFeature
{
    internal class PasteCommand : Command
    {
        public PasteCommand(MainWindow app) : base(app)
        {
        }

        public override bool Execute()
        {
            saveBackup();
            if(_app._clipboard != null)
            {
                IShapeEntity pasteShape = (IShapeEntity)_app._clipboard.Clone();
                pasteShape.pasteShape(_app._newStartPoint, _app._clipboard);
                _app._drawnShapes.Add(pasteShape);
                return true;
            }
            return false;
        }
    }
}
