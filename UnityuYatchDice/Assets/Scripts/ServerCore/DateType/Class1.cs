using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore.DateType
{
    public class Language
    {
        public enum LangugaeType
        {
            KOR =1,ENG =2,
        }

        public int language = 0;
        public void OpneGame(int language)
        {
            switch (language)
            {
                case 0:
                    Console.WriteLine("야추 다이스 게임에 오신 것을 환영 합니다.");
                    Console.WriteLine("게임 시작을 위해서 스페이스바를 눌러주세요 ");
                    break;
                case 1:
                    Console.WriteLine("Welcom to My Yatch Project Games");
                    Console.WriteLine("if you want to entry server, press 1");
                    break;

            }
        }
    }
    
}
