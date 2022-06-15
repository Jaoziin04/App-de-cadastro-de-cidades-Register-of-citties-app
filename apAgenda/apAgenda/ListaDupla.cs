using System;
using System.Windows.Forms;
using System.IO;
using static System.Console;

class ListaDupla<Dado> : IDados<Dado>
                where Dado : IComparable<Dado>, IRegistro<Dado>, new()
{
    NoDuplo<Dado> primeiro, ultimo, atual;
    int quantosNos;
    Situacao situacaoAtual;

    public ListaDupla()
    {
        primeiro = ultimo = atual = null;
        quantosNos = 0;
    }

    public Situacao SituacaoAtual { get => this.situacaoAtual; set => this.situacaoAtual = value; }
    public int PosicaoAtual
    {
        get
        {
            if (Existe(DadoAtual(), out int ondeEsta))
                return ondeEsta;
            return -1;

        }
        set => PosicionarEm(value);
    }
    public bool EstaNoInicio { get => atual == primeiro; }
    public bool EstaNoFim { get => atual == ultimo; }
    public bool EstaVazio { get => primeiro == ultimo && ultimo == null; } // (bool) Verificar se está vazia
    public int Tamanho { get => quantosNos; }

    public void LerDados(string nomeArquivo)    // fará a leitura e armazenamento dos dados do arquivo cujo nome é passado por parâmetro
    {

        if (nomeArquivo != null)
        {
            StreamReader Ler = new StreamReader(nomeArquivo);

            while (!Ler.EndOfStream) // enquanto a variavel não estiver nula
            {
                Dado leitura = new Dado().LerRegistro(Ler); // lê o arquivo 

                IncluirAposFim(leitura); // inclui os dados no final do arquivo
            }
            Ler.Close();
        }


    }
    public void GravarDados(string nomeArquivo)  // gravará sequencialmente, no arquivo cujo nome é passado por parâmetro, os dados armazenados na lista
    {
        if (nomeArquivo != null)
        {
            StreamWriter Escrever = new StreamWriter(nomeArquivo);

            PosicionarNoPrimeiro();

            while (atual != null)
            {
                Dado escrita = DadoAtual(); // pegamos o dado atual

                escrita.GravarRegistro(Escrever); // dado atual grava o registro no arquivo

                AvancarPosicao();
            }

            Escrever.Close(); // fechamos o arquivo
        }
    }
    public void PosicionarNoPrimeiro()        // Posicionar atual no primeiro nó para ser acessado
    {
        atual = primeiro;

    }
    public void RetrocederPosicao()        // Retroceder atual para o nó anterior para ser acessado
    {
        if (atual != null) // se atual é defirente de nulo
            atual = atual.Ant; // voltamos a posição 
    }
    public void AvancarPosicao()
    {
        if (atual != null) // se atual é diferente de nulo
            atual = atual.Prox; // avançamos o atual
    }
    public void PosicionarNoUltimo()        // posicionar atual no último nó para ser acessado
    {

        if (!EstaNoFim && !EstaVazio)
        {
            atual = ultimo;
        }
    }
    public void PosicionarEm(int posicaoDesejada)
    {

        PosicionarNoPrimeiro(); // usamos o método posicionar no primeiro

        for (int i = 0; i < posicaoDesejada; i++) // fazemos um for, até a posição desejada
        {
            if (atual != null)
            {
                AvancarPosicao(); // usamos o método avançar posição
            }
        }
    }

    // (bool) Pesquisar Dado procurado em ordem crescente; a pesquisa
    // posicionará o ponteiro atual no nó procurado quando este
    // or encontrado ou, se não achar, no nó seguinte a local
    // onde deveria estar o nó procurado
    public bool Existe(Dado procurado, out int ondeEsta)
    {
        bool achou = false; // variavél que verifica se achamos método
        bool fim = false;  // variavél que verifica se estamos no fim da lista
        ondeEsta = 0;      // indica onde está

        PosicionarNoPrimeiro();

        if (procurado == null) // se o dado procurado for igual a null
        {
            ondeEsta = -1; // não achamos a posição
            return false;
        }


        if (EstaVazio == true) // se a lista está vazia, não faz sentido procurar o dado!
        {
            ondeEsta = -1;  // a posição não existe
            return false;
        }
        else
        {
            if (procurado.CompareTo(ultimo.Info) > 0) // se o conteudo for maior que ultimo
            {
                PosicionarNoUltimo();
                ondeEsta = -1; // a posição não existe
                return false;
            }
            else
                if (procurado.CompareTo(primeiro.Info) < 0) // se o conteúdo for menor que o primeiro
            {
                ondeEsta = -1; // a posição não existe
                return false;
            }



            while (!achou && !fim) // enquanto não acharmos e não for o fim
            {
                if (atual == null)
                    fim = true;

                else
                {
                    if (atual.Info.CompareTo(procurado) == 0) // se o conteudo do atua for igual ao dado procurado
                    {
                        achou = true; // achamos a posição
                    }
                    else // se o conteudo não for igual
                    {
                        if (atual.Info.CompareTo(procurado) > 0) // vefica se atual é maior que o procurado
                        {
                            fim = true; // o procurado não existe, portanto fim = true
                        }
                        else
                        {
                            AvancarPosicao();
                            ondeEsta++; // onde está fica com 1, o que indica que achamos a posição
                        }
                    }
                }
            }
            return achou; // retornamos achou
        }
    }
    public bool Excluir(Dado dadoAExcluir)
    {     

        if (Existe(dadoAExcluir, out int ondeEsta))
        {
            if (atual == primeiro)
            { // primeiro = p
                primeiro = primeiro.Prox;

                if (primeiro.Ant != null)
                {
                    primeiro.Ant.Prox = null;
                }

                primeiro.Ant = null;

                if (EstaVazio == true)
                    ultimo = null;
            }
            else
                if (atual == ultimo)
            {
                ultimo = atual.Ant;
                
                if (ultimo.Prox != null)
                {
                    ultimo.Prox.Ant = null;
                }

                ultimo.Prox = null;
            }
            else
            {
                atual.Ant.Prox = atual.Prox;
                atual.Prox.Ant = atual.Ant;
                atual.Ant = null;
                atual.Prox = null;
            }

            quantosNos--;
            return true;
        }
        return false;
    }
    public bool IncluirNoInicio(Dado novoValor)
    {

        if (novoValor == null)
            return false;

        if (Existe(novoValor, out int onde)) // se o dado existe, não faz sentido incluirmos ele.
            return false;

        var novoNo = new NoDuplo<Dado>(novoValor);
        if (EstaVazio)
        {
            ultimo = novoNo;
            primeiro = novoNo;
        }
        else
        {
            novoNo.Prox = primeiro;
            primeiro.Ant = novoNo;
            primeiro = novoNo;
        }
        quantosNos++;

        return true;
    }
    public bool IncluirAposFim(Dado novoValor)
    {

             if (Existe(novoValor, out int onde)) // se o dado existe, não faz sentido incluirmos ele.
            return false;

        var novoNo = new NoDuplo<Dado>(novoValor);
        if (EstaVazio == true)
            primeiro = ultimo = novoNo; // primeiro e ultimo são o mesmo nó
        else
        {
            novoNo.Ant = ultimo; // anterior do novono rece ultimop
            novoNo.Prox = null; // proximo do novono recebe null
            ultimo.Prox = novoNo; // proximo do ultimo recebe novono
            ultimo = novoNo; // ultimo rece novono

        }

        quantosNos++;

        return true;
    }
    public bool Incluir(Dado novoValor)         // (bool) Inserir nó com Dado em ordem crescente
    {

        if (novoValor == null)
            return false;

        if (!Existe(novoValor, out int onde))  // se o dado existe, não faz sentido incluirmos ele.
        {
            if (EstaVazio)
                IncluirNoInicio(novoValor);
            else
            {

                //if (atual == null) // se atual é igual a nulo, não incluimos o valor
                //  return false;


                if (EstaNoFim && onde == -1) // se for maior que o ultimo
                    IncluirAposFim(novoValor); // incluimos após o fim
                else
                   if (EstaNoInicio && onde == -1) // se for menor que o primeiro
                    IncluirNoInicio(novoValor); // incluimos no inicio

                else
                {
                    var newNo = new NoDuplo<Dado>(novoValor);

                    atual.Ant.Prox = newNo; // proximo do anterior do atual recebe o newNo
                    newNo.Prox = atual;

                    if (atual != null)
                        atual.Ant = newNo; // anterior do atual recebe o newNo

                    newNo.Ant = atual.Ant;
                    atual = newNo;


                }
            }
            return true;

        }

        return false;
    }
    public bool Incluir(Dado novoValor, int posicaoDeInclusao)  // inclui novo nó na posição indicada da lista
    {

        if (novoValor == null)
            return false;

        if (!Existe(novoValor, out int onde))  // se o dado existe, não faz sentido incluirmos ele.
        {
            if (posicaoDeInclusao > quantosNos + 1) // se a posição for maior que o tamanho da lista
                return false;

            if (posicaoDeInclusao < 0) // se a posição de inclusão for menor que o primeiro
                return false;

            var novoNo = new NoDuplo<Dado>(novoValor);

            PosicionarEm(posicaoDeInclusao); // posicionamos na posicaoDeInclusao

            atual.Ant.Prox = novoNo; // proximo do anterior do atual recebe o novoNo
            novoNo.Prox = atual;

            if (atual != null)
                atual.Ant = novoNo;  // anterior do atual recebe o novoNo

            novoNo.Ant = atual.Ant;
            atual = novoNo;

            return true;

        }

        return false;


    }
    public Dado this[int indice]
    {
        get
        {
            if (indice < 0 && indice > Tamanho - 1) // se o indice estiver fora dos limites
                throw new Exception("Fora dos limites");

            PosicionarEm(indice); // posicionamos na posição indicada no indice
            return DadoAtual();
        }
        set
        {
            if (indice < 0 && indice > Tamanho - 1)
                throw new Exception("Fora dos limites");

            PosicionarEm(indice);
            atual.Info = value;
        }
    }
    public Dado DadoAtual()  // retorna o dado atualmente visitado
    {
        if (atual != null)
            return atual.Info;

        return default(Dado);
    }
    public void ExibirDados()   // lista os dados armazenados na lista em modo console
    {


        Clear(); // limpa o console
        PosicionarNoPrimeiro();

        while (atual != null)
        {
            WriteLine(DadoAtual().ToString()); // escreve os dados na tela
            AvancarPosicao();
        }
    }
    public void ExibirDados(ListBox lista)  // lista os dados armazenados na lista no listbox passado como parâmetro
    {

        lista.Items.Clear(); // limpa o listBox
        PosicionarNoPrimeiro(); // posiciona no primeiro

        while (atual != null) // enquanto o atual for diferente de nulo
        {
            lista.Items.Add(DadoAtual()); // listamos o dado atual
            AvancarPosicao(); // avançamos a posição
        }

    }
    public void ExibirDados(ComboBox lista) // lista os dados armazenados na lista no combobox passado como parâmetro
    {
        lista.Items.Clear(); // limpamos o combobox
        PosicionarNoPrimeiro(); // posicionamos no primeiro

        while (atual != null) // enqunato o atual for diferente de nulo
        {
            lista.Items.Add(DadoAtual().ToString()); // listamos os intens
            AvancarPosicao(); // avançamos a posição
        }
    }
    public void ExibirDados(TextBox lista)
    {
        lista.Clear(); // limpamos o textBox
        PosicionarNoPrimeiro(); // posicionamos no primeiro

        while (atual != null) // enquaqnto o atual for diferente de nulo
        {
            lista.Text = DadoAtual().ToString(); // listamos o dado atual
            AvancarPosicao(); // avançamos a posição
        }
    }
    public void Ordenar()
    {

        var ordenar = new ListaDupla<Dado>();

        PosicionarNoPrimeiro();

        while (atual != null)
        {
            ordenar.Incluir(DadoAtual()); // incluimos o dado atual
            AvancarPosicao();
        }

        primeiro = ordenar.primeiro;
        ultimo = ordenar.ultimo;
    }
}