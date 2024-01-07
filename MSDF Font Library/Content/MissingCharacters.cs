using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDF_Font_Library.Content
{
    public static class MissingCharacters
    {
        public static List<MissingCharacterInfo> CharacterList = new();

        public static void Add(string font, char character)
        {
            if (!CharacterList.Where(x => x.Font == font && x.Character == character).Any())
            {
                CharacterList.Add(new MissingCharacterInfo(font, character));
            }
        }
    }

    public class MissingCharacterInfo
    {
        public MissingCharacterInfo(string font, char character)
        {
            Font = font;
            Character = character;
        }

        public string Font;
        public char Character;
    }
}
