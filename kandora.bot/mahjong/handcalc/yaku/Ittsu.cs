﻿
using System.Collections.Generic;
using U = kandora.bot.mahjong.Utils;
using C = kandora.bot.mahjong.Constants;
using System.Linq;
using kandora.bot.utils;

namespace kandora.bot.mahjong.handcalc.yaku.yakuman
{
    // 
    //      All group contains AT LEAST one terminal or one honor
    //     
    public class Ittsu : Yaku
    {

        public Ittsu(int id) : base(id)
        {
        }

        public override void setAttributes()
        {
            this.name = "Ittsu";
            this.tenhouId = 24;
            this.nbHanClosed = 2;
            this.nbHanOpen = 1;
        }

        public override bool isConditionMet(List<List<int>> hand, params object[] args)
        {
            var chis = hand.Where(x => Utils.IsShuntsu(x));
            if(chis.Count() < 3)
            {
                return false;
            }
            var sou = new List<List<int>>();
            var pin = new List<List<int>>();
            var man = new List<List<int>>();
            foreach (var chi in chis)
            {
                if (U.IsSou(chi[0]))
                {
                    sou.Add(chi);
                }
                else if (U.IsMan(chi[0]))
                {
                    man.Add(chi);
                }
                else if (U.IsPin(chi[0]))
                {
                    pin.Add(chi);
                }
            }
            var suits = new List<List<List<int>>>
            {
                sou, man, pin
            };

            var one = new List<int> { 0, 1, 2 };
            var two = new List<int> { 3, 4, 5 };
            var three = new List<int> { 6, 7, 8 };
            var comp = new GroupComparer<int>();
            foreach (var suit in suits)
            {
                if (suit.Count() < 3)
                {
                    continue;
                }
                var simpleSets = new List<List<int>>();
                foreach(var set in suit)
                {
                    simpleSets.Add(new List<int> { U.Simplify(set[0]), U.Simplify(set[1]) , U.Simplify(set[2]) });
                }
                if (simpleSets.Contains(one, comp) && simpleSets.Contains(two, comp) && simpleSets.Contains(three, comp))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
