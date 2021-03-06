﻿using NHibernate;
using SA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SA.DAO
{
    public class ProdutosDAO
    {
        ISession session;
        public ProdutosDAO (ISession session)
        {
            this.session = session;
        }

        /// <summary>
        /// Retorna uma lista de produtos pela descrição aproximada
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>IList&lt;Produtos&gt;.</returns>
        public List<Produtos> ListaProdutosByDesc(string text)
        {
            string hql = " select p from Produtos p" +
            " where p.Tipo in ('MM')" +
            " and (p.Descricao LIKE :descri )" +
            
            " order by p.Descricao";

            IList<Produtos> produtosDesc = session.QueryOver<Produtos>().Where(p => p.Tipo == "MM" )
                                                                    .AndRestrictionOn(p =>p.Descricao).IsLike(text+"%")                                                                    
                                                                    .List();

            IList<Produtos> produtosCod = session.QueryOver<Produtos>().Where(p => p.Tipo == "MM")
                                                                    .AndRestrictionOn(p => p.Codigo).IsLike(text + "%")
                                                                    .List();
            
            IEnumerable<Produtos> produtos = produtosDesc.Union(produtosCod) ;
            //IList<Produtos> produtos = (IList<Produtos>)iprodutos;

            return produtos.ToList<Produtos>();
                       
            //IQuery query = session.CreateQuery(hql);
            //query.SetParameter("descri", text.ToUpper() + "%");
            //return query.List<Produtos>();

        }

        /// <summary>
        /// Retorna o saldo emestoque de um produto 
        /// </summary>
        /// <param name="codProduto"></param>
        /// <returns></returns>
        public double GetEstoque(string codProduto)
        {
            string hql = "select s.Saldo from SaldoEstoque s " +
                         "where s.Codigo = :cod and s.Local = :local";
            IQuery query = session.CreateQuery(hql);
            query.SetParameter("cod", codProduto.Trim().ToUpper());
            query.SetParameter("local", "03");

            double retorno = 0;

            if (!Convert.IsDBNull(query.UniqueResult()))
            {
                retorno = Convert.ToDouble(query.UniqueResult());
            }

            return retorno;

        }

        /// <summary>
        /// Retorna um produto bucando pelo código
        /// </summary>
        /// <param name="cod">The cod.</param>
        /// <returns></returns>
        public Produtos GetProdutoByCod(string cod)
        {
            string hql = " select p from Produtos p" +
            " where p.Tipo in ('MM')" +
            " and p.Codigo = :cod " +
            " order by p.Descricao";

            IQuery query = session.CreateQuery(hql);
            query.SetParameter("cod", cod.ToUpper());
            return query.UniqueResult<Produtos>();
        }

    }
}