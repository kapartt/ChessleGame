using System.Collections.Generic;

namespace ChessleGame.UI.Model
{
    public class PositionsHistory
    {
        public PositionsHistory()
        {
            PositionsList = new List<PositionVm>();
        }

        public List<PositionVm> PositionsList { get; set; }
    }
}
