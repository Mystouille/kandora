using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kandora.bot.utils
{

    public class AcceptanceData
    {
        public AcceptanceData(int tileDiscarded_34, int shanten)
        {
            Shanten = shanten;
            TileDiscarded = tileDiscarded_34;
            NbAcceptedTiles = 0;
            NbAcceptedTilesForGoodTenpai = 0;
            DrawnTileData = new DrawnTileInfo[34];
        }

        public void AddNbAcceptedTiles(int d)
        {
            NbAcceptedTiles += d;
        }
        public void AddNbAcceptedTilesForGoodTenpai(int d)
        {
            NbAcceptedTilesForGoodTenpai += d;
        }
        public void SeeInnerData(int tileIdx, DrawnTileInfo d)
        {
            DrawnTileData[tileIdx] = d;
        }

        public int Shanten { get; init; }
        public int TileDiscarded { get; set; }
        public int NbAcceptedTiles { get; set; }
        public int NbAcceptedTilesForGoodTenpai { get; set; }
        public DrawnTileInfo[] DrawnTileData { get; set; }
    }

    public class DrawnTileInfo
    {
        public DrawnTileInfo(bool isAccepted = false, bool leadToGoodTenpai = false)
        {
            LeadToGoodTenpai = leadToGoodTenpai;
            IsAccepted = isAccepted;
        }

        public bool LeadToGoodTenpai { get; set; }
        public bool IsAccepted { get; set; }
    }
}
