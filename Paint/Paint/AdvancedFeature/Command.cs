using IContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint.AdvancedFeature
{
    public abstract class Command
    {
        protected MainWindow _app;
        public List<IShapeEntity> _backup;
        protected static CommandHistory _undoHistory = new CommandHistory();
        protected static CommandHistory _redoHistory = new CommandHistory();

        public Command(MainWindow app)
        {
            this._app = app;
        }

        public abstract bool Execute();

        public static void executeCommand(Command c)
        {
            if (c.Execute())
            {
                _redoHistory.clear(); // clear all the redo because new command added 
                _undoHistory.push(c);
            }
        } 

        public void saveBackup()
        {
            _backup = new List<IShapeEntity>(_app._drawnShapes);
        }
    }
}
