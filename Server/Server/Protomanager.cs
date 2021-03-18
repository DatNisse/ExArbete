using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;


namespace Server
{
    class Protomanager
    {
        public Protomanager()
        { }

        public GameMessage ParseMessage(byte[] recivingBytes)
        {

            try
            {
                return GameMessage.Parser.ParseFrom(recivingBytes);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return new GameMessage();
        }
        public byte[] GreetMessage(string playerId)
        {
            return new GameMessage { IsGreet = true, Move = "", MoveNumber = 0, PlayerId = playerId }.ToByteArray();            
        }
        public byte[] NewGameP1(string playerId)
        {
            return new GameMessage { IsGreet = true, Move = "newgame", MoveNumber = -1, PlayerId = playerId }.ToByteArray();
        }
        public byte[] NewGameP2(string playerId)
        {
            return new GameMessage { IsGreet = true, Move = "newgame", MoveNumber = 0, PlayerId = playerId }.ToByteArray();
        }
    }
}
