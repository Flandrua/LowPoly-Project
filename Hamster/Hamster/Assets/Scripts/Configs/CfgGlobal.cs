/*
* 自动生成ConfigTools
*/
using System.Collections.Generic;

namespace Configs
{
    public class CfgGlobal
    {
        public Dictionary<string, GlobalData> mDataMap;
    }

    public class GlobalData
    {
        //ID
        public int id;
        //描述
        public string desc;
        //参数
        public string value;
    }
}
