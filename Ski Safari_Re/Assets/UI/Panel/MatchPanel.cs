using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MatchPanel : BasePanel
{
    private Transform content;
    private GameObject gridObj;

    public class PlayerMatch
    {
        public string Match_name;
    }

    //初始化UI
    public override void OnInit()
    {
        this.skinPath = "MatchPanel";
        this.layer = PanelManager.Layer.Panel;
    }

    //UI绑定
    public override void OnShow(params object[] args)
    {
        content = skin.transform.Find("PlayerPanel/Scroll View/Viewport/Content");
        gridObj = skin.transform.Find("PlayerPanel/Grid").gameObject;

        gridObj.SetActive(false);

        AddPlayer(3);
    }

    public async void AddPlayer(float time)
    {
        await Task.Delay((int)(time * 1000));
        PlayerMatch match = new PlayerMatch();
        match.Match_name = GetChinessName();

        GenerateItem(match);
        float timei = Random.Range(3f, 6f);
        if (content.childCount < 4)
        {
            AddPlayer(timei);
        }else
        {
            GUIMatchButton.match.SkiStart();
            Close();
        }
    }

    public void GenerateItem(PlayerMatch match)
    {
        GameObject Object = Instantiate(gridObj);
        Object.name = match.Match_name.ToString();
        Object.tag = "ItemData";
        Object.transform.SetParent(this.content);
        Object.SetActive(true);
        Object.transform.localScale = Vector3.one;

        Transform trans = Object.transform;
        Text text = Object.transform.Find("Text").GetComponent<Text>();

        text.text = match.Match_name;
    }

    /// <summary>
    /// 随机获取三个字的名字
    /// </summary>
    /// <returns></returns>
    public static string GetChinessName()
    {
        string name = "";
        string[] _crabofirstName = new string[]{
            "白","毕","卞","蔡","曹","岑","常","车","陈","成" ,"程","池","邓","丁","范","方","樊","闫","倪","周",
            "费","冯","符","元","袁","岳","云","曾","詹","张","章","赵","郑" ,"钟","周","邹","朱","褚","庄","卓"
           ,"傅","甘","高","葛","龚","古","关","郭","韩","何" ,"贺","洪","侯","胡","华","黄","霍","姬","简","江"
           ,"姜","蒋","金","康","柯","孔","赖","郎","乐","雷" ,"黎","李","连","廉","梁","廖","林","凌","刘","柳"
           ,"龙","卢","鲁","陆","路","吕","罗","骆","马","梅" ,"孟","莫","母","穆","倪","宁","欧","区","潘","彭"
           ,"蒲","皮","齐","戚","钱","强","秦","丘","邱","饶" ,"任","沈","盛","施","石","时","史","司徒","苏","孙"
           ,"谭","汤","唐","陶","田","童","涂","王","危","韦" ,"卫","魏","温","文","翁","巫","邬","吴","伍","武"
           ,"席","夏","萧","谢","辛","邢","徐","许","薛","严" ,"颜","杨","叶","易","殷","尤","于","余","俞","虞"
           };

        string _lastName = "震南洛栩嘉光琛潇闻鹏宇斌威汉火科技梦琪忆柳之召腾飞慕青问兰尔岚元香初夏沛菡傲珊曼文乐菱痴珊恨玉惜香寒新柔语蓉海安夜蓉涵柏水桃醉蓝春语琴从彤" +
            "傲晴语菱碧彤元霜怜梦紫寒妙彤曼易南莲紫翠雨寒易烟如萱若南寻真晓亦向珊慕灵以蕊寻雁映易雪柳孤岚笑霜海云凝天沛珊寒云冰旋宛儿" +
            "绿真盼晓霜碧凡夏菡曼香若烟半梦雅绿冰蓝灵槐平安书翠翠风香巧代云梦曼幼翠友巧听寒梦柏醉易访旋亦玉凌萱访卉怀亦笑蓝春翠靖柏夜蕾" +
            "冰夏梦松书雪乐枫念薇靖雁寻春恨山从寒忆香觅波静曼凡旋以亦念露芷蕾千帅新波代真新蕾雁玉冷卉紫千琴恨天傲芙盼山怀蝶冰山柏翠萱恨松问旋" +
            "南白易问筠如霜半芹丹珍冰彤亦寒寒雁怜云寻文乐丹翠柔谷山之瑶冰露尔珍谷雪乐萱涵菡海莲傲蕾青槐洛冬易梦惜雪宛海之柔夏青妙菡春竹痴梦紫蓝晓巧幻柏" +
            "元风冰枫访蕊南春芷蕊凡蕾凡柔安蕾天荷含玉书雅琴书瑶春雁从安夏槐念芹怀萍代曼幻珊谷丝秋翠白晴海露代荷含玉书蕾听访琴灵雁秋春雪青乐瑶含烟涵双" +
            "平蝶雅蕊傲之灵薇绿春含蕾梦蓉初丹听听蓉语芙夏彤凌瑶忆翠幻灵怜菡紫南依珊妙竹访烟怜蕾映寒友绿冰萍惜霜凌香芷蕾雁卉迎梦元柏代萱紫真千青凌寒" +
            "紫安寒安怀蕊秋荷涵雁以山凡梅盼曼翠彤谷新巧冷安千萍冰烟雅友绿南松诗云飞风寄灵书芹幼蓉以蓝笑寒忆寒秋烟芷巧水香映之醉波幻莲夜山芷卉向彤小玉幼";

        name = _crabofirstName[Random.Range(0, _crabofirstName.Length - 1)] + _lastName[Random.Range(0, _lastName.Length - 1)] + _lastName[Random.Range(0, _lastName.Length - 1)];
        return name;
    }

}
