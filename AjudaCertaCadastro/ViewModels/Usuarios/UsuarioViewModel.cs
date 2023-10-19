using AjudaCertaCadastro.Models;
using AjudaCertaCadastro.Models.Enuns;
using AjudaCertaCadastro.Services.Pessoas;
using AjudaCertaCadastro.Services.Usuarios;
using AjudaCertaCadastro.Views.Usuarios;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AjudaCertaCadastro.ViewModels.Usuarios
{
    public class UsuarioViewModel : BaseViewModel
    {
        private UsuarioService uService;
        private PessoaService pService;
        public ICommand RegistrarUsuarioCommand { get; set; }
        public ICommand AutenticarCommand { get; set; }
        public ICommand DirecionarCadastroCommand { get; set; }
        public UsuarioViewModel()
        {
            uService = new UsuarioService();
            _ = ObterTipoPessoas();
            _ = ObterFisicaJuridica();
            InicializarCommands();
        }
        public void InicializarCommands()
        {
            RegistrarUsuarioCommand = new Command(async () => await RegistrarUsuario());
            AutenticarCommand = new Command(async () => await AutenticarUsuario());
            DirecionarCadastroCommand = new Command(async () => await DirecionarParaCadastro());
        }

        #region AtributosPropriedades

        private string login = string.Empty;
        public string Login
        {
            get { return login; }
            set
            {
                login = value;
                OnPropertyChanged();
            }
        }

        private string senha = string.Empty;
        public string Senha
        {
            get { return senha; }
            set
            {
                senha = value;
                OnPropertyChanged();
            }
        }
        private string nome = string.Empty;
        public string Nome
        {
            get { return nome; }
            set
            {
                nome = value;
                OnPropertyChanged();
            }
        }

        private string documento = string.Empty;
        public string Documento
        {
            get { return documento; }
            set
            {
                documento = value;
                OnPropertyChanged();
            }
        }

        private string telefone = string.Empty;
        public string Telefone
        {
            get { return telefone; }
            set
            {
                telefone = value;
                OnPropertyChanged();
            }
        }

        private DateTime datanasc = DateTime.MinValue;
        public DateTime Datanasc
        {
            get { return datanasc; }
            set
            {
                datanasc = value;
                OnPropertyChanged();
            }
        }

        private string genero = string.Empty;
        public string Genero
        {
            get { return genero; }
            set
            {
                genero = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<TipoPessoa> listaTipoPessoa;
        public ObservableCollection<TipoPessoa> ListaTipoPessoa
        {
            get { return listaTipoPessoa; }
            set
            {
                if (value != null)
                {
                    listaTipoPessoa = value;
                    OnPropertyChanged();
                }
            }
        }

        private TipoPessoa tipoPessoaSelecionado;
        public TipoPessoa TipoPessoaSelecionado
        {
            get { return tipoPessoaSelecionado; }
            set
            {
                if (value != null)
                {
                    tipoPessoaSelecionado = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<FisicaJuridica> listaFisicaJuridica;
        public ObservableCollection<FisicaJuridica> ListaFisicaJuridica
        {
            get { return listaFisicaJuridica; }
            set
            {
                if (value != null)
                {
                    listaFisicaJuridica = value;
                    OnPropertyChanged();
                }
            }
        }

        private FisicaJuridica fisicaJuridicaSelecionado;
        public FisicaJuridica FisicaJuridicaSelecionado
        {
            get { return fisicaJuridicaSelecionado; }
            set
            {
                if (value != null)
                {
                    fisicaJuridicaSelecionado = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Métodos
        public async Task ObterFisicaJuridica()
        {
            try
            {
                ListaFisicaJuridica = new ObservableCollection<FisicaJuridica>();
                ListaFisicaJuridica.Add(new FisicaJuridica() { Id = 1, Descricao = "Pessoa Física" });
                ListaFisicaJuridica.Add(new FisicaJuridica() { Id = 2, Descricao = "Pessoa Jurídica" });
                OnPropertyChanged(nameof(ListaFisicaJuridica));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }
        public async Task ObterTipoPessoas()
        {
            try
            {
                ListaTipoPessoa = new ObservableCollection<TipoPessoa>();
                ListaTipoPessoa.Add(new TipoPessoa() { Id = 1, Descricao = "DOADOR" });
                ListaTipoPessoa.Add(new TipoPessoa() { Id = 2, Descricao = "ONG" });
                ListaTipoPessoa.Add(new TipoPessoa() { Id = 3, Descricao = "BENEFICIARIO" });
                OnPropertyChanged(nameof(ListaTipoPessoa));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }
        public async Task RegistrarUsuario()
        {
            try
            {
                Usuario u = new Usuario();
                u.Email = Login;
                u.Senha = Senha;

                Pessoa p = new Pessoa();
                p.Nome = Nome;
                p.Documento = Documento;
                p.Telefone = Telefone;
                p.DataNasc = Convert.ToDateTime(Datanasc);
                p.Genero = Genero;
                p.Tipo = (TipoPessoaEnum)tipoPessoaSelecionado.Id;
                p.fisicaJuridica = (FisicaJuridicaEnum)fisicaJuridicaSelecionado.Id;

                Usuario uRegistrado = await uService.PostRegistrarUsuarioAsync(u);

                Preferences.Set("UsuarioId", uRegistrado.Id);
                Preferences.Set("UsuarioEmail", uRegistrado.Email);
                Preferences.Set("UsuarioToken", uRegistrado.Token);

                string token = Preferences.Get("UsuarioToken", string.Empty);
                pService = new PessoaService(token);

                Pessoa pRegistrado = await pService.PostRegistrarPessoaAsync(p);
                if (uRegistrado.Id != 0 && pRegistrado.Id != 0)
                {
                    string mensagem = $"Usuário Id {uRegistrado.Id} registrado com sucesso.";
                    await Application.Current.MainPage.DisplayAlert("Informação", mensagem, "Ok");

                    await Application.Current.MainPage
                        .Navigation.PopAsync();
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Informação", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }

        public async Task AutenticarUsuario()
        {
            try
            {
                Usuario u = new Usuario();
                u.Email = Login;
                u.Senha = Senha;

                Usuario uAutenticado = await uService.PostAutenticarUsuarioAsync(u);

                if (!string.IsNullOrEmpty(uAutenticado.Token))
                {
                    string mensagem = $"Bem-vindo(a) {uAutenticado.Email}";

                    Preferences.Set("UsuarioId", uAutenticado.Id);
                    Preferences.Set("UsuarioEmail", uAutenticado.Email);
                    Preferences.Set("UsuarioToken", uAutenticado.Token);

                    await Application.Current.MainPage
                        .DisplayAlert("Informação", mensagem, "Ok");

                    Application.Current.MainPage = new MainPage();
                }
                else
                {
                    await Application.Current.MainPage
                        .DisplayAlert("Informação", "Dados incorretos :(", "Ok");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Informação", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }

        public async Task DirecionarParaCadastro()
        {
            try
            {
                await Application.Current.MainPage
                    .Navigation.PushAsync(new Views.Usuarios.CadastroView());
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Informação", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }
        #endregion
    }

}
