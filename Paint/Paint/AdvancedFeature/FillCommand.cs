using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint.AdvancedFeature
{
    internal class FillCommand : Command
    {
        public FillCommand(MainWindow app) : base(app)
        {
        }

        public override bool Execute()
        {
            if (_app._choosenShape != null)
            {
                saveBackup();
                _app._choosenShape.HandleBackground(_app._currentColor);
                return true;
            }
            return false;
        }
    }
}
