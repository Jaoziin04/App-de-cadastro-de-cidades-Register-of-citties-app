using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace apCaminhos
{
  public partial class FrmCidades : Form
  {
        private ListaDupla<Cidade> cidadeLista;
    public FrmCidades()
    {
      InitializeComponent();
    }

    private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
    {

    }

    private void tabPage1_Click(object sender, EventArgs e)
    {

    }

        private void FrmAgenda_Load(object sender, EventArgs e)
        {
            

                int indice = 0;
                udTempo.ImageList = imlBotoes;
                foreach (ToolStripItem item in udTempo.Items)
                    if (item is ToolStripButton)
                        (item as ToolStripButton).ImageIndex = indice++;

            cidadeLista = new ListaDupla<Cidade>();
            
            if (dlgAbrir.ShowDialog() == DialogResult.OK)
            {
                cidadeLista.LerDados(dlgAbrir.FileName);
                cidadeLista.ExibirDados(lbResultado);
                cidadeLista.PosicionarNoPrimeiro();
            }
            txtCodigo.Text = cidadeLista.DadoAtual().Codigo; //  exibe o codigo da cidade na tela
            txtNome.Text = cidadeLista.DadoAtual().Nome;
            udX.Value = cidadeLista.DadoAtual().X;
            udY.Value = cidadeLista.DadoAtual().Y;



            cidadeLista.SituacaoAtual = Situacao.incluindo;
        }

        private void btnInicio_Click(object sender, EventArgs e)
        {
            cidadeLista.SituacaoAtual = Situacao.navegando;

            btnAnterior.Enabled = false;
            btnProximo.Enabled = true;

            cidadeLista.PosicionarNoPrimeiro();


            txtCodigo.Text = cidadeLista.DadoAtual().Codigo; 
            txtNome.Text = cidadeLista.DadoAtual().Nome;
            udX.Value = cidadeLista.DadoAtual().X;
            udY.Value = cidadeLista.DadoAtual().Y;
        }

        private void btnAnterior_Click(object sender, EventArgs e)
        {
            cidadeLista.SituacaoAtual = Situacao.navegando;

            if(cidadeLista.EstaNoFim)
                btnProximo.Enabled = false;
            btnProximo.Enabled = true;

            cidadeLista.RetrocederPosicao();

            if (cidadeLista.DadoAtual() != null)
            {
                txtCodigo.Text = cidadeLista.DadoAtual().Codigo;
                txtNome.Text = cidadeLista.DadoAtual().Nome;
                udX.Value = cidadeLista.DadoAtual().X;
                udY.Value = cidadeLista.DadoAtual().Y;
            }

        }

        private void btnProximo_Click(object sender, EventArgs e)
        {
            cidadeLista.SituacaoAtual = Situacao.navegando;

            if(cidadeLista.EstaNoInicio)
                btnAnterior.Enabled = false;
            btnAnterior.Enabled=true;

            cidadeLista.AvancarPosicao();

            txtCodigo.Text = cidadeLista.DadoAtual().Codigo;
            txtNome.Text = cidadeLista.DadoAtual().Nome;
            udX.Value = cidadeLista.DadoAtual().X;
            udY.Value = cidadeLista.DadoAtual().Y;


        }

        private void btnUltimo_Click(object sender, EventArgs e)
        {
            cidadeLista.SituacaoAtual = Situacao.navegando;

            btnProximo.Enabled = false;
            

            cidadeLista.PosicionarNoUltimo();

            txtCodigo.Text = cidadeLista.DadoAtual().Codigo;
            txtNome.Text = cidadeLista.DadoAtual().Nome;
            udX.Value = cidadeLista.DadoAtual().X;
            udY.Value = cidadeLista.DadoAtual().Y;
        }

        private void btnNovo_Click(object sender, EventArgs e)
        {
            cidadeLista.SituacaoAtual = Situacao.incluindo;
            if (txtCodigo.Text == "" ||
                txtNome.Text == "")
            {
                MessageBox.Show("Não é possível incluir campos inexistentes.");
            }

            txtCodigo.Text = ""; // limpa o texto do codigo
            txtNome.Text = ""; // limpa o texto do nome
            udX.Value = 0; // zera o udX
            udY.Value = 0; // zera o udY
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            var newData = new Cidade(txtCodigo.Text.PadLeft(3, ' '), txtNome.Text.PadRight(15, ' '), udX.Value, udY.Value); 

            cidadeLista.Incluir(newData);
            cidadeLista.ExibirDados(lbResultado);
            cidadeLista.Existe(newData, out _);
        }

        private void btnProcurar_Click(object sender, EventArgs e)
        {
            cidadeLista.SituacaoAtual = Situacao.pesquisando;

            var localizador = new Cidade(txtCodigo.Text.PadLeft(3, ' '), txtNome.Text.PadRight(15, ' '), udX.Value, udY.Value); // variavel para procurar a cidade

            if (cidadeLista.Existe(localizador, out int onde)) // verifica se existe e mostra onde
            {
                txtCodigo.Text = cidadeLista.DadoAtual().Codigo;
                txtNome.Text = cidadeLista.DadoAtual().Nome;
                udX.Value = cidadeLista.DadoAtual().X;
                udY.Value = cidadeLista.DadoAtual().Y;
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            cidadeLista.PosicionarNoPrimeiro();

            while (cidadeLista.DadoAtual() != null)
            {
                Cidade cidadeAtual = cidadeLista.DadoAtual();

                int x = (int)(cidadeAtual.X * pictureBox1.Width); // valor de x da cidade vezes a altura do mapa
                int y = (int)(cidadeAtual.Y * pictureBox1.Height); // valor de y da cidade vezes a altura do mapa

                e.Graphics.FillEllipse(Brushes.Blue, new Rectangle(x, y, 10, 10)); // coloca pontos no mapa retangular

                cidadeLista.AvancarPosicao(); // avançamos a posição
            }

            cidadeLista.PosicionarNoPrimeiro(); // retorna o atual para a primeira posição após percorrer a lista
        }

        private void FrmCidades_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (dlgAbrir.FileName != "")
            {
                cidadeLista.Ordenar();
                cidadeLista.GravarDados(dlgAbrir.FileName);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e) // cancela se clicamos em incluir sem querer
        {
            cidadeLista.SituacaoAtual = Situacao.navegando;
            txtCodigo.Text = cidadeLista.DadoAtual().Codigo;
            txtNome.Text = cidadeLista.DadoAtual().Nome;
            udX.Value = cidadeLista.DadoAtual().X;
            udY.Value = cidadeLista.DadoAtual().Y;
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            cidadeLista.SituacaoAtual = Situacao.excluindo;

            var cidadeAExcluir = new Cidade(txtCodigo.Text.PadLeft(3, ' '), txtNome.Text.PadRight(15, ' '), udX.Value, udY.Value);

            if (cidadeLista.Existe(cidadeAExcluir, out _))
            {
                cidadeLista.Excluir(cidadeAExcluir); 
                txtCodigo.Clear();
                txtNome.Clear();
                udX.Value = 0;
                udY.Value = 0;
                cidadeLista.ExibirDados(lbResultado);
            }
            else
                MessageBox.Show("A cidadae não existe, portanto não há como excluirmos ela");

            
        }
    }
}
