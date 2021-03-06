﻿using NHibernate;
using SA.Helpers;
using SA.Models;
using SA.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SA.DAO
{
    public class UsuarioDAO
    {
        ISession session;
        DepartamentoDAO departamentoDAO;
        CentroCustoDAO centrodeCustoDao;

        public UsuarioDAO(ISession session, DepartamentoDAO dep, CentroCustoDAO ccdao)
        {
            this.session = session;
            this.departamentoDAO = dep;
            this.centrodeCustoDao = ccdao;
        }

        //Lista os Usuarios 
        public  IList<Usuario> Lista()
        {
            string hql = "select d from Usuario d  ";
            IQuery query = session.CreateQuery(hql);
            

            //ISQLQuery query = session.CreateSQLQuery("select * from Z13010");
            //var lista = query.List();

            IList<Usuario> l = new List<Usuario>();

            return  query.List<Usuario>();
        }

        /// <summary>
        /// Grava um Usuario no banco de dados 
        /// </summary>
        /// <param name="dep">The dep.</param>
        public void Add(Usuario user)
        {
            //Completa cadastro
            user = this.CompletaCadastro(user);

            //Salva o usuário
            ITransaction tran = session.BeginTransaction();
            session.Save(user);
            tran.Commit();

        }

        

        /// <summary>
        /// Altera um Usuario no banco de dados 
        /// </summary>
        /// <param name="dep">The dep.</param>
        public void Alter(Usuario user)
        {
            //Completa cadastro
            user = this.CompletaCadastro(user);

            ITransaction tran = session.BeginTransaction();
            session.Merge(user);
            tran.Commit();
        }

        /// <summary>
        /// Exclui um Usuario no banco de dados
        /// </summary>
        /// <param name="dep">The dep.</param>
        public void Delete(Usuario user)
        {
            ITransaction tran = session.BeginTransaction();
            session.Delete(user);
            tran.Commit();
        }

        
        /// <summary>
        /// Procura um usuario pelo CPF
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Usuario GetByCpf(string cpf)
        {
            string hql = "select d from Usuario d where d.Cpf= :cpf";
            IQuery query = session.CreateQuery(hql);
            query.SetParameter("cpf", cpf);
            return query.UniqueResult<Usuario>();
        }

        /// <summary>
        /// Verifica se já existe o usuário na Base
        /// </summary>
        /// <param name="usuario">The login.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public Usuario ExisteUsuario(Usuario user)
        {
            
            string hql = "select u from Usuario u where u.Cpf = :cpf ";
            IQuery query = this.session.CreateQuery(hql);
            query.SetParameter("cpf", user.Cpf);
            return query.UniqueResult<Usuario>();
            
        }

        public Usuario CompletaCadastro(Usuario user)
        {
            //Preeche os campos faltantes

            user.Filial = "01";

            Departamento dep = departamentoDAO.GetById(Convert.ToInt32(user.Departamento));
            user.DescricaoDepartamento = dep.Departamentos;

            user.DescricaoCentroCusto = centrodeCustoDao.GetNomeCentroDeCusto(user.CentroCusto);

            user.Tercerizado = user.Tercerizado.Substring(0, 1);
            if (String.IsNullOrEmpty(user.EmpresaTercerizada))
            {
                user.EmpresaTercerizada = "";
            }

            user.CodImpressaora = ""; //Compatibilidade
            user.PathImpressora = ""; //Compatibilidade
            user.NomeImpressora = ""; //Compatibilidade
            user.DELETE = "";
            user.R_E_C_N_O_ = RECNO.GetNextRecno("Z13010");
            return user;
        }
    }
}