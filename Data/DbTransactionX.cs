using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/****************************************************************
*  Copyright © (2015) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2015-12-26 11:56:36                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Data
{
    /// <summary>
    /// 数据库事务
    /// </summary>
    public class DbTransactionX : Disposable, IDbTransactionX
    {
        /// <summary>
        /// 无参构造器
        /// </summary>
        public DbTransactionX() { this.BeginTransaction(); }
        /// <summary>
        /// 设置数据库连接
        /// </summary>
        /// <param name="connection">数据库连接</param>
        public DbTransactionX(DbConnection connection)
        {
            this.Connection = connection;
        }
        /// <summary>
        /// 数据库连接
        /// </summary>
        private DbConnection Connection { get; set; }
        ///<inheritdoc/>
        public IsolationLevel IsolationLevel { get; set; } = IsolationLevel.Unspecified;
        ///<inheritdoc/>
        public Boolean IsOpen { get; set; } = false;
        /// <summary>
        /// 事务
        /// </summary>
        private DbTransaction _Transaction = null;
        ///<inheritdoc/>
        public DbTransaction Transaction { get { return this.IsOpen ? this._Transaction : null; } set { this._Transaction = value; this.IsOpen = true;this.Connection = value.Connection;this.Scope = null; } }
        /// <summary>
        /// 事务
        /// </summary>
        private System.Transactions.TransactionScope Scope { get; set; }
        ///<inheritdoc/>
        public void BeginTransaction()
        {
            if (this.IsOpen) return;
            if (this.Connection == null)
            {
                this.Scope = new System.Transactions.TransactionScope();
                this.IsOpen = true;
                return;
            }
            if (this.Connection.State != ConnectionState.Open && this.Connection.State != ConnectionState.Connecting)
            {
                this.Connection.Open();
                this.IsOpen = true;
            }
            if (!this.IsOpen) return;
            this.Transaction = this.Connection.BeginTransaction(this.IsolationLevel.ToString().ToEnum<System.Data.IsolationLevel>());
        }
        ///<inheritdoc/>
        public void Commit()
        {
            if (!this.IsOpen) return;
            if (this.Connection == null)
            {
                this.Scope?.Complete();
                this.EndTransaction();
                return;
            }
            if (this.Transaction != null && this.Transaction.Connection != null)
                this.Transaction.Commit();
        }
        ///<inheritdoc/>
        public void Rollback()
        {
            if (!this.IsOpen) return;
            if (this.Connection == null)
            {
                this.EndTransaction();
                return;
            }
            if (this.Transaction != null && this.Transaction.Connection != null)
                this.Transaction.Rollback();
        }
        ///<inheritdoc/>
        public void EndTransaction()
        {
            if (!this.IsOpen) return;
            this.IsOpen = false;
            if (this.Connection == null)
            {
                this.Scope?.Dispose();
                this.Scope = null;
                return;
            }
            if (this.Transaction != null)
            {
                this.Transaction.Dispose();
                this.Transaction = null;
            }
        }
        ///<inheritdoc/>
        public DbCommand CreateCommand()
        {
            this.BeginTransaction();
            if (this.Connection == null) return null;
            if (this.Connection.State != ConnectionState.Open && this.Connection.State != ConnectionState.Connecting)
            {
                this.Connection.Open();
                this.IsOpen = true;
            }
            var cmd = this.Connection.CreateCommand();
            cmd.Transaction = this.Transaction;
            return cmd;
        }
        /// <summary>
        /// 释放
        /// </summary>
        public override void Dispose()
        {
            this.EndTransaction();
            base.Dispose();
        }
    }
}