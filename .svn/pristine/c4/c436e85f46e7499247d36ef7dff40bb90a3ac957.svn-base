using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

/****************************************************
 *  Copyright © www.fayelf.com All Rights Reserved  *
 *  Author : jacky                                  *
 *  QQ : 7092734                                    *
 *  Email : jacky@fayelf.com                        *
 *  Site : www.fayelf.com                           *
 *  Create Time : 2020-05-01 下午 04:37:31          *
 *  Version : v 1.0.0                               *
 ****************************************************/
namespace XiaoFeng.Expressions
{
    /// <summary>
    /// ReplaceVisitor 类说明
    /// </summary>
    public class ReplaceVisitor : ExpressionVisitor
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly Expression from, to;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public ReplaceVisitor(Expression from, Expression to)
        {
            this.from = from;
            this.to = to;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override Expression Visit(Expression node)
        {
            return node == from ? to : base.Visit(node);
        }
    }
}