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
        public static CommandHistory _undoHistory = new CommandHistory();
        public static CommandHistory _redoHistory = new CommandHistory();

        public Command(MainWindow app)
        {
            this._app = app;
        }

        // return true if this function change the state of canvas (i.e cut, paste, add/edit/delete drawing)
        // else return false (i.e copy, selection)
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
            _backup = new List<IShapeEntity>();
            foreach (var shape in _app._drawnShapes)
            {
                _backup.Add((IShapeEntity)shape.Clone());
            }
        }
    }
}
