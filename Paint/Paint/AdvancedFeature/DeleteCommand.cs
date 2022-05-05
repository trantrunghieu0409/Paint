using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint.AdvancedFeature
{
    internal class DeleteCommand : Command
    {
        public DeleteCommand(MainWindow app) : base(app)
        {
        }

        public override bool Execute()
        {
            if(_app._choosenShape != null)
            {
                saveBackup();
                _app._drawnShapes.Remove(_app._choosenShape);
                _app._choosenShape = null;
                return true;
            }
            return false;
        }
    }
}
