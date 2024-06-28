/*
* 自动生成ConfigTools
*/
using System.Collections.Generic;

namespace Configs
{
    public class CfgQuestion
    {
        public Dictionary<string, QuestionData> mDataMap;
    }

    public class QuestionData
    {
        //ID
        public int ID;
        //问题
        public string question;
        //A
        public string A;
        //B
        public string B;
        //C
        public string C;
        //D
        public string D;
        //正确答案
        public string rightAnswer;
    }
}
