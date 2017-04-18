﻿// ***********************************************************************
// Assembly         : MonitorFichaPallet
// Author           : daniel
// Created          : 04-10-2017
//
// Last Modified By : daniel
// Last Modified On : 04-17-2017
// ***********************************************************************
// <copyright file="Form1.cs" company="Sanchez Cano Ltda e Fini Guloseiamas">
//     Copyright ©  2017
// </copyright>
// <summary>Rotina resposnável pela monitoria do trânsito de palles, entre o
// envase, expedição e recebimento do CD na Finiguloseimas</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MonitorFichaPallet.Helpers;
using System.Threading;
using System.Data.SqlClient;

/// <summary>
/// The MonitorFichaPallet namespace.
/// </summary>
namespace MonitorFichaPallet
{
    /// <summary>
    /// Classe do formulário
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Form" />
    public partial class Form1 : Form
    {
        public BindingSource binding = new BindingSource();
        public Font oFontg = new Font(new FontFamily("Calibri"), 14);
        public Font oFonth = new Font(new FontFamily("Calibri"), 14, FontStyle.Bold);
        public CancellationTokenSource tcancel = new CancellationTokenSource();
        public int time = 60000;

        string query = "SELECT	TOP 300 ZD3_STATUS  as [Status],	    Cast(ZD3_EMISSA as Date)  as [Emissão],	        ZD3_HORA    as [Hora]," +
                                 " ZD3_SEQ           as [Sequência],	    ZD3_COD     as [Produto],	        ZD3_OP      as [OP]," +
                                 " ZD3_LOTECT        as [Lote], 	        ZD3_NFICHA  as [Ficha Pallet],	    ZD3_CARGA   as [Carga]," +
                                 " ZD3_NF            as [Nota Fiscal],   ZD3_QTDLID  as [Quantidade Lida],	ZD3_QUANT   as [Quantidade Apontada]" +
                                 " FROM  ZD3010" +
                                 " WHERE D_E_L_E_T_ = '' ";
        string order =           " ORDER BY ZD3_EMISSA desc, ZD3_HORA desc ";

        public string queryOri = "";

        public Form1()
        {    
            InitializeComponent();
        }

        /// <summary>
        /// Lida com os eventos que carregam os formulário em tela
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Form1_Load(object sender, EventArgs e)
        {

            dgMonitor.ColumnHeadersDefaultCellStyle.Font = oFonth;
            queryOri = query;

            CarregaDados();
            CarregaDadosasync();
            Tempoasync();
          

        }

        /// <summary>
        /// Monta o relório na tela do formulário
        /// </summary>
        private async void Tempoasync()
        {
           


            while (true)
            {
                //Inicia uma  Tarefa
                try
                {
                    await Task.Delay(60, tcancel.Token);
                }
                catch (TaskCanceledException ex)
                {
                    //Verifica se a task foi cancelada 
                    if (tcancel.IsCancellationRequested)
                    {
                        tcancel.Dispose();
                        tcancel = new CancellationTokenSource();
                        Thread.Sleep(2000); //Da um tempinho para parar a thread, dá só uma seguradinha fera                    
                    }
                }

                
                lbtime.Text = Convert.ToString(DateTime.Now);
            }
        }


        /// <summary>
        /// Carrega de forma assíncrona os dados em tela com um time refresh de 1 minuto
        /// </summary>
        private async void CarregaDadosasync()
        {
            while (true)
            {
                //Inicia uma  Tarefa
                try
                {
                    await Task.Delay(time, tcancel.Token);
                }
                catch (TaskCanceledException ex)
                {
                    //Verifica se a task foi cancelada 
                    if (tcancel.IsCancellationRequested)
                    {
                        tcancel.Dispose();
                        tcancel = new CancellationTokenSource();
                        Thread.Sleep(2000); //Da um tempinho para parar a thread, dá só uma seguradinha fera                    
                    }
                }

                CarregaDados();
            }
            
        }

        /// <summary>
        /// Método responsável por carregar os dados na grid        
        /// </summary>
        private void CarregaDados()
        {
            SqlDataReader leitor;
            SqlCommand cmd;

                using (IDbConnection conexao = ConnectionFactory.CriaConexao())
                using (IDbCommand comando = conexao.CreateCommand())
                {

                    cmd = (SqlCommand)comando;
                    cmd.CommandText = query + order;

                    //comando.CommandText = query;


                leitor = cmd.ExecuteReader();// comando.ExecuteReader();

                if (!leitor.HasRows)
                {
                    MessageBox.Show("Não há Pallets neste status", "Aviso", MessageBoxButtons.OK);
                    return;
                }
                
                    binding.DataSource = leitor;
                    dgMonitor.DataSource = binding;

            }
            

        }


        /// <summary>
        /// Realiza a formatação da Grid, na questão de tamenho das colunas
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void FormataGrid(object sender, EventArgs e)
        {
            dgMonitor.Columns["Status"].Width = 120;
            dgMonitor.Columns["Emissão"].Width = 120;
            dgMonitor.Columns["Sequência"].Width = 120;
            dgMonitor.Columns["Lote"].Width = 120;
            dgMonitor.Columns["Produto"].Width = 150;
            dgMonitor.Columns["Ficha Pallet"].Width = 120;
            dgMonitor.Columns["OP"].Width = 150;
            dgMonitor.Columns["Nota Fiscal"].Width = 120;
            dgMonitor.Columns["Quantidade Lida"].Width = 180;
            dgMonitor.Columns["Quantidade Apontada"].Width = 230;


        }

        /// <summary>
        /// Formata a grid com as cores e legendas
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DataGridViewCellFormattingEventArgs"/> instance containing the event data.</param>
        private void formatagrid2(object sender, DataGridViewCellFormattingEventArgs e)
        {

            if (dgMonitor.Rows[e.RowIndex].Cells["Status"].Value.ToString().Trim() == "DEVOLVIDO")
            {                
                dgMonitor.Rows[e.RowIndex].Cells["Status"].Style.BackColor = Color.OrangeRed;
            }                  
                               
            if (dgMonitor.Rows[e.RowIndex].Cells["Status"].Value.ToString().Trim() == "EXPEDICAO")
            {                  
                dgMonitor.Rows[e.RowIndex].Cells["Status"].Style.BackColor = Color.DeepSkyBlue;
            }                  
                               
            if (dgMonitor.Rows[e.RowIndex].Cells["Status"].Value.ToString().Trim() == "RECEBIDO")
            {                  
                dgMonitor.Rows[e.RowIndex].Cells["Status"].Style.BackColor = Color.GreenYellow;
            }                  
            if (dgMonitor.Rows[e.RowIndex].Cells["Status"].Value.ToString().Trim() == "TRANSITO")
            {                  
                dgMonitor.Rows[e.RowIndex].Cells["Status"].Style.BackColor = Color.Gold;
            }


            dgMonitor.Rows[e.RowIndex].Height = 50;

            dgMonitor.Rows[e.RowIndex].Cells["Status"].Style.Font = oFontg;
            dgMonitor.Rows[e.RowIndex].Cells["Emissão"].Style.Font = oFontg;
            dgMonitor.Rows[e.RowIndex].Cells["Hora"].Style.Font = oFontg;
            dgMonitor.Rows[e.RowIndex].Cells["Sequência"].Style.Font = oFontg;
            dgMonitor.Rows[e.RowIndex].Cells["Produto"].Style.Font = oFontg;
            dgMonitor.Rows[e.RowIndex].Cells["OP"].Style.Font = oFontg;
            dgMonitor.Rows[e.RowIndex].Cells["Lote"].Style.Font = oFontg;
            dgMonitor.Rows[e.RowIndex].Cells["Ficha Pallet"].Style.Font = oFontg;
            dgMonitor.Rows[e.RowIndex].Cells["Carga"].Style.Font = oFontg;
            dgMonitor.Rows[e.RowIndex].Cells["Nota Fiscal"].Style.Font = oFontg;
            dgMonitor.Rows[e.RowIndex].Cells["Quantidade Lida"].Style.Font = oFontg;
            dgMonitor.Rows[e.RowIndex].Cells["Quantidade Apontada"].Style.Font = oFontg;
        }



        /// <summary>
        /// Filtra os Devolvidos
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void FiltraDevolvidos(object sender, EventArgs e)
        {
            CheckBox ck = (CheckBox)sender;
            Filtra("DEVOLVIDO", ck.Checked);

            checkbExpedidos.Checked = false;
            checkbTransito.Checked = false;
            checkbRecebido.Checked = false;

        }

        /// <summary>
        /// Filtras Expedidos.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void FiltraExpedidos(object sender, EventArgs e)
        {
            CheckBox ck = (CheckBox)sender;
            Filtra("EXPEDICAO", ck.Checked);

            checkbDevolvidos.Checked = false;
            checkbTransito.Checked = false;
            checkbRecebido.Checked = false;
        }

        /// <summary>
        /// Filtra Transito
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void FiltraTransito(object sender, EventArgs e)
        {
            CheckBox ck = (CheckBox)sender;
            Filtra("TRANSITO", ck.Checked);
            checkbDevolvidos.Checked = false;
            checkbExpedidos.Checked = false;
            checkbRecebido.Checked = false;
        }

        /// <summary>
        /// Filtra Recebido
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void FiltraRecebido(object sender, EventArgs e)
        {

            CheckBox ck = (CheckBox)sender;
            Filtra("RECEBIDO", ck.Checked);

            checkbDevolvidos.Checked = false;
            checkbExpedidos.Checked = false;
            checkbTransito.Checked = false;
        }

        /// <summary>
        /// Função que executa os filtros
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="fazFiltro">if set to <c>true</c> [faz filtro].</param>
        private void Filtra(string status, bool fazFiltro)
        {
            if (fazFiltro)
            {
                query += "AND ZD3_STATUS ='"+ status + "' ";
            }
            else
            {
                query = queryOri;
            }

            CarregaDados();
            query = queryOri;
        }

    }

    

}
