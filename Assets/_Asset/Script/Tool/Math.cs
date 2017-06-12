

using UnityEngine;

namespace XYH
{
    static public class Math
    {
        //added by slu for getting the  point of intersection of tow lines
        public static Vector2 intersectionOf2Lines(Vector2 begin1, Vector2 end1, Vector2 begin2, Vector2 end2)
        {
            //a是line1的斜率，b是line2的斜率
            float a = 0, b = 0;
            int state = 0;

            if (begin1.x != end1.x)//line1不是垂直x轴的
            {
                a = (end1.y - begin1.y) / (end1.x - begin1.x);
                state |= 1;
            }

            if (begin2.x != end2.x)//line2不是垂直x轴的
            {
                b = (end2.y - begin2.y) / (end2.x - begin2.x);
                state |= 2;
            }

            //到这里如果state执行了上面的两次按位或，则被置为3，则说明line1和line2都不是垂直于x轴的

            switch (state)
            {
                case 0: //L1与L2都平行Y轴
                    {   //两条直线互相重合或者两条直线互相平行，且平行于Y轴;
                        return new Vector2(0, 0);
                    }

                case 1: //L1存在斜率, L2平行Y轴
                    {
                        float x = begin2.x;
                        float y = (begin1.x - x) * (-a) + begin1.y;
                        return new Vector2(x, y);
                    }
                case 2: //L1 平行Y轴，L2存在斜率
                    {
                        float x = begin1.x;
                        float y = (begin2.x - x) * (-b) + begin2.y;
                        return new Vector2(x, y);
                    }
                case 3: //L1，L2都存在斜率
                    {
                        if (a == b)
                        {
                            //两条直线平行或重合
                            return new Vector2(0, 0);
                        }
                        float x = (a * begin1.x - b * begin2.x - begin1.y + begin2.y) / (a - b);
                        float y = a * x - a * begin1.x + begin1.y;
                        return new Vector2(x, y);
                    }
            }

            //代码不可能走到这里
            return new Vector2(0, 0);
        }
    }

}
