using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Radical
{
    public class RadicalHelp
    {
        

        /*
         
        待优化：1. 整数存到 StringBuilder 逆序更好。
         
         */
        #region static



        /// <summary>
        /// 正整数字符串 相减
        /// 若 n1大于 n2,则计算 (n1-n2) 存放到 dest中，并返回true.
        /// 否则 返回false;
        ///    
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <returns></returns>
        private static bool subtract(StringBuilder n1, StringBuilder n2_,StringBuilder dest)
        {
            StringBuilder n2 = new StringBuilder();
            n2.Append(n2_);
            if (n1.Length < n2.Length)
            {
                return false;
            }
            dest.Length = n1.Length;

            while (n1.Length > n2.Length) 
            {
                n2.Insert(0,'0');
            }
            
            int c1, c2;
            int carry = 0;
            for (int t = n1.Length-1; t >=0; t--)
            {
                c1 = n1[t]-'0';
                c2 = n2[t]-'0';

                c1 += carry;
                if (c1 < c2)
                {
                    c1 += 10;
                    carry = -1;
                }
                else 
                {
                    carry = 0;
                }
                dest[t] = (char)(c1 - c2 + '0');
                
            }

            if (0 == carry) 
            {
                int zCount = 0;
                while (zCount < dest.Length && '0' == dest[zCount]) zCount++;
                if (zCount > 0) 
                {
                    dest.Remove(0, zCount);
                    if (0 == dest.Length) 
                    {
                        dest.Append("0");
                    }
                }


                return true;
            }
            return false;
        }

        /// <summary> 
        ///  计算 n1 * n2  
        /// </summary>
        /// <param name="n1">必须为正整数</param>
        /// <param name="n2">必须为一位正整数（1~9）</param>
        private static void times(StringBuilder n1, int n2, StringBuilder result) 
        {
          

            result.Length = n1.Length + 1;
            result[0] = '0';
           

            int carry = 0;

            int midRes;
            for (int t = n1.Length - 1; t >= 0; t--) 
            {
                midRes = (n1[t] - '0') * n2 + carry;

                carry = midRes / 10;

                result[t + 1] = (char)('0' + midRes % 10);
            }
            if (carry > 0)
            {
                result[0] = (char)('0' + carry);
            }
            else
            {
                result.Remove(0, 1);
            }
        }
        #endregion

        //param_Ori  * 10^eCount  = param_Int
        int eCount;

        /// <summary>
        /// 要开根号的数字
        /// </summary>
        String param_Ori;

        /// <summary>
        /// 要开根号的数字,去除小数点
        /// </summary>
        String param_Int;

    
        
        public string getProcess() 
        {
            return process.ToString();
        }
        public string getResult()
        {

            string result=destValue.ToString();

            int pointIndex=param_Int.Length/2;
            pointIndex -= eCount / 2;
            if (pointIndex <= 0) 
            {                
                return "0.".PadRight(-pointIndex+2,'0') + result;
            }
           

            if (pointIndex >= result.Length)
            {
                return result.PadRight(pointIndex, '0');
            }
            return result.Insert(pointIndex, ".");
        }
        private StringBuilder process = new StringBuilder();

        private int process_prefixLen = 0;
        private StringBuilder destValue = new StringBuilder();


        public void Calc(String param) 
        {
            Init(param);
            Calc();
        }
    
        private  void Init(String param)
        { 
          
            this.param_Ori = param;
             eCount = 0;
            int indexOfPoint = param.IndexOf('.');
            if (indexOfPoint >= 0)
            {
                eCount = param.Length - indexOfPoint - 1;
                param_Int = param.Replace(".", "");
               
            }
            else 
            {
                param_Int = param;
            }

            string param_Int_t=param_Int.TrimEnd(new char[] { '0' });
            int len = param_Int.Length - param_Int_t.Length;
            if (len >0) 
            {
                param_Int = param_Int_t;
                eCount -= len;
            }

            if (1 == Math.Abs( eCount) % 2)
            {
                eCount++;
                param_Int += "0";
            }
            param_Int = param_Int.TrimStart(new char[] { '0' });

            if (1 == param_Int.Length % 2)
            {
                param_Int = "0" + param_Int;   
            }




            leftValue.Length = 0;
            process.Length = 0;
            destValue.Length = 0;
            process_prefixLen = 1;
        }

        public int calcLength = 15;

        StringBuilder leftValue = new StringBuilder();
        StringBuilder leftValue_new = new StringBuilder();
        StringBuilder middleware = new StringBuilder();
        StringBuilder midValue = new StringBuilder();
        
        private void appendPrifex()
        {
            //process.Append("\n\t");
            process.AppendLine();
            process.Append(' ', process_prefixLen);
        }

        private void Calc()
        {
            appendPrifex();
            process.Append(param_Ori);
            process.Append("  ^ 0.5 ");
            process.AppendLine();

            int repeatCount;

            for (int t = 0; t < calcLength; t++) 
            {
                if (!appendLeft2(leftValue,2*t) && 3 == leftValue.Length && '0' == leftValue[0]) 
                {
                    break;
                }
         

                //  --------------
                appendPrifex();
                process.Length--;
                process.Append('-', leftValue.Length+10);


                // leftValue
                appendPrifex();
                process.Append(leftValue);

                //  calc()
                CalcMidValue();     


                // middleware * []
                appendPrifex();
                repeatCount=leftValue.Length - middleware.Length;
                if (repeatCount < 0)
                {
                    process.Length += repeatCount;
                }
                else 
                {
                    process.Append(' ', repeatCount);
                }
              
                process.Append(middleware);
                process.Append(" * ");
                process.Append(middleware[middleware.Length - 1]);

                process.Append("  [").Append(t).Append("]");
                

                // midValue
                appendPrifex();
                repeatCount = leftValue.Length - midValue.Length;
                if (repeatCount < 0)
                {
                    process.Length += repeatCount;
                }
                else
                {
                    process.Append(' ', repeatCount);
                }
               
                process.Append(midValue);


                process_prefixLen += leftValue.Length - leftValue_new.Length;

                var temp = leftValue_new;
                leftValue_new = leftValue;
                leftValue = temp;
            }


            //  --------------
            appendPrifex();
            process.Length--;
            process.Append('-', leftValue.Length + 10);


            // leftValue
            appendPrifex();
            process.Append(leftValue);

        }
       
        private void CalcMidValue()
        {
            times(destValue, 2, middleware);

            middleware.Append(' ');
            int t = 9;
            for ( t = 9; t >0; t--) 
            {
                middleware[middleware.Length - 1] = (char)('0' + t);
                times(middleware, t, midValue);

                if (subtract(leftValue, midValue, leftValue_new)) 
                {                  
                   
                    break;
                }
            }
            if (0 == t)
            {
                middleware[middleware.Length - 1] = '0';
                midValue.Length = 0;
                midValue.Append("0");

                leftValue_new.Length = 0;
                leftValue_new.Append(leftValue);

                destValue.Append('0');               
            }
            else 
            {
                destValue.Append((char)('0'+t));
               
            }

        }

   

        /// <summary>
        /// 
        /// </summary>
        /// <param name="leftValue"></param>
        /// <returns> 是否获取到数据。若没获取到，则append "00"</returns>
        private bool appendLeft2(StringBuilder leftValue,int index)
        {

            if ((index += 2) > param_Int.Length) 
            {
                leftValue.Append("00");
                return false;
            }else
            {
                leftValue.Append(param_Int.Substring(index - 2, 2));
               return true;
            }
            
        }
        

    }
}
