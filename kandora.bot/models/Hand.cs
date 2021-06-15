using System.Collections.Generic;

namespace kandora.bot.models
{
    class Hand
    {
        public void Setup() {
            Dictionary<string, char> stoc = new Dictionary<string, char>();
            stoc.Add("1p", '1');
            stoc.Add("2p", '2');
            stoc.Add("3p", '3');
            stoc.Add("4p", '4');
            stoc.Add("5p", '5');
            stoc.Add("6p", '6');
            stoc.Add("7p", '7');
            stoc.Add("8p", '8');
            stoc.Add("9p", '9');
            stoc.Add("0p", '0');
            stoc.Add("1m", 'a');
            stoc.Add("2m", 'b');
            stoc.Add("3m", 'c');
            stoc.Add("4m", 'd');
            stoc.Add("5m", 'e');
            stoc.Add("6m", 'f');
            stoc.Add("7m", 'g');
            stoc.Add("8m", 'h');
            stoc.Add("9m", 'i');
            stoc.Add("0m", 'j');
            stoc.Add("1s", 'A');
            stoc.Add("2s", 'B');
            stoc.Add("3s", 'C');
            stoc.Add("4s", 'D');
            stoc.Add("5s", 'E');
            stoc.Add("6s", 'F');
            stoc.Add("7s", 'G');
            stoc.Add("8s", 'H');
            stoc.Add("9s", 'I');
            stoc.Add("0s", 'J');
            stoc.Add("1z", 't');
            stoc.Add("2z", 'u');
            stoc.Add("3z", 'v');
            stoc.Add("4z", 'w');
            stoc.Add("5z", 'X');
            stoc.Add("6z", 'Y');
            stoc.Add("7z", 'Z');

            Dictionary<char, string> ctos = new Dictionary<char, string>();
            ctos.Add('1',"1p");
            ctos.Add('2', "2p");
            ctos.Add('3', "3p");
            ctos.Add('4', "4p");
            ctos.Add('5', "5p");
            ctos.Add('6', "6p");
            ctos.Add('7', "7p");
            ctos.Add('8', "8p");
            ctos.Add('9', "9p");
            ctos.Add('0', "0p");
            ctos.Add('a', "1m");
            ctos.Add('b', "2m");
            ctos.Add('c', "3m");
            ctos.Add('d', "4m");
            ctos.Add('e', "5m");
            ctos.Add('f', "6m");
            ctos.Add('g', "7m");
            ctos.Add('h', "8m");
            ctos.Add('i', "9m");
            ctos.Add('j', "0m");
            ctos.Add('A', "1s");
            ctos.Add('B', "2s");
            ctos.Add('C', "3s");
            ctos.Add('D', "4s");
            ctos.Add('E', "5s");
            ctos.Add('F', "6s");
            ctos.Add('G', "7s");
            ctos.Add('H', "8s");
            ctos.Add('I', "9s");
            ctos.Add('J', "0s");
            ctos.Add('t', "1z");
            ctos.Add('u', "2z");
            ctos.Add('v', "3z");
            ctos.Add('w', "4z");
            ctos.Add('X', "5z");
            ctos.Add('Y', "6z");
            ctos.Add('Z', "7z");
        }
    }

}
